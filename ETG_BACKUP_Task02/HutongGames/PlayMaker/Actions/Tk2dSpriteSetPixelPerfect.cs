// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dSpriteSetPixelPerfect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory("2D Toolkit/Sprite")]
[HutongGames.PlayMaker.Tooltip("Set the pixel perfect flag of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
public class Tk2dSpriteSetPixelPerfect : FsmStateAction
{
  [CheckForComponent(typeof (tk2dBaseSprite))]
  [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.FsmBool)]
  [HutongGames.PlayMaker.Tooltip("Does the sprite needs to be kept pixelPerfect? This is only necessary when using a perspective camera.")]
  public FsmBool pixelPerfect;
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
    this.pixelPerfect = (FsmBool) null;
    this.everyframe = false;
  }

  public override void OnEnter()
  {
    this._getSprite();
    this.DoSetSpritePixelPerfect();
    if (this.everyframe)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoSetSpritePixelPerfect();

  private void DoSetSpritePixelPerfect()
  {
    if ((Object) this._sprite == (Object) null)
    {
      this.LogWarning("Missing tk2dBaseSprite component");
    }
    else
    {
      if (!this.pixelPerfect.Value)
        return;
      this._sprite.MakePixelPerfect();
    }
  }
}
