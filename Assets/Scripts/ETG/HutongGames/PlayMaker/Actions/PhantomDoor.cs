// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.PhantomDoor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Triggers phantom door events.")]
[ActionCategory(".NPCs")]
public class PhantomDoor : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Seals the room the Owner is in.")]
  public FsmBool seal;

  public override void Reset() => this.seal = (FsmBool) false;

  public override void OnEnter()
  {
    TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
    component.specRigidbody.Initialize();
    RoomHandler roomFromPosition = GameManager.Instance.Dungeon.GetRoomFromPosition(component.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
    if (this.seal.Value)
      BraveUtility.GetClosestToPosition<DungeonDoorSubsidiaryBlocker>(new List<DungeonDoorSubsidiaryBlocker>((IEnumerable<DungeonDoorSubsidiaryBlocker>) Object.FindObjectsOfType<DungeonDoorSubsidiaryBlocker>()), roomFromPosition.area.Center).Seal();
    this.Finish();
  }
}
