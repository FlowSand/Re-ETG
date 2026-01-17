// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorIsParameterControlledByCurve
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Returns true if a parameter is controlled by an additional curve on an animation")]
[ActionCategory(ActionCategory.Animator)]
public class GetAnimatorIsParameterControlledByCurve : FsmStateAction
{
  [CheckForComponent(typeof (Animator))]
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The parameter's name")]
  public FsmString parameterName;
  [HutongGames.PlayMaker.Tooltip("True if controlled by curve")]
  [ActionSection("Results")]
  [UIHint(UIHint.Variable)]
  public FsmBool isControlledByCurve;
  [HutongGames.PlayMaker.Tooltip("Event send if controlled by curve")]
  public FsmEvent isControlledByCurveEvent;
  [HutongGames.PlayMaker.Tooltip("Event send if not controlled by curve")]
  public FsmEvent isNotControlledByCurveEvent;
  private Animator _animator;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.parameterName = (FsmString) null;
    this.isControlledByCurve = (FsmBool) null;
    this.isControlledByCurveEvent = (FsmEvent) null;
    this.isNotControlledByCurveEvent = (FsmEvent) null;
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
        this.DoCheckIsParameterControlledByCurve();
        this.Finish();
      }
    }
  }

  private void DoCheckIsParameterControlledByCurve()
  {
    if ((Object) this._animator == (Object) null)
      return;
    bool flag = this._animator.IsParameterControlledByCurve(this.parameterName.Value);
    this.isControlledByCurve.Value = flag;
    if (flag)
      this.Fsm.Event(this.isControlledByCurveEvent);
    else
      this.Fsm.Event(this.isNotControlledByCurveEvent);
  }
}
