using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Get the value of a Texture Variable from another FSM.")]
    [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
    [ActionCategory(ActionCategory.StateMachine)]
    public class GetFsmTexture : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
        [UIHint(UIHint.FsmName)]
        public FsmString fsmName;
        [RequiredField]
        [UIHint(UIHint.FsmTexture)]
        public FsmString variableName;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmTexture storeValue;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        private GameObject goLastFrame;
        protected PlayMakerFSM fsm;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.fsmName = (FsmString) string.Empty;
            this.variableName = (FsmString) string.Empty;
            this.storeValue = (FsmTexture) null;
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
            FsmTexture fsmTexture = this.fsm.FsmVariables.GetFsmTexture(this.variableName.Value);
            if (fsmTexture == null)
                return;
            this.storeValue.Value = fsmTexture.Value;
        }
    }
}
