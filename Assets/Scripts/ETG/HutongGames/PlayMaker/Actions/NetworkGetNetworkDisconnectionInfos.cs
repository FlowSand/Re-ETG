using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Get the network OnDisconnectedFromServer.")]
    [ActionCategory(ActionCategory.Network)]
    public class NetworkGetNetworkDisconnectionInfos : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Disconnection label")]
        public FsmString disconnectionLabel;
        [HutongGames.PlayMaker.Tooltip("The connection to the system has been lost, no reliable packets could be delivered.")]
        public FsmEvent lostConnectionEvent;
        [HutongGames.PlayMaker.Tooltip("The connection to the system has been closed.")]
        public FsmEvent disConnectedEvent;

        public override void Reset()
        {
            this.disconnectionLabel = (FsmString) null;
            this.lostConnectionEvent = (FsmEvent) null;
            this.disConnectedEvent = (FsmEvent) null;
        }

        public override void OnEnter()
        {
            this.doGetNetworkDisconnectionInfo();
            this.Finish();
        }

        private void doGetNetworkDisconnectionInfo()
        {
            NetworkDisconnection disconnectionInfo = Fsm.EventData.DisconnectionInfo;
            this.disconnectionLabel.Value = disconnectionInfo.ToString();
            switch (disconnectionInfo)
            {
                case NetworkDisconnection.Disconnected:
                    if (this.disConnectedEvent == null)
                        break;
                    this.Fsm.Event(this.disConnectedEvent);
                    break;
                case NetworkDisconnection.LostConnection:
                    if (this.lostConnectionEvent == null)
                        break;
                    this.Fsm.Event(this.lostConnectionEvent);
                    break;
            }
        }
    }
}
