// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dSpriteSetColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory("2D Toolkit/Sprite")]
[HutongGames.PlayMaker.Tooltip("Set the color of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
public class Tk2dSpriteSetColor : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
  [RequiredField]
  [CheckForComponent(typeof (tk2dBaseSprite))]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The color")]
  [UIHint(UIHint.FsmColor)]
  public FsmColor color;
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
    this.color = (FsmColor) null;
    this.everyframe = false;
  }

  public override void OnEnter()
  {
    this._getSprite();
    this.DoSetSpriteColor();
    if (this.everyframe)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoSetSpriteColor();

  private void DoSetSpriteColor()
  {
    if ((Object) this._sprite == (Object) null)
    {
      this.LogWarning("Missing tk2dBaseSprite component");
    }
    else
    {
      if (!(this._sprite.color != this.color.Value))
        return;
      this._sprite.color = this.color.Value;
    }
  }
}
