﻿using BCA.Common.Enums;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour CustomCustomizationWindow.xaml
    /// </summary>
    public partial class CustomCustomizationWindow : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        private CustomizationType _ctype;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public event Action<string> SelectedURL;
        public CustomCustomizationWindow(CustomizationType ctype)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            LoadStyle();
            _ctype = ctype;
            AdaptImageSize();

            this.MouseDown += Window_MouseDown;

            btn_load.MouseLeftButtonDown += LoadCustomization;
            btn_validate.MouseLeftButtonDown += ValidURL;
        }

        private void ValidURL(object sender, MouseButtonEventArgs e)
        {
            if (tb_url.Text == string.Empty || tb_url.Text == "Une erreur s'est produite. Veuillez vérifier votre URL (web non local)")
                return;

            string[] urlformat = tb_url.Text.Split('.');
            if (urlformat[urlformat.Length - 1] != "png" || (!tb_url.Text.StartsWith("http://") && !tb_url.Text.StartsWith("https://")))
            {
                tb_url.Text = "Une erreur s'est produite. Veuillez vérifier votre URL. FORMAT PNG OBLIGATOIRE.";
                FormExecution.Client_PopMessageBox("Une erreur s'est produite. Veuillez vérifier votre URL. FORMAT PNG OBLIGATOIRE ET HEBERGEMENT SUR LE WEB.", "Erreur");
                return;
            }

            SelectedURL?.Invoke(tb_url.Text);
            Close();
        }

        private void LoadCustomization(object sender, MouseButtonEventArgs e)
        {
            string[] urlformat = tb_url.Text.Split('.');
            if (urlformat[urlformat.Length - 1] != "png")
            {
                tb_url.Text = "Une erreur s'est produite. Veuillez vérifier votre URL. FORMAT PNG OBLIGATOIRE.";
                FormExecution.Client_PopMessageBox("Une erreur s'est produite. Veuillez vérifier votre URL. FORMAT PNG OBLIGATOIRE.", "Erreur");
                return;
            }

            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileAsync(
                        new System.Uri(tb_url.Text),
                        Path.Combine(FormExecution.path, "Assets", "temp.png")
                        );
                    wc.DownloadFileCompleted += LoadDownloadedCustom;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ERROR WHEN LOADING CUSTOM CUSTOMIZATION {0}", ex.ToString());
                tb_url.Text = "Une erreur s'est produite. Veuillez vérifier votre URL. FORMAT PNG OBLIGATOIRE.";
            }
        }

        private void LoadDownloadedCustom(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(Path.Combine(FormExecution.path, "Assets", "temp.png"));
                image.EndInit();

                img_custom.Source = image;
            }
            catch (Exception ex)
            {
                logger.Error("ERROR WHEN LOADING CUSTOM CUSTOMIZATION {0}", ex.ToString());
                tb_url.Text = "Une erreur s'est produite lors du téléchargement de l'URL... ";
            }
        }

        private void AdaptImageSize()
        {
            switch (_ctype)
            {
                case CustomizationType.Avatar:
                    img_custom.Width = 256;
                    img_custom.Height = 256;
                    break;
                case CustomizationType.Sleeve:
                    img_custom.Width = 177;
                    img_custom.Height = 254;
                    break;
                case CustomizationType.Border:
                    img_custom.Width = 306;
                    img_custom.Height = 136;
                    break;
            }
        }
        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_validate, btn_load });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1PrestigeShopButton");
                btn.Color2 = style.GetGameColor("Color2PrestigeShopButton");
                btn.Update();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(40, 0, 40, 40);
            }
            else if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                this.bg_border.CornerRadius = new CornerRadius(0);
            }
        }
        private void minimizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch { }
        }
    }
}
