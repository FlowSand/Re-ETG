// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetKeyDown
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sends an Event when a Key is pressed.")]
[ActionCategory(ActionCategory.Input)]
public class GetKeyDown : FsmStateAction
{
  [RequiredField]
  public KeyCode key;
  public FsmEvent sendEvent;
  [UIHint(UIHint.Variable)]
  public FsmBool storeResult;

  public override void Reset()
  {
    this.sendEvent = (FsmEvent) null;
    this.key = KeyCode.None;
    this.storeResult = (FsmBool) null;
  }

  public override void OnUpdate()
  {
    bool keyDown = Input.GetKeyDown(this.key);
    if (keyDown)
      this.Fsm.Event(this.sendEvent);
    this.storeResult.Value = keyDown;
  }
}
