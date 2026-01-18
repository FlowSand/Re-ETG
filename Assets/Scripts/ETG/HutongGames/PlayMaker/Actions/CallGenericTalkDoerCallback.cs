#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(".NPCs")]
    public class CallGenericTalkDoerCallback : FsmStateAction
    {
        public FsmBool CallCallbackA;
        public FsmBool CallCallbackB;
        public FsmBool CallCallbackC;
        public FsmBool CallCallbackD;
        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        public override void Reset() => this.everyFrame = false;

        private void DoCallbacks()
        {
            TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
            if (this.CallCallbackA.Value && component.OnGenericFSMActionA != null)
                component.OnGenericFSMActionA();
            if (this.CallCallbackB.Value && component.OnGenericFSMActionB != null)
                component.OnGenericFSMActionB();
            if (this.CallCallbackC.Value && component.OnGenericFSMActionC != null)
                component.OnGenericFSMActionC();
            if (!this.CallCallbackD.Value || component.OnGenericFSMActionD == null)
                return;
            component.OnGenericFSMActionD();
        }

        public override void OnEnter()
        {
            if (this.everyFrame)
                return;
            this.DoCallbacks();
            this.Finish();
        }

        public override void OnUpdate() => this.DoCallbacks();
    }
}
