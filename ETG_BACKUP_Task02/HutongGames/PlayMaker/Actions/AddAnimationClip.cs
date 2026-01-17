// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AddAnimationClip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Adds a named Animation Clip to a Game Object. Optionally trims the Animation.")]
[ActionCategory(ActionCategory.Animation)]
public class AddAnimationClip : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The GameObject to add the Animation Clip to.")]
  [CheckForComponent(typeof (Animation))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The animation clip to add. NOTE: Make sure the clip is compatible with the object's hierarchy.")]
  [ObjectType(typeof (AnimationClip))]
  [RequiredField]
  public FsmObject animationClip;
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("Name the animation. Used by other actions to reference this animation.")]
  public FsmString animationName;
  [HutongGames.PlayMaker.Tooltip("Optionally trim the animation by specifying a first and last frame.")]
  public FsmInt firstFrame;
  [HutongGames.PlayMaker.Tooltip("Optionally trim the animation by specifying a first and last frame.")]
  public FsmInt lastFrame;
  [HutongGames.PlayMaker.Tooltip("Add an extra looping frame that matches the first frame.")]
  public FsmBool addLoopFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.animationClip = (FsmObject) null;
    this.animationName = (FsmString) string.Empty;
    this.firstFrame = (FsmInt) 0;
    this.lastFrame = (FsmInt) 0;
    this.addLoopFrame = (FsmBool) false;
  }

  public override void OnEnter()
  {
    this.DoAddAnimationClip();
    this.Finish();
  }

  private void DoAddAnimationClip()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    AnimationClip clip = this.animationClip.Value as AnimationClip;
    if ((Object) clip == (Object) null)
      return;
    Animation component = ownerDefaultTarget.GetComponent<Animation>();
    if (this.firstFrame.Value == 0 && this.lastFrame.Value == 0)
      component.AddClip(clip, this.animationName.Value);
    else
      component.AddClip(clip, this.animationName.Value, this.firstFrame.Value, this.lastFrame.Value, this.addLoopFrame.Value);
  }
}
