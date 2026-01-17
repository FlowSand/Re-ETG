// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorCurrentStateInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animator)]
[HutongGames.PlayMaker.Tooltip("Gets the current State information on a specified layer")]
public class GetAnimatorCurrentStateInfo : FsmStateActionAnimatorBase
{
  [CheckForComponent(typeof (Animator))]
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The target.")]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The layer's index")]
  [RequiredField]
  public FsmInt layerIndex;
  [HutongGames.PlayMaker.Tooltip("The layer's name.")]
  [UIHint(UIHint.Variable)]
  [ActionSection("Results")]
  public FsmString name;
  [HutongGames.PlayMaker.Tooltip("The layer's name Hash. Obsolete in Unity 5, use fullPathHash or shortPathHash instead, nameHash will be the same as shortNameHash for legacy")]
  [UIHint(UIHint.Variable)]
  public FsmInt nameHash;
  [HutongGames.PlayMaker.Tooltip("The layer's tag hash")]
  [UIHint(UIHint.Variable)]
  public FsmInt tagHash;
  [HutongGames.PlayMaker.Tooltip("Is the state looping. All animations in the state must be looping")]
  [UIHint(UIHint.Variable)]
  public FsmBool isStateLooping;
  [HutongGames.PlayMaker.Tooltip("The Current duration of the state. In seconds, can vary when the State contains a Blend Tree ")]
  [UIHint(UIHint.Variable)]
  public FsmFloat length;
  [HutongGames.PlayMaker.Tooltip("The integer part is the number of time a state has been looped. The fractional part is the % (0-1) of progress in the current loop")]
  [UIHint(UIHint.Variable)]
  public FsmFloat normalizedTime;
  [HutongGames.PlayMaker.Tooltip("The integer part is the number of time a state has been looped. This is extracted from the normalizedTime")]
  [UIHint(UIHint.Variable)]
  public FsmInt loopCount;
  [HutongGames.PlayMaker.Tooltip("The progress in the current loop. This is extracted from the normalizedTime")]
  [UIHint(UIHint.Variable)]
  public FsmFloat currentLoopProgress;
  private Animator _animator;

  public override void Reset()
  {
    base.Reset();
    this.gameObject = (FsmOwnerDefault) null;
    this.layerIndex = (FsmInt) null;
    this.name = (FsmString) null;
    this.nameHash = (FsmInt) null;
    this.tagHash = (FsmInt) null;
    this.length = (FsmFloat) null;
    this.normalizedTime = (FsmFloat) null;
    this.isStateLooping = (FsmBool) null;
    this.loopCount = (FsmInt) null;
    this.currentLoopProgress = (FsmFloat) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((UnityEngine.Object) ownerDefaultTarget == (UnityEngine.Object) null)
    {
      this.Finish();
    }
    else
    {
      this._animator = ownerDefaultTarget.GetComponent<Animator>();
      if ((UnityEngine.Object) this._animator == (UnityEngine.Object) null)
      {
        this.Finish();
      }
      else
      {
        this.GetLayerInfo();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }
  }

  public override void OnActionUpdate() => this.GetLayerInfo();

  private void GetLayerInfo()
  {
    if (!((UnityEngine.Object) this._animator != (UnityEngine.Object) null))
      return;
    AnimatorStateInfo animatorStateInfo = this._animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value);
    if (!this.nameHash.IsNone)
      this.nameHash.Value = animatorStateInfo.nameHash;
    if (!this.name.IsNone)
      this.name.Value = this._animator.GetLayerName(this.layerIndex.Value);
    if (!this.tagHash.IsNone)
      this.tagHash.Value = animatorStateInfo.tagHash;
    if (!this.length.IsNone)
      this.length.Value = animatorStateInfo.length;
    if (!this.isStateLooping.IsNone)
      this.isStateLooping.Value = animatorStateInfo.loop;
    if (!this.normalizedTime.IsNone)
      this.normalizedTime.Value = animatorStateInfo.normalizedTime;
    if (this.loopCount.IsNone && this.currentLoopProgress.IsNone)
      return;
    this.loopCount.Value = (int) Math.Truncate((double) animatorStateInfo.normalizedTime);
    this.currentLoopProgress.Value = animatorStateInfo.normalizedTime - (float) this.loopCount.Value;
  }
}
