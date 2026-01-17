// Decompiled with JetBrains decompiler
// Type: DownwellBootsItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class DownwellBootsItem : PassiveItem
    {
      public int NumProjectilesToFire = 5;
      public float ProjectileArcAngle = 45f;
      public float FireCooldown = 2f;
      private float m_cooldown;
      private AfterImageTrailController downwellAfterimage;
      [Header("Synergues")]
      public ExplosionData BlastBootsExplosion;
      private PlayerController m_player;

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        base.Pickup(player);
        this.downwellAfterimage = player.gameObject.AddComponent<AfterImageTrailController>();
        this.downwellAfterimage.spawnShadows = false;
        this.downwellAfterimage.shadowTimeDelay = 0.05f;
        this.downwellAfterimage.shadowLifetime = 0.3f;
        this.downwellAfterimage.minTranslation = 0.05f;
        this.downwellAfterimage.dashColor = Color.red;
        this.downwellAfterimage.OverrideImageShader = ShaderCache.Acquire("Brave/Internal/DownwellAfterImage");
        player.OnRollStarted += new Action<PlayerController, Vector2>(this.OnRollStarted);
      }

      private void OnRollStarted(PlayerController sourcePlayer, Vector2 dirVec)
      {
        if ((bool) (UnityEngine.Object) sourcePlayer && sourcePlayer.HasActiveBonusSynergy(CustomSynergyType.DOWNERWELL))
          this.m_cooldown = 0.0f;
        if ((double) this.m_cooldown <= 0.0)
        {
          if ((bool) (UnityEngine.Object) sourcePlayer && sourcePlayer.HasActiveBonusSynergy(CustomSynergyType.BLASTBOOTS))
          {
            Exploder.Explode((Vector3) (sourcePlayer.CenterPosition + -dirVec.normalized), this.BlastBootsExplosion, dirVec, ignoreQueues: true);
          }
          else
          {
            for (int index = 0; index < this.NumProjectilesToFire; ++index)
            {
              float num = 0.0f;
              if (this.NumProjectilesToFire > 1)
                num = (float) ((double) this.ProjectileArcAngle / -2.0 + (double) this.ProjectileArcAngle / (double) (this.NumProjectilesToFire - 1) * (double) index);
              Projectile component = this.bulletBank.CreateProjectileFromBank(sourcePlayer.CenterPosition, BraveMathCollege.Atan2Degrees(dirVec * -1f) + num, "default").GetComponent<Projectile>();
              if ((bool) (UnityEngine.Object) component)
              {
                component.Shooter = sourcePlayer.specRigidbody;
                component.Owner = (GameActor) sourcePlayer;
                component.SpawnedFromNonChallengeItem = true;
                if ((bool) (UnityEngine.Object) component.specRigidbody)
                  component.specRigidbody.PrimaryPixelCollider.CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.EnemyBulletBlocker);
              }
              sourcePlayer.DoPostProcessProjectile(component);
            }
          }
          this.m_cooldown = this.FireCooldown;
        }
        sourcePlayer.StartCoroutine(this.HandleAfterImageStop(sourcePlayer));
      }

      [DebuggerHidden]
      private IEnumerator HandleAfterImageStop(PlayerController player)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DownwellBootsItem.<HandleAfterImageStop>c__Iterator0()
        {
          player = player,
          $this = this
        };
      }

      protected override void Update()
      {
        base.Update();
        this.m_cooldown -= BraveTime.DeltaTime;
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        debrisObject.GetComponent<DownwellBootsItem>().m_pickedUpThisRun = true;
        player.OnRollStarted -= new Action<PlayerController, Vector2>(this.OnRollStarted);
        if ((bool) (UnityEngine.Object) this.downwellAfterimage)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.downwellAfterimage);
        this.downwellAfterimage = (AfterImageTrailController) null;
        return debrisObject;
      }

      protected override void OnDestroy()
      {
        if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
        {
          this.m_owner.OnRollStarted -= new Action<PlayerController, Vector2>(this.OnRollStarted);
          if ((bool) (UnityEngine.Object) this.downwellAfterimage)
            UnityEngine.Object.Destroy((UnityEngine.Object) this.downwellAfterimage);
          this.downwellAfterimage = (AfterImageTrailController) null;
        }
        base.OnDestroy();
      }

      public enum Condition
      {
        WhileDodgeRolling,
      }
    }

}
