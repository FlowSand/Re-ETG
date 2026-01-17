// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AnyKey
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sends an Event when the user hits any Key or Mouse Button.")]
[ActionCategory(ActionCategory.Input)]
public class AnyKey : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Event to send when any Key or Mouse Button is pressed.")]
  [RequiredField]
  public FsmEvent sendEvent;

  public override void Reset() => this.sendEvent = (FsmEvent) null;

  public override void OnUpdate()
  {
    if (!Input.anyKeyDown)
      return;
    this.Fsm.Event(this.sendEvent);
  }
}
