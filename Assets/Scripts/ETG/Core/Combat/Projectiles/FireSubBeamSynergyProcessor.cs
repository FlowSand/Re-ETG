// Decompiled with JetBrains decompiler
// Type: FireSubBeamSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class FireSubBeamSynergyProcessor : MonoBehaviour
    {
      [LongNumericEnum]
      public CustomSynergyType SynergyToCheck;
      public FireSubBeamSynergyProcessor.SubBeamMode Mode;
      public Projectile SubBeamProjectile;
      public int NumberBeams = 3;
      public float BeamAngle = 90f;
      private BasicBeamController m_beam;
      private Projectile m_projectile;
      public float FromProjectileDamageModifier = 0.5f;
      private List<SubbeamData> m_subbeams = new List<SubbeamData>();

      public void Awake()
      {
        this.m_projectile = this.GetComponent<Projectile>();
        this.m_beam = this.GetComponent<BasicBeamController>();
      }

      public void Update()
      {
        bool flag = true;
        if (this.Mode == FireSubBeamSynergyProcessor.SubBeamMode.FROM_BEAM)
        {
          if (!(this.m_beam.Owner is PlayerController) || !(this.m_beam.Owner as PlayerController).HasActiveBonusSynergy(this.SynergyToCheck))
            return;
          flag = this.m_beam.State == BasicBeamController.BeamState.Firing;
        }
        else if (!(this.m_projectile.Owner is PlayerController) || !(this.m_projectile.Owner as PlayerController).HasActiveBonusSynergy(this.SynergyToCheck))
          return;
        float num = !(bool) (Object) this.m_beam ? 0.0f : Vector2.Distance(this.m_beam.GetPointOnBeam(0.0f), this.m_beam.GetPointOnBeam(1f));
        if (flag && (this.Mode != FireSubBeamSynergyProcessor.SubBeamMode.FROM_BEAM || (double) num > 1.5))
        {
          if (this.m_subbeams.Count > 0)
          {
            for (int index = 0; index < this.m_subbeams.Count; ++index)
            {
              Vector2 direction;
              if (this.Mode == FireSubBeamSynergyProcessor.SubBeamMode.FROM_BEAM)
              {
                this.m_subbeams[index].subbeam.Origin = this.m_beam.GetPointOnBeam(this.m_subbeams[index].percent);
                direction = this.m_beam.Direction;
              }
              else
              {
                this.m_subbeams[index].subbeam.Origin = this.m_projectile.specRigidbody.UnitCenter;
                direction = this.m_projectile.Direction;
              }
              this.m_subbeams[index].subbeam.Direction = (Vector2) (Quaternion.Euler(0.0f, 0.0f, this.m_subbeams[index].angle) * (Vector3) direction);
              this.m_subbeams[index].subbeam.LateUpdatePosition((Vector3) this.m_subbeams[index].subbeam.Origin);
            }
          }
          else
          {
            for (int index = 0; index < this.NumberBeams; ++index)
            {
              SubbeamData subbeamData1 = new SubbeamData();
              float t = 1f / (float) (this.NumberBeams + 1) * (float) (index + 1);
              float z1 = this.BeamAngle;
              if (this.Mode == FireSubBeamSynergyProcessor.SubBeamMode.FROM_PROJECTILE_CENTER)
                z1 = 360f / (float) this.NumberBeams * (float) index;
              Vector2 pos = this.Mode != FireSubBeamSynergyProcessor.SubBeamMode.FROM_BEAM ? this.m_projectile.specRigidbody.UnitCenter : this.m_beam.GetPointOnBeam(t);
              Vector2 vector2 = this.Mode != FireSubBeamSynergyProcessor.SubBeamMode.FROM_BEAM ? this.m_projectile.Direction : this.m_beam.Direction;
              subbeamData1.subbeam = this.CreateSubBeam(this.SubBeamProjectile, pos, (Vector2) (Quaternion.Euler(0.0f, 0.0f, z1) * (Vector3) vector2));
              subbeamData1.angle = z1;
              subbeamData1.percent = t;
              this.m_subbeams.Add(subbeamData1);
              if (this.Mode == FireSubBeamSynergyProcessor.SubBeamMode.FROM_BEAM)
              {
                SubbeamData subbeamData2 = new SubbeamData();
                float z2 = -this.BeamAngle;
                subbeamData2.subbeam = this.CreateSubBeam(this.SubBeamProjectile, this.m_beam.GetPointOnBeam(t), (Vector2) (Quaternion.Euler(0.0f, 0.0f, z2) * (Vector3) this.m_beam.Direction));
                subbeamData2.percent = t;
                subbeamData2.angle = z2;
                this.m_subbeams.Add(subbeamData2);
              }
            }
            if (!(bool) (Object) this.m_projectile || !(bool) (Object) this.m_projectile.sprite)
              return;
            this.m_projectile.sprite.ForceRotationRebuild();
          }
        }
        else
        {
          if (this.m_subbeams.Count <= 0)
            return;
          for (int index = 0; index < this.m_subbeams.Count; index = index - 1 + 1)
          {
            this.m_subbeams[index].subbeam.CeaseAttack();
            this.m_subbeams.RemoveAt(index);
          }
        }
      }

      private void OnDestroy()
      {
        if (this.m_subbeams.Count <= 0)
          return;
        for (int index = 0; index < this.m_subbeams.Count; index = index - 1 + 1)
        {
          this.m_subbeams[index].subbeam.CeaseAttack();
          this.m_subbeams.RemoveAt(index);
        }
      }

      private BeamController CreateSubBeam(
        Projectile subBeamProjectilePrefab,
        Vector2 pos,
        Vector2 dir)
      {
        switch (subBeamProjectilePrefab.GetComponent<BeamController>())
        {
          case BasicBeamController _:
            GameObject gameObject1 = Object.Instantiate<GameObject>(subBeamProjectilePrefab.gameObject);
            gameObject1.name = this.gameObject.name + " (Subbeam)";
            BasicBeamController component1 = gameObject1.GetComponent<BasicBeamController>();
            component1.State = BasicBeamController.BeamState.Firing;
            component1.HitsPlayers = false;
            component1.HitsEnemies = true;
            component1.Origin = pos;
            component1.Direction = dir;
            component1.usesChargeDelay = false;
            component1.muzzleAnimation = string.Empty;
            component1.chargeAnimation = string.Empty;
            component1.beamStartAnimation = string.Empty;
            component1.projectile.Owner = this.m_projectile.Owner;
            if (this.Mode == FireSubBeamSynergyProcessor.SubBeamMode.FROM_BEAM)
            {
              component1.Owner = this.m_beam.Owner;
              component1.Gun = this.m_beam.Gun;
              component1.DamageModifier = this.m_beam.DamageModifier;
              component1.playerStatsModified = this.m_beam.playerStatsModified;
            }
            else
            {
              component1.Owner = this.m_projectile.Owner;
              component1.Gun = this.m_projectile.PossibleSourceGun;
              component1.DamageModifier = this.FromProjectileDamageModifier;
            }
            component1.HeightOffset = -0.25f;
            return (BeamController) component1;
          case RaidenBeamController _:
            GameObject gameObject2 = Object.Instantiate<GameObject>(subBeamProjectilePrefab.gameObject);
            gameObject2.name = this.gameObject.name + " (Subbeam)";
            RaidenBeamController component2 = gameObject2.GetComponent<RaidenBeamController>();
            component2.SelectRandomTarget = true;
            component2.HitsPlayers = false;
            component2.HitsEnemies = true;
            component2.Origin = pos;
            component2.Direction = dir;
            component2.usesChargeDelay = false;
            component2.projectile.Owner = this.m_projectile.Owner;
            if (this.Mode == FireSubBeamSynergyProcessor.SubBeamMode.FROM_BEAM)
            {
              component2.Owner = this.m_beam.Owner;
              component2.Gun = this.m_beam.Gun;
              component2.DamageModifier = this.m_beam.DamageModifier;
            }
            else
            {
              component2.Owner = this.m_projectile.Owner;
              component2.Gun = this.m_projectile.PossibleSourceGun;
              component2.DamageModifier = this.FromProjectileDamageModifier;
            }
            return (BeamController) component2;
          default:
            return (BeamController) null;
        }
      }

      public enum SubBeamMode
      {
        FROM_BEAM,
        FROM_PROJECTILE_CENTER,
      }
    }

}
