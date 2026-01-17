// Decompiled with JetBrains decompiler
// Type: tk2dUIScrollbar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    [ExecuteInEditMode]
    [AddComponentMenu("2D Toolkit/UI/tk2dUIScrollbar")]
    public class tk2dUIScrollbar : MonoBehaviour
    {
      public tk2dUIItem barUIItem;
      public float scrollBarLength;
      public tk2dUIItem thumbBtn;
      public Transform thumbTransform;
      public float thumbLength;
      public tk2dUIItem upButton;
      private tk2dUIHoverItem hoverUpButton;
      public tk2dUIItem downButton;
      private tk2dUIHoverItem hoverDownButton;
      public float buttonUpDownScrollDistance = 1f;
      public bool allowScrollWheel = true;
      public tk2dUIScrollbar.Axes scrollAxes = tk2dUIScrollbar.Axes.YAxis;
      public tk2dUIProgressBar highlightProgressBar;
      [HideInInspector]
      [SerializeField]
      private tk2dUILayout barLayoutItem;
      private bool isScrollThumbButtonDown;
      private bool isTrackHoverOver;
      private float percent;
      private Vector3 moveThumbBtnOffset = Vector3.zero;
      private int scrollUpDownButtonState;
      private float timeOfUpDownButtonPressStart;
      private float repeatUpDownButtonHoldCounter;
      private const float WITHOUT_SCROLLBAR_FIXED_SCROLL_WHEEL_PERCENT = 0.1f;
      private const float INITIAL_TIME_TO_REPEAT_UP_DOWN_SCROLL_BUTTON_SCROLLING_ON_HOLD = 0.55f;
      private const float TIME_TO_REPEAT_UP_DOWN_SCROLL_BUTTON_SCROLLING_ON_HOLD = 0.45f;
      public string SendMessageOnScrollMethodName = string.Empty;

      public tk2dUILayout BarLayoutItem
      {
        get => this.barLayoutItem;
        set
        {
          if (!((UnityEngine.Object) this.barLayoutItem != (UnityEngine.Object) value))
            return;
          if ((UnityEngine.Object) this.barLayoutItem != (UnityEngine.Object) null)
            this.barLayoutItem.OnReshape -= new Action<Vector3, Vector3>(this.LayoutReshaped);
          this.barLayoutItem = value;
          if (!((UnityEngine.Object) this.barLayoutItem != (UnityEngine.Object) null))
            return;
          this.barLayoutItem.OnReshape += new Action<Vector3, Vector3>(this.LayoutReshaped);
        }
      }

      public event Action<tk2dUIScrollbar> OnScroll;

      public GameObject SendMessageTarget
      {
        get
        {
          return (UnityEngine.Object) this.barUIItem != (UnityEngine.Object) null ? this.barUIItem.sendMessageTarget : (GameObject) null;
        }
        set
        {
          if (!((UnityEngine.Object) this.barUIItem != (UnityEngine.Object) null) || !((UnityEngine.Object) this.barUIItem.sendMessageTarget != (UnityEngine.Object) value))
            return;
          this.barUIItem.sendMessageTarget = value;
        }
      }

      public float Value
      {
        get => this.percent;
        set
        {
          this.percent = Mathf.Clamp(value, 0.0f, 1f);
          if (this.OnScroll != null)
            this.OnScroll(this);
          this.SetScrollThumbPosition();
          if (!((UnityEngine.Object) this.SendMessageTarget != (UnityEngine.Object) null) || this.SendMessageOnScrollMethodName.Length <= 0)
            return;
          this.SendMessageTarget.SendMessage(this.SendMessageOnScrollMethodName, (object) this, SendMessageOptions.RequireReceiver);
        }
      }

      public void SetScrollPercentWithoutEvent(float newScrollPercent)
      {
        this.percent = Mathf.Clamp(newScrollPercent, 0.0f, 1f);
        this.SetScrollThumbPosition();
      }

      private void OnEnable()
      {
        if ((UnityEngine.Object) this.barUIItem != (UnityEngine.Object) null)
        {
          this.barUIItem.OnDown += new System.Action(this.ScrollTrackButtonDown);
          this.barUIItem.OnHoverOver += new System.Action(this.ScrollTrackButtonHoverOver);
          this.barUIItem.OnHoverOut += new System.Action(this.ScrollTrackButtonHoverOut);
        }
        if ((UnityEngine.Object) this.thumbBtn != (UnityEngine.Object) null)
        {
          this.thumbBtn.OnDown += new System.Action(this.ScrollThumbButtonDown);
          this.thumbBtn.OnRelease += new System.Action(this.ScrollThumbButtonRelease);
        }
        if ((UnityEngine.Object) this.upButton != (UnityEngine.Object) null)
        {
          this.upButton.OnDown += new System.Action(this.ScrollUpButtonDown);
          this.upButton.OnUp += new System.Action(this.ScrollUpButtonUp);
        }
        if ((UnityEngine.Object) this.downButton != (UnityEngine.Object) null)
        {
          this.downButton.OnDown += new System.Action(this.ScrollDownButtonDown);
          this.downButton.OnUp += new System.Action(this.ScrollDownButtonUp);
        }
        if (!((UnityEngine.Object) this.barLayoutItem != (UnityEngine.Object) null))
          return;
        this.barLayoutItem.OnReshape += new Action<Vector3, Vector3>(this.LayoutReshaped);
      }

      private void OnDisable()
      {
        if ((UnityEngine.Object) this.barUIItem != (UnityEngine.Object) null)
        {
          this.barUIItem.OnDown -= new System.Action(this.ScrollTrackButtonDown);
          this.barUIItem.OnHoverOver -= new System.Action(this.ScrollTrackButtonHoverOver);
          this.barUIItem.OnHoverOut -= new System.Action(this.ScrollTrackButtonHoverOut);
        }
        if ((UnityEngine.Object) this.thumbBtn != (UnityEngine.Object) null)
        {
          this.thumbBtn.OnDown -= new System.Action(this.ScrollThumbButtonDown);
          this.thumbBtn.OnRelease -= new System.Action(this.ScrollThumbButtonRelease);
        }
        if ((UnityEngine.Object) this.upButton != (UnityEngine.Object) null)
        {
          this.upButton.OnDown -= new System.Action(this.ScrollUpButtonDown);
          this.upButton.OnUp -= new System.Action(this.ScrollUpButtonUp);
        }
        if ((UnityEngine.Object) this.downButton != (UnityEngine.Object) null)
        {
          this.downButton.OnDown -= new System.Action(this.ScrollDownButtonDown);
          this.downButton.OnUp -= new System.Action(this.ScrollDownButtonUp);
        }
        if (this.isScrollThumbButtonDown)
        {
          if ((UnityEngine.Object) tk2dUIManager.Instance__NoCreate != (UnityEngine.Object) null)
            tk2dUIManager.Instance.OnInputUpdate -= new System.Action(this.MoveScrollThumbButton);
          this.isScrollThumbButtonDown = false;
        }
        if (this.isTrackHoverOver)
        {
          if ((UnityEngine.Object) tk2dUIManager.Instance__NoCreate != (UnityEngine.Object) null)
            tk2dUIManager.Instance.OnScrollWheelChange -= new Action<float>(this.TrackHoverScrollWheelChange);
          this.isTrackHoverOver = false;
        }
        if (this.scrollUpDownButtonState != 0)
        {
          tk2dUIManager.Instance.OnInputUpdate -= new System.Action(this.CheckRepeatScrollUpDownButton);
          this.scrollUpDownButtonState = 0;
        }
        if (!((UnityEngine.Object) this.barLayoutItem != (UnityEngine.Object) null))
          return;
        this.barLayoutItem.OnReshape -= new Action<Vector3, Vector3>(this.LayoutReshaped);
      }

      private void Awake()
      {
        if ((UnityEngine.Object) this.upButton != (UnityEngine.Object) null)
          this.hoverUpButton = this.upButton.GetComponent<tk2dUIHoverItem>();
        if (!((UnityEngine.Object) this.downButton != (UnityEngine.Object) null))
          return;
        this.hoverDownButton = this.downButton.GetComponent<tk2dUIHoverItem>();
      }

      private void Start() => this.SetScrollThumbPosition();

      private void TrackHoverScrollWheelChange(float mouseWheelChange)
      {
        if ((double) mouseWheelChange > 0.0)
        {
          this.ScrollUpFixed();
        }
        else
        {
          if ((double) mouseWheelChange >= 0.0)
            return;
          this.ScrollDownFixed();
        }
      }

      private void SetScrollThumbPosition()
      {
        if ((UnityEngine.Object) this.thumbTransform != (UnityEngine.Object) null)
        {
          float num = (float) -(((double) this.scrollBarLength - (double) this.thumbLength) * (double) this.Value);
          Vector3 localPosition = this.thumbTransform.localPosition;
          if (this.scrollAxes == tk2dUIScrollbar.Axes.XAxis)
            localPosition.x = -num;
          else if (this.scrollAxes == tk2dUIScrollbar.Axes.YAxis)
            localPosition.y = num;
          this.thumbTransform.localPosition = localPosition;
        }
        if (!((UnityEngine.Object) this.highlightProgressBar != (UnityEngine.Object) null))
          return;
        this.highlightProgressBar.Value = this.Value;
      }

      private void MoveScrollThumbButton()
      {
        this.ScrollToPosition(this.CalculateClickWorldPos(this.thumbBtn) + this.moveThumbBtnOffset);
      }

      private Vector3 CalculateClickWorldPos(tk2dUIItem btn)
      {
        Camera cameraForControl = tk2dUIManager.Instance.GetUICameraForControl(this.gameObject);
        Vector2 position = btn.Touch.position;
        return cameraForControl.ScreenToWorldPoint(new Vector3(position.x, position.y, btn.transform.position.z - cameraForControl.transform.position.z)) with
        {
          z = btn.transform.position.z
        };
      }

      private void ScrollToPosition(Vector3 worldPos)
      {
        Vector3 vector3 = this.thumbTransform.parent.InverseTransformPoint(worldPos);
        float num = 0.0f;
        if (this.scrollAxes == tk2dUIScrollbar.Axes.XAxis)
          num = vector3.x;
        else if (this.scrollAxes == tk2dUIScrollbar.Axes.YAxis)
          num = -vector3.y;
        this.Value = num / (this.scrollBarLength - this.thumbLength);
      }

      private void ScrollTrackButtonDown()
      {
        this.ScrollToPosition(this.CalculateClickWorldPos(this.barUIItem));
      }

      private void ScrollTrackButtonHoverOver()
      {
        if (!this.allowScrollWheel)
          return;
        if (!this.isTrackHoverOver)
          tk2dUIManager.Instance.OnScrollWheelChange += new Action<float>(this.TrackHoverScrollWheelChange);
        this.isTrackHoverOver = true;
      }

      private void ScrollTrackButtonHoverOut()
      {
        if (this.isTrackHoverOver)
          tk2dUIManager.Instance.OnScrollWheelChange -= new Action<float>(this.TrackHoverScrollWheelChange);
        this.isTrackHoverOver = false;
      }

      private void ScrollThumbButtonDown()
      {
        if (!this.isScrollThumbButtonDown)
          tk2dUIManager.Instance.OnInputUpdate += new System.Action(this.MoveScrollThumbButton);
        this.isScrollThumbButtonDown = true;
        this.moveThumbBtnOffset = this.thumbBtn.transform.position - this.CalculateClickWorldPos(this.thumbBtn);
        this.moveThumbBtnOffset.z = 0.0f;
        if ((UnityEngine.Object) this.hoverUpButton != (UnityEngine.Object) null)
          this.hoverUpButton.IsOver = true;
        if (!((UnityEngine.Object) this.hoverDownButton != (UnityEngine.Object) null))
          return;
        this.hoverDownButton.IsOver = true;
      }

      private void ScrollThumbButtonRelease()
      {
        if (this.isScrollThumbButtonDown)
          tk2dUIManager.Instance.OnInputUpdate -= new System.Action(this.MoveScrollThumbButton);
        this.isScrollThumbButtonDown = false;
        if ((UnityEngine.Object) this.hoverUpButton != (UnityEngine.Object) null)
          this.hoverUpButton.IsOver = false;
        if (!((UnityEngine.Object) this.hoverDownButton != (UnityEngine.Object) null))
          return;
        this.hoverDownButton.IsOver = false;
      }

      private void ScrollUpButtonDown()
      {
        this.timeOfUpDownButtonPressStart = UnityEngine.Time.realtimeSinceStartup;
        this.repeatUpDownButtonHoldCounter = 0.0f;
        if (this.scrollUpDownButtonState == 0)
          tk2dUIManager.Instance.OnInputUpdate += new System.Action(this.CheckRepeatScrollUpDownButton);
        this.scrollUpDownButtonState = -1;
        this.ScrollUpFixed();
      }

      private void ScrollUpButtonUp()
      {
        if (this.scrollUpDownButtonState != 0)
          tk2dUIManager.Instance.OnInputUpdate -= new System.Action(this.CheckRepeatScrollUpDownButton);
        this.scrollUpDownButtonState = 0;
      }

      private void ScrollDownButtonDown()
      {
        this.timeOfUpDownButtonPressStart = UnityEngine.Time.realtimeSinceStartup;
        this.repeatUpDownButtonHoldCounter = 0.0f;
        if (this.scrollUpDownButtonState == 0)
          tk2dUIManager.Instance.OnInputUpdate += new System.Action(this.CheckRepeatScrollUpDownButton);
        this.scrollUpDownButtonState = 1;
        this.ScrollDownFixed();
      }

      private void ScrollDownButtonUp()
      {
        if (this.scrollUpDownButtonState != 0)
          tk2dUIManager.Instance.OnInputUpdate -= new System.Action(this.CheckRepeatScrollUpDownButton);
        this.scrollUpDownButtonState = 0;
      }

      public void ScrollUpFixed() => this.ScrollDirection(-1);

      public void ScrollDownFixed() => this.ScrollDirection(1);

      private void CheckRepeatScrollUpDownButton()
      {
        if (this.scrollUpDownButtonState == 0)
          return;
        float num1 = UnityEngine.Time.realtimeSinceStartup - this.timeOfUpDownButtonPressStart;
        float num2;
        if ((double) this.repeatUpDownButtonHoldCounter == 0.0)
        {
          if ((double) num1 <= 0.550000011920929)
            return;
          ++this.repeatUpDownButtonHoldCounter;
          num2 = num1 - 0.55f;
          this.ScrollDirection(this.scrollUpDownButtonState);
        }
        else
        {
          if ((double) num1 <= 0.44999998807907104)
            return;
          ++this.repeatUpDownButtonHoldCounter;
          num2 = num1 - 0.45f;
          this.ScrollDirection(this.scrollUpDownButtonState);
        }
      }

      public void ScrollDirection(int dir)
      {
        if (this.scrollAxes == tk2dUIScrollbar.Axes.XAxis)
          this.Value -= this.CalcScrollPercentOffsetButtonScrollDistance() * (float) dir * this.buttonUpDownScrollDistance;
        else
          this.Value += this.CalcScrollPercentOffsetButtonScrollDistance() * (float) dir * this.buttonUpDownScrollDistance;
      }

      private float CalcScrollPercentOffsetButtonScrollDistance() => 0.1f;

      private void LayoutReshaped(Vector3 dMin, Vector3 dMax)
      {
        this.scrollBarLength += this.scrollAxes != tk2dUIScrollbar.Axes.XAxis ? dMax.y - dMin.y : dMax.x - dMin.x;
      }

      public enum Axes
      {
        XAxis,
        YAxis,
      }
    }

}
