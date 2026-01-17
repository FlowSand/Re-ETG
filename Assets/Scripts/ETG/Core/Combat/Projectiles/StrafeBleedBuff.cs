// Decompiled with JetBrains decompiler
// Type: StrafeBleedBuff
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class StrafeBleedBuff : AppliedEffectBase
    {
      public bool PreventExplosion;
      public ExplosionData explosionData;
      public GameObject vfx;
      public GameObject additionalVFX;
      public float CascadeTime = 3f;
      private GameObject instantiatedVFX;
      private Gun m_attachedGun;
      private HealthHaver hh;
      private bool m_initialized;
      private float m_elapsed;
      private Vector2 m_cachedSourceVector = Vector2.zero;

      private void InitializeSelf(StrafeBleedBuff source)
      {
        if (!(bool) (UnityEngine.Object) source)
          return;
        this.m_initialized = true;
        this.explosionData = source.explosionData;
        this.PreventExplosion = source.PreventExplosion;
        this.hh = this.GetComponent<HealthHaver>();
        if ((UnityEngine.Object) this.hh != (UnityEngine.Object) null)
        {
          Projectile component = source.GetComponent<Projectile>();
          if ((UnityEngine.Object) component.PossibleSourceGun != (UnityEngine.Object) null)
          {
            this.m_attachedGun = component.PossibleSourceGun;
            component.PossibleSourceGun.OnFinishAttack += new Action<PlayerController, Gun>(this.HandleCeaseAttack);
          }
          else
          {
            if (!(bool) (UnityEngine.Object) component || !(bool) (UnityEngine.Object) component.Owner || !(bool) (UnityEngine.Object) component.Owner.CurrentGun)
              return;
            this.m_attachedGun = component.Owner.CurrentGun;
            component.Owner.CurrentGun.OnFinishAttack += new Action<PlayerController, Gun>(this.HandleCeaseAttack);
          }
        }
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) this);
      }

      private void Update()
      {
        if (!this.m_initialized)
          return;
        this.m_elapsed += BraveTime.DeltaTime;
        if ((double) this.m_elapsed <= (double) this.CascadeTime)
          return;
        this.DoEffect();
        this.Disconnect();
      }

      private void HandleCeaseAttack(PlayerController arg1, Gun arg2)
      {
        this.DoEffect();
        this.Disconnect();
      }

      private void Disconnect()
      {
        this.m_initialized = false;
        if (!(bool) (UnityEngine.Object) this.m_attachedGun)
          return;
        this.m_attachedGun.OnFinishAttack -= new Action<PlayerController, Gun>(this.HandleCeaseAttack);
      }

      public override void Initialize(AppliedEffectBase source)
      {
        if (source is StrafeBleedBuff)
        {
          StrafeBleedBuff source1 = source as StrafeBleedBuff;
          if ((UnityEngine.Object) this.GetComponent<StrafeBleedBuff>() == (UnityEngine.Object) this && (UnityEngine.Object) source1.additionalVFX != (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.GetComponent<SpeculativeRigidbody>())
          {
            SpeculativeRigidbody component = this.GetComponent<SpeculativeRigidbody>();
            SpawnManager.SpawnVFX(source1.additionalVFX, (Vector3) component.UnitCenter, Quaternion.identity, true).transform.parent = this.transform;
          }
          this.InitializeSelf(source1);
          if (!((UnityEngine.Object) source1.vfx != (UnityEngine.Object) null))
            return;
          this.instantiatedVFX = SpawnManager.SpawnVFX(source1.vfx, this.transform.position, Quaternion.identity, true);
          tk2dSprite component1 = this.instantiatedVFX.GetComponent<tk2dSprite>();
          tk2dSprite component2 = this.GetComponent<tk2dSprite>();
          if ((UnityEngine.Object) component1 != (UnityEngine.Object) null && (UnityEngine.Object) component2 != (UnityEngine.Object) null)
          {
            component2.AttachRenderer((tk2dBaseSprite) component1);
            component1.HeightOffGround = 0.1f;
            component1.IsPerpendicular = true;
            component1.usesOverrideMaterial = true;
          }
          BuffVFXAnimator component3 = this.instantiatedVFX.GetComponent<BuffVFXAnimator>();
          if (!((UnityEngine.Object) component3 != (UnityEngine.Object) null))
            return;
          Projectile component4 = source.GetComponent<Projectile>();
          if ((bool) (UnityEngine.Object) component4 && component4.LastVelocity != Vector2.zero)
          {
            this.m_cachedSourceVector = component4.LastVelocity;
            component3.InitializePierce(this.GetComponent<GameActor>(), component4.LastVelocity);
          }
          else
            component3.Initialize(this.GetComponent<GameActor>());
        }
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) this);
      }

      public override void AddSelfToTarget(GameObject target)
      {
        if ((UnityEngine.Object) target.GetComponent<HealthHaver>() == (UnityEngine.Object) null)
          return;
        target.AddComponent<StrafeBleedBuff>().Initialize((AppliedEffectBase) this);
      }

      private void DoEffect()
      {
        if ((bool) (UnityEngine.Object) this.hh && !this.PreventExplosion)
        {
          float force = this.explosionData.force / 4f;
          this.explosionData.force = 0.0f;
          if ((bool) (UnityEngine.Object) this.hh.specRigidbody)
          {
            if (this.explosionData.ignoreList == null)
              this.explosionData.ignoreList = new List<SpeculativeRigidbody>();
            this.explosionData.ignoreList.Add(this.hh.specRigidbody);
            this.hh.ApplyDamage(this.explosionData.damage, this.m_cachedSourceVector.normalized, "Strafe");
          }
          if ((UnityEngine.Object) this.instantiatedVFX != (UnityEngine.Object) null)
            Exploder.Explode((Vector3) (this.instantiatedVFX.GetComponent<tk2dBaseSprite>().WorldCenter + this.m_cachedSourceVector.normalized * -0.5f), this.explosionData, Vector2.zero, ignoreQueues: true);
          else
            Exploder.Explode((Vector3) this.hh.aiActor.CenterPosition, this.explosionData, Vector2.zero, ignoreQueues: true);
          if ((bool) (UnityEngine.Object) this.hh.knockbackDoer && this.m_cachedSourceVector != Vector2.zero)
            this.hh.knockbackDoer.ApplyKnockback(this.m_cachedSourceVector.normalized, force);
        }
        if ((bool) (UnityEngine.Object) this.instantiatedVFX)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.instantiatedVFX);
        UnityEngine.Object.Destroy((UnityEngine.Object) this);
      }

      private void OnDestroy() => this.Disconnect();
    }

}
