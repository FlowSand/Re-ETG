using System;
using System.Collections.Generic;

#nullable disable
namespace Dungeonator
{
    [Serializable]
    public class RoomCreationRule
    {
        public float percentChance;
        public List<Subrule> subrules;

        public RoomCreationRule() => this.subrules = new List<Subrule>();

        public enum PlacementStrategy
        {
            CENTERPIECE,
            CORNERS,
            WALLS,
            BACK_WALL,
            RANDOM_CENTER,
            RANDOM,
        }
    }
}
