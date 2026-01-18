using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Use a low pass filter to reduce the influence of sudden changes in a quaternion Variable.")]
    [ActionCategory(ActionCategory.Quaternion)]
    public class QuaternionLowPassFilter : QuaternionBaseAction
    {
        [HutongGames.PlayMaker.Tooltip("quaternion Variable to filter. Should generally come from some constantly updated input")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmQuaternion quaternionVariable;
        [HutongGames.PlayMaker.Tooltip("Determines how much influence new changes have. E.g., 0.1 keeps 10 percent of the unfiltered quaternion and 90 percent of the previously filtered value.")]
        public FsmFloat filteringFactor;
        private Quaternion filteredQuaternion;

        public override void Reset()
        {
            this.quaternionVariable = (FsmQuaternion) null;
            this.filteringFactor = (FsmFloat) 0.1f;
            this.everyFrame = true;
            this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.filteredQuaternion = new Quaternion(this.quaternionVariable.Value.x, this.quaternionVariable.Value.y, this.quaternionVariable.Value.z, this.quaternionVariable.Value.w);
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate()
        {
            if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.Update)
                return;
            this.DoQuatLowPassFilter();
        }

        public override void OnLateUpdate()
        {
            if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.LateUpdate)
                return;
            this.DoQuatLowPassFilter();
        }

        public override void OnFixedUpdate()
        {
            if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
                return;
            this.DoQuatLowPassFilter();
        }

        private void DoQuatLowPassFilter()
        {
            this.filteredQuaternion.x = (float) ((double) this.quaternionVariable.Value.x * (double) this.filteringFactor.Value + (double) this.filteredQuaternion.x * (1.0 - (double) this.filteringFactor.Value));
            this.filteredQuaternion.y = (float) ((double) this.quaternionVariable.Value.y * (double) this.filteringFactor.Value + (double) this.filteredQuaternion.y * (1.0 - (double) this.filteringFactor.Value));
            this.filteredQuaternion.z = (float) ((double) this.quaternionVariable.Value.z * (double) this.filteringFactor.Value + (double) this.filteredQuaternion.z * (1.0 - (double) this.filteringFactor.Value));
            this.filteredQuaternion.w = (float) ((double) this.quaternionVariable.Value.w * (double) this.filteringFactor.Value + (double) this.filteredQuaternion.w * (1.0 - (double) this.filteringFactor.Value));
            this.quaternionVariable.Value = new Quaternion(this.filteredQuaternion.x, this.filteredQuaternion.y, this.filteredQuaternion.z, this.filteredQuaternion.w);
        }
    }
}
