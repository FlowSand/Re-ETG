using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Get the network OnFailedToConnect or MasterServer OnFailedToConnectToMasterServer connection error message.")]
    [ActionCategory(ActionCategory.Network)]
    public class NetworkGetOnFailedToConnectProperties : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Error label")]
        [UIHint(UIHint.Variable)]
        public FsmString errorLabel;
        [HutongGames.PlayMaker.Tooltip("No error occurred.")]
        public FsmEvent NoErrorEvent;
        [HutongGames.PlayMaker.Tooltip("We presented an RSA public key which does not match what the system we connected to is using.")]
        public FsmEvent RSAPublicKeyMismatchEvent;
        [HutongGames.PlayMaker.Tooltip("The server is using a password and has refused our connection because we did not set the correct password.")]
        public FsmEvent InvalidPasswordEvent;
        [HutongGames.PlayMaker.Tooltip("onnection attempt failed, possibly because of internal connectivity problems.")]
        public FsmEvent ConnectionFailedEvent;
        [HutongGames.PlayMaker.Tooltip("The server is at full capacity, failed to connect.")]
        public FsmEvent TooManyConnectedPlayersEvent;
        [HutongGames.PlayMaker.Tooltip("We are banned from the system we attempted to connect to (likely temporarily).")]
        public FsmEvent ConnectionBannedEvent;
        [HutongGames.PlayMaker.Tooltip("We are already connected to this particular server (can happen after fast disconnect/reconnect).")]
        public FsmEvent AlreadyConnectedToServerEvent;
        [HutongGames.PlayMaker.Tooltip("Cannot connect to two servers at once. Close the connection before connecting again.")]
        public FsmEvent AlreadyConnectedToAnotherServerEvent;
        [HutongGames.PlayMaker.Tooltip("Internal error while attempting to initialize network interface. Socket possibly already in use.")]
        public FsmEvent CreateSocketOrThreadFailureEvent;
        [HutongGames.PlayMaker.Tooltip("Incorrect parameters given to Connect function.")]
        public FsmEvent IncorrectParametersEvent;
        [HutongGames.PlayMaker.Tooltip("No host target given in Connect.")]
        public FsmEvent EmptyConnectTargetEvent;
        [HutongGames.PlayMaker.Tooltip("Client could not connect internally to same network NAT enabled server.")]
        public FsmEvent InternalDirectConnectFailedEvent;
        [HutongGames.PlayMaker.Tooltip("The NAT target we are trying to connect to is not connected to the facilitator server.")]
        public FsmEvent NATTargetNotConnectedEvent;
        [HutongGames.PlayMaker.Tooltip("Connection lost while attempting to connect to NAT target.")]
        public FsmEvent NATTargetConnectionLostEvent;
        [HutongGames.PlayMaker.Tooltip("NAT punchthrough attempt has failed. The cause could be a too restrictive NAT implementation on either endpoints.")]
        public FsmEvent NATPunchthroughFailedEvent;

        public override void Reset()
        {
            this.errorLabel = (FsmString) null;
            this.NoErrorEvent = (FsmEvent) null;
            this.RSAPublicKeyMismatchEvent = (FsmEvent) null;
            this.InvalidPasswordEvent = (FsmEvent) null;
            this.ConnectionFailedEvent = (FsmEvent) null;
            this.TooManyConnectedPlayersEvent = (FsmEvent) null;
            this.ConnectionBannedEvent = (FsmEvent) null;
            this.AlreadyConnectedToServerEvent = (FsmEvent) null;
            this.AlreadyConnectedToAnotherServerEvent = (FsmEvent) null;
            this.CreateSocketOrThreadFailureEvent = (FsmEvent) null;
            this.IncorrectParametersEvent = (FsmEvent) null;
            this.EmptyConnectTargetEvent = (FsmEvent) null;
            this.InternalDirectConnectFailedEvent = (FsmEvent) null;
            this.NATTargetNotConnectedEvent = (FsmEvent) null;
            this.NATTargetConnectionLostEvent = (FsmEvent) null;
            this.NATPunchthroughFailedEvent = (FsmEvent) null;
        }

        public override void OnEnter()
        {
            this.doGetNetworkErrorInfo();
            this.Finish();
        }

        private void doGetNetworkErrorInfo()
        {
            NetworkConnectionError connectionError = Fsm.EventData.ConnectionError;
            this.errorLabel.Value = connectionError.ToString();
            switch (connectionError + 5)
            {
                case NetworkConnectionError.NoError:
                    if (this.InternalDirectConnectFailedEvent == null)
                        break;
                    this.Fsm.Event(this.InternalDirectConnectFailedEvent);
                    break;
                case ~NetworkConnectionError.CreateSocketOrThreadFailure:
                    if (this.EmptyConnectTargetEvent == null)
                        break;
                    this.Fsm.Event(this.EmptyConnectTargetEvent);
                    break;
                case ~NetworkConnectionError.IncorrectParameters:
                    if (this.IncorrectParametersEvent == null)
                        break;
                    this.Fsm.Event(this.IncorrectParametersEvent);
                    break;
                case ~NetworkConnectionError.EmptyConnectTarget:
                    if (this.CreateSocketOrThreadFailureEvent == null)
                        break;
                    this.Fsm.Event(this.CreateSocketOrThreadFailureEvent);
                    break;
                case ~NetworkConnectionError.InternalDirectConnectFailed:
                    if (this.AlreadyConnectedToAnotherServerEvent == null)
                        break;
                    this.Fsm.Event(this.AlreadyConnectedToAnotherServerEvent);
                    break;
                case (NetworkConnectionError) 5:
                    if (this.NoErrorEvent == null)
                        break;
                    this.Fsm.Event(this.NoErrorEvent);
                    break;
                default:
                    switch (connectionError)
                    {
                        case NetworkConnectionError.ConnectionFailed:
                            if (this.ConnectionFailedEvent == null)
                                return;
                            this.Fsm.Event(this.ConnectionFailedEvent);
                            return;
                        case NetworkConnectionError.AlreadyConnectedToServer:
                            if (this.AlreadyConnectedToServerEvent == null)
                                return;
                            this.Fsm.Event(this.AlreadyConnectedToServerEvent);
                            return;
                        case NetworkConnectionError.TooManyConnectedPlayers:
                            if (this.TooManyConnectedPlayersEvent == null)
                                return;
                            this.Fsm.Event(this.TooManyConnectedPlayersEvent);
                            return;
                        case NetworkConnectionError.RSAPublicKeyMismatch:
                            if (this.RSAPublicKeyMismatchEvent == null)
                                return;
                            this.Fsm.Event(this.RSAPublicKeyMismatchEvent);
                            return;
                        case NetworkConnectionError.ConnectionBanned:
                            if (this.ConnectionBannedEvent == null)
                                return;
                            this.Fsm.Event(this.ConnectionBannedEvent);
                            return;
                        case NetworkConnectionError.InvalidPassword:
                            if (this.InvalidPasswordEvent == null)
                                return;
                            this.Fsm.Event(this.InvalidPasswordEvent);
                            return;
                        default:
                            switch (connectionError - 69)
                            {
                                case NetworkConnectionError.NoError:
                                    if (this.NATTargetNotConnectedEvent == null)
                                        return;
                                    this.Fsm.Event(this.NATTargetNotConnectedEvent);
                                    return;
                                case ~NetworkConnectionError.CreateSocketOrThreadFailure:
                                    return;
                                case ~NetworkConnectionError.IncorrectParameters:
                                    if (this.NATTargetConnectionLostEvent == null)
                                        return;
                                    this.Fsm.Event(this.NATTargetConnectionLostEvent);
                                    return;
                                case ~NetworkConnectionError.EmptyConnectTarget:
                                    return;
                                case ~NetworkConnectionError.InternalDirectConnectFailed:
                                    if (this.NATPunchthroughFailedEvent == null)
                                        return;
                                    this.Fsm.Event(this.NoErrorEvent);
                                    return;
                                default:
                                    return;
                            }
                    }
            }
        }
    }
}
