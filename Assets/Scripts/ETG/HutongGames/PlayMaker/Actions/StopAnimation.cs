// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.StopAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Stops all playing Animations on a Game Object. Optionally, specify a single Animation to Stop.")]
  [ActionCategory(ActionCategory.Animation)]
  public class StopAnimation : BaseAnimationAction
  {
    [CheckForComponent(typeof (Animation))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Animation)]
    [HutongGames.PlayMaker.Tooltip("Leave empty to stop all playing animations.")]
    public FsmString animName;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.animName = (FsmString) null;
    }

    public override void OnEnter()
    {
      this.DoStopAnimation();
      this.Finish();
    }

    private void DoStopAnimation()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      if (FsmString.IsNullOrEmpty(this.animName))
        this.animation.Stop();
      else
        this.animation.Stop(this.animName.Value);
    }
  }
}
