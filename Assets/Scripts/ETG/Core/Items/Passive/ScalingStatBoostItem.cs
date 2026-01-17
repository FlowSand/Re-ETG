// Decompiled with JetBrains decompiler
// Type: ScalingStatBoostItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class ScalingStatBoostItem : PassiveItem
    {
      public PlayerStats.StatType TargetStat = PlayerStats.StatType.Damage;
      public float MinScaling = 1f;
      public float MaxScaling = 2f;
      public float ScalingTargetMin;
      public float ScalingTargetMax = 500f;
      public bool TintBullets;
      public bool TintBeams;
      public Color TintColor = Color.yellow;
      public int TintPriority = 2;
      public AnimationCurve ScalingCurve;
      public ScalingStatBoostItem.ScalingModeTarget ScalingTarget;
      private PlayerController m_player;

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        this.m_player = player;
        base.Pickup(player);
        player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
        player.PostProcessBeam += new Action<BeamController>(this.PostProcessBeam);
      }

      private void PostProcessBeam(BeamController beam)
      {
        if (!(bool) (UnityEngine.Object) beam)
          return;
        Projectile projectile = beam.projectile;
        if ((bool) (UnityEngine.Object) projectile)
          this.PostProcessProjectile(projectile, 1f);
        beam.AdjustPlayerBeamTint(this.TintColor.WithAlpha(this.TintColor.a / 2f), this.TintPriority);
      }

      private void PostProcessProjectile(Projectile obj, float effectChanceScalar)
      {
        if (!(bool) (UnityEngine.Object) this.m_player)
          return;
        float t = 0.0f;
        switch (this.ScalingTarget)
        {
          case ScalingStatBoostItem.ScalingModeTarget.CURRENCY:
            t = this.ScalingCurve.Evaluate(Mathf.Clamp01(Mathf.InverseLerp(this.ScalingTargetMin, this.ScalingTargetMax, (float) this.m_player.carriedConsumables.Currency)));
            break;
          case ScalingStatBoostItem.ScalingModeTarget.CURSE:
            t = this.ScalingCurve.Evaluate(Mathf.Clamp01(Mathf.InverseLerp(this.ScalingTargetMin, this.ScalingTargetMax, this.m_player.stats.GetStatValue(PlayerStats.StatType.Curse))));
            break;
        }
        float num = Mathf.Lerp(this.MinScaling, this.MaxScaling, t);
        if (this.TargetStat == PlayerStats.StatType.Damage)
          obj.baseData.damage *= num;
        if (this.TintBullets)
          obj.AdjustPlayerProjectileTint(this.TintColor, this.TintPriority);
        if (this.ScalingTarget != ScalingStatBoostItem.ScalingModeTarget.CURSE)
          return;
        obj.CurseSparks = true;
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        this.m_player = (PlayerController) null;
        debrisObject.GetComponent<ScalingStatBoostItem>().m_pickedUpThisRun = true;
        player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
        player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
        return debrisObject;
      }

      protected override void OnDestroy()
      {
        base.OnDestroy();
        if (!(bool) (UnityEngine.Object) this.m_player)
          return;
        this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
        this.m_player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
      }

      public enum ScalingModeTarget
      {
        CURRENCY,
        CURSE,
      }
    }

}
