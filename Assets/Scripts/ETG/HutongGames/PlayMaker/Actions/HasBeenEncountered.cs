using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Checks if the owning game object has been encountered before.")]
    [ActionCategory(".Brave")]
    public class HasBeenEncountered : FsmStateAction
    {
        [CheckForComponent(typeof (EncounterTrackable))]
        public FsmOwnerDefault GameObject;
        [HutongGames.PlayMaker.Tooltip("Event to send when the mouse is released while over the GameObject.")]
        public FsmEvent yes;
        [HutongGames.PlayMaker.Tooltip("Event to send when the mouse moves off the GameObject.")]
        public FsmEvent no;
        private EncounterTrackable m_encounterTrackable;

        public override void Reset()
        {
            this.GameObject = (FsmOwnerDefault) null;
            this.yes = (FsmEvent) null;
            this.no = (FsmEvent) null;
        }

        public override void OnEnter()
        {
            this.m_encounterTrackable = this.Fsm.GetOwnerDefaultTarget(this.GameObject).GetComponent<EncounterTrackable>();
            if ((Object) this.m_encounterTrackable != (Object) null)
            {
                if (GameStatsManager.Instance.QueryEncounterable(this.m_encounterTrackable) > 0)
                {
                    if (this.yes != null)
                        this.Fsm.Event(this.yes);
                }
                else if (this.no != null)
                    this.Fsm.Event(this.no);
            }
            this.Finish();
        }
    }
}
