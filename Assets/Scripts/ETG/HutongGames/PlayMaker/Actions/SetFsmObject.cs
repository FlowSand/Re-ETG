using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [HutongGames.PlayMaker.Tooltip("Set the value of an Object Variable in another FSM.")]
    [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
    public class SetFsmObject : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object")]
        [UIHint(UIHint.FsmName)]
        public FsmString fsmName;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The name of the FSM variable.")]
        [UIHint(UIHint.FsmObject)]
        public FsmString variableName;
        [HutongGames.PlayMaker.Tooltip("Set the value of the variable.")]
        public FsmObject setValue;
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
            this.setValue = (FsmObject) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetFsmBool();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        private void DoSetFsmBool()
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
                FsmObject fsmObject = this.fsm.FsmVariables.GetFsmObject(this.variableName.Value);
                if (fsmObject != null)
                    fsmObject.Value = this.setValue.Value;
                else
                    this.LogWarning("Could not find variable: " + this.variableName.Value);
            }
        }

        public override void OnUpdate() => this.DoSetFsmBool();
    }
}
