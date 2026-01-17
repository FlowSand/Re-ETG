// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetAnimatorStabilizeFeet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("If true, automaticaly stabilize feet during transition and blending")]
  [ActionCategory(ActionCategory.Animator)]
  public class SetAnimatorStabilizeFeet : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
    [CheckForComponent(typeof (Animator))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("If true, automaticaly stabilize feet during transition and blending")]
    public FsmBool stabilizeFeet;
    private Animator _animator;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.stabilizeFeet = (FsmBool) null;
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
          this.DoStabilizeFeet();
          this.Finish();
        }
      }
    }

    private void DoStabilizeFeet()
    {
      if ((Object) this._animator == (Object) null)
        return;
      this._animator.stabilizeFeet = this.stabilizeFeet.Value;
    }
  }
}
