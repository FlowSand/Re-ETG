// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TakePickup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Linq;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Takes a pickup from the player (gun or item).")]
[ActionCategory(".NPCs")]
public class TakePickup : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Item to take.")]
  public FsmInt pickupId;
  [HutongGames.PlayMaker.Tooltip("The event to send if the player does not have the pickup.")]
  public FsmEvent failure;

  public override void Reset()
  {
    this.pickupId = (FsmInt) -1;
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
    PlayerController talkingPlayer = component.TalkingPlayer;
    if (talkingPlayer.inventory.AllGuns.Any<Gun>((Func<Gun, bool>) (g => g.PickupObjectId == this.pickupId.Value)))
    {
      Gun gun = component.TalkingPlayer.inventory.AllGuns.Find((Predicate<Gun>) (g => g.PickupObjectId == this.pickupId.Value));
      talkingPlayer.inventory.RemoveGunFromInventory(gun);
      UnityEngine.Object.Destroy((UnityEngine.Object) gun.gameObject);
    }
    else if (talkingPlayer.activeItems.Any<PlayerItem>((Func<PlayerItem, bool>) (a => a.PickupObjectId == this.pickupId.Value)))
      talkingPlayer.RemoveActiveItem(this.pickupId.Value);
    else if (talkingPlayer.passiveItems.Any<PassiveItem>((Func<PassiveItem, bool>) (p => p.PickupObjectId == this.pickupId.Value)))
      talkingPlayer.RemovePassiveItem(this.pickupId.Value);
    else
      this.Fsm.Event(this.failure);
    this.Finish();
  }
}
