using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.ScriptControl)]
    [HutongGames.PlayMaker.Tooltip("Enables/Disables a Behaviour on a GameObject. Optionally reset the Behaviour on exit - useful if you want the Behaviour to be active only while this state is active.")]
    public class EnableBehaviour : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject that owns the Behaviour.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The name of the Behaviour to enable/disable.")]
        [UIHint(UIHint.Behaviour)]
        public FsmString behaviour;
        [HutongGames.PlayMaker.Tooltip("Optionally drag a component directly into this field (behavior name will be ignored).")]
        public Component component;
        [HutongGames.PlayMaker.Tooltip("Set to True to enable, False to disable.")]
        [RequiredField]
        public FsmBool enable;
        public FsmBool resetOnExit;
        private Behaviour componentTarget;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.behaviour = (FsmString) null;
            this.component = (Component) null;
            this.enable = (FsmBool) true;
            this.resetOnExit = (FsmBool) true;
        }

        public override void OnEnter()
        {
            this.DoEnableBehaviour(this.Fsm.GetOwnerDefaultTarget(this.gameObject));
            this.Finish();
        }

        private void DoEnableBehaviour(GameObject go)
        {
            if ((Object) go == (Object) null)
                return;
            this.componentTarget = !((Object) this.component != (Object) null) ? go.GetComponent(ReflectionUtils.GetGlobalType(this.behaviour.Value)) as Behaviour : this.component as Behaviour;
            if ((Object) this.componentTarget == (Object) null)
                this.LogWarning($" {go.name} missing behaviour: {this.behaviour.Value}");
            else
                this.componentTarget.enabled = this.enable.Value;
        }

        public override void OnExit()
        {
            if ((Object) this.componentTarget == (Object) null || !this.resetOnExit.Value)
                return;
            this.componentTarget.enabled = !this.enable.Value;
        }

        public override string ErrorCheck()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null || (Object) this.component != (Object) null || this.behaviour.IsNone || string.IsNullOrEmpty(this.behaviour.Value))
                return (string) null;
            return (Object) (ownerDefaultTarget.GetComponent(ReflectionUtils.GetGlobalType(this.behaviour.Value)) as Behaviour) != (Object) null ? (string) null : "Behaviour missing";
        }
    }
}
