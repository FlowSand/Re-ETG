using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
    [HutongGames.PlayMaker.Tooltip("Get the value of a Float Variable from another FSM.")]
    public class GetFsmFloat : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName)]
        [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;
        [RequiredField]
        [UIHint(UIHint.FsmFloat)]
        public FsmString variableName;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeValue;
        public bool everyFrame;
        private GameObject goLastFrame;
        private PlayMakerFSM fsm;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.fsmName = (FsmString) string.Empty;
            this.storeValue = (FsmFloat) null;
        }

        public override void OnEnter()
        {
            this.DoGetFsmFloat();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetFsmFloat();

        private void DoGetFsmFloat()
        {
            if (this.storeValue.IsNone)
                return;
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            if ((Object) ownerDefaultTarget != (Object) this.goLastFrame)
            {
                this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
                this.goLastFrame = ownerDefaultTarget;
            }
            if ((Object) this.fsm == (Object) null)
                return;
            FsmFloat fsmFloat = this.fsm.FsmVariables.GetFsmFloat(this.variableName.Value);
            if (fsmFloat == null)
                return;
            this.storeValue.Value = fsmFloat.Value;
        }
    }
}
