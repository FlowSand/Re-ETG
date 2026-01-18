using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Input/Gestures/Double Tap")]
public class dfDoubleTapGesture : dfGestureBase
    {
        [SerializeField]
        private float timeout = 0.5f;
        [SerializeField]
        private float maxDistance = 35f;

        public event dfGestureEventHandler<dfDoubleTapGesture> DoubleTapGesture;

        public float Timeout
        {
            get => this.timeout;
            set => this.timeout = value;
        }

        public float MaximumDistance
        {
            get => this.maxDistance;
            set => this.maxDistance = value;
        }

        protected void Start()
        {
        }

        public void OnMouseDown(dfControl source, dfMouseEventArgs args)
        {
            if (this.State == dfGestureState.Possible && (double) (UnityEngine.Time.realtimeSinceStartup - this.StartTime) <= (double) this.timeout && (double) Vector2.Distance(args.Position, this.StartPosition) <= (double) this.maxDistance)
            {
                Vector2 position = args.Position;
                this.CurrentPosition = position;
                this.StartPosition = position;
                this.State = dfGestureState.Began;
                if (this.DoubleTapGesture != null)
                    this.DoubleTapGesture(this);
                this.gameObject.Signal("OnDoubleTapGesture", (object) this);
                this.endGesture();
            }
            else
            {
                Vector2 position = args.Position;
                this.CurrentPosition = position;
                this.StartPosition = position;
                this.State = dfGestureState.Possible;
                this.StartTime = UnityEngine.Time.realtimeSinceStartup;
            }
        }

        public void OnMouseLeave() => this.endGesture();

        public void OnMultiTouchEnd() => this.endGesture();

        public void OnMultiTouch() => this.endGesture();

        private void endGesture()
        {
            if (this.State == dfGestureState.Began || this.State == dfGestureState.Changed)
                this.State = dfGestureState.Ended;
            else if (this.State == dfGestureState.Possible)
                this.State = dfGestureState.Cancelled;
            else
                this.State = dfGestureState.None;
        }
    }

