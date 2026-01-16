// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetFsmEnum
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Get the value of an Enum Variable from another FSM.")]
[ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
[ActionCategory(ActionCategory.StateMachine)]
public class GetFsmEnum : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The target FSM")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.FsmName)]
  [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
  public FsmString fsmName;
  [UIHint(UIHint.FsmBool)]
  [RequiredField]
  public FsmString variableName;
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmEnum storeValue;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
  public bool everyFrame;
  private GameObject goLastFrame;
  private PlayMakerFSM fsm;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.fsmName = (FsmString) string.Empty;
    this.storeValue = (FsmEnum) null;
  }

  public override void OnEnter()
  {
    this.DoGetFsmEnum();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoGetFsmEnum();

  private void DoGetFsmEnum()
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
    FsmEnum fsmEnum = this.fsm.FsmVariables.GetFsmEnum(this.variableName.Value);
    if (fsmEnum == null)
      return;
    this.storeValue.Value = fsmEnum.Value;
  }
}
