using BCA.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.Cards
{
    class PlayerCardComparerNewsFirst : IComparer<PlayerCard>
    {
        public int Compare(PlayerCard x, PlayerCard y)
        {
            if (!FormExecution.Client.PlayerManager.Collections.ContainsKey(x.Id) && !FormExecution.Client.PlayerManager.Collections.ContainsKey(y.Id))
                return x.Name.CompareTo(y.Name);
            if (!FormExecution.Client.PlayerManager.Collections.ContainsKey(x.Id))
                return 1; 
            if (!FormExecution.Client.PlayerManager.Collections.ContainsKey(y.Id))
                return -1;
            else return x.Name.CompareTo(y.Name);
        }
    }
}