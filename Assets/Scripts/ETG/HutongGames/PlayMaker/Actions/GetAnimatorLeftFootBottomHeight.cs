// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorLeftFootBottomHeight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Animator)]
  [HutongGames.PlayMaker.Tooltip("Get the left foot bottom height.")]
  public class GetAnimatorLeftFootBottomHeight : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
    [CheckForComponent(typeof (Animator))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    [ActionSection("Result")]
    [HutongGames.PlayMaker.Tooltip("the left foot bottom height.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat leftFootHeight;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
    public bool everyFrame;
    private Animator _animator;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.leftFootHeight = (FsmFloat) null;
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
          this._getLeftFootBottonHeight();
          if (this.everyFrame)
            return;
          this.Finish();
        }
      }
    }

    public override void OnLateUpdate() => this._getLeftFootBottonHeight();

    private void _getLeftFootBottonHeight()
    {
      if (!((Object) this._animator != (Object) null))
        return;
      this.leftFootHeight.Value = this._animator.leftFeetBottomHeight;
    }
  }
}
