using BCA.Common;
using BCA.Common.Enums;
using hub_client.Windows.Controls.Controls_Stuff;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_RoomList.xaml
    /// </summary>
    public partial class BCA_RoomList : UserControl
    {
        private Dictionary<int, RoomItem> _rooms;

        public BCA_RoomList()
        {
            InitializeComponent();
            _rooms = new Dictionary<int, RoomItem>();
        }

        public void RefreshStyle()
        {
            this.FontFamily = FormExecution.AppDesignConfig.Font;
        }

        public void Clear()
        {
            Itemslist.Items.Clear();
            _rooms.Clear();
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
            if (room.Players[0] == null && room.Players[1] == null && room.Players[2] == null && room.Players[3] == null)
                return;


            RoomItem item = GetItem(room.Id);

            string players1 = "";
            string players2 = "";
            if (room.GetRoomType() != (int)RoomType.Tag)
            {
                players1 = room.Players[0] != null ? room.Players[0].Username : "???";
                players2 = room.Players[1] != null ? room.Players[1].Username : "???";
            }
            else
            {
                players1 = room.Players[0] != null ? room.Players[0].Username : "???";
                players1 += " & " + room.Players[1] != null ? room.Players[1].Username : "???";

                players2 = room.Players[2] != null ? room.Players[2].Username : "???";
                players2 += " & " + room.Players[3] != null ? room.Players[3].Username : "???";
            }
            
            RoomItem newitem = new RoomItem
            {
                Id = room.Id,
                Players1 = players1,
                Players2 = players2,
                Type = room.Config.Type,
                Config = room.Config,
                NeedPassword = room.NeedPassword,
                IsRanked = room.IsRanked()
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
