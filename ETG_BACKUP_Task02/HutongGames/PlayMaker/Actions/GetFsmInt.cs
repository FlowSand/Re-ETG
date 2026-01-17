// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetFsmInt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
[HutongGames.PlayMaker.Tooltip("Get the value of an Integer Variable from another FSM.")]
[ActionCategory(ActionCategory.StateMachine)]
public class GetFsmInt : FsmStateAction
{
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
  [UIHint(UIHint.FsmName)]
  public FsmString fsmName;
  [UIHint(UIHint.FsmInt)]
  [RequiredField]
  public FsmString variableName;
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmInt storeValue;
  public bool everyFrame;
  private GameObject goLastFrame;
  private PlayMakerFSM fsm;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.fsmName = (FsmString) string.Empty;
    this.storeValue = (FsmInt) null;
  }

  public override void OnEnter()
  {
    this.DoGetFsmInt();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetFsmInt();

  private void DoGetFsmInt()
  {
    if (this.storeValue == null)
      return;
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    if ((Object) ownerDefaultTarget != (Object) this.goLastFrame)
    {
      this.goLastFrame = ownerDefaultTarget;
      this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
    }
    if ((Object) this.fsm == (Object) null)
      return;
    FsmInt fsmInt = this.fsm.FsmVariables.GetFsmInt(this.variableName.Value);
    if (fsmInt == null)
      return;
    this.storeValue.Value = fsmInt.Value;
  }
}
