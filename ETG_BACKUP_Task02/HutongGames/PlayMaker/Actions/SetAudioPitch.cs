// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetAudioPitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the Pitch of the Audio Clip played by the AudioSource component on a Game Object.")]
[ActionCategory(ActionCategory.Audio)]
public class SetAudioPitch : ComponentAction<AudioSource>
{
  [CheckForComponent(typeof (AudioSource))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  public FsmFloat pitch;
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.pitch = (FsmFloat) 1f;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoSetAudioPitch();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoSetAudioPitch();

  private void DoSetAudioPitch()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)) || this.pitch.IsNone)
      return;
    this.audio.pitch = this.pitch.Value;
  }
}
