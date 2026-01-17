// Decompiled with JetBrains decompiler
// Type: ClockhairController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class ClockhairController : TimeInvariantMonoBehaviour
    {
      public float ClockhairInDuration = 2f;
      public float ClockhairSpinDuration = 1f;
      public float ClockhairPauseBeforeShot = 0.5f;
      public Transform hourHandPivot;
      public Transform minuteHandPivot;
      public Transform secondHandPivot;
      public tk2dSpriteAnimator hourAnimator;
      public tk2dSpriteAnimator minuteAnimator;
      public tk2dSpriteAnimator secondAnimator;
      private bool m_shouldDesat;
      private float m_desatRadius;
      private bool m_shouldDistort;
      private float m_distortIntensity;
      private float m_distortRadius;
      private float m_edgeRadius = 20f;
      public bool IsSpinningWildly;
      public bool HasMotionCoroutine;
      private float m_motionType;

      private void Start()
      {
        this.Initialize();
        this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unfaded"));
      }

      public void Initialize() => this.SetToTime(DateTime.Now.TimeOfDay);

      public void SetMotionType(float motionType)
      {
        if (this.IsSpinningWildly)
          return;
        if (this.HasMotionCoroutine)
        {
          this.m_motionType = motionType;
        }
        else
        {
          this.m_motionType = motionType;
          this.StartCoroutine(this.HandleSimpleMotion());
        }
      }

      public void UpdateDesat(bool shouldDesat, float desatRadiusUV)
      {
        this.m_desatRadius = desatRadiusUV;
        if (shouldDesat)
        {
          if (this.m_shouldDesat)
            return;
          this.m_shouldDesat = true;
          this.StartCoroutine(this.HandleDesat());
        }
        else
          this.m_shouldDesat = false;
      }

      [DebuggerHidden]
      private IEnumerator HandleDesat()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ClockhairController.<HandleDesat>c__Iterator0()
        {
          $this = this
        };
      }

      [DebuggerHidden]
      public IEnumerator WipeoutDistortionAndFade(float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ClockhairController.<WipeoutDistortionAndFade>c__Iterator1()
        {
          duration = duration,
          $this = this
        };
      }

      public void UpdateDistortion(float distortionPower, float distortRadius, float edgeRadius)
      {
        if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW)
          return;
        if ((double) distortionPower != 0.0)
        {
          this.m_distortIntensity = distortionPower;
          this.m_distortRadius = distortRadius;
          this.m_edgeRadius = edgeRadius;
          if (this.m_shouldDistort)
            return;
          this.m_shouldDistort = true;
          this.StartCoroutine(this.HandleDistortion());
        }
        else
        {
          this.m_shouldDistort = false;
          this.m_distortIntensity = 0.0f;
          this.m_distortRadius = 25f;
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleDistortion()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ClockhairController.<HandleDistortion>c__Iterator2()
        {
          $this = this
        };
      }

      public Vector4 GetCenterPointInScreenUV(Vector2 centerPoint, float dIntensity, float dRadius)
      {
        Vector3 viewportPoint = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp());
        return new Vector4(viewportPoint.x, viewportPoint.y, dRadius, dIntensity);
      }

      public void BeginSpinningWildly()
      {
        this.IsSpinningWildly = true;
        this.StartCoroutine(this.HandleSpinningWildly());
      }

      [DebuggerHidden]
      private IEnumerator HandleSpinningWildly()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ClockhairController.<HandleSpinningWildly>c__Iterator3()
        {
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleSimpleMotion()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ClockhairController.<HandleSimpleMotion>c__Iterator4()
        {
          $this = this
        };
      }

      public void SpinToSessionStart(float duration)
      {
        this.StartCoroutine(this.SpinToTime(DateTime.Now.TimeOfDay.Subtract(new TimeSpan(0, 0, (int) GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TIME_PLAYED))), duration));
      }

      public void SetToTime(TimeSpan time)
      {
        int num = time.Hours % 12;
        int minutes = time.Minutes;
        int seconds = time.Seconds;
        float z1 = (float) (((double) num / 12.0 + (double) minutes / 720.0) * -360.0);
        float z2 = (float) (((double) minutes / 60.0 + (double) seconds / 3600.0) * -360.0);
        float z3 = (float) ((double) seconds / 60.0 * -360.0);
        if ((UnityEngine.Object) this.hourHandPivot != (UnityEngine.Object) null)
          this.hourHandPivot.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, z1);
        if ((UnityEngine.Object) this.minuteHandPivot != (UnityEngine.Object) null)
          this.minuteHandPivot.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, z2);
        if (!((UnityEngine.Object) this.secondHandPivot != (UnityEngine.Object) null))
          return;
        this.secondHandPivot.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, z3);
      }

      [DebuggerHidden]
      private IEnumerator SpinToTime(TimeSpan targetTime, float duration = 5f)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ClockhairController.<SpinToTime>c__Iterator5()
        {
          targetTime = targetTime,
          duration = duration,
          $this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
