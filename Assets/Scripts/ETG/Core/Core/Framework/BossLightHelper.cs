// Decompiled with JetBrains decompiler
// Type: BossLightHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    [RequireComponent(typeof (SpotLightHelper))]
    [RequireComponent(typeof (Light))]
    public class BossLightHelper : TimeInvariantMonoBehaviour
    {
      public float MaxRotation = 360f;
      [Header("Intensity Pulse")]
      public float PulseThreshold = 0.2f;
      public float PulseMaxIntensity = 8f;
      public float PulsePeriod = 1f;
      [Header("On Death")]
      public float PulseStopTime = 5f;
      public float RotationStopTime = 10f;
      private HealthHaver m_bossHealth;
      private Light m_light;
      private SpotLightHelper m_lightHelper;
      private float m_startRotation;
      private float m_startIntensity;
      private float m_pulseTimer;
      private bool m_isDead;

      public void Start()
      {
        this.m_bossHealth = GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY()).GetActiveEnemies(RoomHandler.ActiveEnemyType.All)[0].healthHaver;
        this.m_light = this.GetComponent<Light>();
        this.m_lightHelper = this.GetComponent<SpotLightHelper>();
        this.m_startRotation = this.m_lightHelper.rotationSpeed;
        this.m_startIntensity = this.m_light.intensity;
      }

      protected override void InvariantUpdate(float realDeltaTime)
      {
        if (!this.m_isDead)
          this.m_lightHelper.rotationSpeed = Mathf.Lerp(this.m_startRotation, this.MaxRotation, 1f - this.m_bossHealth.GetCurrentHealthPercentage());
        if (this.m_bossHealth.IsDead && !this.m_isDead)
        {
          this.m_isDead = true;
          this.StartCoroutine(this.DeathEffects());
        }
        if (!this.m_isDead && (double) this.m_bossHealth.GetCurrentHealthPercentage() > (double) this.PulseThreshold)
          return;
        this.m_pulseTimer += realDeltaTime;
        this.m_light.intensity = Mathf.Lerp(this.m_startIntensity, this.PulseMaxIntensity, Mathf.PingPong(this.m_pulseTimer, this.PulsePeriod) / this.PulsePeriod);
      }

      [DebuggerHidden]
      private IEnumerator DeathEffects()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossLightHelper.\u003CDeathEffects\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
