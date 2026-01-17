// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorCurrentTransitionInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Animator)]
  [HutongGames.PlayMaker.Tooltip("Gets the current transition information on a specified layer. Only valid when during a transition.")]
  public class GetAnimatorCurrentTransitionInfo : FsmStateActionAnimatorBase
  {
    [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
    [CheckForComponent(typeof (Animator))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The layer's index")]
    [RequiredField]
    public FsmInt layerIndex;
    [HutongGames.PlayMaker.Tooltip("The unique name of the Transition")]
    [UIHint(UIHint.Variable)]
    [ActionSection("Results")]
    public FsmString name;
    [HutongGames.PlayMaker.Tooltip("The unique name of the Transition")]
    [UIHint(UIHint.Variable)]
    public FsmInt nameHash;
    [HutongGames.PlayMaker.Tooltip("The user-specidied name of the Transition")]
    [UIHint(UIHint.Variable)]
    public FsmInt userNameHash;
    [HutongGames.PlayMaker.Tooltip("Normalized time of the Transition")]
    [UIHint(UIHint.Variable)]
    public FsmFloat normalizedTime;
    private Animator _animator;

    public override void Reset()
    {
      base.Reset();
      this.gameObject = (FsmOwnerDefault) null;
      this.layerIndex = (FsmInt) null;
      this.name = (FsmString) null;
      this.nameHash = (FsmInt) null;
      this.userNameHash = (FsmInt) null;
      this.normalizedTime = (FsmFloat) null;
      this.everyFrame = false;
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
        this._animator = ownerDefaultTarget.GetComponent<Animator>();
        if ((Object) this._animator == (Object) null)
        {
          this.Finish();
        }
        else
        {
          this.GetTransitionInfo();
          if (this.everyFrame)
            return;
          this.Finish();
        }
      }
    }

    public override void OnActionUpdate() => this.GetTransitionInfo();

    private void GetTransitionInfo()
    {
      if (!((Object) this._animator != (Object) null))
        return;
      AnimatorTransitionInfo animatorTransitionInfo = this._animator.GetAnimatorTransitionInfo(this.layerIndex.Value);
      if (!this.name.IsNone)
        this.name.Value = this._animator.GetLayerName(this.layerIndex.Value);
      if (!this.nameHash.IsNone)
        this.nameHash.Value = animatorTransitionInfo.nameHash;
      if (!this.userNameHash.IsNone)
        this.userNameHash.Value = animatorTransitionInfo.userNameHash;
      if (this.normalizedTime.IsNone)
        return;
      this.normalizedTime.Value = animatorTransitionInfo.normalizedTime;
    }
  }
}
