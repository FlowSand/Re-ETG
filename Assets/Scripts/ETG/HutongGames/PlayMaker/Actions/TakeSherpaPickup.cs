// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TakeSherpaPickup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Linq;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  public class TakeSherpaPickup : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The event to send if the player does not have the pickup.")]
    public FsmEvent failure;
    public FsmInt numToTake = (FsmInt) 1;
    [NonSerialized]
    public SherpaDetectItem parentAction;

    protected bool TakeAwayPickup(PlayerController player, int pickupId)
    {
      if (player.inventory.AllGuns.Any<Gun>((Func<Gun, bool>) (g => g.PickupObjectId == pickupId)))
      {
        Gun gun = player.inventory.AllGuns.Find((Predicate<Gun>) (g => g.PickupObjectId == pickupId));
        player.inventory.RemoveGunFromInventory(gun);
        UnityEngine.Object.Destroy((UnityEngine.Object) gun.gameObject);
      }
      else if (player.activeItems.Any<PlayerItem>((Func<PlayerItem, bool>) (a => a.PickupObjectId == pickupId)))
      {
        player.RemoveActiveItem(pickupId);
      }
      else
      {
        if (!player.passiveItems.Any<PassiveItem>((Func<PassiveItem, bool>) (p => p.PickupObjectId == pickupId)))
          return false;
        if (this.numToTake.Value > 1)
        {
          for (int index = 0; index < this.numToTake.Value; ++index)
            player.RemovePassiveItem(pickupId);
        }
        else
          player.RemovePassiveItem(pickupId);
      }
      return true;
    }

    public override void OnEnter()
    {
      PlayerController talkingPlayer = this.Owner.GetComponent<TalkDoerLite>().TalkingPlayer;
      if (this.parentAction == null)
      {
        for (int index = 0; index < this.Fsm.PreviousActiveState.Actions.Length; ++index)
        {
          if (this.Fsm.PreviousActiveState.Actions[index] is SherpaDetectItem)
          {
            this.parentAction = this.Fsm.PreviousActiveState.Actions[index] as SherpaDetectItem;
            break;
          }
        }
      }
      PrepareTakeSherpaPickup takeSherpaPickup = (PrepareTakeSherpaPickup) null;
      for (int index = 0; index < this.Fsm.ActiveState.Actions.Length; ++index)
      {
        if (this.Fsm.ActiveState.Actions[index] is PrepareTakeSherpaPickup)
        {
          takeSherpaPickup = this.Fsm.ActiveState.Actions[index] as PrepareTakeSherpaPickup;
          break;
        }
      }
      PickupObject allValidTarget = this.parentAction.AllValidTargets[takeSherpaPickup.CurrentPickupTargetIndex];
      if (!this.TakeAwayPickup(talkingPlayer, allValidTarget.PickupObjectId))
        this.Fsm.Event(this.failure);
      else
        this.parentAction = (SherpaDetectItem) null;
      this.Finish();
    }
  }
}
