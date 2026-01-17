// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorSpeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Animator)]
  [HutongGames.PlayMaker.Tooltip("Gets the playback speed of the Animator. 1 is normal playback speed")]
  public class GetAnimatorSpeed : FsmStateActionAnimatorBase
  {
    [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
    [CheckForComponent(typeof (Animator))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The playBack speed of the animator. 1 is normal playback speed")]
    [UIHint(UIHint.Variable)]
    public FsmFloat speed;
    private Animator _animator;

    public override void Reset()
    {
      base.Reset();
      this.gameObject = (FsmOwnerDefault) null;
      this.speed = (FsmFloat) null;
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
          this.GetPlaybackSpeed();
          if (this.everyFrame)
            return;
          this.Finish();
        }
      }
    }

    public override void OnActionUpdate() => this.GetPlaybackSpeed();

    private void GetPlaybackSpeed()
    {
      if (!((Object) this._animator != (Object) null))
        return;
      this.speed.Value = this._animator.speed;
    }
  }
}
