using BCA.Common;
using hub_client.Network;
using hub_client.Windows.Controls.Controls_Stuff;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace hub_client.WindowsAdministrator
{
    public class GamesHistoryAdministrator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public GameClient Client;

        public event Action<RoomResultItem[]> GetGamesHistory;

        public GamesHistoryAdministrator(GameClient client)
        {
            Client = client;

            Client.GetGamesHistory += Client_GetGamesHistory;
        }

        private void Client_GetGamesHistory(RoomResult[] results)
        {
            List<RoomResultItem> items = new List<RoomResultItem>();

            foreach (RoomResult r in results)
            {
                RoomResultItem item = new RoomResultItem
                {
                    GameDate = r.GameDate.ToString("dd/MM"),
                    CurrentRank = r.CurrentRank,
                    ELO = r.ELO,
                    Opponent = r.Opponent.Username,
                    TotalRank = r.TotalRank,
                    ResultPic = r.ELO > 0 ? FormExecution.AssetsManager.GetImage("Logo", "win") : FormExecution.AssetsManager.GetImage("Logo", "lose")
                };
            
                if (!r.Opponent.Avatar.IsHost)
                    item.AvatarPic = FormExecution.AssetsManager.GetImage("Avatars", r.Opponent.Avatar.Id.ToString());
                else
                {
                    try
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadFile(
                                new System.Uri(r.Opponent.Avatar.URL),
                                Path.Combine(FormExecution.path, "Assets", "Avatars", "A_" + item.Opponent + ".png")
                                );
                        }
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        image.UriSource = new Uri(Path.Combine(FormExecution.path, "Assets", "Avatars", "A_" + item.Opponent + ".png"));
                        image.EndInit();

                        item.AvatarPic = image;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        FormExecution.Client_PopMessageBox("Une erreur s'est produite lors du chargement de votre image.", "Erreur", true);
                    }
                }
                items.Add(item);
            }

            GetGamesHistory?.Invoke(items.ToArray());
        }

    }
}
