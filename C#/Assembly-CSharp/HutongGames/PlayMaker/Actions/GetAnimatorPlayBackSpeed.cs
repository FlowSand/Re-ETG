// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAnimatorPlayBackSpeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Gets the playback speed of the Animator. 1 is normal playback speed")]
[ActionCategory(ActionCategory.Animator)]
public class GetAnimatorPlayBackSpeed : FsmStateAction
{
  [CheckForComponent(typeof (Animator))]
  [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The playBack speed of the animator. 1 is normal playback speed")]
  [RequiredField]
  public FsmFloat playBackSpeed;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
  public bool everyFrame;
  private Animator _animator;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.playBackSpeed = (FsmFloat) null;
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
        this.GetPlayBackSpeed();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }
  }

  public override void OnUpdate() => this.GetPlayBackSpeed();

  private void GetPlayBackSpeed()
  {
    if (!((Object) this._animator != (Object) null))
      return;
    this.playBackSpeed.Value = this._animator.speed;
  }
}
