// Decompiled with JetBrains decompiler
// Type: BuffVFXAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class BuffVFXAnimator : BraveBehaviour
    {
      [SerializeField]
      protected BuffVFXAnimator.BuffAnimationStyle animationStyle;
      public float motionPeriod = 1f;
      public float ChanceOfApplication = 1f;
      public bool persistsOnDeath = true;
      public float AdditionalPierceDepth;
      public bool UsesVFXToSpawnOnDeath;
      public VFXPool VFXToSpawnOnDeath;
      public GameObject NonPoolVFX;
      public bool DoesSparks;
      public GlobalSparksModule SparksModule;
      [Header("Tetris")]
      public TetrisBuff.TetrisType tetrominoType;
      private bool m_initialized;
      private GameActor m_target;
      private Transform m_transform;
      private float m_elapsed;
      private float parametricStartPoint;
      private float m_pierceAngle;
      private Vector2 m_hitboxOriginOffset;
      private bool ForceFailure;
      private float m_sparksAccum;

      public void InitializeTetris(GameActor target, Vector2 sourceVec)
      {
        if ((Object) target == (Object) null)
          return;
        this.parametricStartPoint = Random.value;
        this.m_target = target;
        this.m_transform = this.transform;
        if (this.animationStyle == BuffVFXAnimator.BuffAnimationStyle.PIERCE && (bool) (Object) this.m_target && (bool) (Object) this.m_target.specRigidbody)
        {
          this.m_hitboxOriginOffset = this.m_target.specRigidbody.HitboxPixelCollider.UnitCenter - this.m_target.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
          this.m_pierceAngle = BraveMathCollege.Atan2Degrees(-sourceVec.normalized);
          this.m_hitboxOriginOffset += sourceVec.normalized * (this.sprite.GetBounds().extents.x * Random.Range(0.15f, 0.5f));
          this.m_transform.position = (this.m_target.specRigidbody.HitboxPixelCollider.UnitBottomLeft + this.m_hitboxOriginOffset).ToVector3ZUp();
          this.m_transform.rotation = Quaternion.Euler(0.0f, 0.0f, this.m_pierceAngle);
        }
        this.m_initialized = true;
      }

      public void InitializePierce(GameActor target, Vector2 sourceVec)
      {
        if ((Object) target == (Object) null)
          return;
        this.parametricStartPoint = Random.value;
        this.m_target = target;
        this.m_transform = this.transform;
        if (this.animationStyle == BuffVFXAnimator.BuffAnimationStyle.PIERCE && (bool) (Object) this.m_target && (bool) (Object) this.m_target.specRigidbody)
        {
          this.m_hitboxOriginOffset = this.m_target.specRigidbody.HitboxPixelCollider.UnitCenter - this.m_target.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
          this.m_pierceAngle = BraveMathCollege.Atan2Degrees(-sourceVec.normalized);
          this.m_hitboxOriginOffset += sourceVec.normalized * (this.sprite.GetBounds().extents.x * Random.Range(0.15f, 0.5f));
          this.m_transform.position = (this.m_target.specRigidbody.HitboxPixelCollider.UnitBottomLeft + this.m_hitboxOriginOffset).ToVector3ZUp();
          this.m_transform.rotation = Quaternion.Euler(0.0f, 0.0f, this.m_pierceAngle);
        }
        this.m_initialized = true;
      }

      public void Initialize(GameActor target)
      {
        if ((Object) target == (Object) null)
          return;
        this.parametricStartPoint = Random.value;
        this.m_target = target;
        this.m_transform = this.transform;
        if (this.animationStyle == BuffVFXAnimator.BuffAnimationStyle.PIERCE)
        {
          if ((bool) (Object) this.m_target && (bool) (Object) this.m_target.specRigidbody)
          {
            this.m_hitboxOriginOffset = new Vector2(Random.Range(0.0f, this.m_target.specRigidbody.HitboxPixelCollider.UnitDimensions.x), Random.Range(0.0f, this.m_target.specRigidbody.HitboxPixelCollider.UnitDimensions.y));
            this.m_pierceAngle = BraveMathCollege.Atan2Degrees(this.m_hitboxOriginOffset + this.m_target.specRigidbody.HitboxPixelCollider.UnitBottomLeft - this.m_target.specRigidbody.HitboxPixelCollider.UnitCenter);
            this.m_hitboxOriginOffset += (this.m_target.specRigidbody.HitboxPixelCollider.UnitCenter - (this.m_hitboxOriginOffset + this.m_target.specRigidbody.HitboxPixelCollider.UnitBottomLeft)).normalized * (this.sprite.GetBounds().extents.x * Random.Range(0.15f + this.AdditionalPierceDepth, 0.5f + this.AdditionalPierceDepth));
            this.m_transform.position = (this.m_target.specRigidbody.HitboxPixelCollider.UnitBottomLeft + this.m_hitboxOriginOffset).ToVector3ZUp();
            this.m_transform.rotation = Quaternion.Euler(0.0f, 0.0f, this.m_pierceAngle);
          }
        }
        else if (this.animationStyle == BuffVFXAnimator.BuffAnimationStyle.TETRIS && (bool) (Object) this.m_target && (bool) (Object) this.m_target.specRigidbody)
        {
          this.m_hitboxOriginOffset = new Vector2(Random.Range(0.0f, this.m_target.specRigidbody.HitboxPixelCollider.UnitDimensions.x), Random.Range(0.0f, this.m_target.specRigidbody.HitboxPixelCollider.UnitDimensions.y));
          this.m_pierceAngle = (float) (90 * Random.Range(0, 4));
          this.m_hitboxOriginOffset += (this.m_target.specRigidbody.HitboxPixelCollider.UnitCenter - (this.m_hitboxOriginOffset + this.m_target.specRigidbody.HitboxPixelCollider.UnitBottomLeft)).normalized * (this.sprite.GetBounds().extents.x * Random.Range(0.15f + this.AdditionalPierceDepth, 0.5f + this.AdditionalPierceDepth));
          this.m_hitboxOriginOffset = this.m_hitboxOriginOffset.Quantize(0.375f);
          if ((double) this.m_pierceAngle == 0.0 || (double) this.m_pierceAngle == 180.0)
          {
            this.m_hitboxOriginOffset -= this.sprite.GetBounds().extents.XY();
          }
          else
          {
            Vector2 vector2 = this.sprite.GetBounds().extents.XY();
            vector2 = new Vector2(vector2.y, vector2.x);
            this.m_hitboxOriginOffset -= vector2;
          }
          this.m_hitboxOriginOffset += new Vector2(0.375f, 0.375f);
          this.m_transform.position = (this.m_target.specRigidbody.HitboxPixelCollider.UnitBottomLeft + this.m_hitboxOriginOffset).ToVector3ZUp();
          this.m_transform.rotation = Quaternion.Euler(0.0f, 0.0f, this.m_pierceAngle);
          this.sprite.HeightOffGround = Random.Range(0.0f, 3f).Quantize(0.05f);
          this.sprite.IsPerpendicular = true;
          this.sprite.UpdateZDepth();
        }
        if ((double) Random.value > (double) this.ChanceOfApplication)
          this.ForceFailure = true;
        this.m_initialized = true;
      }

      public void ForceDrop() => this.ForceFailure = true;

      public void ClearData()
      {
        this.m_initialized = false;
        this.m_target = (GameActor) null;
        this.ForceFailure = false;
      }

      private void OnDespawned()
      {
        this.m_initialized = false;
        this.m_target = (GameActor) null;
      }

      private void Update()
      {
        if (this.m_initialized)
        {
          if (!(bool) (Object) this.m_target || this.m_target.healthHaver.IsDead || this.ForceFailure)
          {
            if (this.UsesVFXToSpawnOnDeath && (bool) (Object) this.m_target && (bool) (Object) this.m_target.specRigidbody)
            {
              this.VFXToSpawnOnDeath.SpawnAtPosition(this.transform.position, sourceNormal: new Vector2?(Vector2.zero), sourceVelocity: new Vector2?(this.m_target.specRigidbody.HitboxPixelCollider.UnitCenter - (this.m_target.specRigidbody.HitboxPixelCollider.UnitBottomLeft + this.m_hitboxOriginOffset)), heightOffGround: new float?(1f));
              if ((bool) (Object) this.NonPoolVFX)
              {
                GameObject gameObject = SpawnManager.SpawnVFX(this.NonPoolVFX, this.transform.position, this.transform.rotation);
                Vector2 vector2 = this.m_target.specRigidbody.HitboxPixelCollider.UnitCenter - (this.m_target.specRigidbody.HitboxPixelCollider.UnitBottomLeft + this.m_hitboxOriginOffset);
                DebrisObject component = gameObject.GetComponent<DebrisObject>();
                if ((bool) (Object) component)
                {
                  tk2dBaseSprite sprite = component.sprite;
                  sprite.IsPerpendicular = false;
                  sprite.usesOverrideMaterial = true;
                  Vector2 normalized = (-vector2).normalized;
                  Vector2 vector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, Random.Range(-20f, 20f)) * (Vector3) normalized * 10f);
                  component.Trigger(vector.ToVector3ZUp(0.1f), 2f);
                }
              }
            }
            SpawnManager.Despawn(this.gameObject);
          }
          else
          {
            this.m_elapsed += BraveTime.DeltaTime;
            float num1 = this.m_elapsed / this.motionPeriod + this.parametricStartPoint;
            Vector3 vector3 = this.m_transform.position;
            if (this.DoesSparks)
            {
              this.m_sparksAccum += BraveTime.DeltaTime * this.SparksModule.RatePerSecond;
              if ((double) this.m_sparksAccum > 0.0)
              {
                int num2 = Mathf.FloorToInt(this.m_sparksAccum);
                this.m_sparksAccum -= (float) num2;
                GlobalSparksDoer.DoRandomParticleBurst(num2, vector3, vector3, Vector3.up, 30f, 0.5f, new float?(0.1f), new float?(1f), systemType: this.SparksModule.sparksType);
              }
            }
            switch (this.animationStyle)
            {
              case BuffVFXAnimator.BuffAnimationStyle.CIRCLE:
                vector3 = this.m_target.specRigidbody.UnitCenter.ToVector3ZUp() + Quaternion.Euler(0.0f, 0.0f, num1 * 360f) * Vector3.right;
                break;
              case BuffVFXAnimator.BuffAnimationStyle.SWARM:
                float f = num1 * 3.14159274f;
                vector3 = this.m_target.specRigidbody.UnitCenter.ToVector3ZUp() + new Vector3(Mathf.Sin(f) + 2f * Mathf.Sin(2f * f), Mathf.Cos(f) - 2f * Mathf.Cos(2f * f), 0.0f) / 2f;
                break;
              case BuffVFXAnimator.BuffAnimationStyle.PIERCE:
                this.m_transform.position = (this.m_target.specRigidbody.HitboxPixelCollider.UnitBottomLeft + this.m_hitboxOriginOffset).ToVector3ZUp();
                return;
              case BuffVFXAnimator.BuffAnimationStyle.TETRIS:
                this.m_transform.position = (this.m_target.specRigidbody.HitboxPixelCollider.UnitBottomLeft + this.m_hitboxOriginOffset).ToVector3ZUp();
                return;
            }
            this.m_transform.position = vector3;
          }
        }
        else
          SpawnManager.Despawn(this.gameObject);
      }

      protected enum BuffAnimationStyle
      {
        CIRCLE,
        SWARM,
        PIERCE,
        TETRIS,
      }
    }

}
