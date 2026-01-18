using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Device)]
    [HutongGames.PlayMaker.Tooltip("Sends an event when a swipe is detected.")]
    public class SwipeGestureEvent : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("How far a touch has to travel to be considered a swipe. Uses normalized distance (e.g. 1 = 1 screen diagonal distance). Should generally be a very small number.")]
        public FsmFloat minSwipeDistance;
        [HutongGames.PlayMaker.Tooltip("Event to send when swipe left detected.")]
        public FsmEvent swipeLeftEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send when swipe right detected.")]
        public FsmEvent swipeRightEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send when swipe up detected.")]
        public FsmEvent swipeUpEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send when swipe down detected.")]
        public FsmEvent swipeDownEvent;
        private float screenDiagonalSize;
        private float minSwipeDistancePixels;
        private bool touchStarted;
        private Vector2 touchStartPos;

        public override void Reset()
        {
            this.minSwipeDistance = (FsmFloat) 0.1f;
            this.swipeLeftEvent = (FsmEvent) null;
            this.swipeRightEvent = (FsmEvent) null;
            this.swipeUpEvent = (FsmEvent) null;
            this.swipeDownEvent = (FsmEvent) null;
        }

        public override void OnEnter()
        {
            this.screenDiagonalSize = Mathf.Sqrt((float) (Screen.width * Screen.width + Screen.height * Screen.height));
            this.minSwipeDistancePixels = this.minSwipeDistance.Value * this.screenDiagonalSize;
        }

        public override void OnUpdate()
        {
            if (Input.touchCount <= 0)
                return;
            Touch touch = Input.touches[0];
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    this.touchStarted = true;
                    this.touchStartPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    if (!this.touchStarted)
                        break;
                    this.TestForSwipeGesture(touch);
                    this.touchStarted = false;
                    break;
                case TouchPhase.Canceled:
                    this.touchStarted = false;
                    break;
            }
        }

        private void TestForSwipeGesture(Touch touch)
        {
            Vector2 position = touch.position;
            if ((double) Vector2.Distance(position, this.touchStartPos) <= (double) this.minSwipeDistancePixels)
                return;
            float x = position.y - this.touchStartPos.y;
            float message = (float) ((360.0 + (double) (57.29578f * Mathf.Atan2(position.x - this.touchStartPos.x, x)) - 45.0) % 360.0);
            Debug.Log((object) message);
            if ((double) message < 90.0)
                this.Fsm.Event(this.swipeRightEvent);
            else if ((double) message < 180.0)
                this.Fsm.Event(this.swipeDownEvent);
            else if ((double) message < 270.0)
                this.Fsm.Event(this.swipeLeftEvent);
            else
                this.Fsm.Event(this.swipeUpEvent);
        }
    }
}
