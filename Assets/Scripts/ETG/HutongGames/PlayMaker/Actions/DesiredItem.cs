using System;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    public class DesiredItem
    {
        public GungeonFlags flagToSet;
        public DesiredItem.DetectType type;
        public int specificItemId;
        public int amount;

        public enum DetectType
        {
            SPECIFIC_ITEM,
            CURRENCY,
            META_CURRENCY,
            KEYS,
        }
    }
}
