using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Gets the number of Touches.")]
    [ActionCategory(ActionCategory.Device)]
    public class GetTouchCount : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmInt storeCount;
        public bool everyFrame;

        public override void Reset()
        {
            this.storeCount = (FsmInt) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetTouchCount();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetTouchCount();

        private void DoGetTouchCount() => this.storeCount.Value = Input.touchCount;
    }
}
