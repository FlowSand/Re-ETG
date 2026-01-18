using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
    [HutongGames.PlayMaker.Tooltip("Enables/Disables an FSM component on a GameObject.")]
    [ActionCategory(ActionCategory.StateMachine)]
    public class EnableFSM : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM component.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName)]
        [HutongGames.PlayMaker.Tooltip("Optional name of FSM on GameObject. Useful if you have more than one FSM on a GameObject.")]
        public FsmString fsmName;
        [HutongGames.PlayMaker.Tooltip("Set to True to enable, False to disable.")]
        public FsmBool enable;
        [HutongGames.PlayMaker.Tooltip("Reset the initial enabled state when exiting the state.")]
        public FsmBool resetOnExit;
        private PlayMakerFSM fsmComponent;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.fsmName = (FsmString) string.Empty;
            this.enable = (FsmBool) true;
            this.resetOnExit = (FsmBool) true;
        }

        public override void OnEnter()
        {
            this.DoEnableFSM();
            this.Finish();
        }

        private void DoEnableFSM()
        {
            GameObject gameObject = this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner;
            if ((Object) gameObject == (Object) null)
                return;
            if (!string.IsNullOrEmpty(this.fsmName.Value))
            {
                foreach (PlayMakerFSM component in gameObject.GetComponents<PlayMakerFSM>())
                {
                    if (component.FsmName == this.fsmName.Value)
                    {
                        this.fsmComponent = component;
                        break;
                    }
                }
            }
            else
                this.fsmComponent = gameObject.GetComponent<PlayMakerFSM>();
            if ((Object) this.fsmComponent == (Object) null)
                this.LogError("Missing FsmComponent!");
            else
                this.fsmComponent.enabled = this.enable.Value;
        }

        public override void OnExit()
        {
            if ((Object) this.fsmComponent == (Object) null || !this.resetOnExit.Value)
                return;
            this.fsmComponent.enabled = !this.enable.Value;
        }
    }
}
