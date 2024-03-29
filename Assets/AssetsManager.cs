﻿using BCA.Common;
using BCA.Common.Enums;
using hub_client.Stuff;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hub_client.Assets
{
    public class AssetsManager
    {
        private string path;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Dictionary<string, List<Smiley>> Smileys;
        private Dictionary<int, string> TeamEmblemsURL;

        public AssetsManager()
        {
            path = FormExecution.path;
            if (File.Exists(Path.Combine(FormExecution.path, "Assets", "Team", "emblems.json")))
                TeamEmblemsURL = JsonConvert.DeserializeObject<Dictionary<int, string>>(File.ReadAllText(Path.Combine(FormExecution.path, "Assets", "Team", "emblems.json")));
            else
                TeamEmblemsURL = new Dictionary<int, string>();
        }
        public Smiley CheckSmiley(string word)
        {
            foreach (var groups in Smileys)
                foreach (Smiley s in groups.Value)
                    if (s.Name == word)
                        return s;
            return null;
        }
        public void LoadSmileys()
        {
            Smileys = new Dictionary<string, List<Smiley>>();
            List<string> smileyGroups = new List<string>(Directory.EnumerateDirectories(Path.Combine(FormExecution.path, "Assets", "smileys")));
            foreach (string directory in smileyGroups)
            {
                string d = directory.Split('\\').Last();
                List<string> smileys = new List<string>(Directory.EnumerateFiles(Path.Combine(FormExecution.path, "Assets", "smileys", d)));
                smileys.Sort();
                foreach (string smiley in smileys)
                {
                    if (!smiley.EndsWith(".png"))
                        continue;
                    string[] name = smiley.Split('\\');
                    if (!Smileys.ContainsKey(d))
                        Smileys.Add(d, new List<Smiley>());
                    Smileys[d].Add(CreateSmiley(name[name.Length - 1], d));
                }
            }
        }
        private Smiley CreateSmiley(string name, string directory)
        {
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = GetImage(new string[] { "Smileys", directory, name });
            return new Smiley
            {
                Name = name.Split('.')[0],
                Pic = image
            };
        }

        public BitmapImage GetImage(string directory, string img)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(Path.Combine(path, "Assets", directory, img + ".png"));
                image.EndInit();
                return image;
            }
            catch (Exception ex)
            {
                logger.Error("IMAGE LOADING - {0}", ex);
                return null;
            }
        }

        public BitmapImage GetImage(string[] directory)
        {
            try
            {
                string img_path = Path.Combine(path, "Assets");
                foreach (string item in directory)
                    img_path = Path.Combine(img_path, item);
                return new BitmapImage(new Uri(img_path));
            }
            catch (Exception)
            {
                return GetUnknownCardPic();
            }
        }

        public BitmapImage GetPics(string[] directory)
        {
            string img_path = FormExecution.path;
            foreach (string item in directory)
                img_path = Path.Combine(img_path, item);
            try
            {
                return new BitmapImage(new Uri(img_path));
            }
            catch (Exception ex)
            {
                logger.Warn("PICS LOADED:" + ex.ToString());
                return GetUnknownCardPic();
            }
        }

        public Brush GetBrush(string directory, string img)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(Path.Combine(path, "Assets", directory, img));
                image.EndInit();
                return new ImageBrush(image);
            }
            catch (Exception ex)
            {
                logger.Error("IMAGE LOADING - {0}", ex);
                return null;
            }
        }

        public Brush GetBrush(string directory, string img, string extension)
        {
            return new ImageBrush(new BitmapImage(new Uri(Path.Combine(path, "Assets", directory, img + "." + extension))));
        }

        public string GetSource(string directory, string img)
        {
            return Path.Combine(path, "Assets", directory, img);
        }

        public BitmapImage GetUnknownCardPic()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "unknown.jpg"));
            image.EndInit();
            return image;
        }

        public BitmapImage GetCustom(Customization custom)
        {
            string d = "";
            switch (custom.CustomizationType)
            {
                case CustomizationType.Avatar:
                    d = "Avatars";
                    break;
                case CustomizationType.Sleeve:
                    d = "Sleeves";
                    break;
                case CustomizationType.Border:
                    d = "Borders";
                    break;
                case CustomizationType.Partner:
                    d = "Partners";
                    break;
            }

            if (!File.Exists(Path.Combine(FormExecution.path, "Assets", d, custom.Id + ".png")))
            {               
                if (!custom.IsHost)
                    return GetUnknownCardPic();
                else
                {
                    try
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadFile(
                                new System.Uri(custom.URL),
                                Path.Combine(FormExecution.path, "Assets", d, custom.Id + ".png")
                                );
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        FormExecution.Client_PopMessageBox("Une erreur s'est produite lors du chargement de votre image.", "Erreur");
                    }
                }
            }

            return GetImage(d, custom.Id.ToString());
        }
        public BitmapImage GetTeamEmblem(int teamID, string emblem)
        {
            if (!Directory.Exists(Path.Combine(FormExecution.path, "Assets", "Team")))
                Directory.CreateDirectory(Path.Combine(FormExecution.path, "Assets", "Team"));

            if (!File.Exists(Path.Combine(FormExecution.path, "Assets", "Team", teamID + ".png")) && emblem == "")
                return null;

            if (!TeamEmblemsURL.ContainsKey(teamID) || (TeamEmblemsURL[teamID] != emblem && emblem != "") || !File.Exists(Path.Combine(FormExecution.path, "Assets", "Team", teamID + ".png")))
            {
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFile(
                            new System.Uri(emblem),
                            Path.Combine(FormExecution.path, "Assets", "Team", teamID + ".png")
                            );
                    }
                    TeamEmblemsURL[teamID] = emblem;
                    SaveTeamEmblem();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    FormExecution.Client_PopMessageBox("Une erreur s'est produite lors du chargement de votre image.", "Erreur");
                }
            }

            return GetImage("Team", teamID.ToString());
        }

        private void SaveTeamEmblem()
        {
            File.WriteAllText(Path.Combine(FormExecution.path, "Assets", "Team", "emblems.json"), JsonConvert.SerializeObject(TeamEmblemsURL));
        }
    }
}
