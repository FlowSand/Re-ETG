// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.RemoveMixingTransform
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Removes a mixing transform previously added with Add Mixing Transform. If transform has been added as recursive, then it will be removed as recursive. Once you remove all mixing transforms added to animation state all curves become animated again.")]
  [ActionCategory(ActionCategory.Animation)]
  public class RemoveMixingTransform : BaseAnimationAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject playing the animation.")]
    [CheckForComponent(typeof (Animation))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The name of the animation.")]
    public FsmString animationName;
    [HutongGames.PlayMaker.Tooltip("The mixing transform to remove. E.g., root/upper_body/left_shoulder")]
    [RequiredField]
    public FsmString transfrom;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.animationName = (FsmString) string.Empty;
    }

    public override void OnEnter()
    {
      this.DoRemoveMixingTransform();
      this.Finish();
    }

    private void DoRemoveMixingTransform()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if (!this.UpdateCache(ownerDefaultTarget))
        return;
      AnimationState animationState = this.animation[this.animationName.Value];
      if ((TrackedReference) animationState == (TrackedReference) null)
        return;
      Transform mix = ownerDefaultTarget.transform.Find(this.transfrom.Value);
      animationState.AddMixingTransform(mix);
    }
  }
}
