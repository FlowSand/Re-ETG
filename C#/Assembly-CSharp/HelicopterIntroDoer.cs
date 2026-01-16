// Decompiled with JetBrains decompiler
// Type: HelicopterIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (GenericIntroDoer))]
public class HelicopterIntroDoer : SpecificIntroDoer
{
  private bool m_isFinished;

  public bool IsCameraModified { get; set; }

  public override bool IsIntroFinished => this.m_isFinished && base.IsIntroFinished;

  protected override void OnDestroy()
  {
    this.ModifyCamera(false);
    base.OnDestroy();
  }

  public override void StartIntro(List<tk2dSpriteAnimator> animators)
  {
    base.StartIntro(animators);
    this.StartCoroutine(this.DoIntro());
  }

  [DebuggerHidden]
  public IEnumerator DoIntro()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new HelicopterIntroDoer.\u003CDoIntro\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
  {
    GameManager.Instance.MainCameraController.SetZoomScaleImmediate(0.75f);
    int num1 = (int) AkSoundEngine.PostEvent("Play_boss_helicopter_loop_01", this.gameObject);
    int num2 = (int) AkSoundEngine.PostEvent("Play_State_Volume_Lower_01", this.gameObject);
    this.aiActor.ParentRoom.CompletelyPreventLeaving = true;
  }

  public override Vector2? OverrideOutroPosition
  {
    get
    {
      this.ModifyCamera(true);
      return new Vector2?();
    }
  }

  public void ModifyCamera(bool value)
  {
    if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach || this.IsCameraModified == value)
      return;
    CameraController cameraController = GameManager.Instance.MainCameraController;
    if (value)
    {
      cameraController.OverrideZoomScale = 0.75f;
      cameraController.LockToRoom = true;
      cameraController.controllerCamera.isTransitioning = false;
    }
    else
    {
      cameraController.SetZoomScaleImmediate(1f);
      cameraController.LockToRoom = false;
      int num = (int) AkSoundEngine.PostEvent("Stop_State_Volume_Lower_01", this.gameObject);
    }
    this.IsCameraModified = value;
  }
}
