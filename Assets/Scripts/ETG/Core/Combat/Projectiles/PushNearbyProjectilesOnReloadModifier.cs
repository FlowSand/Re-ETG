using System;
using UnityEngine;

#nullable disable

public class PushNearbyProjectilesOnReloadModifier : MonoBehaviour
  {
    public float DistanceCutoff = 5f;
    public float AngleCutoff = 45f;
    public float SpeedMultiplier = 10f;
    public AnimationCurve NewSlowdownCurve;
    public float CurveTime = 1f;
    public bool IsSynergyContingent;
    [ShowInInspectorIf("IsSynergyContingent", false)]
    public CustomSynergyType RequiredSynergy = CustomSynergyType.BUBBLE_BUSTER;
    [ShowInInspectorIf("IsSynergyContingent", false)]
    public bool OnlyInSpecificForm;
    [ShowInInspectorIf("OnlyInSpecificForm", false)]
    public ProjectileVolleyData RequiredVolley;
    private Gun m_gun;

    public void Awake()
    {
      this.m_gun = this.GetComponent<Gun>();
      this.m_gun.CanReloadNoMatterAmmo = true;
      this.m_gun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.HandleReload);
    }

    private void HandleReload(PlayerController ownerPlayer, Gun ownerGun, bool someBool)
    {
      if (!(bool) (UnityEngine.Object) ownerGun || !(bool) (UnityEngine.Object) ownerPlayer || !ownerGun.IsReloading || this.IsSynergyContingent && !ownerPlayer.HasActiveBonusSynergy(this.RequiredSynergy) || this.OnlyInSpecificForm && (UnityEngine.Object) ownerGun.RawSourceVolley != (UnityEngine.Object) this.RequiredVolley)
        return;
      for (int index = 0; index < StaticReferenceManager.AllProjectiles.Count; ++index)
      {
        Projectile allProjectile = StaticReferenceManager.AllProjectiles[index];
        if ((bool) (UnityEngine.Object) allProjectile && (UnityEngine.Object) allProjectile.Owner == (UnityEngine.Object) ownerPlayer && (bool) (UnityEngine.Object) allProjectile.specRigidbody && (UnityEngine.Object) allProjectile.PossibleSourceGun == (UnityEngine.Object) ownerGun)
        {
          Vector2 vector = allProjectile.specRigidbody.UnitCenter - ownerPlayer.CenterPosition;
          float magnitude = vector.magnitude;
          if ((double) Mathf.Abs(Mathf.DeltaAngle(ownerGun.CurrentAngle, vector.ToAngle())) < (double) this.AngleCutoff && (double) magnitude < (double) this.DistanceCutoff)
          {
            allProjectile.baseData.speed *= this.SpeedMultiplier;
            allProjectile.baseData.AccelerationCurve = this.NewSlowdownCurve;
            allProjectile.baseData.IgnoreAccelCurveTime = allProjectile.ElapsedTime;
            allProjectile.baseData.CustomAccelerationCurveDuration = this.CurveTime;
            allProjectile.UpdateSpeed();
          }
        }
      }
    }
  }

