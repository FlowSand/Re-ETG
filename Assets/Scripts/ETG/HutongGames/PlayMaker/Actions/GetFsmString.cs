using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get the value of a String Variable from another FSM.")]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  [ActionCategory(ActionCategory.StateMachine)]
  public class GetFsmString : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
    [UIHint(UIHint.FsmName)]
    public FsmString fsmName;
    [UIHint(UIHint.FsmString)]
    [RequiredField]
    public FsmString variableName;
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmString storeValue;
    public bool everyFrame;
    private GameObject goLastFrame;
    private PlayMakerFSM fsm;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.storeValue = (FsmString) null;
    }

    public override void OnEnter()
    {
      this.DoGetFsmString();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetFsmString();

    private void DoGetFsmString()
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
      FsmString fsmString = this.fsm.FsmVariables.GetFsmString(this.variableName.Value);
      if (fsmString == null)
        return;
      this.storeValue.Value = fsmString.Value;
    }
  }
}
