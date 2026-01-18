using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Store the current send rate for all NetworkViews")]
    [ActionCategory(ActionCategory.Network)]
    public class NetworkGetSendRate : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Store the current send rate for NetworkViews")]
        [RequiredField]
        public FsmFloat sendRate;

        public override void Reset() => this.sendRate = (FsmFloat) null;

        public override void OnEnter()
        {
            this.DoGetSendRate();
            this.Finish();
        }

        private void DoGetSendRate() => this.sendRate.Value = Network.sendRate;
    }
}
