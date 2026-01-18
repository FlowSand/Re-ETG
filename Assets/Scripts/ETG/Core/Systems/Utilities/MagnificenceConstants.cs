using UnityEngine;

#nullable disable

    public static class MagnificenceConstants
    {
        public const float COMMON_MAGNFIICENCE_ADJUSTMENT = 0.0f;
        public const float A_MAGNIFICENCE_ADJUSTMENT = 1f;
        public const float S_MAGNIFICENCE_ADJUSTMENT = 1f;

        public static PickupObject.ItemQuality ModifyQualityByMagnificence(
            PickupObject.ItemQuality targetQuality,
            float CurrentMagnificence,
            float dChance,
            float cChance,
            float bChance)
        {
            if (targetQuality != PickupObject.ItemQuality.S && targetQuality != PickupObject.ItemQuality.A)
                return targetQuality;
            float num1 = Mathf.Clamp01((float) (0.006260341964662075 + 0.99359208345413208 * (double) Mathf.Exp(-1.626339f * CurrentMagnificence)));
            float num2 = Random.value;
            if ((double) Random.value <= (double) num1)
                return targetQuality;
            float num3 = Random.value * (dChance + cChance + bChance);
            if ((double) num3 < (double) dChance)
                return PickupObject.ItemQuality.D;
            return (double) num3 < (double) dChance + (double) cChance || (double) num3 >= (double) dChance + (double) cChance + (double) bChance && (double) Random.value < 0.5 ? PickupObject.ItemQuality.C : PickupObject.ItemQuality.B;
        }
    }

