// Decompiled with JetBrains decompiler
// Type: BossFinalRogueIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (GenericIntroDoer))]
public class BossFinalRogueIntroDoer : SpecificIntroDoer
{
  private bool m_isFinished;

  protected override void OnDestroy() => base.OnDestroy();

  public override Vector2? OverrideIntroPosition
  {
    get
    {
      GameManager.Instance.MainCameraController.OverrideZoomScale = 0.6666f;
      return new Vector2?(this.GetComponent<BossFinalRogueController>().CameraPos);
    }
  }

  public override void StartIntro(List<tk2dSpriteAnimator> animators)
  {
    this.StartCoroutine(this.DoIntro());
  }

  [DebuggerHidden]
  public IEnumerator DoIntro()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BossFinalRogueIntroDoer.\u003CDoIntro\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public override bool IsIntroFinished => this.m_isFinished;

  public override Vector2? OverrideOutroPosition
  {
    get
    {
      BossFinalRogueController component = this.GetComponent<BossFinalRogueController>();
      component.InitCamera();
      return new Vector2?(component.CameraPos);
    }
  }

  public override void EndIntro()
  {
    this.GetComponent<BossFinalRogueController>().InitCamera();
    GameManager.Instance.MainCameraController.SetManualControl(true);
    GameManager.Instance.MainCameraController.OverrideZoomScale = 0.6666f;
  }
}
