using UnityEngine;

#nullable disable

public class LockpicksItem : PlayerItem
  {
    public float ChanceToUnlock = 0.2f;
    public float MasterOfUnlocking_ChanceToUnlock = 0.8f;
    private bool m_isTransformed;

    public override bool CanBeUsed(PlayerController user)
    {
      if (!(bool) (Object) user || user.CurrentRoom == null)
        return false;
      if (!this.m_isTransformed && user.HasActiveBonusSynergy(CustomSynergyType.MASTER_OF_UNLOCKING))
      {
        this.m_isTransformed = true;
        this.sprite.SetSprite("lockpicks_upgrade_001");
      }
      else if (this.m_isTransformed && !user.HasActiveBonusSynergy(CustomSynergyType.MASTER_OF_UNLOCKING))
      {
        this.m_isTransformed = false;
        this.sprite.SetSprite("lockpicks_001");
      }
      IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user);
      switch (nearestInteractable)
      {
        case InteractableLock _:
        case Chest _:
        case DungeonDoorController _:
          switch (nearestInteractable)
          {
            case InteractableLock _:
              InteractableLock interactableLock = nearestInteractable as InteractableLock;
              if ((bool) (Object) interactableLock && !interactableLock.IsBusted && interactableLock.transform.position.GetAbsoluteRoom() == user.CurrentRoom && interactableLock.IsLocked && !interactableLock.HasBeenPicked && interactableLock.lockMode == InteractableLock.InteractableLockMode.NORMAL)
                return base.CanBeUsed(user);
              break;
            case DungeonDoorController _:
              DungeonDoorController dungeonDoorController = nearestInteractable as DungeonDoorController;
              if ((Object) dungeonDoorController != (Object) null && dungeonDoorController.Mode == DungeonDoorController.DungeonDoorMode.COMPLEX && dungeonDoorController.isLocked && !dungeonDoorController.lockIsBusted)
                return base.CanBeUsed(user);
              break;
            case Chest _:
              Chest chest = nearestInteractable as Chest;
              return (bool) (Object) chest && chest.GetAbsoluteParentRoom() == user.CurrentRoom && chest.IsLocked && !chest.IsLockBroken && base.CanBeUsed(user);
          }
          break;
      }
      return false;
    }

    protected override void DoEffect(PlayerController user)
    {
      base.DoEffect(user);
      float num1 = this.ChanceToUnlock;
      if ((bool) (Object) user && user.HasActiveBonusSynergy(CustomSynergyType.MASTER_OF_UNLOCKING))
        num1 = this.MasterOfUnlocking_ChanceToUnlock;
      IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.CenterPosition, 1f, user);
      switch (nearestInteractable)
      {
        case InteractableLock _:
        case Chest _:
        case DungeonDoorController _:
          switch (nearestInteractable)
          {
            case InteractableLock _:
              InteractableLock interactableLock = nearestInteractable as InteractableLock;
              if (interactableLock.lockMode != InteractableLock.InteractableLockMode.NORMAL)
                return;
              interactableLock.HasBeenPicked = true;
              if ((double) Random.value < (double) num1)
              {
                int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_lock_pick_01", this.gameObject);
                interactableLock.ForceUnlock();
                return;
              }
              int num3 = (int) AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", this.gameObject);
              interactableLock.BreakLock();
              return;
            case DungeonDoorController _:
              DungeonDoorController dungeonDoorController = nearestInteractable as DungeonDoorController;
              if (!((Object) dungeonDoorController != (Object) null) || dungeonDoorController.Mode != DungeonDoorController.DungeonDoorMode.COMPLEX || !dungeonDoorController.isLocked)
                return;
              if ((double) Random.value < (double) num1)
              {
                int num4 = (int) AkSoundEngine.PostEvent("Play_OBJ_lock_pick_01", this.gameObject);
                dungeonDoorController.Unlock();
                return;
              }
              int num5 = (int) AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", this.gameObject);
              dungeonDoorController.BreakLock();
              return;
            case Chest _:
              Chest chest = nearestInteractable as Chest;
              if (!chest.IsLocked || chest.IsLockBroken)
                return;
              if ((double) Random.value < (double) num1)
              {
                int num6 = (int) AkSoundEngine.PostEvent("Play_OBJ_lock_pick_01", this.gameObject);
                chest.ForceUnlock();
                return;
              }
              int num7 = (int) AkSoundEngine.PostEvent("Play_WPN_gun_empty_01", this.gameObject);
              chest.BreakLock();
              return;
            default:
              return;
          }
      }
    }
  }

