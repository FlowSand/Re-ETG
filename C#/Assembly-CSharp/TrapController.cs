// Decompiled with JetBrains decompiler
// Type: TrapController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
public class TrapController : DungeonPlaceableBehaviour
{
  public string TrapSwitchState;
  protected bool m_markCellOccupied = true;

  public virtual void Start()
  {
    if (string.IsNullOrEmpty(this.TrapSwitchState))
      return;
    int num = (int) AkSoundEngine.SetSwitch("ENV_Trap", this.TrapSwitchState, this.gameObject);
  }

  protected override void OnDestroy() => base.OnDestroy();

  public override GameObject InstantiateObject(
    RoomHandler targetRoom,
    IntVector2 loc,
    bool deferConfiguration = false)
  {
    IntVector2 intVector2 = loc + targetRoom.area.basePosition;
    for (int x = intVector2.x; x < intVector2.x + this.placeableWidth; ++x)
    {
      for (int y = intVector2.y; y < intVector2.y + this.placeableHeight; ++y)
      {
        if (this.m_markCellOccupied)
          GameManager.Instance.Dungeon.data.cellData[x][y].isOccupied = true;
        GameManager.Instance.Dungeon.data.cellData[x][y].containsTrap = true;
      }
    }
    return base.InstantiateObject(targetRoom, loc, deferConfiguration);
  }
}
