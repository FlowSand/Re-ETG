// Decompiled with JetBrains decompiler
// Type: MovementBehaviorBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public abstract class MovementBehaviorBase : BehaviorBase
    {
      private static float FleePathInterval = 0.25f;
      private BehaviorSpeculator m_behaviorSpeculator;
      private tk2dSpriteAnimator m_extantFearVFX;
      private float m_fleeRepathTimer;
      private bool m_isFleeing;

      public virtual float DesiredCombatDistance => -1f;

      public override void Start()
      {
        base.Start();
        this.m_behaviorSpeculator = this.m_aiActor.behaviorSpeculator;
      }

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_fleeRepathTimer);
        this.UpdateFearVFX();
      }

      public override BehaviorResult Update()
      {
        int num = (int) base.Update();
        if (this.ShouldFleePlayer())
        {
          if (!this.m_isFleeing && this.m_behaviorSpeculator.IsInterruptable)
            this.m_behaviorSpeculator.Interrupt();
          this.m_isFleeing = true;
          this.UpdateFearVFX();
          FleePlayerData fleeData = this.m_behaviorSpeculator.FleePlayerData;
          if ((double) this.m_fleeRepathTimer <= 0.0)
          {
            Vector2 pointOfFear = fleeData.Player.CenterPosition;
            CellValidator cellValidator = (CellValidator) (p => (double) Vector2.Distance(p.ToCenterVector2(), pointOfFear) > (double) fleeData.StopDistance);
            CellTypes pathableTiles = this.m_aiActor.PathableTiles;
            if ((double) this.m_aiActor.DistanceToTarget < (double) fleeData.DeathDistance)
              pathableTiles |= CellTypes.PIT;
            IntVector2? nearestAvailableCell = this.m_aiActor.ParentRoom.GetNearestAvailableCell(this.m_aiActor.specRigidbody.UnitCenter, new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(pathableTiles), cellValidator: cellValidator);
            if (nearestAvailableCell.HasValue)
            {
              AIActor aiActor = this.m_aiActor;
              Vector2 centerVector2 = nearestAvailableCell.Value.ToCenterVector2();
              CellTypes? nullable = new CellTypes?(pathableTiles);
              Vector2 targetPosition = centerVector2;
              Vector2? overridePathEnd = new Vector2?();
              CellTypes? overridePathableTiles = nullable;
              aiActor.PathfindToPosition(targetPosition, overridePathEnd, overridePathableTiles: overridePathableTiles);
              this.m_fleeRepathTimer = MovementBehaviorBase.FleePathInterval;
            }
            else
              this.m_aiActor.ClearPath();
          }
          return BehaviorResult.SkipRemainingClassBehaviors;
        }
        this.m_isFleeing = false;
        this.UpdateFearVFX();
        return base.Update();
      }

      public override void OnActorPreDeath()
      {
        base.OnActorPreDeath();
        this.m_isFleeing = false;
        if (!((Object) this.m_extantFearVFX != (Object) null))
          return;
        SpawnManager.Despawn(this.m_extantFearVFX.gameObject);
        this.m_extantFearVFX = (tk2dSpriteAnimator) null;
      }

      public virtual bool AllowFearRunState => false;

      private bool ShouldFleePlayer()
      {
        if ((Object) this.m_behaviorSpeculator == (Object) null)
          return false;
        FleePlayerData fleePlayerData = this.m_behaviorSpeculator.FleePlayerData;
        if (fleePlayerData == null || !(bool) (Object) fleePlayerData.Player)
          return false;
        float num = Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, fleePlayerData.Player.CenterPosition);
        return this.m_isFleeing && (double) num < (double) fleePlayerData.StopDistance || (double) num < (double) fleePlayerData.StartDistance;
      }

      protected virtual void UpdateFearVFX()
      {
        if (!this.m_isFleeing && (Object) this.m_extantFearVFX != (Object) null)
        {
          if (this.m_extantFearVFX.IsPlaying("fear_face_vfx"))
          {
            this.m_extantFearVFX.Play("fear_face_vfx_out");
          }
          else
          {
            if (this.m_extantFearVFX.Playing)
              return;
            SpawnManager.Despawn(this.m_extantFearVFX.gameObject);
            this.m_extantFearVFX = (tk2dSpriteAnimator) null;
          }
        }
        else if (this.m_isFleeing && (Object) this.m_extantFearVFX == (Object) null)
        {
          this.m_extantFearVFX = this.m_aiActor.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Fear") as GameObject, (Vector3) (this.m_aiActor.sprite.WorldTopCenter - this.m_aiActor.CenterPosition).WithX(0.0f), alreadyMiddleCenter: true).GetComponent<tk2dSpriteAnimator>();
        }
        else
        {
          if (!this.m_isFleeing || !((Object) this.m_extantFearVFX != (Object) null))
            return;
          if (!this.m_extantFearVFX.IsPlaying("fear_face_vfx"))
            this.m_extantFearVFX.Play("fear_face_vfx");
          this.m_extantFearVFX.transform.position = this.m_aiActor.sprite.WorldTopCenter.ToVector3ZUp(this.m_extantFearVFX.transform.position.z);
        }
      }
    }

}
