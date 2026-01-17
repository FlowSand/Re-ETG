// Decompiled with JetBrains decompiler
// Type: MetalGearRatIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    [RequireComponent(typeof (GenericIntroDoer))]
    public class MetalGearRatIntroDoer : SpecificIntroDoer
    {
      public GameObject head;
      private bool m_initialized;
      private GameObject m_introDummy;
      private tk2dSpriteAnimator m_introLightsDummy;
      private bool m_isFinished;
      private bool m_isCameraModified;
      private bool m_musicStarted;

      private void Awake()
      {
        this.m_introDummy = this.transform.Find("intro dummy").gameObject;
        this.m_introLightsDummy = this.transform.Find("intro dummy lights").GetComponent<tk2dSpriteAnimator>();
      }

      protected override void OnDestroy()
      {
        this.ModifyCamera(false);
        base.OnDestroy();
      }

      public void Init()
      {
        if (this.m_initialized)
          return;
        GameManager.Instance.Dungeon.OverrideAmbientLight = true;
        GameManager.Instance.Dungeon.OverrideAmbientColor = this.aiActor.ParentRoom.area.runtimePrototypeData.customAmbient * 0.1f;
        this.aiActor.ParentRoom.BecomeTerrifyingDarkRoom(0.0f, wwiseEvent: "Play_Empty_Event_01");
        int num = (int) AkSoundEngine.PostEvent("Play_BOSS_RatMech_Ambience_01", this.gameObject);
        GameManager.Instance.Dungeon.OverrideAmbientLight = true;
        GameManager.Instance.Dungeon.OverrideAmbientColor = this.aiActor.ParentRoom.area.runtimePrototypeData.customAmbient * 0.1f;
        this.aiAnimator.PlayUntilCancelled("blank");
        this.aiAnimator.ChildAnimator.PlayUntilCancelled("blank");
        this.aiAnimator.ChildAnimator.ChildAnimator.PlayUntilCancelled("blank");
        this.m_introDummy.SetActive(true);
        GameManager.Instance.StartCoroutine(this.MusicStopperCR());
        this.m_initialized = true;
      }

      [DebuggerHidden]
      private IEnumerator MusicStopperCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MetalGearRatIntroDoer.<MusicStopperCR>c__Iterator0()
        {
          $this = this
        };
      }

      public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
      {
        base.PlayerWalkedIn(player, animators);
        this.Init();
      }

      public override void StartIntro(List<tk2dSpriteAnimator> animators)
      {
        animators.Add(this.aiAnimator.ChildAnimator.ChildAnimator.spriteAnimator);
        this.aiAnimator.ChildAnimator.ChildAnimator.spriteAnimator.enabled = false;
        this.StartCoroutine(this.DoIntro());
      }

      public override Vector2? OverrideOutroPosition
      {
        get
        {
          this.ModifyCamera(true);
          return new Vector2?();
        }
      }

      [DebuggerHidden]
      public IEnumerator DoIntro()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MetalGearRatIntroDoer.<DoIntro>c__Iterator1()
        {
          $this = this
        };
      }

      public override bool IsIntroFinished => this.m_isFinished;

      public override void OnCleanup()
      {
        this.aiAnimator.ChildAnimator.ChildAnimator.spriteAnimator.enabled = true;
        base.OnCleanup();
      }

      public override void EndIntro()
      {
        this.StopAllCoroutines();
        this.aiAnimator.EndAnimationIf("intro");
        this.m_introDummy.SetActive(false);
        this.m_introLightsDummy.gameObject.SetActive(false);
        this.aiAnimator.EndAnimationIf("blank");
        this.aiAnimator.ChildAnimator.EndAnimationIf("blank");
        this.aiAnimator.ChildAnimator.ChildAnimator.EndAnimationIf("blank");
        GameManager.Instance.Dungeon.OverrideAmbientLight = false;
        this.aiActor.ParentRoom.EndTerrifyingDarkRoom(wwiseEvent: "Play_Empty_Event_01");
        GameManager.Instance.Dungeon.OverrideAmbientLight = false;
        if (this.m_musicStarted)
          return;
        GameManager.Instance.DungeonMusicController.SwitchToBossMusic("Play_MUS_Rat_Theme_02", this.gameObject);
        this.m_musicStarted = true;
      }

      public void ModifyCamera(bool value)
      {
        if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach || this.m_isCameraModified == value)
          return;
        CameraController cameraController = GameManager.Instance.MainCameraController;
        if (value)
        {
          cameraController.OverrideZoomScale = 0.66f;
          cameraController.LockToRoom = true;
          cameraController.AddFocusPoint(this.head);
          cameraController.controllerCamera.isTransitioning = false;
          Projectile.SetGlobalProjectileDepth(4f);
          BasicBeamController.SetGlobalBeamHeight(4f);
        }
        else
        {
          cameraController.SetZoomScaleImmediate(1f);
          cameraController.LockToRoom = false;
          cameraController.RemoveFocusPoint(this.head);
          Projectile.ResetGlobalProjectileDepth();
          BasicBeamController.ResetGlobalBeamHeight();
        }
        this.m_isCameraModified = value;
      }
    }

}
