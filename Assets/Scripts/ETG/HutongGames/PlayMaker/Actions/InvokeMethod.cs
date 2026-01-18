using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Invokes a Method in a Behaviour attached to a Game Object. See Unity InvokeMethod docs.")]
    [ActionCategory(ActionCategory.ScriptControl)]
    public class InvokeMethod : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The game object that owns the behaviour.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The behaviour that contains the method.")]
        [UIHint(UIHint.Script)]
        [RequiredField]
        public FsmString behaviour;
        [UIHint(UIHint.Method)]
        [HutongGames.PlayMaker.Tooltip("The name of the method to invoke.")]
        [RequiredField]
        public FsmString methodName;
        [HutongGames.PlayMaker.Tooltip("Optional time delay in seconds.")]
        [HasFloatSlider(0.0f, 10f)]
        public FsmFloat delay;
        [HutongGames.PlayMaker.Tooltip("Call the method repeatedly.")]
        public FsmBool repeating;
        [HutongGames.PlayMaker.Tooltip("Delay between repeated calls in seconds.")]
        [HasFloatSlider(0.0f, 10f)]
        public FsmFloat repeatDelay;
        [HutongGames.PlayMaker.Tooltip("Stop calling the method when the state is exited.")]
        public FsmBool cancelOnExit;
        private MonoBehaviour component;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.behaviour = (FsmString) null;
            this.methodName = (FsmString) string.Empty;
            this.delay = (FsmFloat) null;
            this.repeating = (FsmBool) false;
            this.repeatDelay = (FsmFloat) 1f;
            this.cancelOnExit = (FsmBool) false;
        }

        public override void OnEnter()
        {
            this.DoInvokeMethod(this.Fsm.GetOwnerDefaultTarget(this.gameObject));
            this.Finish();
        }

        private void DoInvokeMethod(GameObject go)
        {
            if ((Object) go == (Object) null)
                return;
            this.component = go.GetComponent(ReflectionUtils.GetGlobalType(this.behaviour.Value)) as MonoBehaviour;
            if ((Object) this.component == (Object) null)
                this.LogWarning($"InvokeMethod: {go.name} missing behaviour: {this.behaviour.Value}");
            else if (this.repeating.Value)
                this.component.InvokeRepeating(this.methodName.Value, this.delay.Value, this.repeatDelay.Value);
            else
                this.component.Invoke(this.methodName.Value, this.delay.Value);
        }

        public override void OnExit()
        {
            if ((Object) this.component == (Object) null || !this.cancelOnExit.Value)
                return;
            this.component.CancelInvoke(this.methodName.Value);
        }
    }
}
