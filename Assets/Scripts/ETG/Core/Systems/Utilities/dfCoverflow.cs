using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[RequireComponent(typeof (dfPanel))]
[AddComponentMenu("Daikon Forge/Examples/Coverflow/Scroller")]
[Serializable]
public class dfCoverflow : MonoBehaviour
    {
        [SerializeField]
        public int selectedIndex;
        [SerializeField]
        public int itemSize = 200;
        [SerializeField]
        public float time = 0.33f;
        [SerializeField]
        public int spacing = 5;
        [SerializeField]
        protected AnimationCurve rotationCurve = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
        [SerializeField]
        protected AnimationCurve opacityCurve = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
        [SerializeField]
        protected bool autoSelectOnStart = true;
        private dfPanel container;
        private dfList<dfControl> controls;
        private dfAnimatedFloat currentX;
        private Vector2 touchStartPosition;
        private int lastSelected = -1;
        private bool isMouseDown;

        public event ValueChangedEventHandler<int> SelectedIndexChanged;

        public void OnEnable()
        {
            this.container = this.GetComponent<dfPanel>();
            this.container.Pivot = dfPivotPoint.MiddleCenter;
            this.container.ControlAdded += new ChildControlEventHandler(this.container_ControlCollectionChanged);
            this.container.ControlRemoved += new ChildControlEventHandler(this.container_ControlCollectionChanged);
            this.controls = new dfList<dfControl>((IList<dfControl>) this.container.Controls);
            if (this.rotationCurve.keys.Length != 0)
                return;
            this.rotationCurve.AddKey(0.0f, 0.0f);
            this.rotationCurve.AddKey(1f, 1f);
        }

        public void OnDisable()
        {
            if (!((UnityEngine.Object) this.container != (UnityEngine.Object) null))
                return;
            this.container.ControlAdded -= new ChildControlEventHandler(this.container_ControlCollectionChanged);
            this.container.ControlRemoved -= new ChildControlEventHandler(this.container_ControlCollectionChanged);
        }

        public void Update()
        {
            if (this.controls == null || this.controls.Count == 0)
            {
                this.setSelectedIndex(0);
            }
            else
            {
                if (this.isMouseDown)
                {
                    dfControl closestItemToCenter = this.findClosestItemToCenter();
                    if ((UnityEngine.Object) closestItemToCenter != (UnityEngine.Object) null)
                    {
                        this.setSelectedIndex(this.controls.IndexOf(closestItemToCenter));
                        this.lastSelected = this.selectedIndex;
                    }
                }
                this.setSelectedIndex(Mathf.Min(this.controls.Count - 1, Mathf.Max(0, this.selectedIndex)));
                if (Application.isPlaying)
                    this.updateSlides();
                else
                    this.layoutSlidesForEditor();
            }
        }

        public void OnMouseEnter(dfControl control, dfMouseEventArgs args)
        {
            this.touchStartPosition = args.Position;
        }

        public void OnMouseDown(dfControl control, dfMouseEventArgs args)
        {
            this.touchStartPosition = args.Position;
            this.isMouseDown = true;
        }

        public void OnDragStart(dfControl control, dfDragEventArgs args)
        {
            if (!args.Used)
                return;
            this.isMouseDown = false;
        }

        public void OnMouseUp(dfControl control, dfMouseEventArgs args)
        {
            if (!this.isMouseDown)
                return;
            this.isMouseDown = false;
            dfControl closestItemToCenter = this.findClosestItemToCenter();
            if (!((UnityEngine.Object) closestItemToCenter != (UnityEngine.Object) null))
                return;
            this.lastSelected = -1;
            this.setSelectedIndex(this.controls.IndexOf(closestItemToCenter));
        }

        public void OnMouseMove(dfControl control, dfMouseEventArgs args)
        {
            if (!(args is dfTouchEventArgs) && !this.isMouseDown || args.Used || (double) (args.Position - this.touchStartPosition).magnitude <= 5.0)
                return;
            dfCoverflow dfCoverflow = this;
            dfCoverflow.currentX = (dfAnimatedFloat) ((float) (dfAnimatedValue<float>) dfCoverflow.currentX + args.MoveDelta.x);
            args.Use();
        }

        public void OnResolutionChanged(
            dfControl control,
            Vector2 previousResolution,
            Vector2 currentResolution)
        {
            this.lastSelected = -1;
        }

        private void container_ControlCollectionChanged(dfControl panel, dfControl child)
        {
            this.controls = new dfList<dfControl>((IList<dfControl>) panel.Controls);
            if (!this.autoSelectOnStart || !Application.isPlaying)
                return;
            this.setSelectedIndex(this.controls.Count / 2);
        }

        public void OnKeyDown(dfControl sender, dfKeyEventArgs args)
        {
            if (args.Used)
                return;
            if (args.KeyCode == KeyCode.RightArrow)
                this.setSelectedIndex(this.selectedIndex + 1);
            else if (args.KeyCode == KeyCode.LeftArrow)
                this.setSelectedIndex(this.selectedIndex - 1);
            else if (args.KeyCode == KeyCode.Home)
            {
                this.setSelectedIndex(0);
            }
            else
            {
                if (args.KeyCode != KeyCode.End)
                    return;
                this.setSelectedIndex(this.controls.Count - 1);
            }
        }

        public void OnMouseWheel(dfControl sender, dfMouseEventArgs args)
        {
            if (args.Used)
                return;
            args.Use();
            this.container.Focus(true);
            this.setSelectedIndex(this.selectedIndex - (int) Mathf.Sign(args.WheelDelta));
        }

        public void OnClick(dfControl sender, dfMouseEventArgs args)
        {
            if ((UnityEngine.Object) args.Source == (UnityEngine.Object) this.container || (double) Vector2.Distance(args.Position, this.touchStartPosition) > 20.0)
                return;
            dfControl dfControl = args.Source;
            while ((UnityEngine.Object) dfControl != (UnityEngine.Object) null && !this.controls.Contains(dfControl))
                dfControl = dfControl.Parent;
            if (!((UnityEngine.Object) dfControl != (UnityEngine.Object) null))
                return;
            this.lastSelected = -1;
            this.setSelectedIndex(this.controls.IndexOf(dfControl));
            this.isMouseDown = false;
        }

        private void setSelectedIndex(int value)
        {
            if (value == this.selectedIndex)
                return;
            this.selectedIndex = value;
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged((object) this, value);
            this.gameObject.Signal("OnSelectedIndexChanged", (object) this, (object) value);
        }

        private dfControl findClosestItemToCenter()
        {
            float num = float.MaxValue;
            dfControl closestItemToCenter = (dfControl) null;
            for (int index = 0; index < this.controls.Count; ++index)
            {
                dfControl control = this.controls[index];
                float sqrMagnitude = (control.transform.position - this.container.transform.position).sqrMagnitude;
                if ((double) sqrMagnitude <= (double) num)
                {
                    num = sqrMagnitude;
                    closestItemToCenter = control;
                }
            }
            return closestItemToCenter;
        }

        private void layoutSlidesForEditor()
        {
            dfList<dfControl> controls = this.container.Controls;
            int x = 0;
            float y = (float) (((double) this.container.Height - (double) this.itemSize) * 0.5);
            Vector2 vector2 = Vector2.one * (float) this.itemSize;
            for (int index = 0; index < controls.Count; ++index)
            {
                controls[index].Size = vector2;
                controls[index].RelativePosition = new Vector3((float) x, y);
                x += this.itemSize + Mathf.Max(0, this.spacing);
            }
        }

        private void updateSlides()
        {
            if (this.currentX == null || this.selectedIndex != this.lastSelected)
            {
                dfAnimatedFloat dfAnimatedFloat = new dfAnimatedFloat(this.currentX == null ? 0.0f : this.currentX.Value, this.calculateTargetPosition(), this.time);
                dfAnimatedFloat.EasingType = dfEasingType.SineEaseOut;
                this.currentX = dfAnimatedFloat;
                this.lastSelected = this.selectedIndex;
            }
            Vector3 vector3 = new Vector3((float) (dfAnimatedValue<float>) this.currentX, (float) (((double) this.container.Height - (double) this.itemSize) * 0.5));
            int count = this.controls.Count;
            for (int index = 0; index < count; ++index)
            {
                dfControl control = this.controls[index];
                control.Size = new Vector2((float) this.itemSize, (float) this.itemSize);
                control.RelativePosition = vector3;
                control.Pivot = dfPivotPoint.MiddleCenter;
                if (Application.isPlaying)
                {
                    Quaternion quaternion = Quaternion.Euler(0.0f, this.calcHorzRotation(vector3.x), 0.0f);
                    control.transform.localRotation = quaternion;
                    float num = this.calcScale(vector3.x);
                    control.transform.localScale = Vector3.one * num;
                    control.Opacity = this.calcItemOpacity(vector3.x);
                }
                else
                {
                    control.transform.localScale = Vector3.one;
                    control.transform.localRotation = Quaternion.identity;
                }
                vector3.x += (float) (this.itemSize + this.spacing);
            }
            if (!Application.isPlaying)
                return;
            int num1 = 0;
            for (int index = 0; index < this.selectedIndex; ++index)
                this.controls[index].ZOrder = num1++;
            for (int index = count - 1; index >= this.selectedIndex; --index)
                this.controls[index].ZOrder = num1++;
        }

        private float calcScale(float offset)
        {
            return Mathf.Max((float) (1.0 - (double) Mathf.Abs((float) (((double) this.container.Width - (double) this.itemSize) * 0.5) - offset) / (double) this.getTotalSize()), 0.85f);
        }

        private float calcItemOpacity(float offset)
        {
            return 1f - this.opacityCurve.Evaluate(Mathf.Abs((float) (((double) this.container.Width - (double) this.itemSize) * 0.5) - offset) / (float) this.getTotalSize());
        }

        private float calcHorzRotation(float offset)
        {
            float num1 = (float) (((double) this.container.Width - (double) this.itemSize) * 0.5);
            float num2 = Mathf.Abs(num1 - offset);
            float num3 = Mathf.Sign(num1 - offset);
            int totalSize = this.getTotalSize();
            return (float) ((double) this.rotationCurve.Evaluate(num2 / (float) totalSize) * 90.0 * -(double) num3);
        }

        private int getTotalSize()
        {
            int count = this.controls.Count;
            return count * this.itemSize + Mathf.Max(count, 0) * this.spacing;
        }

        private float calculateTargetPosition()
        {
            float targetPosition = (float) (((double) this.container.Width - (double) this.itemSize) * 0.5) - (float) (this.selectedIndex * this.itemSize);
            if (this.selectedIndex > 0)
                targetPosition -= (float) (this.selectedIndex * this.spacing);
            return targetPosition;
        }
    }

