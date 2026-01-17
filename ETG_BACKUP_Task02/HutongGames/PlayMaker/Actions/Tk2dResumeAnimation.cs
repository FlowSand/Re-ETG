// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dResumeAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W721")]
[HutongGames.PlayMaker.Tooltip("Resume a sprite animation. Use Tk2dPauseAnimation for dynamic control. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
[ActionCategory("2D Toolkit/SpriteAnimator")]
public class Tk2dResumeAnimation : FsmStateAction
{
  [CheckForComponent(typeof (tk2dSpriteAnimator))]
  [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  private tk2dSpriteAnimator _sprite;

  private void _getSprite()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
  }

  public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

  public override void OnEnter()
  {
    this._getSprite();
    this.DoResumeAnimation();
    this.Finish();
  }

  private void DoResumeAnimation()
  {
    if ((Object) this._sprite == (Object) null)
    {
      this.LogWarning("Missing tk2dSpriteAnimator component");
    }
    else
    {
      if (!this._sprite.Paused)
        return;
      this._sprite.Resume();
    }
  }
}
