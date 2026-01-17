// Decompiled with JetBrains decompiler
// Type: InteractableLock
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class InteractableLock : BraveBehaviour, IPlayerInteractable
    {
      public bool Suppress;
      [NonSerialized]
      public bool IsLocked = true;
      [NonSerialized]
      public bool HasBeenPicked;
      public InteractableLock.InteractableLockMode lockMode;
      [PickupIdentifier]
      public int JailCellKeyId = -1;
      [CheckAnimation(null)]
      public string IdleAnimName;
      [CheckAnimation(null)]
      public string UnlockAnimName;
      [CheckAnimation(null)]
      public string NoKeyAnimName;
      [CheckAnimation(null)]
      public string SpitAnimName;
      [CheckAnimation(null)]
      public string BustedAnimName;
      [NonSerialized]
      public bool IsBusted;
      public System.Action OnUnlocked;
      private bool m_lockHasApproached;
      private bool m_lockHasLaughed;
      private bool m_lockHasSpit;

      private void Awake() => StaticReferenceManager.AllLocks.Add(this);

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new InteractableLock.<Start>c__Iterator0()
        {
          $this = this
        };
      }

      private void Update()
      {
        if (this.IsBusted || !this.IsLocked || string.IsNullOrEmpty(this.SpitAnimName))
          return;
        float num = Vector2.Distance(this.sprite.WorldCenter, GameManager.Instance.PrimaryPlayer.specRigidbody.UnitCenter);
        if (!this.m_lockHasApproached && (double) num < 2.5)
        {
          this.spriteAnimator.Play(this.IdleAnimName);
          this.m_lockHasApproached = true;
        }
        else if ((double) num > 2.5)
        {
          if (this.m_lockHasLaughed)
            this.spriteAnimator.Play(this.SpitAnimName);
          this.m_lockHasLaughed = false;
          this.m_lockHasApproached = false;
        }
        if (this.m_lockHasSpit || !((UnityEngine.Object) this.spriteAnimator != (UnityEngine.Object) null) || !this.spriteAnimator.IsPlaying(this.SpitAnimName) || this.spriteAnimator.CurrentFrame != 3)
          return;
        this.m_lockHasSpit = true;
        tk2dSprite componentInChildren = SpawnManager.SpawnVFX(BraveResources.Load("Global VFX/VFX_Lock_Spit") as GameObject).GetComponentInChildren<tk2dSprite>();
        componentInChildren.UpdateZDepth();
        componentInChildren.PlaceAtPositionByAnchor((Vector3) this.spriteAnimator.sprite.WorldCenter, tk2dBaseSprite.Anchor.UpperCenter);
      }

      public void OnEnteredRange(PlayerController interactor)
      {
        if (!(bool) (UnityEngine.Object) this || this.Suppress)
          return;
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 0.1f);
        this.sprite.UpdateZDepth();
      }

      public void OnExitRange(PlayerController interactor)
      {
        if (!(bool) (UnityEngine.Object) this || this.Suppress)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        this.sprite.UpdateZDepth();
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        if (this.IsBusted || !this.IsLocked || this.Suppress)
          return 10000f;
        Bounds bounds = this.sprite.GetBounds();
        bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
        float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
        float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
        return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
      }

      public float GetOverrideMaxDistance() => -1f;

      public void BreakLock()
      {
        if (!this.IsLocked || this.IsBusted || this.lockMode != InteractableLock.InteractableLockMode.NORMAL)
          return;
        this.IsBusted = true;
        if (string.IsNullOrEmpty(this.BustedAnimName) || this.spriteAnimator.IsPlaying(this.BustedAnimName))
          return;
        this.spriteAnimator.Play(this.BustedAnimName);
      }

      public void ForceUnlock()
      {
        if (!this.IsLocked)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        this.sprite.UpdateZDepth();
        this.IsLocked = false;
        if (this.OnUnlocked != null)
          this.OnUnlocked();
        if (string.IsNullOrEmpty(this.UnlockAnimName))
          return;
        this.spriteAnimator.PlayAndDisableObject(this.UnlockAnimName);
      }

      public void Interact(PlayerController player)
      {
        if (this.IsBusted || !this.IsLocked)
          return;
        bool flag = false;
        if (this.lockMode == InteractableLock.InteractableLockMode.NORMAL)
          flag = player.carriedConsumables.InfiniteKeys || player.carriedConsumables.KeyBullets >= 1;
        else if (this.lockMode == InteractableLock.InteractableLockMode.RESOURCEFUL_RAT)
        {
          for (int index = 0; index < player.passiveItems.Count; ++index)
          {
            if (player.passiveItems[index] is SpecialKeyItem && (player.passiveItems[index] as SpecialKeyItem).keyType == SpecialKeyItem.SpecialKeyType.RESOURCEFUL_RAT_LAIR)
            {
              flag = true;
              int pickupObjectId = player.passiveItems[index].PickupObjectId;
              player.RemovePassiveItem(pickupObjectId);
              GameUIRoot.Instance.UpdatePlayerConsumables(player.carriedConsumables);
            }
          }
        }
        else if (this.lockMode == InteractableLock.InteractableLockMode.NPC_JAIL)
        {
          for (int index = 0; index < player.additionalItems.Count; ++index)
          {
            if (player.additionalItems[index] is NPCCellKeyItem)
            {
              flag = true;
              GameManager.BroadcastRoomFsmEvent("npcCellUnlocked", this.transform.position.GetAbsoluteRoom());
              UnityEngine.Object.Destroy((UnityEngine.Object) player.additionalItems[index].gameObject);
              player.additionalItems.RemoveAt(index);
              GameUIRoot.Instance.UpdatePlayerConsumables(player.carriedConsumables);
            }
          }
        }
        else if (this.lockMode == InteractableLock.InteractableLockMode.RAT_REWARD && player.carriedConsumables.ResourcefulRatKeys > 0)
        {
          --player.carriedConsumables.ResourcefulRatKeys;
          flag = true;
          GameUIRoot.Instance.UpdatePlayerConsumables(player.carriedConsumables);
        }
        if (flag)
        {
          this.OnExitRange(player);
          this.IsLocked = false;
          if (this.OnUnlocked != null)
            this.OnUnlocked();
          if (this.lockMode == InteractableLock.InteractableLockMode.NORMAL && !player.carriedConsumables.InfiniteKeys)
            --player.carriedConsumables.KeyBullets;
          if (string.IsNullOrEmpty(this.UnlockAnimName))
            return;
          this.spriteAnimator.PlayAndDisableObject(this.UnlockAnimName);
        }
        else
        {
          if (string.IsNullOrEmpty(this.NoKeyAnimName))
            return;
          if (!string.IsNullOrEmpty(this.IdleAnimName) && this.spriteAnimator.GetClipByName(this.IdleAnimName) != null)
          {
            if (!string.IsNullOrEmpty(this.SpitAnimName))
              this.spriteAnimator.Play(this.NoKeyAnimName);
            else
              this.spriteAnimator.PlayForDuration(this.NoKeyAnimName, 1f, this.IdleAnimName);
            this.m_lockHasSpit = false;
            this.m_lockHasLaughed = true;
          }
          else
            this.spriteAnimator.Play(this.NoKeyAnimName);
        }
      }

      private void ChangeToSpit(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2)
      {
        if (!(bool) (UnityEngine.Object) this.spriteAnimator)
          return;
        this.spriteAnimator.PlayForDuration(this.SpitAnimName, -1f, this.IdleAnimName);
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      protected override void OnDestroy()
      {
        StaticReferenceManager.AllLocks.Remove(this);
        base.OnDestroy();
      }

      public enum InteractableLockMode
      {
        NORMAL,
        RESOURCEFUL_RAT,
        NPC_JAIL,
        RAT_REWARD,
      }
    }

}
