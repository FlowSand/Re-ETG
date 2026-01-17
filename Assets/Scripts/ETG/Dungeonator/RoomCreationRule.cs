// Decompiled with JetBrains decompiler
// Type: Dungeonator.RoomCreationRule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
