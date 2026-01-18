using System;
using System.Linq;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    public class TakeBlacksmithPickup : FsmStateAction
    {
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
                player.RemovePassiveItem(pickupId);
            }
            return true;
        }

        public override void OnEnter()
        {
            PlayerController talkingPlayer = this.Owner.GetComponent<TalkDoerLite>().TalkingPlayer;
            BlacksmithDetectItem blacksmithDetectItem = (BlacksmithDetectItem) null;
            for (int index = 0; index < this.Fsm.PreviousActiveState.Actions.Length; ++index)
            {
                if (this.Fsm.PreviousActiveState.Actions[index] is BlacksmithDetectItem)
                {
                    blacksmithDetectItem = this.Fsm.PreviousActiveState.Actions[index] as BlacksmithDetectItem;
                    break;
                }
            }
            PickupObject targetPickupObject = blacksmithDetectItem.GetTargetPickupObject();
            DesiredItem currentDesire = blacksmithDetectItem.GetCurrentDesire();
            bool flag = false;
            if (currentDesire.type == DesiredItem.DetectType.SPECIFIC_ITEM)
                flag = this.TakeAwayPickup(talkingPlayer, targetPickupObject.PickupObjectId);
            else if (currentDesire.type == DesiredItem.DetectType.CURRENCY)
            {
                talkingPlayer.carriedConsumables.Currency -= currentDesire.amount;
                flag = true;
            }
            else if (currentDesire.type == DesiredItem.DetectType.META_CURRENCY)
            {
                int num = Mathf.RoundToInt(GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.META_CURRENCY));
                GameStatsManager.Instance.ClearStatValueGlobal(TrackedStats.META_CURRENCY);
                GameStatsManager.Instance.SetStat(TrackedStats.META_CURRENCY, (float) (num - currentDesire.amount));
                flag = true;
            }
            else if (currentDesire.type == DesiredItem.DetectType.KEYS)
            {
                talkingPlayer.carriedConsumables.KeyBullets -= currentDesire.amount;
                flag = true;
            }
            if (flag)
                GameStatsManager.Instance.SetFlag(currentDesire.flagToSet, true);
            this.Finish();
        }
    }
}
