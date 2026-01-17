// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AudioMute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Mute/unmute the Audio Clip played by an Audio Source component on a Game Object.")]
[ActionCategory(ActionCategory.Audio)]
public class AudioMute : FsmStateAction
{
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The GameObject with an Audio Source component.")]
  [CheckForComponent(typeof (AudioSource))]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Check to mute, uncheck to unmute.")]
  [RequiredField]
  public FsmBool mute;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.mute = (FsmBool) false;
  }

  public override void OnEnter()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget != (Object) null)
    {
      AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
      if ((Object) component != (Object) null)
        component.mute = this.mute.Value;
    }
    this.Finish();
  }
}
