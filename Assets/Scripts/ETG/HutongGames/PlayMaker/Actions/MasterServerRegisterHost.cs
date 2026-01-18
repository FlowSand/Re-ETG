using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Network)]
    [HutongGames.PlayMaker.Tooltip("Register this server on the master server.\n\nIf the master server address information has not been changed the default Unity master server will be used.")]
    public class MasterServerRegisterHost : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The unique game type name.")]
        [RequiredField]
        public FsmString gameTypeName;
        [HutongGames.PlayMaker.Tooltip("The game name.")]
        [RequiredField]
        public FsmString gameName;
        [HutongGames.PlayMaker.Tooltip("Optional comment")]
        public FsmString comment;

        public override void Reset()
        {
            this.gameTypeName = (FsmString) null;
            this.gameName = (FsmString) null;
            this.comment = (FsmString) null;
        }

        public override void OnEnter()
        {
            this.DoMasterServerRegisterHost();
            this.Finish();
        }

        private void DoMasterServerRegisterHost()
        {
            MasterServer.RegisterHost(this.gameTypeName.Value, this.gameName.Value, this.comment.Value);
        }
    }
}
