// Decompiled with JetBrains decompiler
// Type: HeatRingModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class HeatRingModule
    {
      private HeatIndicatorController m_indicator;

      public bool IsActive => (bool) (UnityEngine.Object) this.m_indicator;

      public void Trigger(
        float Radius,
        float Duration,
        GameActorFireEffect HeatEffect,
        tk2dBaseSprite sprite)
      {
        if ((bool) (UnityEngine.Object) this.m_indicator)
          return;
        sprite.StartCoroutine(this.HandleHeatEffectsCR(Radius, Duration, HeatEffect, sprite));
      }

      [DebuggerHidden]
      private IEnumerator HandleHeatEffectsCR(
        float Radius,
        float Duration,
        GameActorFireEffect HeatEffect,
        tk2dBaseSprite sprite)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HeatRingModule__HandleHeatEffectsCRc__Iterator0()
        {
          Radius = Radius,
          sprite = sprite,
          HeatEffect = HeatEffect,
          Duration = Duration,
          _this = this
        };
      }

      private void HandleRadialIndicator(float Radius, tk2dBaseSprite sprite)
      {
        if ((bool) (UnityEngine.Object) this.m_indicator)
          return;
        Vector3 vector3ZisY = sprite.WorldCenter.ToVector3ZisY();
        this.m_indicator = ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), vector3ZisY, Quaternion.identity, sprite.transform)).GetComponent<HeatIndicatorController>();
        this.m_indicator.CurrentRadius = Radius;
      }

      private void UnhandleRadialIndicator()
      {
        if (!(bool) (UnityEngine.Object) this.m_indicator)
          return;
        this.m_indicator.EndEffect();
        this.m_indicator = (HeatIndicatorController) null;
      }
    }

}
