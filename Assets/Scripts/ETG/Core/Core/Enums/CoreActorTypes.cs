using System;

#nullable disable

[Flags]
public enum CoreActorTypes
    {
        Ghost = 2,
        Robot = 4,
        Something = 8,
        Bullet = 16, // 0x00000010
    }

