// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetAnimationSpeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Animation)]
  [HutongGames.PlayMaker.Tooltip("Sets the Speed of an Animation. Check Every Frame to update the animation time continuosly, e.g., if you're manipulating a variable that controls animation speed.")]
  public class SetAnimationSpeed : BaseAnimationAction
  {
    [CheckForComponent(typeof (Animation))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Animation)]
    [RequiredField]
    public FsmString animName;
    public FsmFloat speed = (FsmFloat) 1f;
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.animName = (FsmString) null;
      this.speed = (FsmFloat) 1f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetAnimationSpeed(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner);
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      this.DoSetAnimationSpeed(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner);
    }

    private void DoSetAnimationSpeed(GameObject go)
    {
      if (!this.UpdateCache(go))
        return;
      AnimationState animationState = this.animation[this.animName.Value];
      if ((TrackedReference) animationState == (TrackedReference) null)
        this.LogWarning("Missing animation: " + this.animName.Value);
      else
        animationState.speed = this.speed.Value;
    }
  }
}
