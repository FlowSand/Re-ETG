using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
    [HutongGames.PlayMaker.Tooltip("Get the value of a Vector3 Variable from another FSM.")]
    [ActionCategory(ActionCategory.StateMachine)]
    public class GetFsmVector3 : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName)]
        [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;
        [RequiredField]
        [UIHint(UIHint.FsmVector3)]
        public FsmString variableName;
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmVector3 storeValue;
        public bool everyFrame;
        private GameObject goLastFrame;
        private PlayMakerFSM fsm;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.fsmName = (FsmString) string.Empty;
            this.storeValue = (FsmVector3) null;
        }

        public override void OnEnter()
        {
            this.DoGetFsmVector3();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetFsmVector3();

        private void DoGetFsmVector3()
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
            FsmVector3 fsmVector3 = this.fsm.FsmVariables.GetFsmVector3(this.variableName.Value);
            if (fsmVector3 == null)
                return;
            this.storeValue.Value = fsmVector3.Value;
        }
    }
}
