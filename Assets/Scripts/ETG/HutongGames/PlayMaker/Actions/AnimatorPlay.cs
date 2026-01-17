// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AnimatorPlay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Plays a state. This could be used to synchronize your animation with audio or synchronize an Animator over the network.")]
  [ActionCategory(ActionCategory.Animator)]
  public class AnimatorPlay : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
    [CheckForComponent(typeof (Animator))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The name of the state that will be played.")]
    public FsmString stateName;
    [HutongGames.PlayMaker.Tooltip("The layer where the state is.")]
    public FsmInt layer;
    [HutongGames.PlayMaker.Tooltip("The normalized time at which the state will play")]
    public FsmFloat normalizedTime;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when using normalizedTime to manually control the animation.")]
    public bool everyFrame;
    private Animator _animator;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.stateName = (FsmString) null;
      FsmInt fsmInt = new FsmInt();
      fsmInt.UseVariable = true;
      this.layer = fsmInt;
      FsmFloat fsmFloat = new FsmFloat();
      fsmFloat.UseVariable = true;
      this.normalizedTime = fsmFloat;
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
        this.DoAnimatorPlay();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }

    public override void OnUpdate() => this.DoAnimatorPlay();

    private void DoAnimatorPlay()
    {
      if (!((Object) this._animator != (Object) null))
        return;
      this._animator.Play(this.stateName.Value, !this.layer.IsNone ? this.layer.Value : -1, !this.normalizedTime.IsNone ? this.normalizedTime.Value : float.NegativeInfinity);
    }
  }
}
