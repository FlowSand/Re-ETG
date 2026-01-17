// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TouchEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Device)]
[HutongGames.PlayMaker.Tooltip("Sends events based on Touch Phases. Optionally filter by a fingerID.")]
public class TouchEvent : FsmStateAction
{
  public FsmInt fingerId;
  public TouchPhase touchPhase;
  public FsmEvent sendEvent;
  [UIHint(UIHint.Variable)]
  public FsmInt storeFingerId;

  public override void Reset()
  {
    FsmInt fsmInt = new FsmInt();
    fsmInt.UseVariable = true;
    this.fingerId = fsmInt;
    this.storeFingerId = (FsmInt) null;
  }

  public override void OnUpdate()
  {
    if (Input.touchCount <= 0)
      return;
    foreach (Touch touch in Input.touches)
    {
      if ((this.fingerId.IsNone || touch.fingerId == this.fingerId.Value) && touch.phase == this.touchPhase)
      {
        this.storeFingerId.Value = touch.fingerId;
        this.Fsm.Event(this.sendEvent);
      }
    }
  }
}
