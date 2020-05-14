using BCA.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.Stuff
{
    public class CollectionCardItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public CardRarity Rarity { get; set; }
    }
}
