#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UnityObject)]
    [Tooltip("Sets the value of an Object Variable.")]
    public class SetObjectValue : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmObject objectVariable;
        [RequiredField]
        public FsmObject objectValue;
        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.objectVariable = (FsmObject) null;
            this.objectValue = (FsmObject) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.objectVariable.Value = this.objectValue.Value;
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.objectVariable.Value = this.objectValue.Value;
    }
}
