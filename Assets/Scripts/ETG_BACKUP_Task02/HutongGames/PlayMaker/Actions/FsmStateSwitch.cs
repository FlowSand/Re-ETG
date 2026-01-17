// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FsmStateSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sends Events based on the current State of an FSM.")]
[ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
[ActionCategory(ActionCategory.Logic)]
public class FsmStateSwitch : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
  [RequiredField]
  public FsmGameObject gameObject;
  [UIHint(UIHint.FsmName)]
  [HutongGames.PlayMaker.Tooltip("Optional name of Fsm on GameObject. Useful if there is more than one FSM on the GameObject.")]
  public FsmString fsmName;
  [CompoundArray("State Switches", "Compare State", "Send Event")]
  public FsmString[] compareTo;
  public FsmEvent[] sendEvent;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if you're waiting for a particular result.")]
  public bool everyFrame;
  private GameObject previousGo;
  private PlayMakerFSM fsm;

  public override void Reset()
  {
    this.gameObject = (FsmGameObject) null;
    this.fsmName = (FsmString) null;
    this.compareTo = new FsmString[1];
    this.sendEvent = new FsmEvent[1];
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoFsmStateSwitch();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoFsmStateSwitch();

  private void DoFsmStateSwitch()
  {
    GameObject go = this.gameObject.Value;
    if ((Object) go == (Object) null)
      return;
    if ((Object) go != (Object) this.previousGo)
    {
      this.fsm = ActionHelpers.GetGameObjectFsm(go, this.fsmName.Value);
      this.previousGo = go;
    }
    if ((Object) this.fsm == (Object) null)
      return;
    string activeStateName = this.fsm.ActiveStateName;
    for (int index = 0; index < this.compareTo.Length; ++index)
    {
      if (activeStateName == this.compareTo[index].Value)
      {
        this.Fsm.Event(this.sendEvent[index]);
        break;
      }
    }
  }
}
