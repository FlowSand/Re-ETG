using System;

#nullable disable

[Flags]
public enum dfAnchorStyle
    {
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
        All = Right | Left | Bottom | Top, // 0x0000000F
        CenterHorizontal = 64, // 0x00000040
        CenterVertical = 128, // 0x00000080
        Proportional = 256, // 0x00000100
        None = 0,
    }

