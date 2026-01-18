using System;

using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/UI/tk2dUIScrollableArea")]
public class tk2dUIScrollableArea : MonoBehaviour
    {
        [SerializeField]
        private float contentLength = 1f;
        [SerializeField]
        private float visibleAreaLength = 1f;
        public GameObject contentContainer;
        public tk2dUIScrollbar scrollBar;
        public tk2dUIItem backgroundUIItem;
        public tk2dUIScrollableArea.Axes scrollAxes = tk2dUIScrollableArea.Axes.YAxis;
        public bool allowSwipeScrolling = true;
        public bool allowScrollWheel = true;
        [HideInInspector]
        [SerializeField]
        private tk2dUILayout backgroundLayoutItem;
        [HideInInspector]
        [SerializeField]
        private tk2dUILayoutContainer contentLayoutContainer;
        private bool isBackgroundButtonDown;
        private bool isBackgroundButtonOver;
        private Vector3 swipeScrollingPressDownStartLocalPos = Vector3.zero;
        private Vector3 swipeScrollingContentStartLocalPos = Vector3.zero;
        private Vector3 swipeScrollingContentDestLocalPos = Vector3.zero;
        private bool isSwipeScrollingInProgress;
        private const float SWIPE_SCROLLING_FIRST_SCROLL_THRESHOLD = 0.02f;
        private const float WITHOUT_SCROLLBAR_FIXED_SCROLL_WHEEL_PERCENT = 0.1f;
        private Vector3 swipePrevScrollingContentPressLocalPos = Vector3.zero;
        private float swipeCurrVelocity;
        private float snapBackVelocity;
        public string SendMessageOnScrollMethodName = string.Empty;
        private float percent;
        private static readonly Vector3[] boxExtents = new Vector3[8]
        {
            new Vector3(-1f, -1f, -1f),
            new Vector3(1f, -1f, -1f),
            new Vector3(-1f, 1f, -1f),
            new Vector3(1f, 1f, -1f),
            new Vector3(-1f, -1f, 1f),
            new Vector3(1f, -1f, 1f),
            new Vector3(-1f, 1f, 1f),
            new Vector3(1f, 1f, 1f)
        };

        public float ContentLength
        {
            get => this.contentLength;
            set
            {
                this.ContentLengthVisibleAreaLengthChange(this.contentLength, value, this.visibleAreaLength, this.visibleAreaLength);
            }
        }

        public float VisibleAreaLength
        {
            get => this.visibleAreaLength;
            set
            {
                this.ContentLengthVisibleAreaLengthChange(this.contentLength, this.contentLength, this.visibleAreaLength, value);
            }
        }

        public tk2dUILayout BackgroundLayoutItem
        {
            get => this.backgroundLayoutItem;
            set
            {
                if (!((UnityEngine.Object) this.backgroundLayoutItem != (UnityEngine.Object) value))
                    return;
                if ((UnityEngine.Object) this.backgroundLayoutItem != (UnityEngine.Object) null)
                    this.backgroundLayoutItem.OnReshape -= new Action<Vector3, Vector3>(this.LayoutReshaped);
                this.backgroundLayoutItem = value;
                if (!((UnityEngine.Object) this.backgroundLayoutItem != (UnityEngine.Object) null))
                    return;
                this.backgroundLayoutItem.OnReshape += new Action<Vector3, Vector3>(this.LayoutReshaped);
            }
        }

        public tk2dUILayoutContainer ContentLayoutContainer
        {
            get => this.contentLayoutContainer;
            set
            {
                if (!((UnityEngine.Object) this.contentLayoutContainer != (UnityEngine.Object) value))
                    return;
                if ((UnityEngine.Object) this.contentLayoutContainer != (UnityEngine.Object) null)
                    this.contentLayoutContainer.OnChangeContent -= new System.Action(this.ContentLayoutChangeCallback);
                this.contentLayoutContainer = value;
                if (!((UnityEngine.Object) this.contentLayoutContainer != (UnityEngine.Object) null))
                    return;
                this.contentLayoutContainer.OnChangeContent += new System.Action(this.ContentLayoutChangeCallback);
            }
        }

        public GameObject SendMessageTarget
        {
            get
            {
                return (UnityEngine.Object) this.backgroundUIItem != (UnityEngine.Object) null ? this.backgroundUIItem.sendMessageTarget : (GameObject) null;
            }
            set
            {
                if (!((UnityEngine.Object) this.backgroundUIItem != (UnityEngine.Object) null) || !((UnityEngine.Object) this.backgroundUIItem.sendMessageTarget != (UnityEngine.Object) value))
                    return;
                this.backgroundUIItem.sendMessageTarget = value;
            }
        }

        public event Action<tk2dUIScrollableArea> OnScroll;

        public float Value
        {
            get => Mathf.Clamp01(this.percent);
            set
            {
                value = Mathf.Clamp(value, 0.0f, 1f);
                if ((double) value != (double) this.percent)
                {
                    this.UnpressAllUIItemChildren();
                    this.percent = value;
                    if (this.OnScroll != null)
                        this.OnScroll(this);
                    if (this.isBackgroundButtonDown || this.isSwipeScrollingInProgress)
                    {
                        if ((UnityEngine.Object) tk2dUIManager.Instance__NoCreate != (UnityEngine.Object) null)
                            tk2dUIManager.Instance.OnInputUpdate -= new System.Action(this.BackgroundOverUpdate);
                        this.isBackgroundButtonDown = false;
                        this.isSwipeScrollingInProgress = false;
                    }
                    this.TargetOnScrollCallback();
                }
                if ((UnityEngine.Object) this.scrollBar != (UnityEngine.Object) null)
                    this.scrollBar.SetScrollPercentWithoutEvent(this.percent);
                this.SetContentPosition();
            }
        }

        public void SetScrollPercentWithoutEvent(float newScrollPercent)
        {
            this.percent = Mathf.Clamp(newScrollPercent, 0.0f, 1f);
            this.UnpressAllUIItemChildren();
            if ((UnityEngine.Object) this.scrollBar != (UnityEngine.Object) null)
                this.scrollBar.SetScrollPercentWithoutEvent(this.percent);
            this.SetContentPosition();
        }

        public float MeasureContentLength()
        {
            Vector3 vector3_1 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 vector3_2 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3[] minMax = new Vector3[2]
            {
                vector3_2,
                vector3_1
            };
            Transform transform = this.contentContainer.transform;
            tk2dUIScrollableArea.GetRendererBoundsInChildren(transform.worldToLocalMatrix, minMax, transform);
            if (minMax[0] != vector3_2 && minMax[1] != vector3_1)
            {
                minMax[0] = Vector3.Min(minMax[0], Vector3.zero);
                minMax[1] = Vector3.Max(minMax[1], Vector3.zero);
                return this.scrollAxes == tk2dUIScrollableArea.Axes.YAxis ? minMax[1].y - minMax[0].y : minMax[1].x - minMax[0].x;
            }
            Debug.LogError((object) "Unable to measure content length");
            return this.VisibleAreaLength * 0.9f;
        }

        private void OnEnable()
        {
            if ((UnityEngine.Object) this.scrollBar != (UnityEngine.Object) null)
                this.scrollBar.OnScroll += new Action<tk2dUIScrollbar>(this.ScrollBarMove);
            if ((UnityEngine.Object) this.backgroundUIItem != (UnityEngine.Object) null)
            {
                this.backgroundUIItem.OnDown += new System.Action(this.BackgroundButtonDown);
                this.backgroundUIItem.OnRelease += new System.Action(this.BackgroundButtonRelease);
                this.backgroundUIItem.OnHoverOver += new System.Action(this.BackgroundButtonHoverOver);
                this.backgroundUIItem.OnHoverOut += new System.Action(this.BackgroundButtonHoverOut);
            }
            if ((UnityEngine.Object) this.backgroundLayoutItem != (UnityEngine.Object) null)
                this.backgroundLayoutItem.OnReshape += new Action<Vector3, Vector3>(this.LayoutReshaped);
            if (!((UnityEngine.Object) this.contentLayoutContainer != (UnityEngine.Object) null))
                return;
            this.contentLayoutContainer.OnChangeContent += new System.Action(this.ContentLayoutChangeCallback);
        }

        private void OnDisable()
        {
            if ((UnityEngine.Object) this.scrollBar != (UnityEngine.Object) null)
                this.scrollBar.OnScroll -= new Action<tk2dUIScrollbar>(this.ScrollBarMove);
            if ((UnityEngine.Object) this.backgroundUIItem != (UnityEngine.Object) null)
            {
                this.backgroundUIItem.OnDown -= new System.Action(this.BackgroundButtonDown);
                this.backgroundUIItem.OnRelease -= new System.Action(this.BackgroundButtonRelease);
                this.backgroundUIItem.OnHoverOver -= new System.Action(this.BackgroundButtonHoverOver);
                this.backgroundUIItem.OnHoverOut -= new System.Action(this.BackgroundButtonHoverOut);
            }
            if (this.isBackgroundButtonOver)
            {
                if ((UnityEngine.Object) tk2dUIManager.Instance__NoCreate != (UnityEngine.Object) null)
                    tk2dUIManager.Instance.OnScrollWheelChange -= new Action<float>(this.BackgroundHoverOverScrollWheelChange);
                this.isBackgroundButtonOver = false;
            }
            if (this.isBackgroundButtonDown || this.isSwipeScrollingInProgress)
            {
                if ((UnityEngine.Object) tk2dUIManager.Instance__NoCreate != (UnityEngine.Object) null)
                    tk2dUIManager.Instance.OnInputUpdate -= new System.Action(this.BackgroundOverUpdate);
                this.isBackgroundButtonDown = false;
                this.isSwipeScrollingInProgress = false;
            }
            if ((UnityEngine.Object) this.backgroundLayoutItem != (UnityEngine.Object) null)
                this.backgroundLayoutItem.OnReshape -= new Action<Vector3, Vector3>(this.LayoutReshaped);
            if ((UnityEngine.Object) this.contentLayoutContainer != (UnityEngine.Object) null)
                this.contentLayoutContainer.OnChangeContent -= new System.Action(this.ContentLayoutChangeCallback);
            this.swipeCurrVelocity = 0.0f;
        }

        private void Start() => this.UpdateScrollbarActiveState();

        private void BackgroundHoverOverScrollWheelChange(float mouseWheelChange)
        {
            if ((double) mouseWheelChange > 0.0)
            {
                if ((bool) (UnityEngine.Object) this.scrollBar)
                    this.scrollBar.ScrollUpFixed();
                else
                    this.Value -= 0.1f;
            }
            else
            {
                if ((double) mouseWheelChange >= 0.0)
                    return;
                if ((bool) (UnityEngine.Object) this.scrollBar)
                    this.scrollBar.ScrollDownFixed();
                else
                    this.Value += 0.1f;
            }
        }

        private void ScrollBarMove(tk2dUIScrollbar scrollBar)
        {
            this.Value = scrollBar.Value;
            this.isSwipeScrollingInProgress = false;
            if (!this.isBackgroundButtonDown)
                return;
            this.BackgroundButtonRelease();
        }

        private Vector3 ContentContainerOffset
        {
            get => Vector3.Scale(new Vector3(-1f, 1f, 1f), this.contentContainer.transform.localPosition);
            set
            {
                this.contentContainer.transform.localPosition = Vector3.Scale(new Vector3(-1f, 1f, 1f), value);
            }
        }

        private void SetContentPosition()
        {
            Vector3 contentContainerOffset = this.ContentContainerOffset;
            float num = (this.contentLength - this.visibleAreaLength) * this.Value;
            if ((double) num < 0.0)
                num = 0.0f;
            if (this.scrollAxes == tk2dUIScrollableArea.Axes.XAxis)
                contentContainerOffset.x = num;
            else if (this.scrollAxes == tk2dUIScrollableArea.Axes.YAxis)
                contentContainerOffset.y = num;
            this.ContentContainerOffset = contentContainerOffset;
        }

        private void BackgroundButtonDown()
        {
            if (!this.allowSwipeScrolling || (double) this.contentLength <= (double) this.visibleAreaLength)
                return;
            if (!this.isBackgroundButtonDown && !this.isSwipeScrollingInProgress)
                tk2dUIManager.Instance.OnInputUpdate += new System.Action(this.BackgroundOverUpdate);
            this.swipeScrollingPressDownStartLocalPos = this.transform.InverseTransformPoint(this.CalculateClickWorldPos(this.backgroundUIItem));
            this.swipePrevScrollingContentPressLocalPos = this.swipeScrollingPressDownStartLocalPos;
            this.swipeScrollingContentStartLocalPos = this.ContentContainerOffset;
            this.swipeScrollingContentDestLocalPos = this.swipeScrollingContentStartLocalPos;
            this.isBackgroundButtonDown = true;
            this.swipeCurrVelocity = 0.0f;
        }

        private void BackgroundOverUpdate()
        {
            if (this.isBackgroundButtonDown)
                this.UpdateSwipeScrollDestintationPosition();
            if (!this.isSwipeScrollingInProgress)
                return;
            float percent = this.percent;
            float current = 0.0f;
            if (this.scrollAxes == tk2dUIScrollableArea.Axes.XAxis)
                current = this.swipeScrollingContentDestLocalPos.x;
            else if (this.scrollAxes == tk2dUIScrollableArea.Axes.YAxis)
                current = this.swipeScrollingContentDestLocalPos.y;
            float num1 = 0.0f;
            float num2 = this.contentLength - this.visibleAreaLength;
            float num3;
            if (this.isBackgroundButtonDown)
            {
                if ((double) current < (double) num1)
                {
                    current += (float) (-(double) current / (double) this.visibleAreaLength / 2.0);
                    if ((double) current > (double) num1)
                        current = num1;
                }
                else if ((double) current > (double) num2)
                {
                    current -= (float) (((double) current - (double) num2) / (double) this.visibleAreaLength / 2.0);
                    if ((double) current < (double) num2)
                        current = num2;
                }
                if (this.scrollAxes == tk2dUIScrollableArea.Axes.XAxis)
                    this.swipeScrollingContentDestLocalPos.x = current;
                else if (this.scrollAxes == tk2dUIScrollableArea.Axes.YAxis)
                    this.swipeScrollingContentDestLocalPos.y = current;
                num3 = current / (this.contentLength - this.visibleAreaLength);
            }
            else
            {
                float num4 = this.visibleAreaLength * (1f / 1000f);
                if ((double) current < (double) num1 || (double) current > (double) num2)
                {
                    float target = (double) current >= (double) num1 ? num2 : num1;
                    current = Mathf.SmoothDamp(current, target, ref this.snapBackVelocity, 0.05f, float.PositiveInfinity, tk2dUITime.deltaTime);
                    if ((double) Mathf.Abs(this.snapBackVelocity) < (double) num4)
                    {
                        current = target;
                        this.snapBackVelocity = 0.0f;
                    }
                    this.swipeCurrVelocity = 0.0f;
                }
                else if ((double) this.swipeCurrVelocity != 0.0)
                {
                    current += (float) ((double) this.swipeCurrVelocity * (double) tk2dUITime.deltaTime * 20.0);
                    this.swipeCurrVelocity = (double) this.swipeCurrVelocity > (double) num4 || (double) this.swipeCurrVelocity < -(double) num4 ? Mathf.Lerp(this.swipeCurrVelocity, 0.0f, tk2dUITime.deltaTime * 2.5f) : 0.0f;
                }
                else
                {
                    this.isSwipeScrollingInProgress = false;
                    tk2dUIManager.Instance.OnInputUpdate -= new System.Action(this.BackgroundOverUpdate);
                }
                if (this.scrollAxes == tk2dUIScrollableArea.Axes.XAxis)
                    this.swipeScrollingContentDestLocalPos.x = current;
                else if (this.scrollAxes == tk2dUIScrollableArea.Axes.YAxis)
                    this.swipeScrollingContentDestLocalPos.y = current;
                num3 = current / (this.contentLength - this.visibleAreaLength);
            }
            if ((double) num3 != (double) this.percent)
            {
                this.percent = num3;
                this.ContentContainerOffset = this.swipeScrollingContentDestLocalPos;
                if (this.OnScroll != null)
                    this.OnScroll(this);
                this.TargetOnScrollCallback();
            }
            if (!((UnityEngine.Object) this.scrollBar != (UnityEngine.Object) null))
                return;
            float newScrollPercent = this.percent;
            if (this.scrollAxes == tk2dUIScrollableArea.Axes.XAxis)
                newScrollPercent = this.ContentContainerOffset.x / (this.contentLength - this.visibleAreaLength);
            else if (this.scrollAxes == tk2dUIScrollableArea.Axes.YAxis)
                newScrollPercent = this.ContentContainerOffset.y / (this.contentLength - this.visibleAreaLength);
            this.scrollBar.SetScrollPercentWithoutEvent(newScrollPercent);
        }

        private void UpdateSwipeScrollDestintationPosition()
        {
            Vector3 vector3_1 = this.transform.InverseTransformPoint(this.CalculateClickWorldPos(this.backgroundUIItem));
            Vector3 vector3_2 = vector3_1 - this.swipeScrollingPressDownStartLocalPos;
            vector3_2.x *= -1f;
            float f = 0.0f;
            if (this.scrollAxes == tk2dUIScrollableArea.Axes.XAxis)
            {
                f = vector3_2.x;
                this.swipeCurrVelocity = (float) -((double) vector3_1.x - (double) this.swipePrevScrollingContentPressLocalPos.x);
            }
            else if (this.scrollAxes == tk2dUIScrollableArea.Axes.YAxis)
            {
                f = vector3_2.y;
                this.swipeCurrVelocity = vector3_1.y - this.swipePrevScrollingContentPressLocalPos.y;
            }
            if (!this.isSwipeScrollingInProgress && (double) Mathf.Abs(f) > 0.019999999552965164)
            {
                this.isSwipeScrollingInProgress = true;
                tk2dUIManager.Instance.OverrideClearAllChildrenPresses(this.backgroundUIItem);
            }
            if (!this.isSwipeScrollingInProgress)
                return;
            Vector3 vector3_3 = (this.swipeScrollingContentStartLocalPos + vector3_2) with
            {
                z = this.ContentContainerOffset.z
            };
            if (this.scrollAxes == tk2dUIScrollableArea.Axes.XAxis)
                vector3_3.y = this.ContentContainerOffset.y;
            else if (this.scrollAxes == tk2dUIScrollableArea.Axes.YAxis)
                vector3_3.x = this.ContentContainerOffset.x;
            vector3_3.z = this.ContentContainerOffset.z;
            this.swipeScrollingContentDestLocalPos = vector3_3;
            this.swipePrevScrollingContentPressLocalPos = vector3_1;
        }

        private void BackgroundButtonRelease()
        {
            if (!this.allowSwipeScrolling)
                return;
            if (this.isBackgroundButtonDown && !this.isSwipeScrollingInProgress)
                tk2dUIManager.Instance.OnInputUpdate -= new System.Action(this.BackgroundOverUpdate);
            this.isBackgroundButtonDown = false;
        }

        private void BackgroundButtonHoverOver()
        {
            if (!this.allowScrollWheel)
                return;
            if (!this.isBackgroundButtonOver)
                tk2dUIManager.Instance.OnScrollWheelChange += new Action<float>(this.BackgroundHoverOverScrollWheelChange);
            this.isBackgroundButtonOver = true;
        }

        private void BackgroundButtonHoverOut()
        {
            if (this.isBackgroundButtonOver)
                tk2dUIManager.Instance.OnScrollWheelChange -= new Action<float>(this.BackgroundHoverOverScrollWheelChange);
            this.isBackgroundButtonOver = false;
        }

        private Vector3 CalculateClickWorldPos(tk2dUIItem btn)
        {
            Vector2 position = btn.Touch.position;
            Camera cameraForControl = tk2dUIManager.Instance.GetUICameraForControl(this.gameObject);
            return cameraForControl.ScreenToWorldPoint(new Vector3(position.x, position.y, btn.transform.position.z - cameraForControl.transform.position.z)) with
            {
                z = btn.transform.position.z
            };
        }

        private void UpdateScrollbarActiveState()
        {
            bool isActive = (double) this.contentLength > (double) this.visibleAreaLength;
            if (!((UnityEngine.Object) this.scrollBar != (UnityEngine.Object) null) || this.scrollBar.gameObject.activeSelf == isActive)
                return;
            tk2dUIBaseItemControl.ChangeGameObjectActiveState(this.scrollBar.gameObject, isActive);
        }

        private void ContentLengthVisibleAreaLengthChange(
            float prevContentLength,
            float newContentLength,
            float prevVisibleAreaLength,
            float newVisibleAreaLength)
        {
            float num = (double) newContentLength - (double) this.visibleAreaLength == 0.0 ? 0.0f : (float) (((double) prevContentLength - (double) prevVisibleAreaLength) * (double) this.Value / ((double) newContentLength - (double) newVisibleAreaLength));
            this.contentLength = newContentLength;
            this.visibleAreaLength = newVisibleAreaLength;
            this.UpdateScrollbarActiveState();
            this.Value = num;
        }

        private void UnpressAllUIItemChildren()
        {
        }

        private void TargetOnScrollCallback()
        {
            if (!((UnityEngine.Object) this.SendMessageTarget != (UnityEngine.Object) null) || this.SendMessageOnScrollMethodName.Length <= 0)
                return;
            this.SendMessageTarget.SendMessage(this.SendMessageOnScrollMethodName, (object) this, SendMessageOptions.RequireReceiver);
        }

        private static void GetRendererBoundsInChildren(
            Matrix4x4 rootWorldToLocal,
            Vector3[] minMax,
            Transform t)
        {
            MeshFilter component = t.GetComponent<MeshFilter>();
            if ((UnityEngine.Object) component != (UnityEngine.Object) null && (UnityEngine.Object) component.sharedMesh != (UnityEngine.Object) null)
            {
                Bounds bounds = component.sharedMesh.bounds;
                Matrix4x4 matrix4x4 = rootWorldToLocal * t.localToWorldMatrix;
                for (int index = 0; index < 8; ++index)
                {
                    Vector3 point = bounds.center + Vector3.Scale(bounds.extents, tk2dUIScrollableArea.boxExtents[index]);
                    Vector3 rhs = matrix4x4.MultiplyPoint(point);
                    minMax[0] = Vector3.Min(minMax[0], rhs);
                    minMax[1] = Vector3.Max(minMax[1], rhs);
                }
            }
            int childCount = t.childCount;
            for (int index = 0; index < childCount; ++index)
            {
                Transform child = t.GetChild(index);
                if (t.gameObject.activeSelf)
                    tk2dUIScrollableArea.GetRendererBoundsInChildren(rootWorldToLocal, minMax, child);
            }
        }

        private void LayoutReshaped(Vector3 dMin, Vector3 dMax)
        {
            this.VisibleAreaLength += this.scrollAxes != tk2dUIScrollableArea.Axes.XAxis ? dMax.y - dMin.y : dMax.x - dMin.x;
        }

        private void ContentLayoutChangeCallback()
        {
            if (!((UnityEngine.Object) this.contentLayoutContainer != (UnityEngine.Object) null))
                return;
            Vector2 innerSize = this.contentLayoutContainer.GetInnerSize();
            this.ContentLength = this.scrollAxes != tk2dUIScrollableArea.Axes.XAxis ? innerSize.y : innerSize.x;
        }

        public enum Axes
        {
            XAxis,
            YAxis,
        }
    }

