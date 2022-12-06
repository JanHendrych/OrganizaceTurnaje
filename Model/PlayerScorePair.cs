using OrganizaceTurnaje.Model;
using System.Collections.Generic;

namespace OrganizaceTurnaje.Model
{

    public class PlayerScorePair
    {
        public Player? PlayerPair { get; set; }
        public int? Score { get; set; } = 0;

        public override bool Equals(object? obj)
        {
            return obj is PlayerScorePair pair &&
                   EqualityComparer<Player?>.Default.Equals(PlayerPair, pair.PlayerPair);
        }

        public override string? ToString()
        {
            return PlayerPair.ToString() + Score;
        }
    }
}
