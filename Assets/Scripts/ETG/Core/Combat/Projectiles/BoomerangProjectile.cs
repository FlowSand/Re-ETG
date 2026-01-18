// Decompiled with JetBrains decompiler
// Type: BoomerangProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class BoomerangProjectile : Projectile
  {
    public float StunDuration = 5f;
    public float trackingSpeed = 5f;
    public bool stopTrackingIfLeaveRadius;
    public bool UsesMouseAimPoint;
    public int MaximumNumberOfTargets = -1;
    public float MaximumTraversalDistance = -1f;
    private PlayerController throwerPlayer;
    private Queue<AIActor> RemainingEnemiesToHit;
    private float m_targetlessTime;
    private float m_elapsedTargetlessTime;

    public override void Start()
    {
      base.Start();
      if ((bool) (UnityEngine.Object) this.PossibleSourceGun && this.PossibleSourceGun.OwnerHasSynergy(CustomSynergyType.CRAVE_THE_GLAIVE))
      {
        this.MaximumNumberOfTargets = -1;
        this.MaximumTraversalDistance = -1f;
      }
      this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
      this.specRigidbody.OnTileCollision += new SpeculativeRigidbody.OnTileCollisionDelegate(this.HandleTileCollision);
      this.RemainingEnemiesToHit = new Queue<AIActor>();
      this.GatherTargetEnemies();
      if (this.RemainingEnemiesToHit.Count > 0)
      {
        AIActor aiActor = this.RemainingEnemiesToHit.Peek();
        if (!((UnityEngine.Object) aiActor != (UnityEngine.Object) null))
          return;
        Vector2 v = aiActor.CenterPosition - this.transform.position.XY();
        this.transform.Rotate(0.0f, 0.0f, Mathf.DeltaAngle(BraveMathCollege.Atan2Degrees(this.specRigidbody.Velocity), BraveMathCollege.Atan2Degrees(v)));
      }
      else
      {
        if (!(bool) (UnityEngine.Object) this.Owner || !(this.Owner is PlayerController) || !(bool) (UnityEngine.Object) this.Owner.CurrentGun)
          return;
        this.transform.Rotate(0.0f, 0.0f, Mathf.DeltaAngle(BraveMathCollege.Atan2Degrees(this.specRigidbody.Velocity), this.Owner.CurrentGun.CurrentAngle));
      }
    }

    private void GatherTargetEnemies()
    {
      List<AIActor> activeEnemies = this.transform.position.GetAbsoluteRoom().GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
      Vector2 vector2 = this.transform.position.XY();
      this.throwerPlayer = GameManager.Instance.GetActivePlayerClosestToPoint(vector2);
      if (this.UsesMouseAimPoint)
        vector2 = this.throwerPlayer.unadjustedAimPoint.XY();
      if (activeEnemies == null || this.RemainingEnemiesToHit == null)
        return;
      while (this.RemainingEnemiesToHit.Count < activeEnemies.Count && (this.MaximumNumberOfTargets <= 0 || this.RemainingEnemiesToHit.Count < this.MaximumNumberOfTargets))
      {
        AIActor aiActor = (double) this.MaximumTraversalDistance <= 0.0 || this.RemainingEnemiesToHit.Count <= 0 ? BraveUtility.GetClosestToPosition<AIActor>(activeEnemies, vector2, this.RemainingEnemiesToHit.ToArray()) : BraveUtility.GetClosestToPosition<AIActor>(activeEnemies, vector2, (Func<AIActor, bool>) null, this.MaximumTraversalDistance, this.RemainingEnemiesToHit.ToArray());
        if ((UnityEngine.Object) aiActor == (UnityEngine.Object) null)
          break;
        this.RemainingEnemiesToHit.Enqueue(aiActor);
        vector2 = aiActor.CenterPosition;
      }
    }

    private void HandleTileCollision(CollisionData tileCollision)
    {
      if (this.RemainingEnemiesToHit.Count == 0)
      {
        if ((double) this.m_elapsedTargetlessTime >= 5.0)
        {
          this.DieInAir();
        }
        else
        {
          if ((double) this.m_targetlessTime > 0.0)
            return;
          this.m_targetlessTime = 0.2f;
        }
      }
      else if ((double) this.m_elapsedTargetlessTime >= 1.0)
      {
        this.m_elapsedTargetlessTime = 0.0f;
        this.m_targetlessTime = 0.0f;
        this.RemainingEnemiesToHit.Dequeue();
      }
      else
      {
        if ((double) this.m_targetlessTime > 0.0)
          return;
        this.m_targetlessTime = 0.5f;
      }
    }

    private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
    {
      if (!(bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody.aiActor)
        return;
      if ((bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody.aiActor.behaviorSpeculator)
        rigidbodyCollision.OtherRigidbody.aiActor.behaviorSpeculator.Stun(this.StunDuration);
      if (this.RemainingEnemiesToHit.Count <= 0 || !((UnityEngine.Object) this.RemainingEnemiesToHit.Peek() == (UnityEngine.Object) rigidbodyCollision.OtherRigidbody.aiActor))
        return;
      this.m_elapsedTargetlessTime = 0.0f;
      this.m_targetlessTime = 0.0f;
      this.RemainingEnemiesToHit.Dequeue();
    }

    protected override void Move()
    {
      GameActor gameActor = this.RemainingEnemiesToHit.Count <= 0 ? (GameActor) this.throwerPlayer : (GameActor) this.RemainingEnemiesToHit.Peek();
      if ((double) this.m_targetlessTime <= 0.0)
      {
        if ((UnityEngine.Object) gameActor != (UnityEngine.Object) null)
        {
          Vector2 v = gameActor.CenterPosition - this.transform.position.XY();
          float f = Mathf.DeltaAngle(BraveMathCollege.Atan2Degrees(this.specRigidbody.Velocity), BraveMathCollege.Atan2Degrees(v));
          this.transform.Rotate(0.0f, 0.0f, Mathf.Min(Mathf.Abs(f), this.trackingSpeed * BraveTime.DeltaTime) * Mathf.Sign(f));
        }
      }
      else
      {
        this.m_targetlessTime -= BraveTime.DeltaTime;
        this.m_elapsedTargetlessTime += BraveTime.DeltaTime;
      }
      this.specRigidbody.Velocity = (Vector2) (this.transform.right * this.baseData.speed);
      this.LastVelocity = this.specRigidbody.Velocity;
      if (!((UnityEngine.Object) gameActor == (UnityEngine.Object) this.throwerPlayer) || (double) Vector2.Distance(gameActor.CenterPosition, (Vector2) this.transform.position) >= 1.0)
        return;
      this.DieInAir(true);
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

