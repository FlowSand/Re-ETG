// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AxisEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sends events based on the direction of Input Axis (Left/Right/Up/Down...).")]
[ActionCategory(ActionCategory.Input)]
public class AxisEvent : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Horizontal axis as defined in the Input Manager")]
  public FsmString horizontalAxis;
  [HutongGames.PlayMaker.Tooltip("Vertical axis as defined in the Input Manager")]
  public FsmString verticalAxis;
  [HutongGames.PlayMaker.Tooltip("Event to send if input is to the left.")]
  public FsmEvent leftEvent;
  [HutongGames.PlayMaker.Tooltip("Event to send if input is to the right.")]
  public FsmEvent rightEvent;
  [HutongGames.PlayMaker.Tooltip("Event to send if input is to the up.")]
  public FsmEvent upEvent;
  [HutongGames.PlayMaker.Tooltip("Event to send if input is to the down.")]
  public FsmEvent downEvent;
  [HutongGames.PlayMaker.Tooltip("Event to send if input is in any direction.")]
  public FsmEvent anyDirection;
  [HutongGames.PlayMaker.Tooltip("Event to send if no axis input (centered).")]
  public FsmEvent noDirection;

  public override void Reset()
  {
    this.horizontalAxis = (FsmString) "Horizontal";
    this.verticalAxis = (FsmString) "Vertical";
    this.leftEvent = (FsmEvent) null;
    this.rightEvent = (FsmEvent) null;
    this.upEvent = (FsmEvent) null;
    this.downEvent = (FsmEvent) null;
    this.anyDirection = (FsmEvent) null;
    this.noDirection = (FsmEvent) null;
  }

  public override void OnUpdate()
  {
    float x = !(this.horizontalAxis.Value != string.Empty) ? 0.0f : Input.GetAxis(this.horizontalAxis.Value);
    float y = !(this.verticalAxis.Value != string.Empty) ? 0.0f : Input.GetAxis(this.verticalAxis.Value);
    if (((float) ((double) x * (double) x + (double) y * (double) y)).Equals(0.0f))
    {
      if (this.noDirection == null)
        return;
      this.Fsm.Event(this.noDirection);
    }
    else
    {
      float num1 = (float) ((double) Mathf.Atan2(y, x) * 57.295780181884766 + 45.0);
      if ((double) num1 < 0.0)
        num1 += 360f;
      int num2 = (int) ((double) num1 / 90.0);
      if (num2 == 0 && this.rightEvent != null)
        this.Fsm.Event(this.rightEvent);
      else if (num2 == 1 && this.upEvent != null)
        this.Fsm.Event(this.upEvent);
      else if (num2 == 2 && this.leftEvent != null)
        this.Fsm.Event(this.leftEvent);
      else if (num2 == 3 && this.downEvent != null)
      {
        this.Fsm.Event(this.downEvent);
      }
      else
      {
        if (this.anyDirection == null)
          return;
        this.Fsm.Event(this.anyDirection);
      }
    }
  }
}
