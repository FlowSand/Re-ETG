// Decompiled with JetBrains decompiler
// Type: ScalingProjectileModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class ScalingProjectileModifier : MonoBehaviour
  {
    public bool IsSynergyContingent;
    [LongNumericEnum]
    public CustomSynergyType SynergyToTest;
    public float PercentGainPerUnit = 2f;
    [NonSerialized]
    public float ScaleMultiplier = 1f;
    [NonSerialized]
    public float DamageMultiplier = 1f;
    public float MaximumDamageMultiplier = -1f;
    [NonSerialized]
    public float ScaleToDamageRatio = 1f;
    private Projectile m_projectile;
    private float m_lastElapsedDistance;
    private float m_totalElapsedDistance;
    private float m_elapsedSizeGain = 1f;
    private float m_elapsedDamageGain = 1f;

    public void Start()
    {
      this.m_projectile = this.GetComponent<Projectile>();
      if (this.IsSynergyContingent && (!(bool) (UnityEngine.Object) this.m_projectile.PossibleSourceGun || !(this.m_projectile.PossibleSourceGun.CurrentOwner is PlayerController) || !(this.m_projectile.PossibleSourceGun.CurrentOwner as PlayerController).HasActiveBonusSynergy(this.SynergyToTest)))
        return;
      this.m_projectile.specRigidbody.UpdateCollidersOnScale = true;
      this.m_projectile.OnPostUpdate += new Action<Projectile>(this.HandlePostUpdate);
    }

    public virtual void OnDespawned()
    {
      if ((bool) (UnityEngine.Object) this.m_projectile)
      {
        this.m_projectile.RuntimeUpdateScale(1f / this.m_projectile.AdditionalScaleMultiplier);
        this.m_projectile.baseData.damage /= this.m_elapsedDamageGain;
      }
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }

    private void HandlePostUpdate(Projectile proj)
    {
      if (!(bool) (UnityEngine.Object) proj)
        return;
      float elapsedDistance = proj.GetElapsedDistance();
      if ((double) elapsedDistance < (double) this.m_lastElapsedDistance)
      {
        this.m_lastElapsedDistance = 0.0f;
        this.m_totalElapsedDistance = 0.0f;
      }
      this.m_totalElapsedDistance += elapsedDistance - this.m_lastElapsedDistance;
      this.m_lastElapsedDistance = elapsedDistance;
      this.m_totalElapsedDistance = Mathf.Clamp(this.m_totalElapsedDistance, 0.0f, 160f);
      float num1 = (float) (1.0 + (double) this.m_totalElapsedDistance / 100.0 * (double) this.PercentGainPerUnit);
      float num2 = (float) (((double) num1 - 1.0) * (double) this.ScaleToDamageRatio + 1.0);
      float num3 = (double) this.MaximumDamageMultiplier <= 0.0 ? num2 * this.DamageMultiplier : Mathf.Min(this.MaximumDamageMultiplier, num2 * this.DamageMultiplier);
      if ((double) (num1 * this.ScaleMultiplier / this.m_elapsedSizeGain) > 1.25)
      {
        this.m_projectile.RuntimeUpdateScale(num1 * this.ScaleMultiplier / this.m_elapsedSizeGain);
        this.m_elapsedSizeGain = num1 * this.ScaleMultiplier;
      }
      this.m_projectile.baseData.damage *= num3 / this.m_elapsedDamageGain;
      this.m_elapsedDamageGain = num3;
    }
  }

