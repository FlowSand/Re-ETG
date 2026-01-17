// Decompiled with JetBrains decompiler
// Type: CrestDoorController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using HutongGames.PlayMaker;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class CrestDoorController : BraveBehaviour, IPlayerInteractable
    {
      public SpeculativeRigidbody AltarRigidbody;
      public SpeculativeRigidbody SarcoRigidbody;
      public ScreenShakeSettings SlideShake;
      public string displayTextKey;
      public string acceptOptionKey;
      public string declineOptionKey;
      public tk2dSprite CrestSprite;
      public Transform talkPoint;
      public tk2dSpriteAnimator cryoAnimator;
      public string cryoArriveAnimation;
      public string cyroDepartAnimation;
      private bool m_isOpen;
      private FsmBool m_cryoBool;
      private FsmBool m_normalBool;
      private float m_transitionTime;
      private float m_previousTransitionTime;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CrestDoorController.\u003CStart\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private void RescindCryoElevator()
      {
        this.m_cryoBool.Value = false;
        this.m_normalBool.Value = true;
        if (!(bool) (Object) this.cryoAnimator || string.IsNullOrEmpty(this.cyroDepartAnimation))
          return;
        this.cryoAnimator.Play(this.cyroDepartAnimation);
      }

      private void SwitchToCryoElevator()
      {
        this.m_cryoBool.Value = true;
        this.m_normalBool.Value = false;
        if (!(bool) (Object) this.cryoAnimator || string.IsNullOrEmpty(this.cryoArriveAnimation))
          return;
        this.cryoAnimator.Play(this.cryoArriveAnimation);
      }

      private void HandleGoToCathedral(
        SpeculativeRigidbody specRigidbody,
        SpeculativeRigidbody sourceSpecRigidbody,
        CollisionData collisionData)
      {
        if (GameManager.Instance.IsLoadingLevel || !this.m_isOpen || !(bool) (Object) specRigidbody.gameActor || !(specRigidbody.gameActor is PlayerController))
          return;
        PlayerController gameActor = specRigidbody.gameActor as PlayerController;
        if (gameActor.IsDodgeRolling)
        {
          this.m_transitionTime = 0.0f;
        }
        else
        {
          this.m_transitionTime += BraveTime.DeltaTime;
          if ((double) this.m_transitionTime <= 0.5)
            return;
          Pixelator.Instance.FadeToBlack(0.5f);
          gameActor.CurrentInputState = PlayerInputState.NoInput;
          specRigidbody.Velocity.x = 0.0f;
          if (this.m_cryoBool != null && this.m_cryoBool.Value)
          {
            GameUIRoot.Instance.HideCoreUI(string.Empty);
            GameUIRoot.Instance.ToggleLowerPanels(false, source: string.Empty);
            int num = (int) AkSoundEngine.PostEvent("Stop_MUS_All", this.gameObject);
            GameManager.DoMidgameSave(GlobalDungeonData.ValidTilesets.CATHEDRALGEON);
            GameManager.Instance.DelayedLoadCharacterSelect(0.6f, true, true);
          }
          else
          {
            GameManager.DoMidgameSave(GlobalDungeonData.ValidTilesets.CATHEDRALGEON);
            GameManager.Instance.DelayedLoadCustomLevel(0.5f, "tt_cathedral");
          }
        }
      }

      private void LateUpdate()
      {
        if ((double) this.m_transitionTime == (double) this.m_previousTransitionTime)
          this.m_transitionTime = 0.0f;
        this.m_previousTransitionTime = this.m_transitionTime;
      }

      [DebuggerHidden]
      private IEnumerator Open()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CrestDoorController.\u003COpen\u003Ec__Iterator1()
        {
          \u0024this = this
        };
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        Bounds bounds = this.AltarRigidbody.sprite.GetBounds();
        bounds.SetMinMax(bounds.min + this.AltarRigidbody.sprite.transform.position, bounds.max + this.AltarRigidbody.sprite.transform.position);
        float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
        float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
        return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
      }

      public float GetOverrideMaxDistance() => -1f;

      public void OnEnteredRange(PlayerController interactor)
      {
        if (!(bool) (Object) this || this.m_isOpen)
          return;
        SpriteOutlineManager.AddOutlineToSprite(this.AltarRigidbody.sprite, Color.white);
      }

      public void OnExitRange(PlayerController interactor)
      {
        if (!(bool) (Object) this || this.m_isOpen)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.AltarRigidbody.sprite);
      }

      public void Interact(PlayerController interactor)
      {
        if (this.m_isOpen)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.AltarRigidbody.sprite);
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleShrineConversation(interactor));
      }

      [DebuggerHidden]
      private IEnumerator HandleShrineConversation(PlayerController interactor)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CrestDoorController.\u003CHandleShrineConversation\u003Ec__Iterator2()
        {
          interactor = interactor,
          \u0024this = this
        };
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }
    }

}
