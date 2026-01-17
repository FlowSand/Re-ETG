// Decompiled with JetBrains decompiler
// Type: MindControlEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Effects
{
    public class MindControlEffect : MonoBehaviour
    {
      [NonSerialized]
      public PlayerController owner;
      private AIActor m_aiActor;
      private BehaviorSpeculator m_behaviorSpeculator;
      private bool m_attackedThisCycle = true;
      private NonActor m_fakeActor;
      private SpeculativeRigidbody m_fakeTargetRigidbody;
      private ArbitraryCableDrawer m_cable;
      private GameObject m_overheadVFX;

      private void Start()
      {
        this.m_aiActor = this.GetComponent<AIActor>();
        this.m_behaviorSpeculator = this.m_aiActor.behaviorSpeculator;
        GameObject gameObject = new GameObject("fake target");
        this.m_fakeActor = gameObject.AddComponent<NonActor>();
        this.m_fakeActor.HasShadow = false;
        this.m_fakeTargetRigidbody = gameObject.AddComponent<SpeculativeRigidbody>();
        this.m_fakeTargetRigidbody.PixelColliders = new List<PixelCollider>();
        this.m_fakeTargetRigidbody.CollideWithTileMap = false;
        this.m_fakeTargetRigidbody.CollideWithOthers = false;
        this.m_fakeTargetRigidbody.CanBeCarried = false;
        this.m_fakeTargetRigidbody.CanBePushed = false;
        this.m_fakeTargetRigidbody.CanCarry = false;
        this.m_fakeTargetRigidbody.PixelColliders.Add(new PixelCollider()
        {
          ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
          CollisionLayer = CollisionLayer.TileBlocker,
          ManualWidth = 4,
          ManualHeight = 4
        });
        this.m_cable = this.m_aiActor.gameObject.AddComponent<ArbitraryCableDrawer>();
        this.m_cable.Attach1Offset = this.owner.CenterPosition - this.owner.transform.position.XY();
        this.m_cable.Attach2Offset = this.m_aiActor.CenterPosition - this.m_aiActor.transform.position.XY();
        this.m_cable.Initialize(this.owner.transform, this.m_aiActor.transform);
        this.m_overheadVFX = this.m_aiActor.PlayEffectOnActor((GameObject) ResourceCache.Acquire("Global VFX/VFX_Controller_Status"), new Vector3(0.0f, this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitDimensions.y, 0.0f), useHitbox: true);
      }

      private Vector2 GetPlayerAimPointController(Vector2 aimBase, Vector2 aimDirection)
      {
        Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (Func<SpeculativeRigidbody, bool>) (otherRigidbody => (bool) (UnityEngine.Object) otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets);
        Vector2 aimPointController = aimBase + aimDirection * 10f;
        int mask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.EnemyHitBox, CollisionLayer.BulletBreakable);
        RaycastResult result;
        if (PhysicsEngine.Instance.Raycast(aimBase, aimDirection, 50f, out result, rayMask: mask, rigidbodyExcluder: rigidbodyExcluder))
          aimPointController = aimBase + aimDirection * result.Distance;
        RaycastResult.Pool.Free(ref result);
        return aimPointController;
      }

      private void UpdateAimTargetPosition()
      {
        PlayerController owner = this.owner;
        BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(owner.PlayerIDX);
        GungeonActions activeActions = instanceForPlayer.ActiveActions;
        if (instanceForPlayer.IsKeyboardAndMouse())
          this.m_fakeTargetRigidbody.transform.position = (Vector3) owner.unadjustedAimPoint.XY();
        else
          this.m_fakeTargetRigidbody.transform.position = (Vector3) this.GetPlayerAimPointController(owner.CenterPosition, activeActions.Aim.Vector);
        this.m_fakeTargetRigidbody.Reinitialize();
      }

      private void Update()
      {
        this.m_fakeActor.specRigidbody = this.m_fakeTargetRigidbody;
        if ((bool) (UnityEngine.Object) this.m_aiActor)
        {
          this.m_aiActor.CanTargetEnemies = true;
          this.m_aiActor.CanTargetPlayers = false;
          this.m_aiActor.PlayerTarget = (GameActor) this.m_fakeActor;
          this.m_aiActor.OverrideTarget = (SpeculativeRigidbody) null;
          this.UpdateAimTargetPosition();
          if ((bool) (UnityEngine.Object) this.m_aiActor.aiShooter)
            this.m_aiActor.aiShooter.AimAtPoint(this.m_behaviorSpeculator.PlayerTarget.CenterPosition);
        }
        if (!(bool) (UnityEngine.Object) this.m_behaviorSpeculator)
          return;
        GungeonActions activeActions = BraveInput.GetInstanceForPlayer(this.owner.PlayerIDX).ActiveActions;
        if ((double) this.m_behaviorSpeculator.AttackCooldown <= 0.0)
        {
          if (!this.m_attackedThisCycle && this.m_behaviorSpeculator.ActiveContinuousAttackBehavior != null)
            this.m_attackedThisCycle = true;
          if (this.m_attackedThisCycle && this.m_behaviorSpeculator.ActiveContinuousAttackBehavior == null)
            this.m_behaviorSpeculator.AttackCooldown = float.MaxValue;
        }
        else if (activeActions.ShootAction.WasPressed)
        {
          this.m_attackedThisCycle = false;
          this.m_behaviorSpeculator.AttackCooldown = 0.0f;
        }
        if (this.m_behaviorSpeculator.TargetBehaviors != null && this.m_behaviorSpeculator.TargetBehaviors.Count > 0)
          this.m_behaviorSpeculator.TargetBehaviors.Clear();
        if (this.m_behaviorSpeculator.MovementBehaviors != null && this.m_behaviorSpeculator.MovementBehaviors.Count > 0)
          this.m_behaviorSpeculator.MovementBehaviors.Clear();
        AIActor aiActor = this.m_aiActor;
        aiActor.ImpartedVelocity = aiActor.ImpartedVelocity + activeActions.Move.Value * this.m_aiActor.MovementSpeed;
        if (this.m_behaviorSpeculator.AttackBehaviors == null)
          return;
        for (int index = 0; index < this.m_behaviorSpeculator.AttackBehaviors.Count; ++index)
          this.ProcessAttack(this.m_behaviorSpeculator.AttackBehaviors[index]);
      }

      private void ProcessAttack(AttackBehaviorBase attack)
      {
        switch (attack)
        {
          case BasicAttackBehavior _:
            BasicAttackBehavior basicAttackBehavior = attack as BasicAttackBehavior;
            basicAttackBehavior.Cooldown = 0.0f;
            basicAttackBehavior.RequiresLineOfSight = false;
            basicAttackBehavior.MinRange = -1f;
            basicAttackBehavior.Range = -1f;
            if (attack is TeleportBehavior)
            {
              basicAttackBehavior.RequiresLineOfSight = true;
              basicAttackBehavior.MinRange = 1000f;
              basicAttackBehavior.Range = 0.1f;
            }
            if (!(basicAttackBehavior is ShootGunBehavior))
              break;
            ShootGunBehavior shootGunBehavior = basicAttackBehavior as ShootGunBehavior;
            shootGunBehavior.LineOfSight = false;
            shootGunBehavior.EmptiesClip = false;
            shootGunBehavior.RespectReload = false;
            break;
          case AttackBehaviorGroup _:
            AttackBehaviorGroup attackBehaviorGroup = attack as AttackBehaviorGroup;
            for (int index = 0; index < attackBehaviorGroup.AttackBehaviors.Count; ++index)
              this.ProcessAttack(attackBehaviorGroup.AttackBehaviors[index].Behavior);
            break;
        }
      }
    }

}
