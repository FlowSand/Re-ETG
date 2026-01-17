// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AnimatorStopPlayback
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animator)]
[HutongGames.PlayMaker.Tooltip("Stops the animator playback mode. When playback stops, the avatar resumes getting control from game logic")]
public class AnimatorStopPlayback : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
  [CheckForComponent(typeof (Animator))]
  [RequiredField]
  public FsmOwnerDefault gameObject;

  public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

  public override void OnEnter()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
    {
      this.Finish();
    }
    else
    {
      Animator component = ownerDefaultTarget.GetComponent<Animator>();
      if ((Object) component != (Object) null)
        component.StopPlayback();
      this.Finish();
    }
  }
}
