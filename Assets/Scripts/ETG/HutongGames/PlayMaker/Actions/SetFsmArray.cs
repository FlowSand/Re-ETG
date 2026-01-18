#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionTarget(typeof (PlayMakerFSM), "gameObject,fsmName", false)]
    [Tooltip("Copy an Array Variable in another FSM.")]
    public class SetFsmArray : BaseFsmVariableAction
    {
        [Tooltip("The GameObject that owns the FSM.")]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [Tooltip("Optional name of FSM on Game Object.")]
        [UIHint(UIHint.FsmName)]
        public FsmString fsmName;
        [Tooltip("The name of the FSM variable.")]
        [UIHint(UIHint.FsmArray)]
        [RequiredField]
        public FsmString variableName;
        [UIHint(UIHint.Variable)]
        [Tooltip("Set the content of the array variable.")]
        [RequiredField]
        public FsmArray setValue;
        [Tooltip("If true, makes copies. if false, values share the same reference and editing one array item value will affect the source and vice versa. Warning, this only affect the current items of the source array. Adding or removing items doesn't affect other FsmArrays.")]
        public bool copyValues;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.fsmName = (FsmString) string.Empty;
            this.variableName = (FsmString) null;
            this.setValue = (FsmArray) null;
            this.copyValues = true;
        }

        public override void OnEnter()
        {
            this.DoSetFsmArrayCopy();
            this.Finish();
        }

        private void DoSetFsmArrayCopy()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject), this.fsmName.Value))
                return;
            FsmArray fsmArray = this.fsm.FsmVariables.GetFsmArray(this.variableName.Value);
            if (fsmArray != null)
            {
                if (fsmArray.ElementType != this.setValue.ElementType)
                {
                    this.LogError($"Can only copy arrays with the same elements type. Found <{(object) fsmArray.ElementType}> and <{(object) this.setValue.ElementType}>");
                }
                else
                {
                    fsmArray.Resize(0);
                    if (this.copyValues)
                        fsmArray.Values = this.setValue.Values.Clone() as object[];
                    else
                        fsmArray.Values = this.setValue.Values;
                }
            }
            else
                this.DoVariableNotFound(this.variableName.Value);
        }
    }
}
