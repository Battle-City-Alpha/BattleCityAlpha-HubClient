using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromServer;
using hub_client.Assets;
using hub_client.Configuration;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Profil.xaml
    /// </summary>
    public partial class Profil : Window
    {
        private ProfilAdministrator _admin;
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        AssetsManager PicsManager = new AssetsManager();

        public Profil(ProfilAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;

            _admin.UpdateProfil += _admin_UpdateProfil;
        }

        private void _admin_UpdateProfil(int avatarId, string username, int cardNumber, int level, int exp, int rankedwin, int rankedlose, int elo, PlayerRank rank, int unrankedwin, int unrankedlose, int giveup, int ragequit)
        {
            AvatarImg.Source = PicsManager.GetImage("Avatars", avatarId.ToString("D2"));
            tb_username.Text = username;
            tb_cardnumber.Text = cardNumber.ToString();
            tb_level.Text = level.ToString();
            tb_experience.Text = exp.ToString();
            tb_rankedwin.Text = rankedwin.ToString();
            tb_rankedlose.Text = rankedlose.ToString();
            tb_elo.Text = elo.ToString();
            tb_rank.Text = rank.ToString();
            tb_unrankedwin.Text = unrankedwin.ToString();
            tb_unrankedlose.Text = unrankedlose.ToString();
            tb_ragequit.Text = ragequit.ToString();
            tb_giveup.Text = giveup.ToString();
        }
    }
}
