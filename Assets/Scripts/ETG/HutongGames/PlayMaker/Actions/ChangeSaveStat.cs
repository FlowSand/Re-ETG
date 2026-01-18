#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Sends Events based on the data in a player save.")]
    [ActionCategory(".Brave")]
    public class ChangeSaveStat : FsmStateAction
    {
        public TrackedStats stat;
        public FsmFloat statChange;

        public override void Reset()
        {
            this.stat = TrackedStats.BULLETS_FIRED;
            this.statChange = (FsmFloat) 0.0f;
        }

        public override string ErrorCheck() => string.Empty;

        public override void OnEnter()
        {
            GameStatsManager.Instance.RegisterStatChange(this.stat, this.statChange.Value);
            this.Finish();
        }
    }
}
