using System.Collections.Generic;

using FullInspector;
using Pathfinding;
using UnityEngine;

using Dungeonator;

#nullable disable

public class RemoteShootBehavior : BasicAttackBehavior
    {
        public bool DefineRadius;
        [InspectorIndent]
        [InspectorShowIf("DefineRadius")]
        public float MinRadius;
        [InspectorShowIf("DefineRadius")]
        [InspectorIndent]
        public float MaxRadius;
        public IntVector2 RemoteFootprint = new IntVector2(1, 1);
        public float TellTime;
        public BulletScriptSelector remoteBulletScript;
        public float FireTime;
        public bool Multifire;
        [InspectorShowIf("Multifire")]
        [InspectorIndent]
        public int MinShots = 2;
        [InspectorIndent]
        [InspectorShowIf("Multifire")]
        public int MaxShots = 3;
        [InspectorIndent]
        [InspectorShowIf("Multifire")]
        public float MidShotTime;
        [InspectorCategory("Visuals")]
        public bool StopDuringAnimation = true;
        [InspectorCategory("Visuals")]
        public string TellAnim;
        [InspectorCategory("Visuals")]
        public string TellVfx;
        [InspectorCategory("Visuals")]
        public string ShootVfx;
        [InspectorCategory("Visuals")]
        public string RemoteVfx;
        [InspectorCategory("Visuals")]
        public string PostFireAnim;
        [InspectorCategory("Visuals")]
        public bool HideGun;
        private RemoteShootBehavior.State m_state;
        private IntVector2? m_spawnCell;
        private List<IntVector2> m_previousSpawnCells = new List<IntVector2>();
        private float m_timer;
        private int m_shotsRemaining;

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_timer);
        }

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady() || !(bool) (Object) this.m_aiActor.TargetRigidbody)
                return BehaviorResult.Continue;
            this.m_previousSpawnCells.Clear();
            this.ChooseSpawnLocation();
            if (!this.m_spawnCell.HasValue)
                return BehaviorResult.Continue;
            if (!string.IsNullOrEmpty(this.TellAnim))
            {
                this.m_aiAnimator.PlayUntilFinished(this.TellAnim, true);
                if (this.StopDuringAnimation)
                {
                    if (this.HideGun)
                        this.m_aiShooter.ToggleGunAndHandRenderers(false, "SummonEnemyBehavior");
                    this.m_aiActor.ClearPath();
                }
            }
            if (!string.IsNullOrEmpty(this.TellVfx))
                this.m_aiAnimator.PlayVfx(this.TellVfx);
            this.m_timer = this.TellTime;
            if ((bool) (Object) this.m_aiActor && (bool) (Object) this.m_aiActor.knockbackDoer)
                this.m_aiActor.knockbackDoer.SetImmobile(true, "SummonEnemyBehavior");
            this.m_state = RemoteShootBehavior.State.Casting;
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            if (this.m_state == RemoteShootBehavior.State.Casting)
            {
                if ((double) this.m_timer <= 0.0)
                {
                    this.m_shotsRemaining = !this.Multifire ? 1 : Random.Range(this.MinShots, this.MaxShots + 1);
                    Vector2 clearanceOffset = Pathfinder.GetClearanceOffset(this.m_spawnCell.Value, this.RemoteFootprint);
                    if (!string.IsNullOrEmpty(this.ShootVfx))
                        this.m_aiAnimator.PlayVfx(this.ShootVfx);
                    if (!string.IsNullOrEmpty(this.RemoteVfx))
                    {
                        AIAnimator aiAnimator = this.m_aiAnimator;
                        string remoteVfx = this.RemoteVfx;
                        Vector2? nullable = new Vector2?(clearanceOffset);
                        string name = remoteVfx;
                        Vector2? sourceNormal = new Vector2?();
                        Vector2? sourceVelocity = new Vector2?();
                        Vector2? position = nullable;
                        aiAnimator.PlayVfx(name, sourceNormal, sourceVelocity, position);
                    }
                    SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.remoteBulletScript, new Vector2?(clearanceOffset));
                    this.m_state = RemoteShootBehavior.State.Firing;
                    --this.m_shotsRemaining;
                    this.m_timer = this.m_shotsRemaining <= 0 ? this.FireTime : this.MidShotTime;
                    return ContinuousBehaviorResult.Continue;
                }
            }
            else if (this.m_state == RemoteShootBehavior.State.Firing)
            {
                if ((double) this.m_timer <= 0.0)
                {
                    if (this.m_shotsRemaining > 0)
                    {
                        this.ChooseSpawnLocation();
                        if (this.m_spawnCell.HasValue)
                        {
                            Vector2 clearanceOffset = Pathfinder.GetClearanceOffset(this.m_spawnCell.Value, this.RemoteFootprint);
                            if (!string.IsNullOrEmpty(this.RemoteVfx))
                            {
                                AIAnimator aiAnimator = this.m_aiAnimator;
                                string remoteVfx = this.RemoteVfx;
                                Vector2? nullable = new Vector2?(clearanceOffset);
                                string name = remoteVfx;
                                Vector2? sourceNormal = new Vector2?();
                                Vector2? sourceVelocity = new Vector2?();
                                Vector2? position = nullable;
                                aiAnimator.PlayVfx(name, sourceNormal, sourceVelocity, position);
                            }
                            SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.remoteBulletScript, new Vector2?(clearanceOffset));
                        }
                        --this.m_shotsRemaining;
                        this.m_timer = this.m_shotsRemaining <= 0 ? this.FireTime : this.MidShotTime;
                        return ContinuousBehaviorResult.Continue;
                    }
                    if (string.IsNullOrEmpty(this.PostFireAnim))
                        return ContinuousBehaviorResult.Finished;
                    this.m_state = RemoteShootBehavior.State.PostFire;
                    this.m_aiAnimator.PlayUntilFinished(this.PostFireAnim);
                    return ContinuousBehaviorResult.Continue;
                }
            }
            else if (this.m_state == RemoteShootBehavior.State.PostFire && !this.m_aiAnimator.IsPlaying(this.PostFireAnim))
                return ContinuousBehaviorResult.Finished;
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            if (!string.IsNullOrEmpty(this.TellAnim))
                this.m_aiAnimator.EndAnimationIf(this.TellAnim);
            if (!string.IsNullOrEmpty(this.TellVfx))
                this.m_aiAnimator.StopVfx(this.TellVfx);
            if (!string.IsNullOrEmpty(this.ShootVfx))
                this.m_aiAnimator.StopVfx(this.ShootVfx);
            if (!string.IsNullOrEmpty(this.RemoteVfx))
                this.m_aiAnimator.StopVfx(this.RemoteVfx);
            if (!string.IsNullOrEmpty(this.PostFireAnim))
                this.m_aiAnimator.EndAnimationIf(this.PostFireAnim);
            if (this.HideGun)
                this.m_aiShooter.ToggleGunAndHandRenderers(true, "SummonEnemyBehavior");
            if ((bool) (Object) this.m_aiActor && (bool) (Object) this.m_aiActor.knockbackDoer)
                this.m_aiActor.knockbackDoer.SetImmobile(false, "SummonEnemyBehavior");
            this.m_state = RemoteShootBehavior.State.Idle;
            this.m_updateEveryFrame = false;
            this.UpdateCooldowns();
        }

        private void ChooseSpawnLocation()
        {
            if (!(bool) (Object) this.m_aiActor.TargetRigidbody)
            {
                this.m_spawnCell = new IntVector2?();
            }
            else
            {
                Vector2 worldpoint1 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(0.0f, 0.0f), ViewportType.Gameplay);
                Vector2 worldpoint2 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(1f, 1f), ViewportType.Gameplay);
                IntVector2 bottomLeft = worldpoint1.ToIntVector2(VectorConversions.Ceil);
                IntVector2 topRight = worldpoint2.ToIntVector2(VectorConversions.Floor) - IntVector2.One;
                Vector2 targetCenter = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
                Vector2? additionalTargetCenter = new Vector2?();
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && this.m_aiActor.PlayerTarget is PlayerController)
                {
                    PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(this.m_aiActor.PlayerTarget as PlayerController);
                    if ((bool) (Object) otherPlayer)
                        additionalTargetCenter = new Vector2?(otherPlayer.specRigidbody.GetUnitCenter(ColliderType.HitBox));
                }
                float minDistanceSquared = this.MinRadius * this.MinRadius;
                float maxDistanceSquared = this.MaxRadius * this.MaxRadius;
                this.m_spawnCell = this.m_aiActor.ParentRoom.GetRandomAvailableCell(new IntVector2?(this.RemoteFootprint), new CellTypes?(CellTypes.FLOOR | CellTypes.PIT), cellValidator: (CellValidator) (c =>
                {
                    for (int index1 = 0; index1 < this.RemoteFootprint.x; ++index1)
                    {
                        for (int index2 = 0; index2 < this.RemoteFootprint.y; ++index2)
                        {
                            if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index1, c.y + index2))
                                return false;
                        }
                    }
                    if (this.DefineRadius)
                    {
                        float num1 = (float) c.x + 0.5f - targetCenter.x;
                        float num2 = (float) c.y + 0.5f - targetCenter.y;
                        float num3 = (float) ((double) num1 * (double) num1 + (double) num2 * (double) num2);
                        if ((double) num3 < (double) minDistanceSquared || (double) num3 > (double) maxDistanceSquared)
                            return false;
                        if (additionalTargetCenter.HasValue)
                        {
                            float num4 = (float) c.x + 0.5f - additionalTargetCenter.Value.x;
                            float num5 = (float) c.y + 0.5f - additionalTargetCenter.Value.y;
                            float num6 = (float) ((double) num4 * (double) num4 + (double) num5 * (double) num5);
                            if ((double) num6 < (double) minDistanceSquared || (double) num6 > (double) maxDistanceSquared)
                                return false;
                        }
                    }
                    if (c.x < bottomLeft.x || c.y < bottomLeft.y || c.x + this.m_aiActor.Clearance.x - 1 > topRight.x || c.y + this.m_aiActor.Clearance.y - 1 > topRight.y)
                        return false;
                    for (int index = 0; index < this.m_previousSpawnCells.Count; ++index)
                    {
                        if (c.x == this.m_previousSpawnCells[index].x && c.y == this.m_previousSpawnCells[index].y)
                            return false;
                    }
                    return true;
                }));
                if (!this.m_spawnCell.HasValue)
                    return;
                this.m_previousSpawnCells.Add(this.m_spawnCell.Value);
            }
        }

        private enum State
        {
            Idle,
            Casting,
            Firing,
            PostFire,
        }
    }

