// Decompiled with JetBrains decompiler
// Type: HeartDispenser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class HeartDispenser : DungeonPlaceableBehaviour, IPlayerInteractable
    {
      [PickupIdentifier]
      public int halfHeartId = -1;
      public GameObject dustVFX;
      private bool m_isVisible = true;
      private bool m_hasEverBeenRevealed;
      public static bool DispenserOnFloor;
      private static int m_currentHalfHeartsStored;
      private int m_cachedStored;
      private string m_currentBaseAnimation = "heart_dispenser_idle_empty";

      public static void ClearPerLevelData()
      {
        HeartDispenser.CurrentHalfHeartsStored = 0;
        HeartDispenser.DispenserOnFloor = false;
      }

      public static int CurrentHalfHeartsStored
      {
        get => HeartDispenser.m_currentHalfHeartsStored;
        set => HeartDispenser.m_currentHalfHeartsStored = value;
      }

      private void UpdateVisuals()
      {
        if (HeartDispenser.CurrentHalfHeartsStored > 0)
          this.m_currentBaseAnimation = "heart_dispenser_idle_full";
        else
          this.m_currentBaseAnimation = "heart_dispenser_idle_empty";
      }

      public void Awake() => HeartDispenser.DispenserOnFloor = true;

      private void Start() => SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f);

      public void Update()
      {
        if (this.m_cachedStored != HeartDispenser.CurrentHalfHeartsStored)
        {
          this.m_cachedStored = HeartDispenser.CurrentHalfHeartsStored;
          this.UpdateVisuals();
        }
        if ((this.spriteAnimator.IsPlaying("heart_dispenser_idle_empty") || this.spriteAnimator.IsPlaying("heart_dispenser_idle_full")) && this.spriteAnimator.CurrentClip.name != this.m_currentBaseAnimation)
          this.spriteAnimator.Play(this.m_currentBaseAnimation);
        if (this.m_isVisible && !this.m_hasEverBeenRevealed && HeartDispenser.CurrentHalfHeartsStored == 0)
        {
          this.m_isVisible = false;
          this.ToggleRenderers(false);
        }
        else
        {
          if (this.m_isVisible || !this.m_hasEverBeenRevealed && HeartDispenser.CurrentHalfHeartsStored <= 0)
            return;
          this.m_hasEverBeenRevealed = true;
          this.m_isVisible = true;
          this.ToggleRenderers(true);
        }
      }

      private void ToggleRenderers(bool state)
      {
        SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, state);
        this.renderer.enabled = state;
        this.transform.Find("shadow").GetComponent<MeshRenderer>().enabled = state;
        this.specRigidbody.enabled = state;
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        return Vector2.Distance(this.specRigidbody.UnitBottomCenter, point);
      }

      public void OnEnteredRange(PlayerController interactor)
      {
        if ((double) interactor.healthHaver.GetCurrentHealthPercentage() >= 1.0 || HeartDispenser.CurrentHalfHeartsStored <= 0)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 0.1f);
        this.sprite.UpdateZDepth();
      }

      public void OnExitRange(PlayerController interactor)
      {
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f);
      }

      public void Interact(PlayerController interactor)
      {
        if (HeartDispenser.CurrentHalfHeartsStored > 0 && (double) interactor.healthHaver.GetCurrentHealthPercentage() >= 1.0)
          this.spriteAnimator.PlayForDuration("heart_dispenser_no", -1f, this.m_currentBaseAnimation);
        else if (HeartDispenser.CurrentHalfHeartsStored > 0)
        {
          --HeartDispenser.CurrentHalfHeartsStored;
          this.spriteAnimator.PlayForDuration("heart_dispenser_dispense", -1f, this.m_currentBaseAnimation);
          Object.Instantiate<GameObject>(this.dustVFX, this.transform.position, Quaternion.identity);
          this.StartCoroutine(this.DelayedSpawnHalfHeart());
        }
        else
          this.spriteAnimator.PlayForDuration("heart_dispenser_empty", -1f, this.m_currentBaseAnimation);
      }

      [DebuggerHidden]
      private IEnumerator DelayedSpawnHalfHeart()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HeartDispenser__DelayedSpawnHalfHeartc__Iterator0()
        {
          _this = this
        };
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      public float GetOverrideMaxDistance() => -1f;
    }

}
