// Decompiled with JetBrains decompiler
// Type: Dungeonator.Subrule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Dungeonator
{
  [Serializable]
  public class Subrule
  {
    public string ruleName = "Generic";
    public RoomCreationRule.PlacementStrategy placementRule;
    public int minToSpawn = 1;
    public int maxToSpawn = 1;
    public DungeonPlaceable placeableObject;
  }
}
