// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetFsmVariable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get the value of a variable in another FSM and store it in a variable of the same name in this FSM.")]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  [ActionCategory(ActionCategory.StateMachine)]
  public class GetFsmVariable : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM")]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
    [UIHint(UIHint.FsmName)]
    public FsmString fsmName;
    [RequiredField]
    [HideTypeFilter]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the value of the FsmVariable")]
    public FsmVar storeValue;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;
    private GameObject cachedGO;
    private PlayMakerFSM sourceFsm;
    private INamedVariable sourceVariable;
    private NamedVariable targetVariable;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.storeValue = new FsmVar();
    }

    public override void OnEnter()
    {
      this.InitFsmVar();
      this.DoGetFsmVariable();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetFsmVariable();

    private void InitFsmVar()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null || !((Object) ownerDefaultTarget != (Object) this.cachedGO))
        return;
      this.sourceFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
      this.sourceVariable = (INamedVariable) this.sourceFsm.FsmVariables.GetVariable(this.storeValue.variableName);
      this.targetVariable = this.Fsm.Variables.GetVariable(this.storeValue.variableName);
      this.storeValue.Type = this.targetVariable.VariableType;
      if (!string.IsNullOrEmpty(this.storeValue.variableName) && this.sourceVariable == null)
        this.LogWarning("Missing Variable: " + this.storeValue.variableName);
      this.cachedGO = ownerDefaultTarget;
    }

    private void DoGetFsmVariable()
    {
      if (this.storeValue.IsNone)
        return;
      this.InitFsmVar();
      this.storeValue.GetValueFrom(this.sourceVariable);
      this.storeValue.ApplyValueTo((INamedVariable) this.targetVariable);
    }
  }
}
