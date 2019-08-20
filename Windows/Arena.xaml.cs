using BCA.Common;
using BCA.Common.Enums;
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

            _admin.UpdateRoom += UpdateRoom;

            singleList.Itemslist.MouseDoubleClick += Room_MouseDoubleClick; 
        }

        private void Room_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RoomItem room = ((ListBox)sender).SelectedItem as RoomItem;
            if (room != null)
                _admin.SendJoinRoom(room.Id, room.Type);
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> RankedButtons = new List<BCA_ColorButton>();
            RankedButtons.AddRange(new[] { btn_playranked, btn_ranking, btn_host });

            foreach (BCA_ColorButton btn in RankedButtons)
            {
                btn.Color1 = style.Color1ArenaRankedButton;
                btn.Color2 = style.Color2ArenaRankedButton;
                btn.Update();
            }
        }

        public void UpdateRoom(Room room, bool remove)
        {
            if (!remove)
            {
                switch(room.Config.Type)
                {
                    case RoomType.Single:
                        singleList.UpdateRoom(room);
                        break;
                    case RoomType.Match:
                        matchList.UpdateRoom(room);
                        break;
                    case RoomType.Tag:
                        tagList.UpdateRoom(room);
                        break;
                }
            }
            else
            {
                switch (room.Config.Type)
                {
                    case RoomType.Single:
                        singleList.RemoveRoom(room);
                        break;
                    case RoomType.Match:
                        matchList.RemoveRoom(room);
                        break;
                    case RoomType.Tag:
                        tagList.RemoveRoom(room);
                        break;
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.UpdateRoom -= UpdateRoom;
        }

        private void Btn_host_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.OpenDuelRequest(-1);
        }
    }
}
