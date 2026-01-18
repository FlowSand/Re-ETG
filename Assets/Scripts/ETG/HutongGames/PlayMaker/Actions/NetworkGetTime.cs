using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Get the current network time (seconds).")]
    [ActionCategory(ActionCategory.Network)]
    public class NetworkGetTime : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The network time.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat time;

        public override void Reset() => this.time = (FsmFloat) null;

        public override void OnEnter()
        {
            this.time.Value = (float) Network.time;
            this.Finish();
        }
    }
}
