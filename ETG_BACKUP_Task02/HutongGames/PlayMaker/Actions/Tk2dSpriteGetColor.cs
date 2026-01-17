// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dSpriteGetColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Get the color of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
[ActionCategory("2D Toolkit/Sprite")]
public class Tk2dSpriteGetColor : FsmStateAction
{
  [CheckForComponent(typeof (tk2dBaseSprite))]
  [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The color")]
  public FsmColor color;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  [ActionSection("")]
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
    this.color = (FsmColor) null;
    this.everyframe = false;
  }

  public override void OnEnter()
  {
    this._getSprite();
    this.DoGetSpriteColor();
    if (this.everyframe)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetSpriteColor();

  private void DoGetSpriteColor()
  {
    if ((Object) this._sprite == (Object) null)
    {
      this.LogWarning("Missing tk2dBaseSprite component");
    }
    else
    {
      if (!(this._sprite.color != this.color.Value))
        return;
      this.color.Value = this._sprite.color;
    }
  }
}
