using UnityEngine;

#nullable disable
namespace InControl
{
    public class TouchSwipeControl : TouchControl
    {
        [SerializeField]
        [Header("Position")]
        private TouchUnitType areaUnitType;
        [SerializeField]
        private Rect activeArea = new Rect(25f, 25f, 50f, 50f);
        [Range(0.0f, 1f)]
        [Header("Options")]
        public float sensitivity = 0.1f;
        public bool oneSwipePerTouch;
        [Header("Analog Target")]
        public TouchControl.AnalogTarget target;
        public TouchControl.SnapAngles snapAngles;
        [Header("Button Targets")]
        public TouchControl.ButtonTarget upTarget;
        public TouchControl.ButtonTarget downTarget;
        public TouchControl.ButtonTarget leftTarget;
        public TouchControl.ButtonTarget rightTarget;
        public TouchControl.ButtonTarget tapTarget;
        private Rect worldActiveArea;
        private Vector3 currentVector;
        private bool currentVectorIsSet;
        private Vector3 beganPosition;
        private Vector3 lastPosition;
        private Touch currentTouch;
        private bool fireButtonTarget;
        private TouchControl.ButtonTarget nextButtonTarget;
        private TouchControl.ButtonTarget lastButtonTarget;
        private bool dirty;

        public override void CreateControl()
        {
        }

        public override void DestroyControl()
        {
            if (this.currentTouch == null)
                return;
            this.TouchEnded(this.currentTouch);
            this.currentTouch = (Touch) null;
        }

        public override void ConfigureControl()
        {
            this.worldActiveArea = TouchManager.ConvertToWorld(this.activeArea, this.areaUnitType);
        }

        public override void DrawGizmos() => Utility.DrawRectGizmo(this.worldActiveArea, Color.yellow);

        private void Update()
        {
            if (!this.dirty)
                return;
            this.ConfigureControl();
            this.dirty = false;
        }

        public override void SubmitControlState(ulong updateTick, float deltaTime)
        {
            this.SubmitAnalogValue(this.target, (Vector2) TouchControl.SnapTo((Vector2) this.currentVector, this.snapAngles), 0.0f, 1f, updateTick, deltaTime);
            this.SubmitButtonState(this.upTarget, this.fireButtonTarget && this.nextButtonTarget == this.upTarget, updateTick, deltaTime);
            this.SubmitButtonState(this.downTarget, this.fireButtonTarget && this.nextButtonTarget == this.downTarget, updateTick, deltaTime);
            this.SubmitButtonState(this.leftTarget, this.fireButtonTarget && this.nextButtonTarget == this.leftTarget, updateTick, deltaTime);
            this.SubmitButtonState(this.rightTarget, this.fireButtonTarget && this.nextButtonTarget == this.rightTarget, updateTick, deltaTime);
            this.SubmitButtonState(this.tapTarget, this.fireButtonTarget && this.nextButtonTarget == this.tapTarget, updateTick, deltaTime);
            if (!this.fireButtonTarget || this.nextButtonTarget == TouchControl.ButtonTarget.None)
                return;
            this.fireButtonTarget = !this.oneSwipePerTouch;
            this.lastButtonTarget = this.nextButtonTarget;
            this.nextButtonTarget = TouchControl.ButtonTarget.None;
        }

        public override void CommitControlState(ulong updateTick, float deltaTime)
        {
            this.CommitAnalog(this.target);
            this.CommitButton(this.upTarget);
            this.CommitButton(this.downTarget);
            this.CommitButton(this.leftTarget);
            this.CommitButton(this.rightTarget);
            this.CommitButton(this.tapTarget);
        }

        public override void TouchBegan(Touch touch)
        {
            if (this.currentTouch != null)
                return;
            this.beganPosition = TouchManager.ScreenToWorldPoint(touch.position);
            if (!this.worldActiveArea.Contains(this.beganPosition))
                return;
            this.lastPosition = this.beganPosition;
            this.currentTouch = touch;
            this.currentVector = (Vector3) Vector2.zero;
            this.currentVectorIsSet = false;
            this.fireButtonTarget = true;
            this.nextButtonTarget = TouchControl.ButtonTarget.None;
            this.lastButtonTarget = TouchControl.ButtonTarget.None;
        }

        public override void TouchMoved(Touch touch)
        {
            if (this.currentTouch != touch)
                return;
            Vector3 worldPoint = TouchManager.ScreenToWorldPoint(touch.position);
            Vector3 vector3 = worldPoint - this.lastPosition;
            if ((double) vector3.magnitude < (double) this.sensitivity)
                return;
            this.lastPosition = worldPoint;
            if (!this.oneSwipePerTouch || !this.currentVectorIsSet)
            {
                this.currentVector = vector3.normalized;
                this.currentVectorIsSet = true;
            }
            if (!this.fireButtonTarget)
                return;
            TouchControl.ButtonTarget buttonTargetForVector = this.GetButtonTargetForVector((Vector2) this.currentVector);
            if (buttonTargetForVector == this.lastButtonTarget)
                return;
            this.nextButtonTarget = buttonTargetForVector;
        }

        public override void TouchEnded(Touch touch)
        {
            if (this.currentTouch != touch)
                return;
            this.currentTouch = (Touch) null;
            this.currentVector = (Vector3) Vector2.zero;
            this.currentVectorIsSet = false;
            if ((double) (this.beganPosition - TouchManager.ScreenToWorldPoint(touch.position)).magnitude < (double) this.sensitivity)
            {
                this.fireButtonTarget = true;
                this.nextButtonTarget = this.tapTarget;
                this.lastButtonTarget = TouchControl.ButtonTarget.None;
            }
            else
            {
                this.fireButtonTarget = false;
                this.nextButtonTarget = TouchControl.ButtonTarget.None;
                this.lastButtonTarget = TouchControl.ButtonTarget.None;
            }
        }

        private TouchControl.ButtonTarget GetButtonTargetForVector(Vector2 vector)
        {
            Vector2 vector2 = (Vector2) TouchControl.SnapTo(vector, TouchControl.SnapAngles.Four);
            if (vector2 == Vector2.up)
                return this.upTarget;
            if (vector2 == Vector2.right)
                return this.rightTarget;
            if (vector2 == -Vector2.up)
                return this.downTarget;
            return vector2 == -Vector2.right ? this.leftTarget : TouchControl.ButtonTarget.None;
        }

        public Rect ActiveArea
        {
            get => this.activeArea;
            set
            {
                if (!(this.activeArea != value))
                    return;
                this.activeArea = value;
                this.dirty = true;
            }
        }

        public TouchUnitType AreaUnitType
        {
            get => this.areaUnitType;
            set
            {
                if (this.areaUnitType == value)
                    return;
                this.areaUnitType = value;
                this.dirty = true;
            }
        }
    }
}
