using System.Collections.Generic;

#nullable disable

public class SpiceItem : PlayerItem
    {
        public static float ONE_SPICE_WEIGHT = 0.1f;
        public static float TWO_SPICE_WEIGHT = 0.3f;
        public static float THREE_SPICE_WEIGHT = 0.5f;
        public static float FOUR_PLUS_SPICE_WEIGHT = 0.8f;
        public List<StatModifier> FirstTimeStatModifiers;
        public List<StatModifier> SecondTimeStatModifiers;
        public List<StatModifier> ThirdTimeStatModifiers;
        public List<StatModifier> FourthAndBeyondStatModifiers;

        public static float GetSpiceWeight(int spiceCount)
        {
            if (spiceCount <= 0)
                return 0.0f;
            if (spiceCount == 1)
                return SpiceItem.ONE_SPICE_WEIGHT;
            if (spiceCount == 2)
                return SpiceItem.TWO_SPICE_WEIGHT;
            return spiceCount == 3 ? SpiceItem.THREE_SPICE_WEIGHT : SpiceItem.FOUR_PLUS_SPICE_WEIGHT;
        }

        protected override void DoEffect(PlayerController user)
        {
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_power_up_01", this.gameObject);
            if (user.spiceCount == 0)
            {
                for (int index = 0; index < this.FirstTimeStatModifiers.Count; ++index)
                    user.ownerlessStatModifiers.Add(this.FirstTimeStatModifiers[index]);
            }
            else if (user.spiceCount == 1)
            {
                for (int index = 0; index < this.SecondTimeStatModifiers.Count; ++index)
                    user.ownerlessStatModifiers.Add(this.SecondTimeStatModifiers[index]);
            }
            else if (user.spiceCount == 2)
            {
                for (int index = 0; index < this.ThirdTimeStatModifiers.Count; ++index)
                    user.ownerlessStatModifiers.Add(this.ThirdTimeStatModifiers[index]);
            }
            else if (user.spiceCount > 2)
            {
                for (int index = 0; index < this.FourthAndBeyondStatModifiers.Count; ++index)
                    user.ownerlessStatModifiers.Add(this.FourthAndBeyondStatModifiers[index]);
            }
            user.stats.RecalculateStats(user);
            ++user.spiceCount;
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

