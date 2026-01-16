// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetAudioLoop
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Audio)]
[HutongGames.PlayMaker.Tooltip("Sets looping on the AudioSource component on a Game Object.")]
public class SetAudioLoop : ComponentAction<AudioSource>
{
  [CheckForComponent(typeof (AudioSource))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  public FsmBool loop;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.loop = (FsmBool) false;
  }

  public override void OnEnter()
  {
    if (this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      this.audio.loop = this.loop.Value;
    this.Finish();
  }
}
