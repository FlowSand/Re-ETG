using System.Collections.Generic;

using FullInspector;
using Pathfinding;
using UnityEngine;

using Dungeonator;

#nullable disable

public class SummonEnemyBehavior : BasicAttackBehavior
    {
        public bool DefineSpawnRadius;
        [InspectorShowIf("DefineSpawnRadius")]
        [InspectorIndent]
        public float MinSpawnRadius;
        [InspectorShowIf("DefineSpawnRadius")]
        [InspectorIndent]
        public float MaxSpawnRadius;
        public int MaxRoomOccupancy = -1;
        public int MaxSummonedAtOnce = -1;
        public int MaxToSpawn = -1;
        public int NumToSpawn = 1;
        public bool KillSpawnedOnDeath;
        [InspectorShowIf("ShowCraze")]
        [InspectorIndent]
        public bool CrazeAfterMaxSpawned;
        public float BlackPhantomChance;
        public List<string> EnemeyGuids;
        public SummonEnemyBehavior.SelectionType selectionType;
        public GameObject OverrideCorpse;
        public float SummonTime;
        public bool DisableDrops = true;
        public bool HideGun;
        [InspectorCategory("Visuals")]
        public bool StopDuringAnimation = true;
        [InspectorCategory("Visuals")]
        public string SummonAnim;
        [InspectorCategory("Visuals")]
        public string SummonVfx;
        [InspectorCategory("Visuals")]
        public string TargetVfx;
        [InspectorCategory("Visuals")]
        public bool TargetVfxLoops = true;
        [InspectorCategory("Visuals")]
        public string PostSummonAnim;
        public bool ManuallyDefineRoom;
        [InspectorIndent]
        [InspectorShowIf("ManuallyDefineRoom")]
        public Vector2 roomMin;
        [InspectorIndent]
        [InspectorShowIf("ManuallyDefineRoom")]
        public Vector2 roomMax;
        private SummonEnemyBehavior.State m_state;
        private string m_enemyGuid;
        private AIActor m_enemyPrefab;
        private IntVector2? m_spawnCell;
        private float m_timer;
        private AIActor m_spawnedActor;
        private tk2dSpriteAnimationClip m_spawnClip;
        private IntVector2 m_enemyClearance;
        private List<AIActor> m_allSpawnedActors = new List<AIActor>();
        private int m_numToSpawn;
        private int m_spawnCount;
        private int m_lifetimeSpawnCount;
        private CrazedController m_crazeBehavior;

        private bool ShowCraze() => this.MaxToSpawn > 0;

        public override void Start()
        {
            base.Start();
            this.m_crazeBehavior = this.m_aiActor.GetComponent<CrazedController>();
        }

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_timer);
            if (this.MaxToSpawn > 0 && this.m_lifetimeSpawnCount >= this.MaxToSpawn && this.CrazeAfterMaxSpawned && (Object) this.m_crazeBehavior != (Object) null)
                this.m_crazeBehavior.GoCrazed();
            for (int index = this.m_allSpawnedActors.Count - 1; index >= 0; --index)
            {
                if (!(bool) (Object) this.m_allSpawnedActors[index] || !(bool) (Object) this.m_allSpawnedActors[index].healthHaver || this.m_allSpawnedActors[index].healthHaver.IsDead)
                    this.m_allSpawnedActors.RemoveAt(index);
            }
        }

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady() || this.MaxRoomOccupancy >= 0 && this.m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).Count >= this.MaxRoomOccupancy)
                return BehaviorResult.Continue;
            this.PrepareSpawn();
            if (!this.m_spawnCell.HasValue)
                return BehaviorResult.Continue;
            if (!string.IsNullOrEmpty(this.SummonAnim))
            {
                this.m_aiAnimator.PlayUntilFinished(this.SummonAnim, true);
                if (this.StopDuringAnimation)
                {
                    if (this.HideGun)
                        this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (SummonEnemyBehavior));
                    this.m_aiActor.ClearPath();
                }
            }
            if (!string.IsNullOrEmpty(this.SummonVfx))
                this.m_aiAnimator.PlayVfx(this.SummonVfx);
            if (!string.IsNullOrEmpty(this.TargetVfx))
            {
                AIAnimator aiAnimator = this.m_aiAnimator;
                string targetVfx = this.TargetVfx;
                Vector2? nullable = new Vector2?(Pathfinder.GetClearanceOffset(this.m_spawnCell.Value, this.m_enemyClearance));
                string name = targetVfx;
                Vector2? sourceNormal = new Vector2?();
                Vector2? sourceVelocity = new Vector2?();
                Vector2? position = nullable;
                aiAnimator.PlayVfx(name, sourceNormal, sourceVelocity, position);
            }
            this.m_timer = this.SummonTime;
            this.m_spawnCount = 0;
            if ((bool) (Object) this.m_aiActor && (bool) (Object) this.m_aiActor.knockbackDoer)
                this.m_aiActor.knockbackDoer.SetImmobile(true, nameof (SummonEnemyBehavior));
            this.m_numToSpawn = this.NumToSpawn;
            if (this.MaxRoomOccupancy >= 0)
                this.m_numToSpawn = Mathf.Min(this.m_numToSpawn, this.MaxRoomOccupancy - this.m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All).Count);
            if (this.MaxSummonedAtOnce >= 0)
                this.m_numToSpawn = Mathf.Min(this.m_numToSpawn, this.MaxSummonedAtOnce - this.m_allSpawnedActors.Count);
            if (this.MaxToSpawn >= 0)
                this.m_numToSpawn = Mathf.Min(this.m_numToSpawn, this.MaxToSpawn - this.m_lifetimeSpawnCount);
            this.m_state = SummonEnemyBehavior.State.Summoning;
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            if (this.m_state == SummonEnemyBehavior.State.Summoning)
            {
                if ((double) this.m_timer <= 0.0)
                {
                    this.m_spawnedActor = AIActor.Spawn(this.m_enemyPrefab, this.m_spawnCell.Value, this.m_aiActor.ParentRoom, awakenAnimType: AIActor.AwakenAnimationType.Spawn);
                    this.m_spawnedActor.aiAnimator.PlayDefaultSpawnState();
                    this.m_allSpawnedActors.Add(this.m_spawnedActor);
                    this.m_spawnedActor.CanDropCurrency = false;
                    if ((Object) this.OverrideCorpse != (Object) null)
                        this.m_spawnedActor.CorpseObject = this.OverrideCorpse;
                    if ((double) this.BlackPhantomChance > 0.0 && ((double) this.BlackPhantomChance >= 1.0 || (double) Random.value < (double) this.BlackPhantomChance))
                        this.m_spawnedActor.ForceBlackPhantom = true;
                    ++this.m_spawnCount;
                    ++this.m_lifetimeSpawnCount;
                    if (this.m_spawnCount < this.m_numToSpawn)
                    {
                        this.PrepareSpawn();
                        if (this.m_spawnCell.HasValue)
                        {
                            if (!string.IsNullOrEmpty(this.TargetVfx))
                            {
                                if (this.TargetVfxLoops)
                                    this.m_aiAnimator.StopVfx(this.TargetVfx);
                                AIAnimator aiAnimator = this.m_aiAnimator;
                                string targetVfx = this.TargetVfx;
                                Vector2? nullable = new Vector2?(Pathfinder.GetClearanceOffset(this.m_spawnCell.Value, this.m_enemyClearance));
                                string name = targetVfx;
                                Vector2? sourceNormal = new Vector2?();
                                Vector2? sourceVelocity = new Vector2?();
                                Vector2? position = nullable;
                                aiAnimator.PlayVfx(name, sourceNormal, sourceVelocity, position);
                            }
                            this.m_timer = this.SummonTime;
                            return ContinuousBehaviorResult.Continue;
                        }
                    }
                    this.m_spawnClip = this.m_spawnedActor.spriteAnimator.CurrentClip;
                    if (this.m_spawnClip != null && this.m_spawnClip.wrapMode != tk2dSpriteAnimationClip.WrapMode.Loop)
                    {
                        this.m_state = SummonEnemyBehavior.State.WaitingForSummonAnim;
                        return ContinuousBehaviorResult.Continue;
                    }
                    if (string.IsNullOrEmpty(this.PostSummonAnim))
                        return ContinuousBehaviorResult.Finished;
                    this.m_state = SummonEnemyBehavior.State.WaitingForPostAnim;
                    this.m_aiAnimator.PlayUntilFinished(this.PostSummonAnim);
                    return ContinuousBehaviorResult.Continue;
                }
            }
            else if (this.m_state == SummonEnemyBehavior.State.WaitingForSummonAnim)
            {
                if (!(bool) (Object) this.m_spawnedActor || !(bool) (Object) this.m_spawnedActor.healthHaver || this.m_spawnedActor.healthHaver.IsDead || !this.m_spawnedActor.spriteAnimator.IsPlaying(this.m_spawnClip))
                {
                    if (string.IsNullOrEmpty(this.PostSummonAnim))
                        return ContinuousBehaviorResult.Finished;
                    this.m_state = SummonEnemyBehavior.State.WaitingForPostAnim;
                    this.m_aiAnimator.PlayUntilFinished(this.PostSummonAnim);
                    return ContinuousBehaviorResult.Continue;
                }
            }
            else if (this.m_state == SummonEnemyBehavior.State.WaitingForPostAnim && !this.m_aiActor.spriteAnimator.IsPlaying(this.PostSummonAnim))
                return ContinuousBehaviorResult.Finished;
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            if (!string.IsNullOrEmpty(this.SummonAnim))
                this.m_aiAnimator.EndAnimationIf(this.SummonAnim);
            if (!string.IsNullOrEmpty(this.SummonVfx))
                this.m_aiAnimator.StopVfx(this.SummonVfx);
            if (!string.IsNullOrEmpty(this.TargetVfx) && this.TargetVfxLoops)
                this.m_aiAnimator.StopVfx(this.TargetVfx);
            if (!string.IsNullOrEmpty(this.PostSummonAnim))
                this.m_aiAnimator.EndAnimationIf(this.PostSummonAnim);
            if (this.HideGun)
                this.m_aiShooter.ToggleGunAndHandRenderers(true, nameof (SummonEnemyBehavior));
            if ((bool) (Object) this.m_aiActor && (bool) (Object) this.m_aiActor.knockbackDoer)
                this.m_aiActor.knockbackDoer.SetImmobile(false, nameof (SummonEnemyBehavior));
            this.m_state = SummonEnemyBehavior.State.Idle;
            this.m_updateEveryFrame = false;
            this.UpdateCooldowns();
        }

        public override bool IsReady()
        {
            return (this.MaxToSpawn <= 0 || this.m_lifetimeSpawnCount < this.MaxToSpawn) && (this.MaxSummonedAtOnce <= 0 || this.m_allSpawnedActors.Count < this.MaxSummonedAtOnce) && base.IsReady();
        }

        public override void OnActorPreDeath()
        {
            if (!this.KillSpawnedOnDeath)
                return;
            for (int index = 0; index < this.m_allSpawnedActors.Count; ++index)
            {
                AIActor allSpawnedActor = this.m_allSpawnedActors[index];
                if ((bool) (Object) allSpawnedActor && (bool) (Object) allSpawnedActor.healthHaver && allSpawnedActor.healthHaver.IsAlive)
                    allSpawnedActor.healthHaver.ApplyDamage(10000f, Vector2.zero, "Summoner Death", damageCategory: DamageCategory.Unstoppable);
            }
        }

        private void PrepareSpawn()
        {
            this.m_enemyGuid = this.selectionType != SummonEnemyBehavior.SelectionType.Ordered ? BraveUtility.RandomElement<string>(this.EnemeyGuids) : this.EnemeyGuids[this.m_lifetimeSpawnCount];
            this.m_enemyPrefab = EnemyDatabase.GetOrLoadByGuid(this.m_enemyGuid);
            PixelCollider groundPixelCollider = this.m_enemyPrefab.specRigidbody.GroundPixelCollider;
            if (groundPixelCollider != null && groundPixelCollider.ColliderGenerationMode == PixelCollider.PixelColliderGeneration.Manual)
            {
                this.m_enemyClearance = new Vector2((float) groundPixelCollider.ManualWidth / 16f, (float) groundPixelCollider.ManualHeight / 16f).ToIntVector2(VectorConversions.Ceil);
            }
            else
            {
                Debug.LogFormat("Enemy type {0} does not have a manually defined ground collider!", (object) this.m_enemyPrefab.name);
                this.m_enemyClearance = IntVector2.One;
            }
            Vector2 worldpoint1 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(0.0f, 0.0f), ViewportType.Gameplay);
            Vector2 worldpoint2 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(1f, 1f), ViewportType.Gameplay);
            IntVector2 bottomLeft = worldpoint1.ToIntVector2(VectorConversions.Ceil);
            IntVector2 topRight = worldpoint2.ToIntVector2(VectorConversions.Floor) - IntVector2.One;
            Vector2 center = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.Ground);
            float minDistanceSquared = this.MinSpawnRadius * this.MinSpawnRadius;
            float maxDistanceSquared = this.MaxSpawnRadius * this.MaxSpawnRadius;
            this.m_spawnCell = this.m_aiActor.ParentRoom.GetRandomAvailableCell(new IntVector2?(this.m_enemyClearance), new CellTypes?(this.m_enemyPrefab.PathableTiles), true, (CellValidator) (c =>
            {
                for (int index1 = 0; index1 < this.m_enemyClearance.x; ++index1)
                {
                    for (int index2 = 0; index2 < this.m_enemyClearance.y; ++index2)
                    {
                        if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index1, c.y + index2) || this.ManuallyDefineRoom && ((double) (c.x + index1) < (double) this.roomMin.x || (double) (c.x + index1) > (double) this.roomMax.x || (double) (c.y + index2) < (double) this.roomMin.y || (double) (c.y + index2) > (double) this.roomMax.y))
                            return false;
                    }
                }
                if (this.DefineSpawnRadius)
                {
                    float num1 = (float) c.x + 0.5f - center.x;
                    float num2 = (float) c.y + 0.5f - center.y;
                    float num3 = (float) ((double) num1 * (double) num1 + (double) num2 * (double) num2);
                    if ((double) num3 < (double) minDistanceSquared || (double) num3 > (double) maxDistanceSquared)
                        return false;
                }
                else if (c.x < bottomLeft.x || c.y < bottomLeft.y || c.x + this.m_aiActor.Clearance.x - 1 > topRight.x || c.y + this.m_aiActor.Clearance.y - 1 > topRight.y)
                    return false;
                return true;
            }));
        }

        public enum SelectionType
        {
            Random,
            Ordered,
        }

        private enum State
        {
            Idle,
            Summoning,
            WaitingForSummonAnim,
            WaitingForPostAnim,
        }
    }

