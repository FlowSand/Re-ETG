using System;

using FullInspector;
using Pathfinding;
using UnityEngine;

using Dungeonator;

#nullable disable

public class SeekTargetBehavior : RangedMovementBehavior
    {
        public bool StopWhenInRange = true;
        public float CustomRange = -1f;
        [InspectorShowIf("StopWhenInRange")]
        public bool LineOfSight = true;
        public bool ReturnToSpawn = true;
        public float SpawnTetherDistance;
        public float PathInterval = 0.25f;
        [NonSerialized]
        public bool ExternalCooldownSource;
        private float m_repathTimer;
        private SeekTargetBehavior.State m_state;

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_repathTimer);
        }

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            SpeculativeRigidbody targetRigidbody = this.m_aiActor.TargetRigidbody;
            if (this.InRange() && (bool) (UnityEngine.Object) targetRigidbody)
            {
                bool flag = this.m_aiActor.HasLineOfSightToTarget;
                float desiredCombatDistance = this.m_aiActor.DesiredCombatDistance;
                this.m_state = SeekTargetBehavior.State.PathingToTarget;
                if ((bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody && (bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody.aiActor && !this.m_aiActor.TargetRigidbody.CollideWithOthers)
                    flag = true;
                if (this.ExternalCooldownSource)
                {
                    this.m_aiActor.ClearPath();
                    return BehaviorResult.Continue;
                }
                if (this.StopWhenInRange && (double) this.m_aiActor.DistanceToTarget <= (double) desiredCombatDistance && (!this.LineOfSight || flag))
                {
                    this.m_aiActor.ClearPath();
                    return BehaviorResult.Continue;
                }
                if ((double) this.m_repathTimer <= 0.0)
                {
                    CellValidator cellValidator1 = (CellValidator) null;
                    if ((double) this.SpawnTetherDistance > 0.0)
                        cellValidator1 = (CellValidator) (p => (double) Vector2.Distance(p.ToCenterVector2(), (Vector2) this.m_aiActor.SpawnPosition) < (double) this.SpawnTetherDistance);
                    Vector2 unitCenter = targetRigidbody.UnitCenter;
                    AIActor aiActor = this.m_aiActor;
                    Vector2 vector2 = unitCenter;
                    CellValidator cellValidator2 = cellValidator1;
                    Vector2 targetPosition = vector2;
                    Vector2? overridePathEnd = new Vector2?();
                    CellValidator cellValidator3 = cellValidator2;
                    CellTypes? overridePathableTiles = new CellTypes?();
                    aiActor.PathfindToPosition(targetPosition, overridePathEnd, cellValidator: cellValidator3, overridePathableTiles: overridePathableTiles);
                    this.m_repathTimer = this.PathInterval;
                }
                return BehaviorResult.SkipRemainingClassBehaviors;
            }
            if (this.m_state == SeekTargetBehavior.State.PathingToTarget)
            {
                this.m_aiActor.ClearPath();
                this.m_state = SeekTargetBehavior.State.Idle;
            }
            else if (this.m_state == SeekTargetBehavior.State.Idle)
            {
                if (this.ReturnToSpawn && this.m_aiActor.GridPosition != this.m_aiActor.SpawnGridPosition && this.m_aiActor.PathComplete)
                {
                    this.m_aiActor.PathfindToPosition((Vector2) this.m_aiActor.SpawnPosition);
                    this.m_state = SeekTargetBehavior.State.ReturningToSpawn;
                }
            }
            else if (this.m_state == SeekTargetBehavior.State.ReturningToSpawn && this.m_aiActor.PathComplete)
                this.m_state = SeekTargetBehavior.State.Idle;
            return BehaviorResult.Continue;
        }

        public override float DesiredCombatDistance => this.CustomRange;

        public override bool AllowFearRunState => true;

        private enum State
        {
            Idle,
            PathingToTarget,
            ReturningToSpawn,
        }
    }

