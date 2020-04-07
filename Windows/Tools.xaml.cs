using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Assets;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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

            this.Loaded += Tools_Loaded;

            _admin = admin;
        }

        private void Tools_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();

            cb_connectionmsg.IsChecked = client_config.Connexion_Message;
            cb_greet.IsChecked = client_config.Greet;
            cb_traderequest.IsChecked = client_config.Request;
            cb_duelrequest.IsChecked = client_config.Trade;

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
            cb_pics.SelectedIndex = 0;
            cb_pics.SelectionChanged += Cb_pics_SelectionChanged;
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
            Buttons.AddRange(new[] { btn_save});

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
            client_config.Request = (bool)cb_duelrequest.IsChecked;
            client_config.Trade = (bool)cb_traderequest.IsChecked;
            client_config.Connexion_Message = (bool)cb_connectionmsg.IsChecked;

            style.Font = (FontFamily)cb_fontFamily.SelectedItem;
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
    }
}
