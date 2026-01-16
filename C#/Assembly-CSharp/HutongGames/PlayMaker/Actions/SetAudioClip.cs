// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetAudioClip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the Audio Clip played by the AudioSource component on a Game Object.")]
[ActionCategory(ActionCategory.Audio)]
public class SetAudioClip : ComponentAction<AudioSource>
{
  [HutongGames.PlayMaker.Tooltip("The GameObject with the AudioSource component.")]
  [CheckForComponent(typeof (AudioSource))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The AudioClip to set.")]
  [ObjectType(typeof (AudioClip))]
  public FsmObject audioClip;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.audioClip = (FsmObject) null;
  }

  public override void OnEnter()
  {
    if (this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      this.audio.clip = this.audioClip.Value as AudioClip;
    this.Finish();
  }
}
