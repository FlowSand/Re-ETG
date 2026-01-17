// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dIsPlaying
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Check if a sprite animation is playing. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
[ActionCategory("2D Toolkit/SpriteAnimator")]
[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W720")]
public class Tk2dIsPlaying : FsmStateAction
{
  [CheckForComponent(typeof (tk2dSpriteAnimator))]
  [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The clip name to play")]
  [RequiredField]
  public FsmString clipName;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("is the clip playing?")]
  public FsmBool isPlaying;
  [HutongGames.PlayMaker.Tooltip("EVvnt sent if clip is playing")]
  public FsmEvent isPlayingEvent;
  [HutongGames.PlayMaker.Tooltip("Event sent if clip is not playing")]
  public FsmEvent isNotPlayingEvent;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyframe;
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
    this.clipName = (FsmString) null;
    this.everyframe = false;
    this.isPlayingEvent = (FsmEvent) null;
    this.isNotPlayingEvent = (FsmEvent) null;
  }

  public override void OnEnter()
  {
    this._getSprite();
    this.DoIsPlaying();
    if (this.everyframe)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoIsPlaying();

  private void DoIsPlaying()
  {
    if ((Object) this._sprite == (Object) null)
    {
      this.LogWarning("Missing tk2dSpriteAnimator component: " + this._sprite.gameObject.name);
    }
    else
    {
      bool flag = this._sprite.IsPlaying(this.clipName.Value);
      this.isPlaying.Value = flag;
      if (flag)
        this.Fsm.Event(this.isPlayingEvent);
      else
        this.Fsm.Event(this.isNotPlayingEvent);
    }
  }
}
