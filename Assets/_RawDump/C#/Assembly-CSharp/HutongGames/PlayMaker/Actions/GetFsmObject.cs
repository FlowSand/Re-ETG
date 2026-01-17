// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetFsmObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Get the value of an Object Variable from another FSM.")]
[ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
[ActionCategory(ActionCategory.StateMachine)]
public class GetFsmObject : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
  [UIHint(UIHint.FsmName)]
  public FsmString fsmName;
  [UIHint(UIHint.FsmObject)]
  [RequiredField]
  public FsmString variableName;
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmObject storeValue;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;
  private GameObject goLastFrame;
  protected PlayMakerFSM fsm;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.fsmName = (FsmString) string.Empty;
    this.variableName = (FsmString) string.Empty;
    this.storeValue = (FsmObject) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoGetFsmVariable();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetFsmVariable();

  private void DoGetFsmVariable()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    if ((Object) ownerDefaultTarget != (Object) this.goLastFrame)
    {
      this.goLastFrame = ownerDefaultTarget;
      this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
    }
    if ((Object) this.fsm == (Object) null || this.storeValue == null)
      return;
    FsmObject fsmObject = this.fsm.FsmVariables.GetFsmObject(this.variableName.Value);
    if (fsmObject == null)
      return;
    this.storeValue.Value = fsmObject.Value;
  }
}
