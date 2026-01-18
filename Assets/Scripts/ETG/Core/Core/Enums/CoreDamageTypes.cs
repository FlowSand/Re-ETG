using System;

#nullable disable

[Flags]
public enum CoreDamageTypes
    {
        None = 0,
        Void = 1,
        Magic = 2,
        Fire = 4,
        Ice = 8,
        Poison = 16, // 0x00000010
        Water = 32, // 0x00000020
        Electric = 64, // 0x00000040
        SpecialBossDamage = 128, // 0x00000080
    }

