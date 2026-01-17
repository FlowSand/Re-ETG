// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetAnimatorFeetPivotActive
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Animator)]
  [HutongGames.PlayMaker.Tooltip("Activates feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
  public class SetAnimatorFeetPivotActive : FsmStateAction
  {
    [CheckForComponent(typeof (Animator))]
    [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Activates feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
    public FsmFloat feetPivotActive;
    private Animator _animator;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.feetPivotActive = (FsmFloat) null;
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
          this.DoFeetPivotActive();
          this.Finish();
        }
      }
    }

    private void DoFeetPivotActive()
    {
      if ((Object) this._animator == (Object) null)
        return;
      this._animator.feetPivotActive = this.feetPivotActive.Value;
    }
  }
}
