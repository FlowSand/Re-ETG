#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Vector3)]
    [Tooltip("Subtracts a Vector3 value from a Vector3 variable.")]
    public class Vector3Subtract : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmVector3 vector3Variable;
        [RequiredField]
        public FsmVector3 subtractVector;
        public bool everyFrame;

        public override void Reset()
        {
            this.vector3Variable = (FsmVector3) null;
            FsmVector3 fsmVector3 = new FsmVector3();
            fsmVector3.UseVariable = true;
            this.subtractVector = fsmVector3;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.vector3Variable.Value -= this.subtractVector.Value;
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.vector3Variable.Value -= this.subtractVector.Value;
    }
}
