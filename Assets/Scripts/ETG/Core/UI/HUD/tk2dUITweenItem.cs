// Decompiled with JetBrains decompiler
// Type: tk2dUITweenItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    [AddComponentMenu("2D Toolkit/UI/tk2dUITweenItem")]
    public class tk2dUITweenItem : tk2dUIBaseItemControl
    {
      private Vector3 onUpScale;
      public Vector3 onDownScale = new Vector3(0.9f, 0.9f, 0.9f);
      public float tweenDuration = 0.1f;
      public bool canButtonBeHeldDown = true;
      [SerializeField]
      private bool useOnReleaseInsteadOfOnUp;
      private bool internalTweenInProgress;
      private Vector3 tweenTargetScale = Vector3.one;
      private Vector3 tweenStartingScale = Vector3.one;
      private float tweenTimeElapsed;

      public bool UseOnReleaseInsteadOfOnUp => this.useOnReleaseInsteadOfOnUp;

      private void Awake() => this.onUpScale = this.transform.localScale;

      private void OnEnable()
      {
        if ((bool) (UnityEngine.Object) this.uiItem)
        {
          this.uiItem.OnDown += new System.Action(this.ButtonDown);
          if (this.canButtonBeHeldDown)
          {
            if (this.useOnReleaseInsteadOfOnUp)
              this.uiItem.OnRelease += new System.Action(this.ButtonUp);
            else
              this.uiItem.OnUp += new System.Action(this.ButtonUp);
          }
        }
        this.internalTweenInProgress = false;
        this.tweenTimeElapsed = 0.0f;
        this.transform.localScale = this.onUpScale;
      }

      private void OnDisable()
      {
        if (!(bool) (UnityEngine.Object) this.uiItem)
          return;
        this.uiItem.OnDown -= new System.Action(this.ButtonDown);
        if (!this.canButtonBeHeldDown)
          return;
        if (this.useOnReleaseInsteadOfOnUp)
          this.uiItem.OnRelease -= new System.Action(this.ButtonUp);
        else
          this.uiItem.OnUp -= new System.Action(this.ButtonUp);
      }

      private void ButtonDown()
      {
        if ((double) this.tweenDuration <= 0.0)
        {
          this.transform.localScale = this.onDownScale;
        }
        else
        {
          this.transform.localScale = this.onUpScale;
          this.tweenTargetScale = this.onDownScale;
          this.tweenStartingScale = this.transform.localScale;
          if (this.internalTweenInProgress)
            return;
          this.StartCoroutine(this.ScaleTween());
          this.internalTweenInProgress = true;
        }
      }

      private void ButtonUp()
      {
        if ((double) this.tweenDuration <= 0.0)
        {
          this.transform.localScale = this.onUpScale;
        }
        else
        {
          this.tweenTargetScale = this.onUpScale;
          this.tweenStartingScale = this.transform.localScale;
          if (this.internalTweenInProgress)
            return;
          this.StartCoroutine(this.ScaleTween());
          this.internalTweenInProgress = true;
        }
      }

      [DebuggerHidden]
      private IEnumerator ScaleTween()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new tk2dUITweenItem.<ScaleTween>c__Iterator0()
        {
          _this = this
        };
      }

      public void InternalSetUseOnReleaseInsteadOfOnUp(bool state)
      {
        this.useOnReleaseInsteadOfOnUp = state;
      }
    }

}
