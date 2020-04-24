using hub_client.Configuration;
using hub_client.Helpers;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour SoloMode.xaml
    /// </summary>
    public partial class SoloMode : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        public SoloMode()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            LoadStyle();

            this.MouseDown += Window_MouseDown;
            this.Loaded += SoloMode_Loaded;
        }

        private void SoloMode_Loaded(object sender, RoutedEventArgs e)
        {
            cb_AI_decks.Items.Clear();
            cb_AI_decks.Items.Add("Random");
            List<string> Deck = new List<string>(Directory.EnumerateFiles(System.IO.Path.Combine(FormExecution.path, "BattleCityAlpha", "Kaibot", "Decks")));
            foreach (string deck in Deck)
            {
                string[] name = deck.Split('\\');
                string[] nomFinal = name[name.Length - 1].Split('.');
                cb_AI_decks.Items.Add(nomFinal[0].Substring(3));
            }
            cb_AI_decks.SelectedIndex = 0;
        }

        public void LoadStyle()
        {
            btn_duel.Color1 = style.GetGameColor("Color1SoloModeButton");
            btn_duel.Color2 = style.GetGameColor("Color2SoloModeButton");
            btn_duel.Update();

            this.FontFamily = style.Font;
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
                this.bg_border.CornerRadius = new CornerRadius(10, 400, 400, 400);
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

        private void btn_duel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            YgoProHelper.LaunchGameAgainstBot(cb_AI_decks.SelectedIndex == 0 ? "" : cb_AI_decks.Text);
        }
    }
}
