using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Get the number of hosts on the master server.\n\nUse MasterServer Get Host Data to get host data at a specific index.")]
    [ActionCategory(ActionCategory.Network)]
    public class MasterServerGetHostCount : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("The number of hosts on the MasterServer.")]
        [RequiredField]
        public FsmInt count;

        public override void OnEnter()
        {
            this.count.Value = MasterServer.PollHostList().Length;
            this.Finish();
        }
    }
}
