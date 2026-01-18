using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Network)]
    [HutongGames.PlayMaker.Tooltip("Disconnect from the server.")]
    public class NetworkDisconnect : FsmStateAction
    {
        public override void OnEnter()
        {
            Network.Disconnect();
            this.Finish();
        }
    }
}
