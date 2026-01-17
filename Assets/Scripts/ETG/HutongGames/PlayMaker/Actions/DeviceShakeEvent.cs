// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DeviceShakeEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Device)]
  [HutongGames.PlayMaker.Tooltip("Sends an Event when the mobile device is shaken.")]
  public class DeviceShakeEvent : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Amount of acceleration required to trigger the event. Higher numbers require a harder shake.")]
    [RequiredField]
    public FsmFloat shakeThreshold;
    [HutongGames.PlayMaker.Tooltip("Event to send when Shake Threshold is exceded.")]
    [RequiredField]
    public FsmEvent sendEvent;

    public override void Reset()
    {
      this.shakeThreshold = (FsmFloat) 3f;
      this.sendEvent = (FsmEvent) null;
    }

    public override void OnUpdate()
    {
      if ((double) Input.acceleration.sqrMagnitude <= (double) this.shakeThreshold.Value * (double) this.shakeThreshold.Value)
        return;
      this.Fsm.Event(this.sendEvent);
    }
  }
}
