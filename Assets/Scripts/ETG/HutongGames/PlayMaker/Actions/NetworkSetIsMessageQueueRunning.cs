using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Enable or disable the processing of network messages.\n\nIf this is disabled no RPC call execution or network view synchronization takes place.")]
    [ActionCategory(ActionCategory.Network)]
    public class NetworkSetIsMessageQueueRunning : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Is Message Queue Running. If this is disabled no RPC call execution or network view synchronization takes place")]
        public FsmBool isMessageQueueRunning;

        public override void Reset() => this.isMessageQueueRunning = (FsmBool) null;

        public override void OnEnter()
        {
            Network.isMessageQueueRunning = this.isMessageQueueRunning.Value;
            this.Finish();
        }
    }
}
