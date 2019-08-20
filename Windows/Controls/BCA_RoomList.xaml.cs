using BCA.Common;
using hub_client.Windows.Controls.Controls_Stuff;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_RoomList.xaml
    /// </summary>
    public partial class BCA_RoomList : UserControl
    {
        public Dictionary<int, RoomItem> _rooms;

        public BCA_RoomList()
        {
            InitializeComponent();
            _rooms = new Dictionary<int, RoomItem>();
        }

        public void AddItem(RoomItem item)
        {
            _rooms.Add(item.Id, item);
            Itemslist.Items.Add(item);
        }
        public void RemoveItem(int roomId)
        {
            if (_rooms.ContainsKey(roomId))
            {
                Itemslist.Items.Remove(_rooms[roomId]);
                _rooms.Remove(roomId);
            }
        }
        public RoomItem GetItem(int roomId)
        {
            if (_rooms.ContainsKey(roomId))
                return _rooms[roomId];
            return null;
        }

        public void UpdateRoom(Room room)
        {
            RoomItem item = GetItem(room.Id);

            string players = "";
            foreach (PlayerInfo info in room.Players)
                if (info != null)
                    players += info.Username + " (" + info.ELO + ") vs ";
            if (room.Players[room.Players.Length - 1] != null)
                players = players.Substring(0, players.Length - 3);
            else
                players += "???";

            RoomItem newitem = new RoomItem
            {
                Id = room.Id,
                Players = players,
                Type = room.Config.Type
            };

            if (item == null)
                AddItem(newitem);
            else
            {
                int index = Itemslist.Items.IndexOf(item);
                Itemslist.Items.Remove(item);
                Itemslist.Items.Insert(index, newitem);
                _rooms[item.Id] = newitem;
            }
        }
        public void RemoveRoom(Room room)
        {
            RoomItem item = GetItem(room.Id);
            if (item != null)
            {
                int index = Itemslist.Items.IndexOf(item);
                Itemslist.Items.Remove(item);
            }
        }
    }
}
