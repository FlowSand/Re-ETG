// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DwarfEventAreaEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Responds to trigger events with Speculative Rigidbodies.")]
[ActionCategory(".Brave")]
public class DwarfEventAreaEvent : FsmStateAction
{
  [CompoundArray("Events", "Trigger Index", "Send Event")]
  [HutongGames.PlayMaker.Tooltip("Event to play when the corresponding trigger detects a collision.")]
  public FsmInt[] eventIndices;
  public FsmEvent[] events;
  private DwarfEventListener m_eventListener;

  public override void Reset()
  {
    this.eventIndices = new FsmInt[0];
    this.events = new FsmEvent[0];
  }

  public override void OnEnter()
  {
    this.m_eventListener = this.Owner.GetComponent<DwarfEventListener>();
    if (!(bool) (UnityEngine.Object) this.m_eventListener)
      return;
    this.m_eventListener.OnTrigger += new Action<int>(this.OnTrigger);
  }

  public override void OnExit()
  {
    if (!(bool) (UnityEngine.Object) this.m_eventListener)
      return;
    this.m_eventListener.OnTrigger -= new Action<int>(this.OnTrigger);
  }

  private void OnTrigger(int index)
  {
    for (int index1 = 0; index1 < this.eventIndices.Length; ++index1)
    {
      if (this.eventIndices[index1].Value == index)
        this.Fsm.Event(this.events[index1]);
    }
  }
}
