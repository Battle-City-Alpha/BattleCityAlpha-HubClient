using BCA.Common;
using hub_client.Cards;
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
    /// Logique d'interaction pour Trade.xaml
    /// </summary>
    public partial class Trade : Window
    {
        private TradeAdministrator _admin;

        private int _id;
        private PlayerInfo[] _players = new PlayerInfo[2];

        public Trade(TradeAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;


            _admin.InitTrade += _admin_InitTrade;
        }

        private void _admin_InitTrade(int id, PlayerInfo[] players, Dictionary<int, BCA.Common.PlayerCard>[] Collections)
        {
            _id = id;
            _players = players;
            this.Title = _players[0].Username + " & " + _players[1].Username;
            foreach (var args in Collections[0])
                lvPlayer1.Items.Add(new TradeCard { Name = CardManager.GetCard(args.Key).Name, Quantity = args.Value.Quantity });

            foreach (var args in Collections[1])
                lvPlayer2.Items.Add(new TradeCard { Name = CardManager.GetCard(args.Key).Name, Quantity = args.Value.Quantity });
        }
    }

    public class TradeCard
    {
        public string Name;
        public int Quantity;
    }
}
