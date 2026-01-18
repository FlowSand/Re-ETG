using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
    [HutongGames.PlayMaker.Tooltip("Set the value of a Float Variable in another FSM.")]
    public class SetFsmFloat : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName)]
        [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
        public FsmString fsmName;
        [UIHint(UIHint.FsmFloat)]
        [HutongGames.PlayMaker.Tooltip("The name of the FSM variable.")]
        [RequiredField]
        public FsmString variableName;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("Set the value of the variable.")]
        public FsmFloat setValue;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is changing.")]
        public bool everyFrame;
        private GameObject goLastFrame;
        private string fsmNameLastFrame;
        private PlayMakerFSM fsm;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.fsmName = (FsmString) string.Empty;
            this.setValue = (FsmFloat) null;
        }

        public override void OnEnter()
        {
            this.DoSetFsmFloat();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        private void DoSetFsmFloat()
        {
            if (this.setValue == null)
                return;
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            if ((Object) ownerDefaultTarget != (Object) this.goLastFrame || this.fsmName.Value != this.fsmNameLastFrame)
            {
                this.goLastFrame = ownerDefaultTarget;
                this.fsmNameLastFrame = this.fsmName.Value;
                this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
            }
            if ((Object) this.fsm == (Object) null)
            {
                this.LogWarning("Could not find FSM: " + this.fsmName.Value);
            }
            else
            {
                FsmFloat fsmFloat = this.fsm.FsmVariables.GetFsmFloat(this.variableName.Value);
                if (fsmFloat != null)
                    fsmFloat.Value = this.setValue.Value;
                else
                    this.LogWarning("Could not find variable: " + this.variableName.Value);
            }
        }

        public override void OnUpdate() => this.DoSetFsmFloat();
    }
}
