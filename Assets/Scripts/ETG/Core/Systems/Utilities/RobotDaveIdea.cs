// Decompiled with JetBrains decompiler
// Type: RobotDaveIdea
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;

#nullable disable

[Serializable]
public class RobotDaveIdea
  {
    public DungeonPlaceable[] ValidEasyEnemyPlaceables;
    public DungeonPlaceable[] ValidHardEnemyPlaceables;
    public bool UseWallSawblades;
    public bool UseRollingLogsVertical;
    public bool UseRollingLogsHorizontal;
    public bool UseFloorPitTraps;
    public bool UseFloorFlameTraps;
    public bool UseFloorSpikeTraps;
    public bool UseFloorConveyorBelts;
    public bool UseCaveIns;
    public bool UseAlarmMushrooms;
    public bool UseMineCarts;
    public bool UseChandeliers;
    public bool CanIncludePits = true;

    public RobotDaveIdea()
    {
    }

    public RobotDaveIdea(RobotDaveIdea source)
    {
      if (source == null)
        return;
      this.ValidEasyEnemyPlaceables = new DungeonPlaceable[source.ValidEasyEnemyPlaceables == null ? 0 : source.ValidEasyEnemyPlaceables.Length];
      for (int index = 0; index < this.ValidEasyEnemyPlaceables.Length; ++index)
        this.ValidEasyEnemyPlaceables[index] = source.ValidEasyEnemyPlaceables[index];
      this.ValidHardEnemyPlaceables = new DungeonPlaceable[source.ValidHardEnemyPlaceables == null ? 0 : source.ValidHardEnemyPlaceables.Length];
      for (int index = 0; index < this.ValidHardEnemyPlaceables.Length; ++index)
        this.ValidHardEnemyPlaceables[index] = source.ValidHardEnemyPlaceables[index];
      this.UseWallSawblades = source.UseWallSawblades;
      this.UseRollingLogsHorizontal = source.UseRollingLogsHorizontal;
      this.UseRollingLogsVertical = source.UseRollingLogsVertical;
      this.UseFloorPitTraps = source.UseFloorPitTraps;
      this.UseFloorFlameTraps = source.UseFloorFlameTraps;
      this.UseFloorSpikeTraps = source.UseFloorSpikeTraps;
      this.UseFloorConveyorBelts = source.UseFloorConveyorBelts;
      this.UseCaveIns = source.UseCaveIns;
      this.UseAlarmMushrooms = source.UseAlarmMushrooms;
      this.UseMineCarts = source.UseMineCarts;
      this.UseChandeliers = source.UseChandeliers;
      this.CanIncludePits = source.CanIncludePits;
    }
  }

