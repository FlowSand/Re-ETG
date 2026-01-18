#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Checks whether or not the player has a certain amount of money.")]
    [ActionCategory(".NPCs")]
    public class TestConsumable : FsmStateAction
    {
        [Tooltip("Type of consumable to check.")]
        public BravePlayMakerUtility.ConsumableType consumableType;
        [Tooltip("Value to check.")]
        public FsmFloat value;
        [Tooltip("Event sent if the amount is greater than <value>.")]
        public FsmEvent greaterThan;
        [Tooltip("Event sent if the amount is greater than or equal to <value>.")]
        public FsmEvent greaterThanOrEqual;
        [Tooltip("Event sent if the amount equals <value>.")]
        public FsmEvent equal;
        [Tooltip("Event sent if the amount is less than or equal to <value>.")]
        public FsmEvent lessThanOrEqual;
        [Tooltip("Event sent if the amount is less than <value>.")]
        public FsmEvent lessThan;
        public bool everyFrame;
        private TalkDoerLite m_talkDoer;

        public override void Reset()
        {
            this.consumableType = BravePlayMakerUtility.ConsumableType.Currency;
            this.value = (FsmFloat) 0.0f;
            this.greaterThan = (FsmEvent) null;
            this.greaterThanOrEqual = (FsmEvent) null;
            this.equal = (FsmEvent) null;
            this.lessThanOrEqual = (FsmEvent) null;
            this.lessThan = (FsmEvent) null;
            this.everyFrame = false;
        }

        public override string ErrorCheck()
        {
            return FsmEvent.IsNullOrEmpty(this.greaterThan) && FsmEvent.IsNullOrEmpty(this.greaterThanOrEqual) && FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.lessThanOrEqual) && FsmEvent.IsNullOrEmpty(this.lessThan) ? "Action sends no events!" : string.Empty;
        }

        public override void OnEnter()
        {
            this.m_talkDoer = this.Owner.GetComponent<TalkDoerLite>();
            this.DoCompare();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoCompare();

        private void DoCompare()
        {
            float consumableValue = BravePlayMakerUtility.GetConsumableValue(this.m_talkDoer.TalkingPlayer, this.consumableType);
            if ((double) consumableValue > (double) this.value.Value)
                this.Fsm.Event(this.greaterThan);
            if ((double) consumableValue >= (double) this.value.Value)
                this.Fsm.Event(this.greaterThanOrEqual);
            if ((double) consumableValue == (double) this.value.Value)
                this.Fsm.Event(this.equal);
            if ((double) consumableValue <= (double) this.value.Value)
                this.Fsm.Event(this.lessThanOrEqual);
            if ((double) consumableValue >= (double) this.value.Value)
                return;
            this.Fsm.Event(this.lessThan);
        }
    }
}
