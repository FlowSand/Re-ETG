// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetButtonDown
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sends an Event when a Button is pressed.")]
[ActionCategory(ActionCategory.Input)]
public class GetButtonDown : FsmStateAction
{
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The name of the button. Set in the Unity Input Manager.")]
  public FsmString buttonName;
  [HutongGames.PlayMaker.Tooltip("Event to send if the button is pressed.")]
  public FsmEvent sendEvent;
  [HutongGames.PlayMaker.Tooltip("Set to True if the button is pressed.")]
  [UIHint(UIHint.Variable)]
  public FsmBool storeResult;

  public override void Reset()
  {
    this.buttonName = (FsmString) "Fire1";
    this.sendEvent = (FsmEvent) null;
    this.storeResult = (FsmBool) null;
  }

  public override void OnUpdate()
  {
    bool buttonDown = Input.GetButtonDown(this.buttonName.Value);
    if (buttonDown)
      this.Fsm.Event(this.sendEvent);
    this.storeResult.Value = buttonDown;
  }
}
