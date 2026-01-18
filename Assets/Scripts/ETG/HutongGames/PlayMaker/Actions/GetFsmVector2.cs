using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get the value of a Vector2 Variable from another FSM.")]
  [ActionCategory(ActionCategory.StateMachine)]
  [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
  public class GetFsmVector2 : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.FsmName)]
    [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
    public FsmString fsmName;
    [RequiredField]
    [UIHint(UIHint.FsmVector2)]
    public FsmString variableName;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmVector2 storeValue;
    public bool everyFrame;
    private GameObject goLastFrame;
    private PlayMakerFSM fsm;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.storeValue = (FsmVector2) null;
    }

    public override void OnEnter()
    {
      this.DoGetFsmVector2();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetFsmVector2();

    private void DoGetFsmVector2()
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
      FsmVector2 fsmVector2 = this.fsm.FsmVariables.GetFsmVector2(this.variableName.Value);
      if (fsmVector2 == null)
        return;
      this.storeValue.Value = fsmVector2.Value;
    }
  }
}
