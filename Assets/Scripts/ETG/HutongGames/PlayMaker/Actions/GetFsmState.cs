using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the name of the specified FSMs current state. Either reference the fsm component directly, or find it on a game object.")]
  [ActionTarget(typeof (PlayMakerFSM), "fsmComponent", false)]
  [ActionCategory(ActionCategory.StateMachine)]
  public class GetFsmState : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Drag a PlayMakerFSM component here.")]
    public PlayMakerFSM fsmComponent;
    [HutongGames.PlayMaker.Tooltip("If not specifyng the component above, specify the GameObject that owns the FSM")]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Optional name of Fsm on Game Object. If left blank it will find the first PlayMakerFSM on the GameObject.")]
    [UIHint(UIHint.FsmName)]
    public FsmString fsmName;
    [HutongGames.PlayMaker.Tooltip("Store the state name in a string variable.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmString storeResult;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. E.g.,  useful if you're waiting for the state to change.")]
    public bool everyFrame;
    private PlayMakerFSM fsm;

    public override void Reset()
    {
      this.fsmComponent = (PlayMakerFSM) null;
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) string.Empty;
      this.storeResult = (FsmString) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoGetFsmState();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetFsmState();

    private void DoGetFsmState()
    {
      if ((Object) this.fsm == (Object) null)
      {
        if ((Object) this.fsmComponent != (Object) null)
        {
          this.fsm = this.fsmComponent;
        }
        else
        {
          GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
          if ((Object) ownerDefaultTarget != (Object) null)
            this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
        }
        if ((Object) this.fsm == (Object) null)
        {
          this.storeResult.Value = string.Empty;
          return;
        }
      }
      this.storeResult.Value = this.fsm.ActiveStateName;
    }
  }
}
