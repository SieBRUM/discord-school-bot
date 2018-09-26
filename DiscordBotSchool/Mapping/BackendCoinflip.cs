using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotSchool.Mapping
{
    public class BackendCoinflip
    {
        public int Id;
        public int ChallengerId;
        public int EnemyId;
        public long Points;
        public int Side;

        public BackendUser Challenger;
        public BackendUser Enemy;
        public CoinflipVsResults Result;
    }
}
