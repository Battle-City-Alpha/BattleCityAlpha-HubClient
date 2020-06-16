using BCA.Common;
using hub_client.Configuration;
using hub_client.WindowsAdministrator;
using System;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour DuelResult.xaml
    /// </summary>
    public partial class DuelResult : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        private DuelResultAdministrator _admin;

        private RoomConfig _config;
        private int _opponent;
        private int _roomID;

        public DuelResult(DuelResultAdministrator admin, int bp, int exp, bool win, int opponent, RoomConfig config, int roomID)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _admin = admin;
            _admin.RevengeAnswer += _admin_RevengeAnswer;

            string text;
            if (win)
                text = "Félicitations pour ta victoire !";
            else
                text = "Dommage tu viens de perdre... Tu feras mieux la prochaine fois !";
            text += Environment.NewLine + Environment.NewLine;

            text += "Tu as remporté " + bp.ToString() + " BPs et " + exp.ToString() + " points d'expériences.";

            popText.Text = text;

            _config = config;
            _opponent = opponent;
            _roomID = roomID;

            this.Loaded += DuelResult_Loaded;

            btnRevenge.MouseLeftButtonDown += BtnRevenge_MouseLeftButtonDown;
        }

        private void _admin_RevengeAnswer(bool result)
        {
            if (result)
                popText.Text = popText.Text + Environment.NewLine + " Votre adversaire a accepté une revanche !";
            else
                btnRevenge.Visibility = Visibility.Hidden;
        }

        private void BtnRevenge_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.SendDuelResultAnswer(true, _config, _opponent, _roomID);
            _admin.RevengeAnswer -= _admin_RevengeAnswer;
            Close();
        }

        private void DuelResult_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();
        }

        private void btnAgree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _admin.RevengeAnswer -= _admin_RevengeAnswer;
            _admin.SendDuelResultAnswer(false, _config, _opponent, _roomID);
            Close();
        }
        private void LoadStyle()
        {
            btnAgree.Color1 = style.GetGameColor("Color1DuelRequestButton");
            btnAgree.Color2 = style.GetGameColor("Color2DuelRequestButton");
            btnAgree.Update();
            btnRevenge.Color1 = style.GetGameColor("Color1DuelRequestButton");
            btnRevenge.Color2 = style.GetGameColor("Color2DuelRequestButton");
            btnRevenge.Update();

            this.FontFamily = style.Font;
        }
    }
}
