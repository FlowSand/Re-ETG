// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorCurrentTransitionInfoIsUserName
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Check the active Transition user-specified name on a specified layer.")]
  [ActionCategory(ActionCategory.Animator)]
  public class GetAnimatorCurrentTransitionInfoIsUserName : FsmStateActionAnimatorBase
  {
    [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
    [CheckForComponent(typeof (Animator))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The layer's index")]
    [RequiredField]
    public FsmInt layerIndex;
    [HutongGames.PlayMaker.Tooltip("The user-specified name to check the transition against.")]
    public FsmString userName;
    [HutongGames.PlayMaker.Tooltip("True if name matches")]
    [UIHint(UIHint.Variable)]
    [ActionSection("Results")]
    public FsmBool nameMatch;
    [HutongGames.PlayMaker.Tooltip("Event send if name matches")]
    public FsmEvent nameMatchEvent;
    [HutongGames.PlayMaker.Tooltip("Event send if name doesn't match")]
    public FsmEvent nameDoNotMatchEvent;
    private Animator _animator;

    public override void Reset()
    {
      base.Reset();
      this.gameObject = (FsmOwnerDefault) null;
      this.layerIndex = (FsmInt) null;
      this.userName = (FsmString) null;
      this.nameMatch = (FsmBool) null;
      this.nameMatchEvent = (FsmEvent) null;
      this.nameDoNotMatchEvent = (FsmEvent) null;
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
          this.IsName();
          if (this.everyFrame)
            return;
          this.Finish();
        }
      }
    }

    public override void OnActionUpdate() => this.IsName();

    private void IsName()
    {
      if (!((Object) this._animator != (Object) null))
        return;
      bool flag = this._animator.GetAnimatorTransitionInfo(this.layerIndex.Value).IsUserName(this.userName.Value);
      if (!this.nameMatch.IsNone)
        this.nameMatch.Value = flag;
      if (flag)
        this.Fsm.Event(this.nameMatchEvent);
      else
        this.Fsm.Event(this.nameDoNotMatchEvent);
    }
  }
}
