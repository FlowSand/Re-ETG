using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory("2D Toolkit/SpriteAnimator")]
  [HutongGames.PlayMaker.Tooltip("Set the current clip frames per seconds on a animated sprite. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
  public class Tk2dSetAnimationFrameRate : FsmStateAction
  {
    [CheckForComponent(typeof (tk2dSpriteAnimator))]
    [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The frame per seconds of the current clip")]
    [RequiredField]
    public FsmFloat framePerSeconds;
    [HutongGames.PlayMaker.Tooltip("Repeat every Frame")]
    public bool everyFrame;
    private tk2dSpriteAnimator _sprite;

    private void _getSprite()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
    }

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.framePerSeconds = (FsmFloat) 30f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this._getSprite();
      this.DoSetAnimationFPS();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetAnimationFPS();

    private void DoSetAnimationFPS()
    {
      if ((Object) this._sprite == (Object) null)
        this.LogWarning("Missing tk2dSpriteAnimator component");
      else
        this._sprite.CurrentClip.fps = this.framePerSeconds.Value;
    }
  }
}
