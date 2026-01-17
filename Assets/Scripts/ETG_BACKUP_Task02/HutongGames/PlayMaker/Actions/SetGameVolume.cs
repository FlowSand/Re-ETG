// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetGameVolume
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Audio)]
[HutongGames.PlayMaker.Tooltip("Sets the global sound volume.")]
public class SetGameVolume : FsmStateAction
{
  [RequiredField]
  [HasFloatSlider(0.0f, 1f)]
  public FsmFloat volume;
  public bool everyFrame;

  public override void Reset()
  {
    this.volume = (FsmFloat) 1f;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    AudioListener.volume = this.volume.Value;
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => AudioListener.volume = this.volume.Value;
}
