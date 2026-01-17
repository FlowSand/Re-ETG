// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorInt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Animator)]
  [HutongGames.PlayMaker.Tooltip("Gets the value of an int parameter")]
  public class GetAnimatorInt : FsmStateActionAnimatorBase
  {
    [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
    [RequiredField]
    [CheckForComponent(typeof (Animator))]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The animator parameter")]
    [UIHint(UIHint.AnimatorInt)]
    public FsmString parameter;
    [ActionSection("Results")]
    [HutongGames.PlayMaker.Tooltip("The int value of the animator parameter")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmInt result;
    private Animator _animator;
    private int _paramID;

    public override void Reset()
    {
      base.Reset();
      this.gameObject = (FsmOwnerDefault) null;
      this.parameter = (FsmString) null;
      this.result = (FsmInt) null;
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
          this._paramID = Animator.StringToHash(this.parameter.Value);
          this.GetParameter();
          if (this.everyFrame)
            return;
          this.Finish();
        }
      }
    }

    public override void OnActionUpdate() => this.GetParameter();

    private void GetParameter()
    {
      if (!((Object) this._animator != (Object) null))
        return;
      this.result.Value = this._animator.GetInteger(this._paramID);
    }
  }
}
