// Decompiled with JetBrains decompiler
// Type: Dungeonator.RoomCreationStrategy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Dungeonator
{
  [Serializable]
  public class RoomCreationStrategy
  {
    public int minAreaSize;
    public int maxAreaSize = 2;
    public RoomCreationStrategy.RoomType roomType;
    public List<RoomCreationRule> rules;

    public RoomCreationStrategy() => this.rules = new List<RoomCreationRule>();

    public enum RoomType
    {
      SMOOTH_RECTILINEAR_ROOM,
      JAGGED_RECTILINEAR_ROOM,
      CIRCULAR_ROOM,
      SMOOTH_ANNEX,
      JAGGED_ANNEX,
      CAVE_ROOM,
      PREDEFINED_ROOM,
    }
  }
}
