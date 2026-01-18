using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  public class SherpaDetectItem : FsmStateAction
  {
    public SherpaDetectItem.DetectType detectType;
    [HutongGames.PlayMaker.Tooltip("Specific item id to check for.")]
    public FsmInt pickupId;
    public FsmInt numToTake = (FsmInt) 1;
    [HutongGames.PlayMaker.Tooltip("The event to send if the preceeding tests all pass.")]
    public FsmEvent SuccessEvent;
    [HutongGames.PlayMaker.Tooltip("The event to send if the preceeding tests all fail.")]
    public FsmEvent FailEvent;
    [NonSerialized]
    public List<PickupObject> AllValidTargets;
    private PlayerController talkingPlayer;

    public override void Reset()
    {
    }

    public override string ErrorCheck() => string.Empty;

    public override void OnEnter()
    {
      this.talkingPlayer = this.Owner.GetComponent<TalkDoerLite>().TalkingPlayer;
      this.DoCheck();
      this.Finish();
    }

    private bool CheckPlayerForItem(PickupObject targetItem, List<PickupObject> targets)
    {
      bool flag = false;
      switch (targetItem)
      {
        case Gun _:
          for (int index = 0; index < this.talkingPlayer.inventory.AllGuns.Count; ++index)
          {
            if (this.talkingPlayer.inventory.AllGuns[index].PickupObjectId == targetItem.PickupObjectId)
            {
              flag = true;
              targets.Add((PickupObject) this.talkingPlayer.inventory.AllGuns[index]);
            }
          }
          break;
        case PlayerItem _:
          for (int index = 0; index < this.talkingPlayer.activeItems.Count; ++index)
          {
            if (this.talkingPlayer.activeItems[index].PickupObjectId == targetItem.PickupObjectId)
            {
              flag = true;
              targets.Add((PickupObject) this.talkingPlayer.activeItems[index]);
            }
          }
          break;
        case PassiveItem _:
          for (int index = 0; index < this.talkingPlayer.passiveItems.Count; ++index)
          {
            if (this.talkingPlayer.passiveItems[index].PickupObjectId == targetItem.PickupObjectId)
            {
              flag = true;
              targets.Add((PickupObject) this.talkingPlayer.passiveItems[index]);
            }
          }
          break;
      }
      if (this.numToTake.Value > 1 && this.numToTake.Value > targets.Count)
        flag = false;
      return flag;
    }

    private bool FindFlight(List<PickupObject> fliers)
    {
      for (int index = 0; index < this.talkingPlayer.activeItems.Count; ++index)
      {
        PlayerItem activeItem = this.talkingPlayer.activeItems[index];
        bool flag = false;
        if (activeItem is JetpackItem)
          flag = true;
        if (flag)
          fliers.Add((PickupObject) activeItem);
      }
      for (int index = 0; index < this.talkingPlayer.passiveItems.Count; ++index)
      {
        PassiveItem passiveItem = this.talkingPlayer.passiveItems[index];
        bool flag = false;
        if (passiveItem is WingsItem)
          flag = true;
        if (flag)
          fliers.Add((PickupObject) passiveItem);
      }
      return fliers.Count > 0;
    }

    private bool FindGoopers(List<PickupObject> goopers)
    {
      for (int index1 = 0; index1 < this.talkingPlayer.inventory.AllGuns.Count; ++index1)
      {
        Gun allGun = this.talkingPlayer.inventory.AllGuns[index1];
        bool flag = false;
        for (int index2 = 0; index2 < allGun.Volley.projectiles.Count; ++index2)
        {
          ProjectileModule projectile = allGun.Volley.projectiles[index2];
          for (int index3 = 0; index3 < projectile.projectiles.Count; ++index3)
          {
            if ((UnityEngine.Object) projectile.projectiles[index3].GetComponent<GoopModifier>() != (UnityEngine.Object) null)
            {
              flag = true;
              break;
            }
          }
          if (flag)
            break;
        }
        if (flag)
          goopers.Add((PickupObject) allGun);
      }
      for (int index = 0; index < this.talkingPlayer.activeItems.Count; ++index)
      {
        PlayerItem activeItem = this.talkingPlayer.activeItems[index];
        bool flag = false;
        if (activeItem is SpawnObjectPlayerItem && (bool) (UnityEngine.Object) (activeItem as SpawnObjectPlayerItem).objectToSpawn.GetComponent<ThrownGoopItem>())
          flag = true;
        if (flag)
          goopers.Add((PickupObject) activeItem);
      }
      for (int index = 0; index < this.talkingPlayer.passiveItems.Count; ++index)
      {
        PassiveItem passiveItem = this.talkingPlayer.passiveItems[index];
        bool flag = false;
        if (passiveItem is PassiveGooperItem)
          flag = true;
        if (flag)
          goopers.Add((PickupObject) passiveItem);
      }
      return goopers.Count > 0;
    }

    private bool FindExplosives(List<PickupObject> explosives)
    {
      for (int index1 = 0; index1 < this.talkingPlayer.inventory.AllGuns.Count; ++index1)
      {
        Gun allGun = this.talkingPlayer.inventory.AllGuns[index1];
        bool flag = false;
        for (int index2 = 0; index2 < allGun.Volley.projectiles.Count; ++index2)
        {
          ProjectileModule projectile = allGun.Volley.projectiles[index2];
          for (int index3 = 0; index3 < projectile.projectiles.Count; ++index3)
          {
            if ((UnityEngine.Object) projectile.projectiles[index3].GetComponent<ExplosiveModifier>() != (UnityEngine.Object) null)
            {
              flag = true;
              break;
            }
          }
          if (flag)
            break;
        }
        if (flag)
          explosives.Add((PickupObject) allGun);
      }
      for (int index = 0; index < this.talkingPlayer.activeItems.Count; ++index)
      {
        PlayerItem activeItem = this.talkingPlayer.activeItems[index];
        bool flag = false;
        if (activeItem is SpawnObjectPlayerItem || activeItem is RemoteMineItem)
        {
          GameObject gameObject = !(activeItem is SpawnObjectPlayerItem) ? (activeItem as RemoteMineItem).objectToSpawn : (activeItem as SpawnObjectPlayerItem).objectToSpawn;
          if ((bool) (UnityEngine.Object) gameObject.GetComponent<RemoteMineController>() || (bool) (UnityEngine.Object) gameObject.GetComponent<ProximityMine>())
            flag = true;
        }
        if (flag)
          explosives.Add((PickupObject) activeItem);
      }
      return explosives.Count > 0;
    }

    private void DoCheck()
    {
      bool flag = false;
      this.AllValidTargets = new List<PickupObject>();
      switch (this.detectType)
      {
        case SherpaDetectItem.DetectType.SPECIFIC_ITEM:
          flag = this.CheckPlayerForItem(PickupObjectDatabase.Instance.InternalGetById(this.pickupId.Value), this.AllValidTargets);
          break;
        case SherpaDetectItem.DetectType.SOMETHING_EXPLOSIVE:
          flag = this.FindExplosives(this.AllValidTargets);
          break;
        case SherpaDetectItem.DetectType.SOMETHING_GOOPY:
          flag = this.FindGoopers(this.AllValidTargets);
          break;
        case SherpaDetectItem.DetectType.SOMETHING_FLYING:
          flag = this.FindFlight(this.AllValidTargets);
          break;
      }
      if (flag)
      {
        this.Fsm.Event(this.SuccessEvent);
        this.Finish();
      }
      else
      {
        this.Fsm.Event(this.FailEvent);
        this.Finish();
      }
    }

    public enum DetectType
    {
      SPECIFIC_ITEM,
      SOMETHING_EXPLOSIVE,
      SOMETHING_GOOPY,
      SOMETHING_FLYING,
    }
  }
}
