using hub_client.Assets;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Tools.xaml
    /// </summary>
    public partial class Tools : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        private ClientConfig client_config = FormExecution.ClientConfig;
        AssetsManager PicsManager = new AssetsManager();

        private ToolsAdministrator _admin;

        public Tools(ToolsAdministrator admin)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            this.Loaded += Tools_Loaded;
            this.MouseDown += Window_MouseDown;

            _admin = admin;

            cb_duelrequest.Checked += Cb_duelrequest_Checked;
        }

        private void Cb_duelrequest_Checked(object sender, RoutedEventArgs e)
        {
            if (cb_duelrequest.IsChecked == false)
                return;

        }

        private void Tools_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();

            cb_connectionmsg.IsChecked = client_config.Connexion_Message;
            cb_greet.IsChecked = client_config.Greet;
            cb_traderequest.IsChecked = client_config.IgnoreTradeRequest;
            cb_duelrequest.IsChecked = client_config.IgnoreDuelRequest;
            cb_customduelrequest.IsChecked = client_config.IgnoreCustomDuelRequest;
            cb_autoscroll.IsChecked = client_config.Autoscroll;
            cb_popuppm.IsChecked = client_config.PMPopup;
            cb_allowsharedeck.IsChecked = !client_config.AllowDeckShare;
            cb_popupduelend.IsChecked = client_config.PMEndDuel;
            cb_showchatscrollbar.IsChecked = client_config.ShowChatScrollbar;
            cb_showuserlistscrollbar.IsChecked = client_config.UserlistScrollbar;
            cb_backgroundchatpic.IsChecked = client_config.ChatBackgroundIsPic;
            cb_showmpall.IsChecked = client_config.ShowPMAll;

            foreach (var color in style.GameColors)
                cb_colorList.Items.Add(color.Key);

            cb_colorList.SelectedIndex = 0;

            cb_fontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(x => x.Source);
            cb_fontFamily.SelectedItem = style.Font;

            tb_fontsize.Text = style.FontSize.ToString();

            cb_pics.Items.Clear();
            List<string> pics = new List<string>(Directory.EnumerateFiles(Path.Combine(FormExecution.path, "Assets", "Background")));
            foreach (string p in pics)
            {
                string[] name = p.Split('\\');
                cb_pics.Items.Add(name[name.Length - 1]);
            }
            cb_pics.SelectionChanged += Cb_pics_SelectionChanged;
            cb_pics.SelectedIndex = 0;

            cb_ygopro_pics.Items.Clear();
            List<string> ygopropics = new List<string>(Directory.EnumerateFiles(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures")));
            foreach (string p in ygopropics)
            {
                if (!p.EndsWith(".png") && !p.EndsWith("jpg"))
                    continue;
                string[] name = p.Split('\\');
                cb_ygopro_pics.Items.Add(name[name.Length - 1]);
            }
            cb_ygopro_pics.SelectionChanged += Cb_ygopro_pics_SelectionChanged; ;
            cb_ygopro_pics.SelectedIndex = 0;
            showroom_ygopro_pics.MouseLeftButtonDown += Showroom_ygopro_pics_MouseLeftButtonDown;
        }

        private void Showroom_ygopro_pics_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog getpic = new OpenFileDialog();
            getpic.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (getpic.ShowDialog() == true)
            {
                File.Delete(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", cb_ygopro_pics.SelectedItem.ToString()));
                File.Copy(getpic.FileName, Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", cb_ygopro_pics.SelectedItem.ToString()));
            }

            showroom_ygopro_pics.Source = CreateImage(FormExecution.AssetsManager.GetSource("Background", cb_pics.SelectedItem.ToString()));
        }

        private void Cb_ygopro_pics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showroom_ygopro_pics.Source = CreateImage(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", cb_ygopro_pics.SelectedItem.ToString()));
        }

        private void Cb_pics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showroom_pics.Source = CreateImage(FormExecution.AssetsManager.GetSource("Background", cb_pics.SelectedItem.ToString()));
        }

        private BitmapImage CreateImage(string filepath)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(filepath);
            image.EndInit();
            return image;
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_save, btn_datasretrieval, btn_choosepics });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1ToolsButton");
                btn.Color2 = style.GetGameColor("Color2ToolsButton");
                btn.Update();
            }

            this.FontFamily = style.Font;
            this.FontSize = style.FontSize;
        }

        private void btn_save_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            client_config.Greet = (bool)cb_greet.IsChecked;
            client_config.IgnoreDuelRequest = (bool)cb_duelrequest.IsChecked;
            client_config.IgnoreCustomDuelRequest = (bool)cb_customduelrequest.IsChecked;
            client_config.IgnoreTradeRequest = (bool)cb_traderequest.IsChecked;
            client_config.Connexion_Message = (bool)cb_connectionmsg.IsChecked;
            client_config.Autoscroll = (bool)cb_autoscroll.IsChecked;
            client_config.PMPopup = (bool)cb_popuppm.IsChecked;
            client_config.AllowDeckShare = (bool)!cb_allowsharedeck.IsChecked;
            client_config.PMEndDuel = (bool)cb_popupduelend.IsChecked;
            client_config.ShowChatScrollbar = (bool)cb_showchatscrollbar.IsChecked;
            client_config.UserlistScrollbar = (bool)cb_showuserlistscrollbar.IsChecked;
            client_config.ChatBackgroundIsPic = (bool)cb_backgroundchatpic.IsChecked;
            client_config.ShowPMAll = (bool)cb_showmpall.IsChecked;

            style.Font = (FontFamily)cb_fontFamily.SelectedItem;
            if (style.Font == null)
                style.Font = new FontFamily("Arial");
            style.FontSize = Convert.ToInt32(tb_fontsize.Text);

            style.Save();
            client_config.Save();

            FormExecution.RefreshChatStyle();

            FormExecution.Client.OpenPopBox("Configurations mises à jour.", "Configurations");
            Close();
        }

        private void cb_colorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cpicker.SelectedColor = style.GetGameColor((string)cb_colorList.SelectedItem);
        }

        private void cpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            style.SetGameColor((string)cb_colorList.SelectedItem, cpicker.SelectedColor.Value);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void showroom_pics_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog getpic = new OpenFileDialog();
            getpic.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (getpic.ShowDialog() == true)
            {
                File.Delete(Path.Combine(FormExecution.path, "Assets", "Background", cb_pics.SelectedItem.ToString()));
                File.Copy(getpic.FileName, Path.Combine(FormExecution.path, "Assets", "Background", cb_pics.SelectedItem.ToString()));
            }

            showroom_pics.Source = CreateImage(FormExecution.AssetsManager.GetSource("Background", cb_pics.SelectedItem.ToString()));
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.ActivateChat();
            this.Close();
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(0, 0, 100, 100);
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
            catch { };
        }

        private void btn_datasretrieval_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.OpenDatasRetrievalWindow();
        }

        private void btn_choosepics_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => FormExecution.OpenChangePicsWindow());
        }
    }
}
