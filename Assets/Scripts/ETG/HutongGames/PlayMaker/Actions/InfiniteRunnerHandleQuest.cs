using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    public class InfiniteRunnerHandleQuest : FsmStateAction
    {
        private TalkDoerLite m_talkDoer;
        private Vector2 m_lastPosition;
        private float m_elapsed;

        public override void Awake()
        {
            base.Awake();
            this.m_talkDoer = this.Owner.GetComponent<TalkDoerLite>();
        }

        public override void OnEnter()
        {
            this.m_lastPosition = this.m_talkDoer.specRigidbody.UnitCenter;
            this.Owner.GetComponent<InfiniteRunnerController>().StartQuest();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            this.m_elapsed += BraveTime.DeltaTime;
            if ((double) this.m_elapsed < 0.75)
                return;
            if (this.m_talkDoer.CurrentPath != null)
            {
                this.m_talkDoer.specRigidbody.Velocity = this.m_talkDoer.GetPathVelocityContribution(this.m_lastPosition, 32 /*0x20*/);
                this.m_lastPosition = this.m_talkDoer.specRigidbody.UnitCenter;
            }
            else
                this.Finish();
        }
    }
}
