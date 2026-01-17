// Decompiled with JetBrains decompiler
// Type: AgunimReflectProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    [RequireComponent(typeof (Projectile))]
    public class AgunimReflectProjectile : BraveBehaviour
    {
      public int[] NumBounces;
      public float[] SpeedIncreases;
      public float[] AnimSpeedMultipliers;
      public float[] BossReflectSpreads;
      public float[] PlayerReflectFriction;
      public float[] BossReflectFriction;
      public VFXPool PlayerReflectVfx;
      private int m_playerReflects;

      public void Awake()
      {
        this.projectile.OnReflected += new Action<Projectile>(this.OnReflected);
        this.projectile.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
        this.projectile.specRigidbody.OnPreTileCollision += new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreTileCollision);
      }

      public void OnSpawned() => this.m_playerReflects = 0;

      public void OnDespawned() => this.projectile.spriteAnimator.OverrideTimeScale = -1f;

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.projectile)
        {
          this.projectile.OnReflected -= new Action<Projectile>(this.OnReflected);
          if ((bool) (UnityEngine.Object) this.projectile.specRigidbody)
          {
            this.projectile.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
            this.projectile.specRigidbody.OnPreTileCollision -= new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreTileCollision);
          }
        }
        base.OnDestroy();
      }

      private void OnReflected(Projectile p)
      {
        if (!(p.Owner is PlayerController))
          return;
        if ((double) p.spriteAnimator.OverrideTimeScale < 0.0)
          p.spriteAnimator.OverrideTimeScale = 1f;
        p.Speed += this.SpeedIncreases[this.m_playerReflects];
        p.spriteAnimator.OverrideTimeScale *= this.AnimSpeedMultipliers[this.m_playerReflects];
        this.PlayerReflectVfx.SpawnAtPosition(this.transform.position);
        StickyFrictionManager.Instance.RegisterCustomStickyFriction(this.PlayerReflectFriction[this.m_playerReflects], 0.0f, false, true);
        ++this.m_playerReflects;
        int num = (int) AkSoundEngine.PostEvent("Play_BOSS_agunim_deflect_01", this.gameObject);
      }

      private void OnPreRigidbodyCollision(
        SpeculativeRigidbody myRigidbody,
        PixelCollider myPixelCollider,
        SpeculativeRigidbody otherRigidbody,
        PixelCollider otherPixelCollider)
      {
        if (!(this.projectile.Owner is PlayerController) || !(bool) (UnityEngine.Object) otherRigidbody.healthHaver || !otherRigidbody.healthHaver.IsBoss)
          return;
        if (this.m_playerReflects < this.NumBounces[3 - Mathf.RoundToInt(otherRigidbody.healthHaver.GetCurrentHealth())])
        {
          int num = (int) AkSoundEngine.PostEvent("Play_BOSS_agunim_deflect_01", this.gameObject);
          PassiveReflectItem.ReflectBullet(this.projectile, true, (GameActor) otherRigidbody.aiActor, 2f, spread: this.BossReflectSpreads[this.m_playerReflects - 1]);
          StickyFrictionManager.Instance.RegisterCustomStickyFriction(this.BossReflectFriction[this.m_playerReflects - 1], 0.0f, false, true);
          AIActor aiActor = otherRigidbody.aiActor;
          if ((bool) (UnityEngine.Object) aiActor)
          {
            aiActor.aiAnimator.PlayUntilFinished("deflect", true);
            AIAnimator aiAnimator = aiActor.aiAnimator;
            string str = "deflect";
            Vector2? nullable = new Vector2?((aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox) + this.projectile.specRigidbody.UnitCenter) / 2f);
            string name = str;
            Vector2? sourceNormal = new Vector2?();
            Vector2? sourceVelocity = new Vector2?();
            Vector2? position = nullable;
            aiAnimator.PlayVfx(name, sourceNormal, sourceVelocity, position);
          }
          PhysicsEngine.SkipCollision = true;
          PhysicsEngine.PostSliceVelocity = new Vector2?(this.projectile.specRigidbody.Velocity);
        }
        else
        {
          AIActor aiActor = otherRigidbody.aiActor;
          if (!(bool) (UnityEngine.Object) aiActor)
            return;
          aiActor.aiAnimator.PlayUntilFinished("big_hit", true);
          AIAnimator aiAnimator = aiActor.aiAnimator;
          string str = "big_hit";
          Vector2? nullable = new Vector2?((aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox) + this.projectile.specRigidbody.UnitCenter) / 2f);
          string name = str;
          Vector2? sourceNormal = new Vector2?();
          Vector2? sourceVelocity = new Vector2?();
          Vector2? position = nullable;
          aiAnimator.PlayVfx(name, sourceNormal, sourceVelocity, position);
        }
      }

      private void OnPreTileCollision(
        SpeculativeRigidbody myrigidbody,
        PixelCollider mypixelcollider,
        PhysicsEngine.Tile tile,
        PixelCollider tilepixelcollider)
      {
        if (tile == null || !GameManager.Instance.Dungeon.data.isFaceWallHigher(tile.X, tile.Y) && !GameManager.Instance.Dungeon.data.isFaceWallLower(tile.X, tile.Y))
          return;
        PhysicsEngine.SkipCollision = true;
      }
    }

}
