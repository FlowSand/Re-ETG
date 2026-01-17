// Decompiled with JetBrains decompiler
// Type: DodgeRollBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class DodgeRollBehavior : OverrideBehaviorBase
{
  public float Cooldown = 1f;
  public float timeToHitThreshold = 0.25f;
  public float dodgeChance = 0.5f;
  public string dodgeAnim = "dodgeroll";
  public float rollDistance = 3f;
  private float m_cooldownTimer;
  private List<Projectile> m_consideredProjectiles = new List<Projectile>();
  private bool? m_cachedShouldDodge;
  private Vector2? m_cachedRollDirection;

  public override void Start()
  {
    base.Start();
    this.m_updateEveryFrame = true;
    this.m_ignoreGlobalCooldown = true;
    StaticReferenceManager.ProjectileAdded += new Action<Projectile>(this.ProjectileAdded);
    StaticReferenceManager.ProjectileRemoved += new Action<Projectile>(this.ProjectileRemoved);
  }

  public override void Upkeep()
  {
    base.Upkeep();
    this.DecrementTimer(ref this.m_cooldownTimer);
    this.m_cachedShouldDodge = new bool?();
    this.m_cachedRollDirection = new Vector2?();
  }

  public override bool OverrideOtherBehaviors()
  {
    return (double) this.m_cooldownTimer <= 0.0 && this.ShouldDodgeroll(out Vector2 _);
  }

  public override BehaviorResult Update()
  {
    int num = (int) base.Update();
    Vector2 rollDirection;
    if ((double) this.m_cooldownTimer > 0.0 || !this.ShouldDodgeroll(out rollDirection))
      return BehaviorResult.Continue;
    this.m_aiAnimator.LockFacingDirection = true;
    this.m_aiAnimator.FacingDirection = rollDirection.ToAngle();
    this.m_aiAnimator.PlayUntilFinished(this.dodgeAnim);
    float currentClipLength = this.m_aiAnimator.CurrentClipLength;
    this.m_aiActor.BehaviorOverridesVelocity = true;
    this.m_aiActor.BehaviorVelocity = rollDirection * (this.rollDistance / currentClipLength);
    return BehaviorResult.RunContinuous;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    int num = (int) base.ContinuousUpdate();
    if (this.m_aiAnimator.IsPlaying(this.dodgeAnim))
      return ContinuousBehaviorResult.Continue;
    this.m_aiActor.BehaviorOverridesVelocity = false;
    this.m_aiAnimator.LockFacingDirection = false;
    return ContinuousBehaviorResult.Finished;
  }

  public override void Destroy()
  {
    StaticReferenceManager.ProjectileAdded -= new Action<Projectile>(this.ProjectileAdded);
    StaticReferenceManager.ProjectileRemoved -= new Action<Projectile>(this.ProjectileRemoved);
    base.Destroy();
  }

  private void ProjectileAdded(Projectile p)
  {
    if (!(bool) (UnityEngine.Object) p || !(bool) (UnityEngine.Object) p.specRigidbody || !p.specRigidbody.CanCollideWith(this.m_aiActor.specRigidbody))
      return;
    this.m_consideredProjectiles.Add(p);
  }

  private void ProjectileRemoved(Projectile p) => this.m_consideredProjectiles.Remove(p);

  private bool ShouldDodgeroll(out Vector2 rollDirection)
  {
    if (this.m_cachedShouldDodge.HasValue)
    {
      rollDirection = this.m_cachedRollDirection.Value;
      return this.m_cachedShouldDodge.Value;
    }
    PixelCollider hitboxPixelCollider = this.m_aiActor.specRigidbody.HitboxPixelCollider;
    for (int index1 = 0; index1 < this.m_consideredProjectiles.Count; ++index1)
    {
      Projectile consideredProjectile = this.m_consideredProjectiles[index1];
      if ((double) (Vector2.Distance(consideredProjectile.specRigidbody.UnitCenter, hitboxPixelCollider.UnitCenter) / consideredProjectile.Speed) <= (double) this.timeToHitThreshold)
      {
        IntVector2 pixel = PhysicsEngine.UnitToPixel(consideredProjectile.specRigidbody.Velocity * this.timeToHitThreshold * 1.1f);
        CollisionData result1;
        bool flag1 = PhysicsEngine.Instance.RigidbodyCast(consideredProjectile.specRigidbody, pixel, out result1);
        CollisionData.Pool.Free(ref result1);
        if (flag1 && !((UnityEngine.Object) result1.OtherRigidbody == (UnityEngine.Object) null) && !((UnityEngine.Object) result1.OtherRigidbody != (UnityEngine.Object) this.m_aiActor.specRigidbody) && (double) UnityEngine.Random.value <= (double) this.dodgeChance)
        {
          List<Vector2> list = new List<Vector2>();
          Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
          for (int index2 = 0; index2 < 8; ++index2)
          {
            bool flag2 = false;
            Vector2 normalized = IntVector2.CardinalsAndOrdinals[index2].ToVector2().normalized;
            RaycastResult result2;
            bool flag3 = PhysicsEngine.Instance.Raycast(unitCenter, normalized, 3f, out result2, sourceLayer: new CollisionLayer?(CollisionLayer.EnemyCollider), ignoreRigidbody: this.m_aiActor.specRigidbody);
            RaycastResult.Pool.Free(ref result2);
            float num1 = 0.25f;
            for (float num2 = num1; (double) num2 <= (double) this.rollDistance && !flag2 && !flag3; num2 += num1)
            {
              if (GameManager.Instance.Dungeon.ShouldReallyFall((Vector3) (unitCenter + num2 * normalized)))
                flag2 = true;
            }
            if (!flag3 && !flag2)
              list.Add(normalized);
          }
          if (list.Count != 0)
          {
            this.m_cachedShouldDodge = new bool?(true);
            this.m_cachedRollDirection = new Vector2?(BraveUtility.RandomElement<Vector2>(list));
            rollDirection = this.m_cachedRollDirection.Value;
            return this.m_cachedShouldDodge.Value;
          }
        }
        this.m_consideredProjectiles.Remove(consideredProjectile);
      }
    }
    this.m_cachedShouldDodge = new bool?(false);
    this.m_cachedRollDirection = new Vector2?(Vector2.zero);
    rollDirection = this.m_cachedRollDirection.Value;
    return this.m_cachedShouldDodge.Value;
  }
}
