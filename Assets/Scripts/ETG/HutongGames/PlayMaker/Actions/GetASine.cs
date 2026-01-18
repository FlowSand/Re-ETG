using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Trigonometry)]
    [HutongGames.PlayMaker.Tooltip("Get the Arc sine. You can get the result in degrees, simply check on the RadToDeg conversion")]
    public class GetASine : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The value of the sine")]
        [RequiredField]
        public FsmFloat Value;
        [HutongGames.PlayMaker.Tooltip("The resulting angle. Note:If you want degrees, simply check RadToDeg")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmFloat angle;
        [HutongGames.PlayMaker.Tooltip("Check on if you want the angle expressed in degrees.")]
        public FsmBool RadToDeg;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.angle = (FsmFloat) null;
            this.RadToDeg = (FsmBool) true;
            this.everyFrame = false;
            this.Value = (FsmFloat) null;
        }

        public override void OnEnter()
        {
            this.DoASine();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoASine();

        private void DoASine()
        {
            float num = Mathf.Asin(this.Value.Value);
            if (this.RadToDeg.Value)
                num *= 57.29578f;
            this.angle.Value = num;
        }
    }
}
