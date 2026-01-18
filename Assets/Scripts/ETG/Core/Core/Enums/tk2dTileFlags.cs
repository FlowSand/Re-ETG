using System;

#nullable disable

[Flags]
public enum tk2dTileFlags
  {
    None = 0,
    FlipX = 16777216, // 0x01000000
    FlipY = 33554432, // 0x02000000
    Rot90 = 67108864, // 0x04000000
  }

