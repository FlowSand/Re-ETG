// Decompiled with JetBrains decompiler
// Type: KthuliberProjectileController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class KthuliberProjectileController : MonoBehaviour
  {
    public float BossDamage = 50f;
    public float SoulSpeed = 3f;
    public float SlowDuration = 0.3f;
    public float DamageToRoom = 5f;
    public GameObject SuckVFX;
    public GameObject PickupVFX;
    public GameObject ExplodeVFX;
    public GameObject OverheadVFX;
    private Projectile m_projectile;
    private SpeculativeRigidbody m_soulEnemy;
    private GameObject m_overheadVFX;

    private void Start()
    {
      this.m_projectile = this.GetComponent<Projectile>();
      this.m_projectile.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
    }

    private void Update()
    {
      if (!(bool) (UnityEngine.Object) this.m_soulEnemy || !(bool) (UnityEngine.Object) this.m_projectile || this.m_projectile.OverrideMotionModule == null || !(this.m_projectile.OverrideMotionModule is OrbitProjectileMotionModule))
        return;
      this.m_projectile.DieInAir();
    }

    private void HandleHitEnemy(Projectile source, SpeculativeRigidbody target, bool fatal)
    {
      if (fatal || !(bool) (UnityEngine.Object) target)
        return;
      AIActor component = target.GetComponent<AIActor>();
      if (!(bool) (UnityEngine.Object) component || !component.IsNormalEnemy)
        return;
      if ((bool) (UnityEngine.Object) this.SuckVFX)
      {
        SpawnManager.SpawnVFX(this.SuckVFX, (Vector3) Vector2.Lerp(this.m_projectile.specRigidbody.UnitCenter, target.UnitCenter, 0.5f), Quaternion.identity);
        int num = (int) AkSoundEngine.PostEvent("Play_WPN_kthulu_soul_01", this.gameObject);
      }
      if ((bool) (UnityEngine.Object) this.OverheadVFX)
        this.m_overheadVFX = component.PlayEffectOnActor(this.OverheadVFX, new Vector3(1f / 16f, component.specRigidbody.HitboxPixelCollider.UnitDimensions.y, 0.0f), useHitbox: true);
      this.m_soulEnemy = target;
      this.m_projectile.allowSelfShooting = true;
      this.m_projectile.collidesWithEnemies = false;
      this.m_projectile.collidesWithPlayer = true;
      this.m_projectile.UpdateCollisionMask();
      this.m_projectile.SetNewShooter(target);
      this.m_projectile.spriteAnimator.Play("kthuliber_full_projectile");
      this.m_projectile.specRigidbody.PrimaryPixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Circle;
      this.m_projectile.specRigidbody.PrimaryPixelCollider.ManualOffsetX = -8;
      this.m_projectile.specRigidbody.PrimaryPixelCollider.ManualOffsetY = -8;
      this.m_projectile.specRigidbody.PrimaryPixelCollider.ManualDiameter = 16 /*0x10*/;
      this.m_projectile.specRigidbody.PrimaryPixelCollider.Regenerate(this.m_projectile.transform);
      this.m_projectile.specRigidbody.Reinitialize();
      int count = -1;
      if (PlayerController.AnyoneHasActiveBonusSynergy(CustomSynergyType.KALIBER_KPOW, out count))
        this.m_projectile.ModifyVelocity += new Func<Vector2, Vector2>(this.HomeTowardPlayer);
      this.m_projectile.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreRigidbodySoulCollision);
      this.StartCoroutine(this.FrameDelayedPostProcessing(source));
      this.StartCoroutine(this.SlowDownOverTime(source));
    }

    private Vector2 HomeTowardPlayer(Vector2 inVel)
    {
      PlayerController playerClosestToPoint = GameManager.Instance.GetActivePlayerClosestToPoint(this.m_projectile.LastPosition.XY(), true);
      if (!(bool) (UnityEngine.Object) playerClosestToPoint || (double) Vector2.Distance(playerClosestToPoint.CenterPosition, this.m_projectile.LastPosition.XY()) >= 10.0)
        return inVel;
      Vector2 vector2 = playerClosestToPoint.CenterPosition - this.m_projectile.LastPosition.XY();
      return inVel.magnitude * vector2.normalized;
    }

    private void HandlePreRigidbodySoulCollision(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myPixelCollider,
      SpeculativeRigidbody otherRigidbody,
      PixelCollider otherPixelCollider)
    {
      if (!(bool) (UnityEngine.Object) otherRigidbody)
        return;
      PlayerController component1 = otherRigidbody.GetComponent<PlayerController>();
      if (!(bool) (UnityEngine.Object) component1)
        return;
      PhysicsEngine.SkipCollision = true;
      if ((bool) (UnityEngine.Object) this.m_soulEnemy)
      {
        HealthHaver component2 = this.m_soulEnemy.GetComponent<HealthHaver>();
        if ((bool) (UnityEngine.Object) component2 && !component2.IsBoss)
          component2.ApplyDamage(component2.GetMaxHealth(), Vector2.zero, "Soul Burn", CoreDamageTypes.Void, DamageCategory.Unstoppable, true);
        else if ((bool) (UnityEngine.Object) component2 && component2.IsBoss)
        {
          component2.ApplyDamage(this.BossDamage, Vector2.zero, "Soul Burn", CoreDamageTypes.Void, DamageCategory.Unstoppable, ignoreDamageCaps: true);
          if ((bool) (UnityEngine.Object) this.m_overheadVFX)
          {
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_overheadVFX);
            this.m_overheadVFX = (GameObject) null;
          }
        }
        if (component1.CurrentRoom != null)
        {
          List<AIActor> activeEnemies = component1.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
          if (activeEnemies != null)
          {
            for (int index = 0; index < activeEnemies.Count; ++index)
            {
              if ((bool) (UnityEngine.Object) activeEnemies[index] && (bool) (UnityEngine.Object) activeEnemies[index].healthHaver)
                activeEnemies[index].healthHaver.ApplyDamage(this.DamageToRoom, Vector2.zero, "Soul Burn", CoreDamageTypes.Void, DamageCategory.Unstoppable);
            }
          }
        }
        if ((bool) (UnityEngine.Object) this.ExplodeVFX)
        {
          AIActor component3 = this.m_soulEnemy.GetComponent<AIActor>();
          if ((bool) (UnityEngine.Object) component3)
          {
            component3.PlayEffectOnActor(this.ExplodeVFX, Vector3.zero, false, true);
            int num = (int) AkSoundEngine.PostEvent("Play_WPN_kthulu_blast_01", this.gameObject);
          }
        }
      }
      if ((bool) (UnityEngine.Object) this.PickupVFX)
        component1.PlayEffectOnActor(this.PickupVFX, Vector3.zero, false, true);
      this.m_projectile.DieInAir();
    }

    [DebuggerHidden]
    private IEnumerator SlowDownOverTime(Projectile p)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new KthuliberProjectileController__SlowDownOverTimec__Iterator0()
      {
        p = p,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator FrameDelayedPostProcessing(Projectile p)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new KthuliberProjectileController__FrameDelayedPostProcessingc__Iterator1()
      {
        p = p
      };
    }
  }

