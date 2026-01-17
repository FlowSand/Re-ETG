// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetAnimationWeight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the Blend Weight of an Animation. Check Every Frame to update the weight continuosly, e.g., if you're manipulating a variable that controls the weight.")]
[ActionCategory(ActionCategory.Animation)]
public class SetAnimationWeight : BaseAnimationAction
{
  [CheckForComponent(typeof (Animation))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.Animation)]
  [RequiredField]
  public FsmString animName;
  public FsmFloat weight = (FsmFloat) 1f;
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.animName = (FsmString) null;
    this.weight = (FsmFloat) 1f;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoSetAnimationWeight(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner);
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate()
  {
    this.DoSetAnimationWeight(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner);
  }

  private void DoSetAnimationWeight(GameObject go)
  {
    if (!this.UpdateCache(go))
      return;
    AnimationState animationState = this.animation[this.animName.Value];
    if ((TrackedReference) animationState == (TrackedReference) null)
      this.LogWarning("Missing animation: " + this.animName.Value);
    else
      animationState.weight = this.weight.Value;
  }
}
