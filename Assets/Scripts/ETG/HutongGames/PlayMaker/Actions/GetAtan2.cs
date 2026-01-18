using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Get the Arc Tangent 2 as in atan2(y,x). You can get the result in degrees, simply check on the RadToDeg conversion")]
    [ActionCategory(ActionCategory.Trigonometry)]
    public class GetAtan2 : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The x value of the tan")]
        public FsmFloat xValue;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The y value of the tan")]
        public FsmFloat yValue;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("The resulting angle. Note:If you want degrees, simply check RadToDeg")]
        public FsmFloat angle;
        [HutongGames.PlayMaker.Tooltip("Check on if you want the angle expressed in degrees.")]
        public FsmBool RadToDeg;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.xValue = (FsmFloat) null;
            this.yValue = (FsmFloat) null;
            this.RadToDeg = (FsmBool) true;
            this.everyFrame = false;
            this.angle = (FsmFloat) null;
        }

        public override void OnEnter()
        {
            this.DoATan();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoATan();

        private void DoATan()
        {
            float num = Mathf.Atan2(this.yValue.Value, this.xValue.Value);
            if (this.RadToDeg.Value)
                num *= 57.29578f;
            this.angle.Value = num;
        }
    }
}
