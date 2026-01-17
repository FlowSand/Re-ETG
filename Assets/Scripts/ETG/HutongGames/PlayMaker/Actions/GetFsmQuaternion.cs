// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetFsmQuaternion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.StateMachine)]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  [HutongGames.PlayMaker.Tooltip("Get the value of a Quaternion Variable from another FSM.")]
  public class GetFsmQuaternion : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.FsmName)]
    [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
    public FsmString fsmName;
    [UIHint(UIHint.FsmQuaternion)]
    [RequiredField]
    public FsmString variableName;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmQuaternion storeValue;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;
    private GameObject goLastFrame;
    protected PlayMakerFSM fsm;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.variableName = (FsmString) string.Empty;
      this.storeValue = (FsmQuaternion) null;
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
      FsmQuaternion fsmQuaternion = this.fsm.FsmVariables.GetFsmQuaternion(this.variableName.Value);
      if (fsmQuaternion == null)
        return;
      this.storeValue.Value = fsmQuaternion.Value;
    }
  }
}
