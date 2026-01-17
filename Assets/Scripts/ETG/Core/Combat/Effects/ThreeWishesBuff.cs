// Decompiled with JetBrains decompiler
// Type: ThreeWishesBuff
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Effects
{
    public class ThreeWishesBuff : AppliedEffectBase
    {
      public bool SynergyContingent;
      [LongNumericEnum]
      public CustomSynergyType RequiredSynergy;
      public bool OnlyOncePerEnemy;
      public bool TriggersOnFrozenEnemy;
      public int NumRequired = 3;
      public float DamageDelay = 0.4f;
      public float DamageDealt = 50f;
      public Vector3 DirectionalVFXOffset = new Vector3(3f, 0.0f, 0.0f);
      public bool doesExplosion;
      public ExplosionData explosionData;
      public GameObject OverheadVFX;
      public GameObject FinalVFX_Right;
      public GameObject FinalVFX_Left;
      public GameObject FinalVFX_Shared;
      private int m_extantCount = 1;
      private GameObject instantiatedVFX;
      private GameObject instantiatedVFX2;
      private HealthHaver hh;

      public override void Initialize(AppliedEffectBase source)
      {
        this.hh = this.GetComponent<HealthHaver>();
        this.m_extantCount = 1;
        if (source is ThreeWishesBuff)
        {
          ThreeWishesBuff threeWishesBuff = source as ThreeWishesBuff;
          this.NumRequired = threeWishesBuff.NumRequired;
          this.TriggersOnFrozenEnemy = threeWishesBuff.TriggersOnFrozenEnemy;
          this.OnlyOncePerEnemy = threeWishesBuff.OnlyOncePerEnemy;
          if (!((Object) threeWishesBuff.OverheadVFX != (Object) null))
            return;
          GameActor component = this.GetComponent<GameActor>();
          if (!(bool) (Object) component || !(bool) (Object) component.specRigidbody || component.specRigidbody.HitboxPixelCollider == null)
            return;
          this.instantiatedVFX = component.PlayEffectOnActor(threeWishesBuff.OverheadVFX, new Vector3(0.0f, component.specRigidbody.HitboxPixelCollider.UnitDimensions.y, 0.0f), useHitbox: true);
        }
        else
          Object.Destroy((Object) this);
      }

      public void Increment(ThreeWishesBuff source)
      {
        ++this.m_extantCount;
        this.DamageDelay = source.DamageDelay;
        this.DirectionalVFXOffset = source.DirectionalVFXOffset;
        bool flag = this.m_extantCount == this.NumRequired;
        if (this.TriggersOnFrozenEnemy)
          flag = this.hh.gameActor.IsFrozen && this.m_extantCount > 0;
        if (flag)
        {
          if ((bool) (Object) this.instantiatedVFX)
            Object.Destroy((Object) this.instantiatedVFX);
          if ((bool) (Object) this.instantiatedVFX2)
            Object.Destroy((Object) this.instantiatedVFX2);
          this.instantiatedVFX2 = (GameObject) null;
          this.instantiatedVFX = (GameObject) null;
          if ((double) source.transform.position.x < (double) this.hh.gameActor.CenterPosition.x)
          {
            GameObject vfx = this.hh.gameActor.PlayEffectOnActor(source.FinalVFX_Right, this.DirectionalVFXOffset.WithX(this.DirectionalVFXOffset.x * -1f));
            tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
            component.HeightOffGround = 3f;
            component.UpdateZDepth();
            GameManager.Instance.StartCoroutine(this.DelayedDamage(source.DamageDealt, vfx, source.FinalVFX_Shared));
          }
          else
          {
            GameObject vfx = this.hh.gameActor.PlayEffectOnActor(source.FinalVFX_Left, this.DirectionalVFXOffset);
            tk2dBaseSprite component = vfx.GetComponent<tk2dBaseSprite>();
            component.HeightOffGround = 3f;
            component.UpdateZDepth();
            GameManager.Instance.StartCoroutine(this.DelayedDamage(source.DamageDealt, vfx, source.FinalVFX_Shared));
          }
          if (source.doesExplosion)
            Exploder.Explode((Vector3) this.hh.gameActor.CenterPosition, source.explosionData, Vector2.zero);
          if (!this.OnlyOncePerEnemy)
            Object.Destroy((Object) this);
          else
            this.m_extantCount = -1000000;
        }
        else
        {
          if (!((Object) source.OverheadVFX != (Object) null))
            return;
          GameActor component = this.GetComponent<GameActor>();
          if (!(bool) (Object) component || !(bool) (Object) component.specRigidbody || component.specRigidbody.HitboxPixelCollider == null)
            return;
          this.instantiatedVFX2 = component.PlayEffectOnActor(source.OverheadVFX, new Vector3(0.0f, component.specRigidbody.HitboxPixelCollider.UnitDimensions.y + 0.5f, 0.0f), useHitbox: true);
        }
      }

      [DebuggerHidden]
      private IEnumerator DelayedDamage(float source, GameObject vfx, GameObject finalVfx)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ThreeWishesBuff.\u003CDelayedDamage\u003Ec__Iterator0()
        {
          vfx = vfx,
          source = source,
          finalVfx = finalVfx,
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator TimedDestroy(GameObject target, float delay)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ThreeWishesBuff.\u003CTimedDestroy\u003Ec__Iterator1()
        {
          target = target,
          delay = delay
        };
      }

      public override void AddSelfToTarget(GameObject target)
      {
        if (this.SynergyContingent)
        {
          Projectile component = this.GetComponent<Projectile>();
          if ((bool) (Object) component && (bool) (Object) component.PossibleSourceGun && !component.PossibleSourceGun.OwnerHasSynergy(this.RequiredSynergy))
            return;
        }
        HealthHaver healthHaver = target.GetComponent<HealthHaver>();
        if (!(bool) (Object) healthHaver)
        {
          SpeculativeRigidbody component = target.GetComponent<SpeculativeRigidbody>();
          if ((bool) (Object) component)
          {
            healthHaver = component.healthHaver;
            if ((bool) (Object) healthHaver)
              target = healthHaver.gameObject;
          }
        }
        if (!(bool) (Object) healthHaver)
          return;
        ThreeWishesBuff[] components = target.GetComponents<ThreeWishesBuff>();
        if (components.Length > 0)
          components[0].Increment(this);
        else
          target.AddComponent<ThreeWishesBuff>().Initialize((AppliedEffectBase) this);
      }
    }

}
