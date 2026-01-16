// Decompiled with JetBrains decompiler
// Type: PrototypeRectangularFeature
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
[Serializable]
public struct PrototypeRectangularFeature
{
  public IntVector2 basePosition;
  public IntVector2 dimensions;

  public static PrototypeRectangularFeature CreateMirror(
    PrototypeRectangularFeature source,
    IntVector2 roomDimensions)
  {
    PrototypeRectangularFeature mirror = new PrototypeRectangularFeature()
    {
      dimensions = source.dimensions,
      basePosition = source.basePosition
    };
    mirror.basePosition.x = roomDimensions.x - (mirror.basePosition.x + mirror.dimensions.x);
    return mirror;
  }
}
