using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory("2D Toolkit/Sprite")]
  [HutongGames.PlayMaker.Tooltip("Set the scale of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
  public class Tk2dSpriteSetScale : FsmStateAction
  {
    [CheckForComponent(typeof (tk2dBaseSprite))]
    [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The scale Id")]
    [UIHint(UIHint.FsmVector3)]
    public FsmVector3 scale;
    [ActionSection("")]
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyframe;
    private tk2dBaseSprite _sprite;

    private void _getSprite()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      this._sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
    }

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.scale = (FsmVector3) new Vector3(1f, 1f, 1f);
      this.everyframe = false;
    }

    public override void OnEnter()
    {
      this._getSprite();
      this.DoSetSpriteScale();
      if (this.everyframe)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetSpriteScale();

    private void DoSetSpriteScale()
    {
      if ((Object) this._sprite == (Object) null)
      {
        this.LogWarning("Missing tk2dBaseSprite component");
      }
      else
      {
        if (!(this._sprite.scale != this.scale.Value))
          return;
        this._sprite.scale = this.scale.Value;
      }
    }
  }
}
