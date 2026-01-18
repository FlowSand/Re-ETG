using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sends an Event based on the current character.")]
    [ActionCategory(ActionCategory.Logic)]
    public class CharacterClassSwitch : FsmStateAction
    {
        [CompoundArray("Int Switches", "Compare Int", "Send Event")]
        public PlayableCharacters[] compareTo;
        public FsmEvent[] sendEvent;
        public bool everyFrame;

        public override void Reset()
        {
            this.compareTo = new PlayableCharacters[1];
            this.sendEvent = new FsmEvent[1];
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoCharSwitch();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoCharSwitch();

        private void DoCharSwitch()
        {
            for (int index = 0; index < this.compareTo.Length; ++index)
            {
                if ((bool) (Object) this.Owner && (bool) (Object) this.Owner.GetComponent<TalkDoerLite>() && (bool) (Object) this.Owner.GetComponent<TalkDoerLite>().TalkingPlayer)
                {
                    if (this.Owner.GetComponent<TalkDoerLite>().TalkingPlayer.characterIdentity == this.compareTo[index])
                    {
                        this.Fsm.Event(this.sendEvent[index]);
                        break;
                    }
                }
                else if (GameManager.Instance.PrimaryPlayer.characterIdentity == this.compareTo[index])
                {
                    this.Fsm.Event(this.sendEvent[index]);
                    break;
                }
            }
        }
    }
}
