// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetFsmVariable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Set the value of a variable in another FSM.")]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  [ActionCategory(ActionCategory.StateMachine)]
  public class SetFsmVariable : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
    [UIHint(UIHint.FsmName)]
    public FsmString fsmName;
    [HutongGames.PlayMaker.Tooltip("The name of the variable in the target FSM.")]
    public FsmString variableName;
    [RequiredField]
    public FsmVar setValue;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;
    private PlayMakerFSM targetFsm;
    private NamedVariable targetVariable;
    private INamedVariable sourceVariable;
    private GameObject cachedGameObject;
    private string cachedFsmName;
    private string cachedVariableName;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.setValue = new FsmVar();
    }

    public override void OnEnter()
    {
      this.DoSetFsmVariable();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetFsmVariable();

    private void DoSetFsmVariable()
    {
      if (this.setValue.IsNone || string.IsNullOrEmpty(this.variableName.Value))
        return;
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      if ((Object) ownerDefaultTarget != (Object) this.cachedGameObject || this.fsmName.Value != this.cachedFsmName)
      {
        this.targetFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
        if ((Object) this.targetFsm == (Object) null)
          return;
        this.cachedGameObject = ownerDefaultTarget;
        this.cachedFsmName = this.fsmName.Value;
      }
      if (this.variableName.Value != this.cachedVariableName)
      {
        this.targetVariable = this.targetFsm.FsmVariables.FindVariable(this.setValue.Type, this.variableName.Value);
        this.cachedVariableName = this.variableName.Value;
      }
      if (this.targetVariable == null)
        this.LogWarning("Missing Variable: " + this.variableName.Value);
      else
        this.setValue.ApplyValueTo((INamedVariable) this.targetVariable);
    }
  }
}
