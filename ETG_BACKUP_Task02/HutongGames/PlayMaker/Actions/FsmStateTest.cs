// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FsmStateTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Tests if an FSM is in the specified State.")]
[ActionCategory(ActionCategory.Logic)]
[ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
public class FsmStateTest : FsmStateAction
{
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
  public FsmGameObject gameObject;
  [HutongGames.PlayMaker.Tooltip("Optional name of Fsm on Game Object. Useful if there is more than one FSM on the GameObject.")]
  [UIHint(UIHint.FsmName)]
  public FsmString fsmName;
  [HutongGames.PlayMaker.Tooltip("Check to see if the FSM is in this state.")]
  [RequiredField]
  public FsmString stateName;
  [HutongGames.PlayMaker.Tooltip("Event to send if the FSM is in the specified state.")]
  public FsmEvent trueEvent;
  [HutongGames.PlayMaker.Tooltip("Event to send if the FSM is NOT in the specified state.")]
  public FsmEvent falseEvent;
  [HutongGames.PlayMaker.Tooltip("Store the result of this test in a bool variable. Useful if other actions depend on this test.")]
  [UIHint(UIHint.Variable)]
  public FsmBool storeResult;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if you're waiting for a particular state.")]
  public bool everyFrame;
  private GameObject previousGo;
  private PlayMakerFSM fsm;

  public override void Reset()
  {
    this.gameObject = (FsmGameObject) null;
    this.fsmName = (FsmString) null;
    this.stateName = (FsmString) null;
    this.trueEvent = (FsmEvent) null;
    this.falseEvent = (FsmEvent) null;
    this.storeResult = (FsmBool) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoFsmStateTest();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoFsmStateTest();

  private void DoFsmStateTest()
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
    bool flag = false;
    if (this.fsm.ActiveStateName == this.stateName.Value)
    {
      this.Fsm.Event(this.trueEvent);
      flag = true;
    }
    else
      this.Fsm.Event(this.falseEvent);
    this.storeResult.Value = flag;
  }
}
