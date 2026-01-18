using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Vector2)]
    [HutongGames.PlayMaker.Tooltip("Sets the XY channels of a Vector2 Variable. To leave any channel unchanged, set variable to 'None'.")]
    public class SetVector2XY : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The vector2 target")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmVector2 vector2Variable;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("The vector2 source")]
        public FsmVector2 vector2Value;
        [HutongGames.PlayMaker.Tooltip("The x component. Override vector2Value if set")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("The y component.Override vector2Value if set")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            this.vector2Variable = (FsmVector2) null;
            this.vector2Value = (FsmVector2) null;
            FsmFloat fsmFloat1 = new FsmFloat();
            fsmFloat1.UseVariable = true;
            this.x = fsmFloat1;
            FsmFloat fsmFloat2 = new FsmFloat();
            fsmFloat2.UseVariable = true;
            this.y = fsmFloat2;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetVector2XYZ();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoSetVector2XYZ();

        private void DoSetVector2XYZ()
        {
            if (this.vector2Variable == null)
                return;
            Vector2 vector2 = this.vector2Variable.Value;
            if (!this.vector2Value.IsNone)
                vector2 = this.vector2Value.Value;
            if (!this.x.IsNone)
                vector2.x = this.x.Value;
            if (!this.y.IsNone)
                vector2.y = this.y.Value;
            this.vector2Variable.Value = vector2;
        }
    }
}
