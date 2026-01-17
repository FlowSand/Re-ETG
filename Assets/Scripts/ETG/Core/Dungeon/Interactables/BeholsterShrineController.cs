// Decompiled with JetBrains decompiler
// Type: BeholsterShrineController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class BeholsterShrineController : 
      DungeonPlaceableBehaviour,
      IPlayerInteractable,
      IPlaceConfigurable
    {
      public string displayTextKey;
      public string acceptOptionKey;
      public string declineOptionKey;
      public string spentOptionKey = "#SHRINE_GENERIC_SPENT";
      public Transform talkPoint;
      public tk2dSprite AlternativeOutlineTarget;
      [PickupIdentifier]
      public int Gun01ID;
      [PickupIdentifier]
      public int Gun02ID;
      [PickupIdentifier]
      public int Gun03ID;
      [PickupIdentifier]
      public int Gun04ID;
      [PickupIdentifier]
      public int Gun05ID;
      [PickupIdentifier]
      public int Gun06ID;
      public tk2dSprite Gun01Sprite;
      public tk2dSprite Gun02Sprite;
      public tk2dSprite Gun03Sprite;
      public tk2dSprite Gun04Sprite;
      public tk2dSprite Gun05Sprite;
      public tk2dSprite Gun06Sprite;
      public GameObject VFXStonePuff;
      private RoomHandler m_room;
      private int m_useCount;

      public void ConfigureOnPlacement(RoomHandler room)
      {
        this.m_room = room;
        this.m_room.OptionalDoorTopDecorable = ResourceCache.Acquire("Global Prefabs/Shrine_Lantern") as GameObject;
        this.UpdateSpriteVisibility();
      }

      private void UpdateSpriteVisibility()
      {
        this.UpdateSingleSpriteVisibility(this.Gun01Sprite, GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_01));
        this.UpdateSingleSpriteVisibility(this.Gun02Sprite, GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_02));
        this.UpdateSingleSpriteVisibility(this.Gun03Sprite, GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_03));
        this.UpdateSingleSpriteVisibility(this.Gun04Sprite, GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_04));
        this.UpdateSingleSpriteVisibility(this.Gun05Sprite, GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_05));
        this.UpdateSingleSpriteVisibility(this.Gun06Sprite, GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_06));
      }

      private void UpdateSingleSpriteVisibility(tk2dSprite gunSprite, bool visibility)
      {
        if (gunSprite.renderer.enabled == visibility)
          return;
        gunSprite.renderer.enabled = visibility;
        if (!(bool) (Object) this.VFXStonePuff)
          return;
        tk2dSprite component = SpawnManager.SpawnVFX(this.VFXStonePuff, gunSprite.transform.position, Quaternion.identity).GetComponent<tk2dSprite>();
        component.HeightOffGround = 10f;
        component.UpdateZDepth();
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_item_spawn_01", this.gameObject);
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        if ((Object) this.sprite == (Object) null)
          return 100f;
        Vector3 b = (Vector3) BraveMathCollege.ClosestPointOnRectangle(point, this.specRigidbody.UnitBottomLeft, this.specRigidbody.UnitDimensions);
        return Vector2.Distance(point, (Vector2) b) / 1.5f;
      }

      public float GetOverrideMaxDistance() => -1f;

      public void OnEnteredRange(PlayerController interactor)
      {
        if ((Object) this.AlternativeOutlineTarget != (Object) null)
          SpriteOutlineManager.AddOutlineToSprite((tk2dBaseSprite) this.AlternativeOutlineTarget, Color.white);
        else
          SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
      }

      public void OnExitRange(PlayerController interactor)
      {
        if ((Object) this.AlternativeOutlineTarget != (Object) null)
          SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.AlternativeOutlineTarget);
        else
          SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
      }

      private bool NeedsGun(int pickupID)
      {
        return pickupID == this.Gun01ID && !GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_01) || pickupID == this.Gun02ID && !GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_02) || pickupID == this.Gun03ID && !GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_03) || pickupID == this.Gun04ID && !GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_04) || pickupID == this.Gun05ID && !GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_05) || pickupID == this.Gun06ID && !GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_06);
      }

      private bool CheckCanBeUsed(PlayerController interactor)
      {
        return (bool) (Object) interactor && (bool) (Object) interactor.CurrentGun && this.m_useCount <= 10 && this.NeedsGun(interactor.CurrentGun.PickupObjectId);
      }

      private void SetFlagForID(int id)
      {
        if (id == this.Gun01ID)
          GameStatsManager.Instance.SetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_01, true);
        if (id == this.Gun02ID)
          GameStatsManager.Instance.SetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_02, true);
        if (id == this.Gun03ID)
          GameStatsManager.Instance.SetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_03, true);
        if (id == this.Gun04ID)
          GameStatsManager.Instance.SetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_04, true);
        if (id == this.Gun05ID)
          GameStatsManager.Instance.SetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_05, true);
        if (id == this.Gun06ID)
          GameStatsManager.Instance.SetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_06, true);
        this.UpdateSpriteVisibility();
      }

      private void DoShrineEffect(PlayerController interactor)
      {
        this.SetFlagForID(interactor.CurrentGun.PickupObjectId);
        int num = 0;
        if (GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_01))
          ++num;
        if (GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_02))
          ++num;
        if (GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_03))
          ++num;
        if (GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_04))
          ++num;
        if (GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_05))
          ++num;
        if (GameStatsManager.Instance.GetFlag(GungeonFlags.SHRINE_BEHOLSTER_GUN_06))
          ++num;
        if (num == 6)
        {
          LootEngine.TryGiveGunToPlayer(PickupObjectDatabase.GetById(this.Gun01ID).gameObject, interactor);
          LootEngine.TryGiveGunToPlayer(PickupObjectDatabase.GetById(this.Gun02ID).gameObject, interactor);
          LootEngine.TryGiveGunToPlayer(PickupObjectDatabase.GetById(this.Gun03ID).gameObject, interactor);
          LootEngine.TryGiveGunToPlayer(PickupObjectDatabase.GetById(this.Gun04ID).gameObject, interactor);
          LootEngine.TryGiveGunToPlayer(PickupObjectDatabase.GetById(this.Gun05ID).gameObject, interactor);
          LootEngine.TryGiveGunToPlayer(PickupObjectDatabase.GetById(this.Gun06ID).gameObject, interactor);
          this.StartCoroutine(this.HandleShrineCompletionVisuals());
          this.m_useCount = 100;
          interactor.inventory.GunChangeForgiveness = true;
          for (int amt = 0; amt < 100; ++amt)
          {
            if (interactor.inventory.GetTargetGunWithChange(amt).PickupObjectId == this.Gun01ID)
            {
              if (amt != 0)
              {
                interactor.inventory.ChangeGun(amt);
                break;
              }
              break;
            }
          }
          interactor.inventory.GunChangeForgiveness = false;
        }
        else
          interactor.inventory.DestroyCurrentGun();
      }

      [DebuggerHidden]
      private IEnumerator HandleShrineCompletionVisuals()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BeholsterShrineController.<HandleShrineCompletionVisuals>c__Iterator0()
        {
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleShrineConversation(PlayerController interactor)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BeholsterShrineController.<HandleShrineConversation>c__Iterator1()
        {
          interactor = interactor,
          $this = this
        };
      }

      private void ResetForReuse() => --this.m_useCount;

      [DebuggerHidden]
      private IEnumerator HandleSpentText(PlayerController interactor)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BeholsterShrineController.<HandleSpentText>c__Iterator2()
        {
          interactor = interactor,
          $this = this
        };
      }

      public void Interact(PlayerController interactor)
      {
        if (TextBoxManager.HasTextBox(this.talkPoint))
          return;
        if (this.m_useCount > 0)
        {
          if (string.IsNullOrEmpty(this.spentOptionKey))
            return;
          this.StartCoroutine(this.HandleSpentText(interactor));
        }
        else
        {
          ++this.m_useCount;
          this.StartCoroutine(this.HandleShrineConversation(interactor));
        }
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
