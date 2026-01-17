// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.EnableAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animation)]
[HutongGames.PlayMaker.Tooltip("Enables/Disables an Animation on a GameObject.\nAnimation time is paused while disabled. Animation must also have a non zero weight to play.")]
public class EnableAnimation : BaseAnimationAction
{
  [HutongGames.PlayMaker.Tooltip("The GameObject playing the animation.")]
  [CheckForComponent(typeof (Animation))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The name of the animation to enable/disable.")]
  [UIHint(UIHint.Animation)]
  [RequiredField]
  public FsmString animName;
  [HutongGames.PlayMaker.Tooltip("Set to True to enable, False to disable.")]
  [RequiredField]
  public FsmBool enable;
  [HutongGames.PlayMaker.Tooltip("Reset the initial enabled state when exiting the state.")]
  public FsmBool resetOnExit;
  private AnimationState anim;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.animName = (FsmString) null;
    this.enable = (FsmBool) true;
    this.resetOnExit = (FsmBool) false;
  }

  public override void OnEnter()
  {
    this.DoEnableAnimation(this.Fsm.GetOwnerDefaultTarget(this.gameObject));
    this.Finish();
  }

  private void DoEnableAnimation(GameObject go)
  {
    if (!this.UpdateCache(go))
      return;
    this.anim = this.animation[this.animName.Value];
    if (!((TrackedReference) this.anim != (TrackedReference) null))
      return;
    this.anim.enabled = this.enable.Value;
  }

  public override void OnExit()
  {
    if (!this.resetOnExit.Value || !((TrackedReference) this.anim != (TrackedReference) null))
      return;
    this.anim.enabled = !this.enable.Value;
  }
}
