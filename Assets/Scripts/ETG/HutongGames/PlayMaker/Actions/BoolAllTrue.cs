#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Tests if all the given Bool Variables are True.")]
    public class BoolAllTrue : FsmStateAction
    {
        [Tooltip("The Bool variables to check.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmBool[] boolVariables;
        [Tooltip("Event to send if all the Bool variables are True.")]
        public FsmEvent sendEvent;
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a Bool variable.")]
        public FsmBool storeResult;
        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.boolVariables = (FsmBool[]) null;
            this.sendEvent = (FsmEvent) null;
            this.storeResult = (FsmBool) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoAllTrue();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoAllTrue();

        private void DoAllTrue()
        {
            if (this.boolVariables.Length == 0)
                return;
            bool flag = true;
            for (int index = 0; index < this.boolVariables.Length; ++index)
            {
                if (!this.boolVariables[index].Value)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
                this.Fsm.Event(this.sendEvent);
            this.storeResult.Value = flag;
        }
    }
}
