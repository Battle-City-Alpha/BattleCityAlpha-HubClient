using hub_client.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Logique d'interaction pour SoloMode.xaml
    /// </summary>
    public partial class SoloMode : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        private const int _defaultPort = 1111;
        private const string _defaultHost = "127.0.0.1";
        public SoloMode()
        {
            InitializeComponent();

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
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btn_duel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.Client_LaunchYGOPro(String.Format("-h {0} -p {1} -c", _defaultHost, _defaultPort));
            Thread.Sleep(1000);
            LaunchWindbot(cb_AI_decks.SelectedIndex == 0 ? "" : cb_AI_decks.Text);
        }

        private void LaunchWindbot(string deck, string host = _defaultHost, int port = _defaultPort, int version = 0x1340, string dialog = "fr-FR", string name = "Kaibot")
        {
            string info = String.Format("Name={0}  Deck={1} Dialog={2} Host={3} Port={4} Version={5}", name, deck, dialog, host, port, version);
            
            logger.Trace("Windbot start with : " + info);

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = Path.Combine(FormExecution.path, "BattleCityAlpha", "Kaibot", "WindBot.exe");
            p.StartInfo.WorkingDirectory = Path.Combine(FormExecution.path, "BattleCityAlpha", "Kaibot");
            p.StartInfo.Arguments = info;
            p.Start();
        }
    }
}
