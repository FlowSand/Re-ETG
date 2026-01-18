#nullable disable

public class DynamiteGuyController : BraveBehaviour
    {
        public SimpleSparksDoer SparksDoer;

        public void Update()
        {
            if (!this.aiActor.HasBeenAwoken || this.aiAnimator.IsPlaying("spawn"))
                return;
            this.SparksDoer.enabled = true;
            this.enabled = false;
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

