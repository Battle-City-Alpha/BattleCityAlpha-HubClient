using BCA.Common;
using System;

namespace hub_client.Stuff
{
    public class RankingTeamItem : RankingTeamInfos
    {
        public double WinRate => Math.Round(((double)Wins) / (Loses + Wins), 2);
    }
}
