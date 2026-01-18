#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Sends Events.")]
    [ActionCategory(".Brave")]
    public class BroadcastEventInRoom : BraveFsmStateAction
    {
        public FsmString eventString;

        public override void OnEnter()
        {
            base.OnEnter();
            GameManager.BroadcastRoomFsmEvent(this.eventString.Value, this.Owner.transform.position.GetAbsoluteRoom());
            this.Finish();
        }
    }
}
