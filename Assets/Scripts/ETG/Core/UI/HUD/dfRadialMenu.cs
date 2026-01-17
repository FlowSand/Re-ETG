// Decompiled with JetBrains decompiler
// Type: dfRadialMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    [ExecuteInEditMode]
    [AddComponentMenu("Daikon Forge/Examples/Menus/Radial Menu")]
    [Serializable]
    public class dfRadialMenu : MonoBehaviour
    {
      public float radius = 200f;
      public float startAngle;
      public float openAngle = 360f;
      public bool rotateButtons;
      public bool animateOpacity;
      public bool animateOpenAngle;
      public bool animateRadius;
      public bool autoToggle;
      public bool closeOnLostFocus;
      public float animationLength = 0.5f;
      public List<dfControl> excludedControls = new List<dfControl>();
      public dfControl host;
      private bool isAnimating;
      private bool isOpen;

      public event dfRadialMenu.CircularMenuEventHandler BeforeMenuOpened;

      public event dfRadialMenu.CircularMenuEventHandler MenuOpened;

      public event dfRadialMenu.CircularMenuEventHandler MenuClosed;

      public bool IsOpen
      {
        get => this.isOpen;
        set
        {
          if (this.isOpen == value)
            return;
          if (value)
            this.Open();
          else
            this.Close();
        }
      }

      public void Open()
      {
        if (this.isOpen || this.isAnimating || !this.enabled || !this.gameObject.activeSelf)
          return;
        this.StartCoroutine(this.openMenu());
      }

      public void Close()
      {
        if (!this.isOpen || this.isAnimating || !this.enabled || !this.gameObject.activeSelf)
          return;
        this.StartCoroutine(this.closeMenu());
        if (!this.host.ContainsFocus)
          return;
        dfGUIManager.SetFocus((dfControl) null);
      }

      public void Toggle()
      {
        if (this.isAnimating)
          return;
        if (this.isOpen)
          this.Close();
        else
          this.Open();
      }

      public void OnEnable()
      {
        if (!((UnityEngine.Object) this.host == (UnityEngine.Object) null))
          return;
        this.host = this.GetComponent<dfControl>();
      }

      public void Start()
      {
        if (!Application.isPlaying)
          return;
        using (dfList<dfControl> buttons = this.getButtons())
        {
          for (int index = 0; index < buttons.Count; ++index)
            buttons[index].Hide();
        }
      }

      public void Update()
      {
        if (Application.isPlaying)
          return;
        this.arrangeButtons();
      }

      public void OnLeaveFocus(dfControl sender, dfFocusEventArgs args)
      {
        if (!this.closeOnLostFocus || this.host.ContainsFocus || !Application.isPlaying)
          return;
        this.Close();
      }

      public void OnClick(dfControl sender, dfMouseEventArgs args)
      {
        if (!this.autoToggle && !((UnityEngine.Object) args.Source == (UnityEngine.Object) this.host))
          return;
        this.Toggle();
      }

      private dfList<dfControl> getButtons()
      {
        return this.host.Controls.Where((Func<dfControl, bool>) (x => x.enabled && !this.excludedControls.Contains(x)));
      }

      private void arrangeButtons()
      {
        this.arrangeButtons(this.startAngle, this.radius, this.openAngle, 1f);
      }

      [DebuggerHidden]
      private IEnumerator openMenu()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new dfRadialMenu__openMenuc__Iterator0()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator closeMenu()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new dfRadialMenu__closeMenuc__Iterator1()
        {
          _this = this
        };
      }

      private void arrangeButtons(float startAngle, float radius, float openAngle, float opacity)
      {
        float z = this.clampRotation(startAngle);
        float num1 = radius;
        Vector3 vector3_1 = (Vector3) this.host.Size * 0.5f;
        using (dfList<dfControl> buttons = this.getButtons())
        {
          if (buttons.Count == 0)
            return;
          float num2 = Mathf.Sign(openAngle) * Mathf.Min(Mathf.Abs(this.clampRotation(openAngle)) / (float) (buttons.Count - 1), 360f / (float) buttons.Count);
          for (int index = 0; index < buttons.Count; ++index)
          {
            dfControl dfControl = buttons[index];
            Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, z);
            Vector3 vector3_2 = vector3_1 + quaternion * Vector3.down * num1;
            dfControl.RelativePosition = vector3_2 - (Vector3) dfControl.Size * 0.5f;
            if (this.rotateButtons)
            {
              dfControl.Pivot = dfPivotPoint.MiddleCenter;
              dfControl.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -z);
            }
            else
              dfControl.transform.localRotation = Quaternion.identity;
            dfControl.IsVisible = true;
            dfControl.Opacity = opacity;
            z += num2;
          }
        }
      }

      private float clampRotation(float rotation)
      {
        return Mathf.Sign(rotation) * Mathf.Max(0.1f, Mathf.Min(360f, Mathf.Abs(rotation)));
      }

      public delegate void CircularMenuEventHandler(dfRadialMenu sender);
    }

}
