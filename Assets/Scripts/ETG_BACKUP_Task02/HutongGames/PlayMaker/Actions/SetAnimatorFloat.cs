// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetAnimatorFloat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animator)]
[HutongGames.PlayMaker.Tooltip("Sets the value of a float parameter")]
public class SetAnimatorFloat : FsmStateActionAnimatorBase
{
  [CheckForComponent(typeof (Animator))]
  [HutongGames.PlayMaker.Tooltip("The target.")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.AnimatorFloat)]
  [HutongGames.PlayMaker.Tooltip("The animator parameter")]
  [RequiredField]
  public FsmString parameter;
  [HutongGames.PlayMaker.Tooltip("The float value to assign to the animator parameter")]
  public FsmFloat Value;
  [HutongGames.PlayMaker.Tooltip("Optional: The time allowed to parameter to reach the value. Requires everyFrame Checked on")]
  public FsmFloat dampTime;
  private Animator _animator;
  private int _paramID;

  public override void Reset()
  {
    base.Reset();
    this.gameObject = (FsmOwnerDefault) null;
    this.parameter = (FsmString) null;
    FsmFloat fsmFloat = new FsmFloat();
    fsmFloat.UseVariable = true;
    this.dampTime = fsmFloat;
    this.Value = (FsmFloat) null;
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
        this.SetParameter();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }
  }

  public override void OnActionUpdate() => this.SetParameter();

  private void SetParameter()
  {
    if ((Object) this._animator == (Object) null)
      return;
    if ((double) this.dampTime.Value > 0.0)
      this._animator.SetFloat(this._paramID, this.Value.Value, this.dampTime.Value, Time.deltaTime);
    else
      this._animator.SetFloat(this._paramID, this.Value.Value);
  }
}
