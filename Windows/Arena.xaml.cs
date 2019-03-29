using BCA.Common;
using hub_client.Configuration;
using hub_client.Windows.Controls;
using hub_client.Windows.Controls.Controls_Stuff;
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
    /// Logique d'interaction pour Arena.xaml
    /// </summary>
    public partial class Arena : Window
    {
        ArenaAdministrator _admin;
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        public Arena(ArenaAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;
            Title = "Arène de duel";

            LoadStyle();

            RoomItem item = new RoomItem
            {
                Id = 1,
                Players = "Tic-Tac-Toc vs ",
                Type = BCA.Common.Enums.DuelType.Single
            };
            RoomItem item2 = new RoomItem
            {
                Id = 2,
                Players = "Tic-Tac-Toc vs ",
                Type = BCA.Common.Enums.DuelType.Match
            };
            RoomItem item3 = new RoomItem
            {
                Id = 3,
                Players = "Tic-Tac-Toc vs ",
                Type = BCA.Common.Enums.DuelType.Tag
            };
            unrankedList.AddItem(item);
            unrankedList.AddItem(item2);
            unrankedList.AddItem(item3);

            _admin.UpdateRoom += UpdateRoom;
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> RankedButtons = new List<BCA_ColorButton>();
            RankedButtons.AddRange(new[] { btn_playranked, btn_ranking });

            foreach (BCA_ColorButton btn in RankedButtons)
            {
                btn.Color1 = style.Color1ArenaRankedButton;
                btn.Color2 = style.Color2ArenaRankedButton;
                btn.Update();
            }

            btn_single.Color1 = style.Color1ArenaUnrankedSingleButton;
            btn_single.Color2 = style.Color2ArenaUnrankedSingleButton;
            btn_single.Update();

            btn_match.Color1 = style.Color1ArenaUnrankedMatchButton;
            btn_match.Color2 = style.Color2ArenaUnrankedMatchButton;
            btn_match.Update();

            btn_tag.Color1 = style.Color1ArenaUnrankedTagButton;
            btn_tag.Color2 = style.Color2ArenaUnrankedTagButton;
            btn_tag.Update();
        }

        public void UpdateRoom(Room room)
        {
            if (room.IsRanked())
                rankedList.UpdateRoom(room);
            else
                unrankedList.UpdateRoom(room);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.UpdateRoom -= UpdateRoom;
        }
    }
}
