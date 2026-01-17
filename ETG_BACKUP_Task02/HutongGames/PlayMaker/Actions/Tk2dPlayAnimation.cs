// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dPlayAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory("2D Toolkit/SpriteAnimator")]
[HutongGames.PlayMaker.Tooltip("Plays a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
public class Tk2dPlayAnimation : FsmStateAction
{
  [CheckForComponent(typeof (tk2dSpriteAnimator))]
  [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The anim Lib name. Leave empty to use the one current selected")]
  public FsmString animLibName;
  [HutongGames.PlayMaker.Tooltip("The clip name to play")]
  [RequiredField]
  public FsmString clipName;
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
    this.animLibName = (FsmString) null;
    this.clipName = (FsmString) null;
  }

  public override void OnEnter()
  {
    this._getSprite();
    this.DoPlayAnimation();
  }

  private void DoPlayAnimation()
  {
    if ((Object) this._sprite == (Object) null)
    {
      this.LogWarning("Missing tk2dSpriteAnimator component");
    }
    else
    {
      if (this.animLibName.Value.Equals(string.Empty))
        ;
      this._sprite.Play(this.clipName.Value);
    }
  }
}
