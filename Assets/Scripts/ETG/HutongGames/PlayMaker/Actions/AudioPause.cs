// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AudioPause
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Audio)]
  [HutongGames.PlayMaker.Tooltip("Pauses playing the Audio Clip played by an Audio Source component on a Game Object.")]
  public class AudioPause : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject with an Audio Source component.")]
    [RequiredField]
    [CheckForComponent(typeof (AudioSource))]
    public FsmOwnerDefault gameObject;

    public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

    public override void OnEnter()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget != (Object) null)
      {
        AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
        if ((Object) component != (Object) null)
          component.Pause();
      }
      this.Finish();
    }
  }
}
