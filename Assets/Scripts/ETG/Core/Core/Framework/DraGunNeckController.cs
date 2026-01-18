#nullable disable

public class DraGunNeckController : BraveBehaviour
    {
        public void Start() => this.aiActor = this.transform.parent.GetComponent<AIActor>();

        protected override void OnDestroy() => base.OnDestroy();

        public void TriggerAnimationEvent(string eventInfo)
        {
            this.aiActor.behaviorSpeculator.TriggerAnimationEvent(eventInfo);
        }
    }

