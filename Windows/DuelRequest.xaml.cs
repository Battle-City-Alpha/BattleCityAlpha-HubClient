using BCA.Common.Enums;
using hub_client.Configuration;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour DuelRequest.xaml
    /// </summary>
    public partial class DuelRequest : Window
    {
        private DuelRequestAdministrator _admin;
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        private int _id;

        private Dictionary<string, int> _banlists = new Dictionary<string, int>();

        public DuelRequest(DuelRequestAdministrator admin, int id)
        {
            InitializeComponent();
            _admin = admin;

            cb_dueltype.ItemsSource = Enum.GetValues(typeof(RoomType)).Cast<RoomType>();
            _id = id;
        }
        private void LoadStyle()
        { 
            btnSend.Color1 = style.Color1DuelRequestButton;
            btnSend.Color2 = style.Color2DuelRequestButton;
            btnSend.Update();
        }

        private void BtnSend_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_id != -1)
                _admin.SendRequest(_id, (RoomType)cb_dueltype.SelectedIndex, GetBanlistValue(cb_banlist.SelectedItem.ToString()), RoomRules.TCG, Convert.ToInt32(tb_handcard.Text), Convert.ToInt32(tb_lpstartduel.Text));
            else
                _admin.SendHost((RoomType)cb_dueltype.SelectedIndex, GetBanlistValue(cb_banlist.SelectedItem.ToString()), RoomRules.TCG, Convert.ToInt32(tb_handcard.Text), Convert.ToInt32(tb_lpstartduel.Text));

            Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();
            LoadBanlist();

            cb_banlist.SelectedIndex = 0;
            cb_dueltype.SelectedIndex = 0;
            tb_handcard.Text = "5";
            tb_lpstartduel.Text = "8000";
        }

        private void LoadBanlist()
        {
            if (!File.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "lflist.conf")))
                return;
            _banlists.Clear();
            var lines = File.ReadAllLines(Path.Combine(FormExecution.path, "BattleCityAlpha", "lflist.conf"));

            foreach (string nonTrimmerLine in lines)
            {
                string line = nonTrimmerLine.Trim();

                if (line.StartsWith("!"))
                    _banlists.Add(line.Substring(1), _banlists.Count);
            }

            cb_banlist.ItemsSource = _banlists.Keys.ToArray();
        }
        private int GetBanlistValue(string key)
        {
            return _banlists.ContainsKey(key) ? _banlists[key] : 0;
        }

        private void Cb_dueltype_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cb_dueltype.SelectedIndex == 2)
                tb_lpstartduel.Text = "16000";
            else
                tb_lpstartduel.Text = "8000";
        }
    }
}
