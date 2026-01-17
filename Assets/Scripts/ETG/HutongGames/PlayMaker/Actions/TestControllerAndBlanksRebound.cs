// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TestControllerAndBlanksRebound
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".NPCs")]
  public class TestControllerAndBlanksRebound : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Event to send if the player is in the foyer.")]
    public FsmEvent isTrue;
    [HutongGames.PlayMaker.Tooltip("Event to send if the player is not in the foyer.")]
    public FsmEvent isFalse;
    [HutongGames.PlayMaker.Tooltip("Event to send if the player is using a Switch")]
    public FsmEvent isSwitch;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.isTrue = (FsmEvent) null;
      this.isFalse = (FsmEvent) null;
      this.isSwitch = (FsmEvent) null;
      this.everyFrame = false;
    }

    private void HandleEvents()
    {
      if (Application.platform == RuntimePlatform.PS4 || Application.platform == RuntimePlatform.XboxOne)
      {
        if (GameManager.Options.additionalBlankControl != GameOptions.ControllerBlankControl.BOTH_STICKS_DOWN && GameManager.Options.additionalBlankControl != GameOptions.ControllerBlankControl.NONE)
          this.Fsm.Event(this.isTrue);
        else
          this.Fsm.Event(this.isFalse);
      }
      else if ((Object) BraveInput.PrimaryPlayerInstance != (Object) null && !BraveInput.PrimaryPlayerInstance.IsKeyboardAndMouse())
      {
        if (GameManager.Options.additionalBlankControl != GameOptions.ControllerBlankControl.BOTH_STICKS_DOWN)
          this.Fsm.Event(this.isTrue);
        else
          this.Fsm.Event(this.isFalse);
      }
      else
        this.Fsm.Event(this.isFalse);
    }

    public override void OnEnter()
    {
      this.HandleEvents();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.HandleEvents();
  }
}
