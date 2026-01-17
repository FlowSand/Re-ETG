// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetMouseButtonUp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sends an Event when the specified Mouse Button is released. Optionally store the button state in a bool variable.")]
[ActionCategory(ActionCategory.Input)]
public class GetMouseButtonUp : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The mouse button to test.")]
  [RequiredField]
  public MouseButton button;
  [HutongGames.PlayMaker.Tooltip("Event to send if the mouse button is down.")]
  public FsmEvent sendEvent;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Store the pressed state in a Bool Variable.")]
  public FsmBool storeResult;
  [HutongGames.PlayMaker.Tooltip("Uncheck to run when entering the state.")]
  public bool inUpdateOnly;

  public override void Reset()
  {
    this.button = MouseButton.Left;
    this.sendEvent = (FsmEvent) null;
    this.storeResult = (FsmBool) null;
    this.inUpdateOnly = true;
  }

  public override void OnEnter()
  {
    if (this.inUpdateOnly)
      return;
    this.DoGetMouseButtonUp();
  }

  public override void OnUpdate() => this.DoGetMouseButtonUp();

  public void DoGetMouseButtonUp()
  {
    bool mouseButtonUp = Input.GetMouseButtonUp((int) this.button);
    if (mouseButtonUp)
      this.Fsm.Event(this.sendEvent);
    this.storeResult.Value = mouseButtonUp;
  }
}
