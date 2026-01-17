// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AnimatorStartRecording
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the animator in recording mode, and allocates a circular buffer of size frameCount. After this call, the recorder starts collecting up to frameCount frames in the buffer. Note it is not possible to start playback until a call to StopRecording is made")]
[ActionCategory(ActionCategory.Animator)]
public class AnimatorStartRecording : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
  [CheckForComponent(typeof (Animator))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The number of frames (updates) that will be recorded. If frameCount is 0, the recording will continue until the user calls StopRecording. The maximum value for frameCount is 10000.")]
  [RequiredField]
  public FsmInt frameCount;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.frameCount = (FsmInt) 0;
  }

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
        component.StartRecording(this.frameCount.Value);
      this.Finish();
    }
  }
}
