// Decompiled with JetBrains decompiler
// Type: MegalichIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    [RequireComponent(typeof (GenericIntroDoer))]
    public class MegalichIntroDoer : SpecificIntroDoer
    {
      public GameObject head;
      public ScreenShakeSettings screenShake;
      private bool m_isFinished;
      private bool m_isCameraModified;

      public void Start()
      {
        this.aiAnimator.SetBaseAnim("blank");
        this.spriteAnimator.Play("blank");
      }

      protected override void OnDestroy()
      {
        this.ModifyCamera(false);
        this.BlockPitTiles(false);
        base.OnDestroy();
      }

      public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
      {
        this.spriteAnimator.Play("blank");
      }

      public override void StartIntro(List<tk2dSpriteAnimator> animators)
      {
        this.aiAnimator.ClearBaseAnim();
        this.StartCoroutine(this.DoIntro());
      }

      public override Vector2? OverrideOutroPosition
      {
        get
        {
          this.ModifyCamera(true);
          this.BlockPitTiles(true);
          return new Vector2?();
        }
      }

      [DebuggerHidden]
      public IEnumerator DoIntro()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MegalichIntroDoer.<DoIntro>c__Iterator0()
        {
          $this = this
        };
      }

      public override bool IsIntroFinished => this.m_isFinished;

      public override void EndIntro()
      {
        this.StopAllCoroutines();
        this.aiAnimator.EndAnimationIf("intro");
        GameManager.Instance.MainCameraController.StopContinuousScreenShake((Component) this);
        int num = (int) AkSoundEngine.PostEvent("Play_MUS_lich_phase_02", this.gameObject);
      }

      public void ModifyCamera(bool value)
      {
        if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach || this.m_isCameraModified == value)
          return;
        CameraController cameraController = GameManager.Instance.MainCameraController;
        if (value)
        {
          cameraController.OverrideZoomScale = 0.75f;
          cameraController.LockToRoom = true;
          cameraController.AddFocusPoint(this.head);
          cameraController.controllerCamera.isTransitioning = false;
        }
        else
        {
          cameraController.SetZoomScaleImmediate(1f);
          cameraController.LockToRoom = false;
          cameraController.RemoveFocusPoint(this.head);
        }
        this.m_isCameraModified = value;
      }

      public void BlockPitTiles(bool value)
      {
        if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach || (Object) GameManager.Instance.Dungeon == (Object) null)
          return;
        IntVector2 basePosition = this.aiActor.ParentRoom.area.basePosition;
        IntVector2 intVector2 = this.aiActor.ParentRoom.area.basePosition + this.aiActor.ParentRoom.area.dimensions - IntVector2.One;
        DungeonData data = GameManager.Instance.Dungeon.data;
        for (int x = basePosition.x; x <= intVector2.x; ++x)
        {
          for (int y = basePosition.y; y <= intVector2.y; ++y)
          {
            CellData cellData = data[x, y];
            if (cellData != null && cellData.type == CellType.PIT)
              cellData.IsPlayerInaccessible = value;
          }
        }
      }
    }

}
