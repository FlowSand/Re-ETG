// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TestPickup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Linq;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Checks whether or not the player has a specific pickup (gun or item).")]
[ActionCategory(".NPCs")]
public class TestPickup : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Item to check.")]
  public FsmInt pickupId;
  [HutongGames.PlayMaker.Tooltip("The event to send if the player has the pickup.")]
  public FsmEvent success;
  [HutongGames.PlayMaker.Tooltip("The event to send if the player does not have the pickup.")]
  public FsmEvent failure;

  public override void Reset()
  {
    this.pickupId = (FsmInt) -1;
    this.success = (FsmEvent) null;
    this.failure = (FsmEvent) null;
  }

  public override string ErrorCheck()
  {
    string empty = string.Empty;
    if ((UnityEngine.Object) PickupObjectDatabase.GetById(this.pickupId.Value) == (UnityEngine.Object) null)
      empty += "Invalid item ID.\n";
    return empty;
  }

  public override void OnEnter()
  {
    TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
    if (component.TalkingPlayer.inventory.AllGuns.Any<Gun>((Func<Gun, bool>) (g => g.PickupObjectId == this.pickupId.Value)))
      this.Fsm.Event(this.success);
    else if (component.TalkingPlayer.activeItems.Any<PlayerItem>((Func<PlayerItem, bool>) (a => a.PickupObjectId == this.pickupId.Value)))
      this.Fsm.Event(this.success);
    else if (component.TalkingPlayer.passiveItems.Any<PassiveItem>((Func<PassiveItem, bool>) (p => p.PickupObjectId == this.pickupId.Value)))
      this.Fsm.Event(this.success);
    else if (component.TalkingPlayer.additionalItems.Any<PickupObject>((Func<PickupObject, bool>) (p => p.PickupObjectId == this.pickupId.Value)))
      this.Fsm.Event(this.success);
    else
      this.Fsm.Event(this.failure);
    this.Finish();
  }
}
