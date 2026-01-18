using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get the values of multiple variables in another FSM and store in variables of the same name in this FSM.")]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  [ActionCategory(ActionCategory.StateMachine)]
  public class GetFsmVariables : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.FsmName)]
    [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
    public FsmString fsmName;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the values of the FsmVariables")]
    [HideTypeFilter]
    [RequiredField]
    public FsmVar[] getVariables;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;
    private GameObject cachedGO;
    private PlayMakerFSM sourceFsm;
    private INamedVariable[] sourceVariables;
    private NamedVariable[] targetVariables;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.getVariables = (FsmVar[]) null;
    }

    private void InitFsmVars()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null || !((Object) ownerDefaultTarget != (Object) this.cachedGO))
        return;
      this.sourceVariables = new INamedVariable[this.getVariables.Length];
      this.targetVariables = new NamedVariable[this.getVariables.Length];
      for (int index = 0; index < this.getVariables.Length; ++index)
      {
        string variableName = this.getVariables[index].variableName;
        this.sourceFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
        this.sourceVariables[index] = (INamedVariable) this.sourceFsm.FsmVariables.GetVariable(variableName);
        this.targetVariables[index] = this.Fsm.Variables.GetVariable(variableName);
        this.getVariables[index].Type = this.targetVariables[index].VariableType;
        if (!string.IsNullOrEmpty(variableName) && this.sourceVariables[index] == null)
          this.LogWarning("Missing Variable: " + variableName);
        this.cachedGO = ownerDefaultTarget;
      }
    }

    public override void OnEnter()
    {
      this.InitFsmVars();
      this.DoGetFsmVariables();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetFsmVariables();

    private void DoGetFsmVariables()
    {
      this.InitFsmVars();
      for (int index = 0; index < this.getVariables.Length; ++index)
      {
        this.getVariables[index].GetValueFrom(this.sourceVariables[index]);
        this.getVariables[index].ApplyValueTo((INamedVariable) this.targetVariables[index]);
      }
    }
  }
}
