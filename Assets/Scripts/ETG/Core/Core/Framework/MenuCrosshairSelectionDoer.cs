// Decompiled with JetBrains decompiler
// Type: MenuCrosshairSelectionDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class MenuCrosshairSelectionDoer : TimeInvariantMonoBehaviour
    {
      public dfControl controlToPlace;
      public dfControl targetControlToEncrosshair;
      public IntVector2 leftCrosshairPixelOffset;
      public IntVector2 rightCrosshairPixelOffset;
      public bool mouseFocusable = true;
      private List<dfControl> m_extantControls;
      private dfControl m_control;
      private bool m_suppressed;

      public dfControl CrosshairControl
      {
        get
        {
          return (UnityEngine.Object) this.targetControlToEncrosshair == (UnityEngine.Object) null ? this.m_control : this.targetControlToEncrosshair;
        }
      }

      private void Start()
      {
        this.m_control = this.GetComponent<dfControl>();
        this.m_extantControls = new List<dfControl>();
        if (this.mouseFocusable || true)
          this.m_control.MouseEnter += (MouseEventHandler) ((control, mouseArgs) =>
          {
            if (InControlInputAdapter.SkipInputForRestOfFrame)
              return;
            this.m_control.Focus(false);
          });
        this.m_control.GotFocus += new FocusEventHandler(this.GotFocus);
        this.m_control.LostFocus += new FocusEventHandler(this.LostFocus);
        UIKeyControls component1 = this.GetComponent<UIKeyControls>();
        if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
          component1.OnNewControlSelected += new Action<dfControl>(this.DifferentControlSelected);
        BraveOptionsMenuItem component2 = this.GetComponent<BraveOptionsMenuItem>();
        if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null))
          return;
        component2.OnNewControlSelected += new Action<dfControl>(this.DifferentControlSelected);
      }

      private void LateUpdate()
      {
        if (this.m_suppressed || this.m_extantControls == null || this.m_extantControls.Count <= 0)
          return;
        this.UpdatedOwnedControls();
      }

      private void DifferentControlSelected(dfControl newControl)
      {
        MenuCrosshairSelectionDoer component = newControl.GetComponent<MenuCrosshairSelectionDoer>();
        if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || this.m_extantControls.Count != 2)
          return;
        this.m_suppressed = true;
        component.m_suppressed = true;
        this.StartCoroutine(this.HandleLerpyDerpy(component));
      }

      [DebuggerHidden]
      private IEnumerator HandleLerpyDerpy(MenuCrosshairSelectionDoer targetCrosshairDoer)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MenuCrosshairSelectionDoer.<HandleLerpyDerpy>c__Iterator0()
        {
          targetCrosshairDoer = targetCrosshairDoer,
          $this = this
        };
      }

      private void GotFocus(dfControl control, dfFocusEventArgs args)
      {
        int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_select_01", this.gameObject);
        if (!this.m_suppressed)
          ;
      }

      private void LostFocus(dfControl control, dfFocusEventArgs args)
      {
        if (!this.m_suppressed)
          ;
      }

      private void UpdatedOwnedControls()
      {
        dfControl extantControl1 = this.m_extantControls[0];
        dfControl extantControl2 = this.m_extantControls[1];
        float num1 = (float) ((double) this.CrosshairControl.Size.x / 2.0 + (double) extantControl1.Size.x / 2.0 + 6.0) * this.CrosshairControl.transform.lossyScale.x * this.CrosshairControl.PixelsToUnits();
        float num2 = -3f * this.CrosshairControl.PixelsToUnits();
        extantControl1.transform.position = this.CrosshairControl.GetCenter() + new Vector3(num1 * -1f, 0.0f, 0.0f) + this.leftCrosshairPixelOffset.ToVector3(0.0f) * this.CrosshairControl.PixelsToUnits();
        extantControl2.transform.position = this.CrosshairControl.GetCenter() + new Vector3(num1 + num2, 0.0f, 0.0f) + this.rightCrosshairPixelOffset.ToVector3(0.0f) * this.CrosshairControl.PixelsToUnits();
      }

      private void CreateOwnedControls()
      {
        dfControl dfControl1 = this.CrosshairControl.AddPrefab(this.controlToPlace.gameObject);
        dfControl dfControl2 = this.CrosshairControl.AddPrefab(this.controlToPlace.gameObject);
        dfControl1.IsVisible = false;
        dfControl2.IsVisible = false;
        dfControl1.transform.localScale = Vector3.one;
        dfControl2.transform.localScale = Vector3.one;
        dfControl2.GetComponent<dfSpriteAnimation>().Direction = dfPlayDirection.Reverse;
        float num1 = (float) ((double) this.CrosshairControl.Size.x / 2.0 + (double) dfControl1.Size.x / 2.0 + 6.0) * this.CrosshairControl.transform.lossyScale.x * this.CrosshairControl.PixelsToUnits();
        float num2 = -3f * this.CrosshairControl.PixelsToUnits();
        dfControl1.transform.position = this.CrosshairControl.GetCenter() + new Vector3(num1 * -1f, 0.0f, 0.0f) + this.leftCrosshairPixelOffset.ToVector3(0.0f) * this.CrosshairControl.PixelsToUnits();
        dfControl2.transform.position = this.CrosshairControl.GetCenter() + new Vector3(num1 + num2, 0.0f, 0.0f) + this.rightCrosshairPixelOffset.ToVector3(0.0f) * this.CrosshairControl.PixelsToUnits();
        this.m_extantControls.Add(dfControl1);
        this.m_extantControls.Add(dfControl2);
      }

      private void ClearExtantControls()
      {
        if (this.m_extantControls.Count <= 0)
          return;
        for (int index = 0; index < this.m_extantControls.Count; ++index)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantControls[index].gameObject);
        this.m_extantControls.Clear();
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
