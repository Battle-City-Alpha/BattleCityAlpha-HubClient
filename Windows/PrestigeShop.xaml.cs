using BCA.Common.Enums;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour PrestigeShop.xaml
    /// </summary>
    public partial class PrestigeShop : Window
    {
        PrestigeShopAdministrator _admin;
        public PrestigeShop(PrestigeShopAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _admin = admin;

            _admin.UpdatePrestigePoints += _admin_UpdatePrestigePoints;
            _admin.UpdateProgress += _admin_UpdateProgress;

            InitItem();
            LoadStyle();

            this.MouseDown += Window_MouseDown;
            this.Loaded += PrestigeShop_Loaded;
        }

        private void _admin_UpdateProgress(int progress)
        {
            progressBar_donation.Value = progress;
            this.label_progressbar.Content = progressBar_donation.Value.ToString();
        }
        private void _admin_UpdatePrestigePoints(int pp)
        {
            this.label_PP.Content = pp.ToString();
        }

        private void PrestigeShop_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void LoadStyle()
        {
            AppDesignConfig style = FormExecution.AppDesignConfig;
            this.FontFamily = style.Font;

            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_get_PP });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1PrestigeShopButton");
                btn.Color2 = style.GetGameColor("Color2PrestigeShopButton");
                btn.Update();
            }
        }

        private void InitItem()
        {
            MonthPack_Item.Initialize("Pack du mois !", "800 PP", "monthpack", "Le pack du mois vous offre : " + Environment.NewLine + " - un avatar, une bordure, une sleeve EXCLUSIFS." + Environment.NewLine + " - 1 mois de privilèges membre VIP" + Environment.NewLine + " - 24h (à partir du moment de l'achat) de double gains de BPs en duel !");

            VIP_Item.Initialize("Membre VIP (3 mois)", "1000 PP", "vip", "Permet d'obtenir le statut de membre VIP pendant 3 mois." + Environment.NewLine + "Un statut spécial dans le chat." + Environment.NewLine + "Un avatar, une bordure, une sleeve et un titre exclusifs !" + Environment.NewLine + "Les gains de BPs doublés lors des animations !");
            DoubleBP_Item.Initialize("Doublez vos BPs (3J)", "500 PP", "double_bp", "Permet de doubler ses gains de BPs en duels pendant 3 jours");
            UsernameColor_Item.Initialize("Couleur pseudo personnalisée", "300 PP", "username_color", "Permet de changer la couleur de son pseudo dans le chat");

            AvatarURL_Item.Initialize("Avatar personnalisé (1 mois)", "400 PP", "avatarURL", "Permet de mettre une image de votre choix via une URL en tant qu'avatar pendant 1 mois.");
            BorderURL_Item.Initialize("Bordure personnalisée (1 mois)", "400 PP", "borderURL", "Permet de mettre une image de votre choix via une URL en tant que bordure pendant 1 mois.");
            SleeveURL_Item.Initialize("Sleeve personnalisée (1 mois)", "400 PP", "sleeveURL", "Permet de mettre une image de votre choix via une URL en tant que sleeve pendant 1 mois.");

            AvatarList_Item.Initialize("Acheter un avatar", "250 PP", "avatarList", "Permet de choisir un avatar prestigieux et rare parmis une liste pour ensuite l'utiliser.");
            BorderList_Item.Initialize("Acheter une bordure", "250 PP", "borderList", "Permet de choisir une bordure prestigieuse et rare parmis une liste pour ensuite l'utiliser.");
            SleeveList_Item.Initialize("Acheter une sleeve", "250 PP", "sleeveList", "Permet de choisir une sleeve prestigieuse et rare parmis une liste pour ensuite l'utiliser.");

            TitleList_Item.Initialize("Acheter un titre", "100 PP", "titleList", "Permet de choisir un titre prestigieux et rare parmis une liste pour ensuite l'utiliser.");
            Greet_Item.Initialize("Greets illimités", "100 PP", "greet", "Une fois achetée, permet de changer de greets à l'infini !");
            ChangeUsername_Item.Initialize("Changer de pseudo", "300 PP", "change_username", "Permet de changer de pseudo");

            ResetStat_Item.Initialize("Réinitialiser les statistiques", "50 PP", "reset_stats", "Permet de réinitialiser les statistiques");

            btn_get_PP.MouseLeftButtonDown += PP_Item_MouseLeftButtonDown;

            MonthPack_Item.btn_purchase.MouseLeftButtonDown += BuyMonthPack;


            VIP_Item.btn_purchase.MouseLeftButtonDown += BuyVIP;
            DoubleBP_Item.btn_purchase.MouseLeftButtonDown += BuyDoubleBP;
            UsernameColor_Item.btn_purchase.MouseLeftButtonDown += BuyUsernameColor;

            AvatarURL_Item.btn_purchase.MouseLeftButtonDown += BuyCustomAvatar;
            BorderURL_Item.btn_purchase.MouseLeftButtonDown += BuyCustomBorder;
            SleeveURL_Item.btn_purchase.MouseLeftButtonDown += BuyCustomSleeve;

            AvatarList_Item.btn_purchase.MouseLeftButtonDown += BuyPrestigeAvatar;
            BorderList_Item.btn_purchase.MouseLeftButtonDown += BuyPrestigeBorder;
            SleeveList_Item.btn_purchase.MouseLeftButtonDown += BuyPrestigeSleeve;

            TitleList_Item.btn_purchase.MouseLeftButtonDown += BuyPrestigeTitle;
            Greet_Item.btn_purchase.MouseLeftButtonDown += BuyGreetItem;
            ChangeUsername_Item.btn_purchase.MouseLeftButtonDown += BuyChangeUsername;

            ResetStat_Item.btn_purchase.MouseLeftButtonDown += BuyResetStats;
        }

        private void BuyResetStats(object sender, MouseButtonEventArgs e)
        {
            _admin.SendBuyResetStats();
        }
        private void BuyChangeUsername(object sender, MouseButtonEventArgs e)
        {
            InputText form = new InputText();
            form.Title = "Nouveau pseudo";
            form.SelectedText += ChangeUsernameSelectedText;
            form.Owner = this;
            form.Show();
        }
        private void ChangeUsernameSelectedText(string newusername)
        {
            _admin.SendBuyChangeUsername(newusername);
        }
        private void BuyGreetItem(object sender, MouseButtonEventArgs e)
        {
            _admin.SendBuyInfiniGreet();
        }
        private void BuyPrestigeTitle(object sender, MouseButtonEventArgs e)
        {
            _admin.SendAskPrestigeCustomizations(CustomizationType.Title);
        }
        private void BuyPrestigeSleeve(object sender, MouseButtonEventArgs e)
        {
            _admin.SendAskPrestigeCustomizations(CustomizationType.Sleeve);
        }
        private void BuyPrestigeBorder(object sender, MouseButtonEventArgs e)
        {
            _admin.SendAskPrestigeCustomizations(CustomizationType.Border);
        }
        private void BuyPrestigeAvatar(object sender, MouseButtonEventArgs e)
        {
            _admin.SendAskPrestigeCustomizations(CustomizationType.Avatar);
        }
        private void BuyCustomSleeve(object sender, MouseButtonEventArgs e)
        {
            CustomCustomizationWindow window = new CustomCustomizationWindow(CustomizationType.Sleeve);
            window.SelectedURL += (url) => SendCustomCustomizationURL(url, CustomizationType.Sleeve);
            window.Owner = this;
            window.Show();
        }
        private void BuyCustomBorder(object sender, MouseButtonEventArgs e)
        {
            CustomCustomizationWindow window = new CustomCustomizationWindow(CustomizationType.Border);
            window.SelectedURL += (url) => SendCustomCustomizationURL(url, CustomizationType.Border);
            window.Owner = this;
            window.Show();
        }
        private void BuyCustomAvatar(object sender, MouseButtonEventArgs e)
        {
            CustomCustomizationWindow window = new CustomCustomizationWindow(CustomizationType.Avatar);
            window.SelectedURL += (url) => SendCustomCustomizationURL(url, CustomizationType.Avatar);
            window.Owner = this;
            window.Show();
        }
        private void SendCustomCustomizationURL(string url, CustomizationType ctype)
        {
            _admin.SendCustomCustomization(url, ctype);
        }
        private void BuyUsernameColor(object sender, MouseButtonEventArgs e)
        {
            ColorPickerWindow window = new ColorPickerWindow();
            window.SelectedColor += SendUsernameColor;
            window.Owner = this;
            window.Show();
        }
        private void SendUsernameColor(string color)
        {
            _admin.SendBuyUsernameColor(color.Substring(3));
        }

        private void BuyDoubleBP(object sender, MouseButtonEventArgs e)
        {
            _admin.SendBuyDoubleBP();
        }
        private void BuyVIP(object sender, MouseButtonEventArgs e)
        {
            _admin.SendBuyVIP();
        }
        private void BuyMonthPack(object sender, MouseButtonEventArgs e)
        {
            _admin.SendAskMonthPack();
        }

        private void PP_Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "http://battlecityalpha.xyz/donations/donate.php";
            p.Start();
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.ActivateShop();
            this.Close();
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(50);
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
