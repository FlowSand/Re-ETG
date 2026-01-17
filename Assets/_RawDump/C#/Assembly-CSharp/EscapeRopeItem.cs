// Decompiled with JetBrains decompiler
// Type: EscapeRopeItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
public class EscapeRopeItem : PlayerItem
{
  public override bool CanBeUsed(PlayerController user)
  {
    if (!(bool) (Object) user || user.CurrentRoom == null || user.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS && GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON || user.CurrentRoom.CompletelyPreventLeaving || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.RATGEON)
      return false;
    return !user.CurrentRoom.IsWildWestEntrance || true;
  }

  protected override void DoEffect(PlayerController user)
  {
    if (user.CurrentRoom.CompletelyPreventLeaving)
      return;
    if (user.IsInMinecart)
      user.currentMineCart.EvacuateSpecificPlayer(user, true);
    int num = (int) AkSoundEngine.PostEvent("Play_OBJ_rope_escape_01", this.gameObject);
    if (user.CurrentRoom.IsWildWestEntrance)
      return;
    RoomHandler targetRoom = (RoomHandler) null;
    BaseShopController[] componentsInChildren = GameManager.Instance.Dungeon.data.Entrance.hierarchyParent.parent.GetComponentsInChildren<BaseShopController>(true);
    if (componentsInChildren != null && componentsInChildren.Length > 0)
      targetRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(componentsInChildren[0].transform.position.IntXY());
    user.EscapeRoom(PlayerController.EscapeSealedRoomStyle.ESCAPE_SPIN, true, targetRoom);
  }

  protected override void OnDestroy() => base.OnDestroy();
}
