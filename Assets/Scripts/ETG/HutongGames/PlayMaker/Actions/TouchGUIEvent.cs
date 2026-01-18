using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Device)]
    [HutongGames.PlayMaker.Tooltip("Sends events when a GUI Texture or GUI Text is touched. Optionally filter by a fingerID.")]
    public class TouchGUIEvent : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof (GUIElement))]
        [HutongGames.PlayMaker.Tooltip("The Game Object that owns the GUI Texture or GUI Text.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Only detect touches that match this fingerID, or set to None.")]
        public FsmInt fingerId;
        [HutongGames.PlayMaker.Tooltip("Event to send on touch began.")]
        [ActionSection("Events")]
        public FsmEvent touchBegan;
        [HutongGames.PlayMaker.Tooltip("Event to send on touch moved.")]
        public FsmEvent touchMoved;
        [HutongGames.PlayMaker.Tooltip("Event to send on stationary touch.")]
        public FsmEvent touchStationary;
        [HutongGames.PlayMaker.Tooltip("Event to send on touch ended.")]
        public FsmEvent touchEnded;
        [HutongGames.PlayMaker.Tooltip("Event to send on touch cancel.")]
        public FsmEvent touchCanceled;
        [HutongGames.PlayMaker.Tooltip("Event to send if not touching (finger down but not over the GUI element)")]
        public FsmEvent notTouching;
        [ActionSection("Store Results")]
        [HutongGames.PlayMaker.Tooltip("Store the fingerId of the touch.")]
        [UIHint(UIHint.Variable)]
        public FsmInt storeFingerId;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Store the screen position where the GUI element was touched.")]
        public FsmVector3 storeHitPoint;
        [HutongGames.PlayMaker.Tooltip("Normalize the hit point screen coordinates (0-1).")]
        public FsmBool normalizeHitPoint;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Store the offset position of the hit.")]
        public FsmVector3 storeOffset;
        [HutongGames.PlayMaker.Tooltip("How to measure the offset.")]
        public TouchGUIEvent.OffsetOptions relativeTo;
        [HutongGames.PlayMaker.Tooltip("Normalize the offset.")]
        public FsmBool normalizeOffset;
        [HutongGames.PlayMaker.Tooltip("Repeate every frame.")]
        [ActionSection("")]
        public bool everyFrame;
        private Vector3 touchStartPos;
        private GUIElement guiElement;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            FsmInt fsmInt = new FsmInt();
            fsmInt.UseVariable = true;
            this.fingerId = fsmInt;
            this.touchBegan = (FsmEvent) null;
            this.touchMoved = (FsmEvent) null;
            this.touchStationary = (FsmEvent) null;
            this.touchEnded = (FsmEvent) null;
            this.touchCanceled = (FsmEvent) null;
            this.storeFingerId = (FsmInt) null;
            this.storeHitPoint = (FsmVector3) null;
            this.normalizeHitPoint = (FsmBool) false;
            this.storeOffset = (FsmVector3) null;
            this.relativeTo = TouchGUIEvent.OffsetOptions.Center;
            this.normalizeOffset = (FsmBool) true;
            this.everyFrame = true;
        }

        public override void OnEnter()
        {
            this.DoTouchGUIEvent();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoTouchGUIEvent();

        private void DoTouchGUIEvent()
        {
            if (Input.touchCount <= 0)
                return;
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            this.guiElement = (GUIElement) ownerDefaultTarget.GetComponent<GUITexture>() ?? (GUIElement) ownerDefaultTarget.GetComponent<GUIText>();
            if ((Object) this.guiElement == (Object) null)
                return;
            foreach (Touch touch in Input.touches)
                this.DoTouch(touch);
        }

        private void DoTouch(Touch touch)
        {
            if (!this.fingerId.IsNone && touch.fingerId != this.fingerId.Value)
                return;
            Vector3 position = (Vector3) touch.position;
            if (this.guiElement.HitTest(position))
            {
                if (touch.phase == TouchPhase.Began)
                    this.touchStartPos = position;
                this.storeFingerId.Value = touch.fingerId;
                if (this.normalizeHitPoint.Value)
                {
                    position.x /= (float) Screen.width;
                    position.y /= (float) Screen.height;
                }
                this.storeHitPoint.Value = position;
                this.DoTouchOffset(position);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        this.Fsm.Event(this.touchBegan);
                        break;
                    case TouchPhase.Moved:
                        this.Fsm.Event(this.touchMoved);
                        break;
                    case TouchPhase.Stationary:
                        this.Fsm.Event(this.touchStationary);
                        break;
                    case TouchPhase.Ended:
                        this.Fsm.Event(this.touchEnded);
                        break;
                    case TouchPhase.Canceled:
                        this.Fsm.Event(this.touchCanceled);
                        break;
                }
            }
            else
                this.Fsm.Event(this.notTouching);
        }

        private void DoTouchOffset(Vector3 touchPos)
        {
            if (this.storeOffset.IsNone)
                return;
            Rect screenRect = this.guiElement.GetScreenRect();
            Vector3 vector3_1 = new Vector3();
            switch (this.relativeTo)
            {
                case TouchGUIEvent.OffsetOptions.TopLeft:
                    vector3_1.x = touchPos.x - screenRect.x;
                    vector3_1.y = touchPos.y - screenRect.y;
                    break;
                case TouchGUIEvent.OffsetOptions.Center:
                    Vector3 vector3_2 = new Vector3(screenRect.x + screenRect.width * 0.5f, screenRect.y + screenRect.height * 0.5f, 0.0f);
                    vector3_1 = touchPos - vector3_2;
                    break;
                case TouchGUIEvent.OffsetOptions.TouchStart:
                    vector3_1 = touchPos - this.touchStartPos;
                    break;
            }
            if (this.normalizeOffset.Value)
            {
                vector3_1.x /= screenRect.width;
                vector3_1.y /= screenRect.height;
            }
            this.storeOffset.Value = vector3_1;
        }

        public enum OffsetOptions
        {
            TopLeft,
            Center,
            TouchStart,
        }
    }
}
