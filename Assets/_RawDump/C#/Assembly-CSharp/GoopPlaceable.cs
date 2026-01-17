// Decompiled with JetBrains decompiler
// Type: GoopPlaceable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
public class GoopPlaceable : DungeonPlaceableBehaviour, IPlaceConfigurable
{
  public GoopDefinition goop;
  [DwarfConfigurable]
  public float radius = 1f;
  private RoomHandler m_room;

  protected override void OnDestroy()
  {
    if (this.m_room != null)
      this.m_room.Entered -= new RoomHandler.OnEnteredEventHandler(this.PlayerEntered);
    base.OnDestroy();
  }

  public void ConfigureOnPlacement(RoomHandler room)
  {
    this.m_room = room;
    this.m_room.Entered += new RoomHandler.OnEnteredEventHandler(this.PlayerEntered);
  }

  public void PlayerEntered(PlayerController playerController)
  {
    DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goop).AddGoopCircle(this.transform.position.XY() + new Vector2(0.5f, 0.5f), this.radius);
    if (this.m_room == null)
      return;
    this.m_room.Entered -= new RoomHandler.OnEnteredEventHandler(this.PlayerEntered);
  }
}
