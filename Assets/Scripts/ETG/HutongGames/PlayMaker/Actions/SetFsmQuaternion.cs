using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Set the value of a Quaternion Variable in another FSM.")]
    [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
    [ActionCategory(ActionCategory.StateMachine)]
    public class SetFsmQuaternion : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
        [UIHint(UIHint.FsmName)]
        public FsmString fsmName;
        [RequiredField]
        [UIHint(UIHint.FsmQuaternion)]
        [HutongGames.PlayMaker.Tooltip("The name of the FSM variable.")]
        public FsmString variableName;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("Set the value of the variable.")]
        public FsmQuaternion setValue;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is changing.")]
        public bool everyFrame;
        private GameObject goLastFrame;
        private string fsmNameLastFrame;
        private PlayMakerFSM fsm;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.fsmName = (FsmString) string.Empty;
            this.variableName = (FsmString) string.Empty;
            this.setValue = (FsmQuaternion) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetFsmQuaternion();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        private void DoSetFsmQuaternion()
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
                FsmQuaternion fsmQuaternion = this.fsm.FsmVariables.GetFsmQuaternion(this.variableName.Value);
                if (fsmQuaternion != null)
                    fsmQuaternion.Value = this.setValue.Value;
                else
                    this.LogWarning("Could not find variable: " + this.variableName.Value);
            }
        }

        public override void OnUpdate() => this.DoSetFsmQuaternion();
    }
}
