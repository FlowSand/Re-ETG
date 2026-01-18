using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class MajorBreakable : PersistentVFXManagerBehaviour
    {
        public float HitPoints = 100f;
        public float DamageReduction;
        public int MinHits;
        public int EnemyDamageOverride = -1;
        public bool ImmuneToBeastMode;
        public bool ScaleWithEnemyHealth;
        public bool OnlyExplosions;
        public bool IgnoreExplosions;
        [NonSerialized]
        public bool IsSecretDoor;
        public bool GameActorMotionBreaks;
        public bool PlayerRollingBreaks;
        public bool spawnShards = true;
        [ShowInInspectorIf("spawnShards", true)]
        public bool distributeShards;
        public ShardCluster[] shardClusters;
        [ShowInInspectorIf("spawnShards", true)]
        public float minShardPercentSpeed = 0.05f;
        [ShowInInspectorIf("spawnShards", true)]
        public float maxShardPercentSpeed = 0.3f;
        [ShowInInspectorIf("spawnShards", true)]
        public MinorBreakable.BreakStyle shardBreakStyle;
        public bool usesTemporaryZeroHitPointsState;
        [ShowInInspectorIf("usesTemporaryZeroHitPointsState", true)]
        public string spriteNameToUseAtZeroHP;
        [NonSerialized]
        public string overrideSpriteNameToUseAtZeroHP;
        public bool destroyedOnBreak;
        public List<GameObject> childrenToDestroy;
        public bool playsAnimationOnNotBroken;
        [ShowInInspectorIf("playsAnimationOnNotBroken", true)]
        public string notBreakAnimation;
        public bool handlesOwnBreakAnimation;
        [ShowInInspectorIf("handlesOwnBreakAnimation", true)]
        public string breakAnimation;
        public bool handlesOwnPrebreakFrames;
        public BreakFrame[] prebreakFrames;
        public VFXPool damageVfx;
        [ShowInInspectorIf("damageVfx", true)]
        public float damageVfxMinTimeBetween = 0.2f;
        public VFXPool breakVfx;
        [ShowInInspectorIf("breakVfx", true)]
        public GameObject breakVfxParent;
        [ShowInInspectorIf("breakVfx", true)]
        public bool delayDamageVfx;
        public bool SpawnItemOnBreak;
        [ShowInInspectorIf("SpawnItemOnBreak", false)]
        [PickupIdentifier]
        public int ItemIdToSpawnOnBreak = -1;
        public bool HandlePathBlocking;
        private OccupiedCells m_occupiedCells;
        public System.Action OnBreak;
        public Action<float> OnDamaged;
        [NonSerialized]
        public bool InvulnerableToEnemyBullets;
        [NonSerialized]
        public bool TemporarilyInvulnerable;
        private bool m_inZeroHPState;
        private bool m_isBroken;
        private int m_numHits;
        private float m_damageVfxTimer;

        public bool ReportZeroDamage { get; set; }

        public bool IsDestroyed => this.m_isBroken;

        public int NumHits => this.m_numHits;

        public float MinHitPointsFromNonExplosions { get; set; }

        public float MaxHitPoints { get; set; }

        public Vector2 CenterPoint
        {
            get
            {
                if ((bool) (UnityEngine.Object) this.specRigidbody)
                    return this.specRigidbody.GetUnitCenter(ColliderType.HitBox);
                return (bool) (UnityEngine.Object) this.sprite ? this.sprite.WorldCenter : this.transform.position.XY();
            }
        }

        public void Awake() => StaticReferenceManager.AllMajorBreakables.Add(this);

        public void Start()
        {
            if (this.HandlePathBlocking)
                this.m_occupiedCells = new OccupiedCells(this.specRigidbody, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY()));
            if ((double) this.MaxHitPoints <= 0.0)
                this.MaxHitPoints = this.HitPoints;
            if (this.ScaleWithEnemyHealth)
            {
                float levelHealthModifier = AIActor.BaseLevelHealthModifier;
                this.HitPoints *= levelHealthModifier;
                this.MaxHitPoints *= levelHealthModifier;
            }
            if (!this.GameActorMotionBreaks && !this.PlayerRollingBreaks)
                return;
            this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
        }

        public void Update() => this.m_damageVfxTimer += BraveTime.DeltaTime;

        public float GetCurrentHealthPercentage() => this.HitPoints / this.MaxHitPoints;

        protected override void OnDestroy()
        {
            StaticReferenceManager.AllMajorBreakables.Remove(this);
            base.OnDestroy();
        }

        private void OnPreCollision(
            SpeculativeRigidbody myRigidbody,
            PixelCollider myCollider,
            SpeculativeRigidbody otherRigidbody,
            PixelCollider otherCollider)
        {
            if (!this.enabled)
                return;
            if (this.m_isBroken)
            {
                PhysicsEngine.SkipCollision = true;
            }
            else
            {
                if (!(bool) (UnityEngine.Object) otherRigidbody.gameActor)
                    return;
                if (this.GameActorMotionBreaks)
                {
                    this.Break(otherRigidbody.Velocity);
                    PhysicsEngine.SkipCollision = true;
                }
                else
                {
                    if (!this.PlayerRollingBreaks || !(otherRigidbody.gameActor is PlayerController) || !(otherRigidbody.gameActor as PlayerController).IsDodgeRolling)
                        return;
                    this.Break(otherRigidbody.Velocity);
                    PhysicsEngine.SkipCollision = true;
                }
            }
        }

        public void ApplyDamage(
            float damage,
            Vector2 sourceDirection,
            bool isSourceEnemy,
            bool isExplosion = false,
            bool ForceDamageOverride = false)
        {
            if (this.IsDestroyed || this.TemporarilyInvulnerable || !ForceDamageOverride && (this.OnlyExplosions || this.IsSecretDoor && (double) this.HitPoints <= 1.0) && !isExplosion || !this.enabled)
                return;
            if (this.EnemyDamageOverride > 0 && isSourceEnemy)
                damage = (float) this.EnemyDamageOverride;
            float b = Mathf.Max(0.0f, damage - this.DamageReduction);
            if (this.IsSecretDoor && !ForceDamageOverride && (double) this.HitPoints - (double) b < 1.0)
                b = Mathf.Min(this.HitPoints - 1f, b);
            if ((double) this.MinHitPointsFromNonExplosions > 0.0 && !isExplosion)
                b = Mathf.Min(this.HitPoints - this.MinHitPointsFromNonExplosions, b);
            if (ForceDamageOverride)
                b = damage;
            if ((double) b <= 0.0)
            {
                if (!this.ReportZeroDamage || this.OnDamaged == null)
                    return;
                this.OnDamaged(b);
            }
            else
            {
                this.HitPoints -= b;
                ++this.m_numHits;
                if (this.OnDamaged != null)
                    this.OnDamaged(b);
                if ((double) this.m_damageVfxTimer > (double) this.damageVfxMinTimeBetween)
                {
                    if (this.damageVfx != null)
                    {
                        VFXPool damageVfx = this.damageVfx;
                        Vector3 centerPoint = (Vector3) this.CenterPoint;
                        Vector2? nullable = new Vector2?(-sourceDirection);
                        Vector3 position = centerPoint;
                        Vector2? sourceNormal = new Vector2?();
                        Vector2? sourceVelocity = nullable;
                        float? heightOffGround = new float?();
                        damageVfx.SpawnAtPosition(position, sourceNormal: sourceNormal, sourceVelocity: sourceVelocity, heightOffGround: heightOffGround);
                    }
                    this.m_damageVfxTimer = 0.0f;
                }
                if ((double) this.HitPoints <= 0.0 && this.m_numHits >= this.MinHits)
                {
                    if (this.usesTemporaryZeroHitPointsState && !this.m_inZeroHPState)
                    {
                        this.m_inZeroHPState = true;
                        string spriteName = string.IsNullOrEmpty(this.overrideSpriteNameToUseAtZeroHP) ? this.spriteNameToUseAtZeroHP : this.overrideSpriteNameToUseAtZeroHP;
                        if (string.IsNullOrEmpty(spriteName))
                            return;
                        this.sprite.SetSprite(spriteName);
                    }
                    else
                        this.Break(sourceDirection);
                }
                else
                {
                    if (this.handlesOwnPrebreakFrames)
                    {
                        for (int index = this.prebreakFrames.Length - 1; index >= 0; --index)
                        {
                            if ((double) this.GetCurrentHealthPercentage() <= (double) this.prebreakFrames[index].healthPercentage / 100.0)
                            {
                                this.sprite.SetSprite(this.prebreakFrames[index].sprite);
                                return;
                            }
                        }
                    }
                    if (!this.playsAnimationOnNotBroken)
                        return;
                    this.spriteAnimator.Play(this.notBreakAnimation);
                }
            }
        }

        public void Break(Vector2 sourceDirection)
        {
            if (this.m_isBroken)
                return;
            this.m_isBroken = true;
            this.TriggerPersistentVFXClear();
            if (this.OnBreak != null)
                this.OnBreak();
            if (this.spawnShards)
            {
                switch (this.shardBreakStyle)
                {
                    case MinorBreakable.BreakStyle.CONE:
                        this.SpawnShards(sourceDirection, -45f, 45f, 0.5f, sourceDirection.magnitude * this.minShardPercentSpeed, sourceDirection.magnitude * this.maxShardPercentSpeed);
                        break;
                    case MinorBreakable.BreakStyle.BURST:
                        this.SpawnShards(sourceDirection, -180f, 180f, 0.5f, sourceDirection.magnitude * this.minShardPercentSpeed, sourceDirection.magnitude * this.maxShardPercentSpeed);
                        break;
                    case MinorBreakable.BreakStyle.JET:
                        this.SpawnShards(sourceDirection, -15f, 15f, 0.5f, sourceDirection.magnitude * this.minShardPercentSpeed, sourceDirection.magnitude * this.maxShardPercentSpeed);
                        break;
                    case MinorBreakable.BreakStyle.WALL_DOWNWARD_BURST:
                        this.SpawnShards(Vector2.down, -45f, 45f, 0.5f, sourceDirection.magnitude * this.minShardPercentSpeed, sourceDirection.magnitude * this.maxShardPercentSpeed);
                        break;
                }
            }
            if (this.childrenToDestroy != null)
            {
                for (int index = 0; index < this.childrenToDestroy.Count; ++index)
                    UnityEngine.Object.Destroy((UnityEngine.Object) this.childrenToDestroy[index]);
            }
            if (this.breakVfx != null && !this.delayDamageVfx)
            {
                if ((bool) (UnityEngine.Object) this.breakVfxParent)
                    this.breakVfx.SpawnAtLocalPosition(Vector3.zero, 0.0f, this.breakVfxParent.transform);
                else
                    this.breakVfx.SpawnAtPosition((Vector3) this.CenterPoint);
            }
            if (this.HandlePathBlocking)
                this.m_occupiedCells.Clear();
            if (this.SpawnItemOnBreak)
                LootEngine.SpawnItem(PickupObjectDatabase.GetById(this.ItemIdToSpawnOnBreak).gameObject, (Vector3) this.sprite.WorldCenter, Vector2.zero, 1f, doDefaultItemPoof: true);
            if (this.destroyedOnBreak)
            {
                if (this.handlesOwnBreakAnimation)
                {
                    if (this.breakVfx != null && this.breakVfx.type != VFXPoolType.None)
                        this.spriteAnimator.PlayAndDestroyObject(this.breakAnimation, (System.Action) (() =>
                        {
                            if ((bool) (UnityEngine.Object) this.breakVfxParent)
                                this.breakVfx.SpawnAtLocalPosition(Vector3.zero, 0.0f, this.breakVfxParent.transform);
                            else
                                this.breakVfx.SpawnAtPosition((Vector3) this.CenterPoint);
                        }));
                    else
                        this.spriteAnimator.PlayAndDestroyObject(this.breakAnimation);
                }
                else
                    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
            }
            else
            {
                if (!this.handlesOwnBreakAnimation)
                    return;
                this.spriteAnimator.Play(this.breakAnimation);
                this.specRigidbody.enabled = false;
            }
        }

        public void SpawnShards(
            Vector2 direction,
            float minAngle,
            float maxAngle,
            float verticalSpeed,
            float minMagnitude,
            float maxMagnitude)
        {
            if (GameManager.Options.DebrisQuantity == GameOptions.GenericHighMedLowOption.VERY_LOW)
                return;
            Vector3 vector3 = this.sprite.GetBounds().extents + this.transform.position;
            if (this.shardClusters == null || this.shardClusters.Length <= 0)
                return;
            int iterator = UnityEngine.Random.Range(0, 10);
            Bounds bounds = this.sprite.GetBounds();
            for (int index1 = 0; index1 < this.shardClusters.Length; ++index1)
            {
                float discrepancyRandom = BraveMathCollege.GetLowDiscrepancyRandom(iterator);
                ++iterator;
                float z = Mathf.Lerp(minAngle, maxAngle, discrepancyRandom);
                ShardCluster shardCluster = this.shardClusters[index1];
                int num1 = UnityEngine.Random.Range(shardCluster.minFromCluster, shardCluster.maxFromCluster + 1);
                int num2 = UnityEngine.Random.Range(0, shardCluster.clusterObjects.Length);
                for (int index2 = 0; index2 < num1; ++index2)
                {
                    Vector3 position = vector3;
                    if (this.distributeShards)
                        position = this.sprite.transform.position + new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x), UnityEngine.Random.Range(bounds.min.y, bounds.max.y), UnityEngine.Random.Range(bounds.min.z, bounds.max.z));
                    Vector3 startingForce = Quaternion.Euler(0.0f, 0.0f, z) * (direction.normalized * UnityEngine.Random.Range(minMagnitude, maxMagnitude)).ToVector3ZUp(verticalSpeed);
                    if (this.shardBreakStyle == MinorBreakable.BreakStyle.BURST)
                        startingForce = ((position - vector3).normalized * UnityEngine.Random.Range(minMagnitude, maxMagnitude)).WithZ(verticalSpeed);
                    int index3 = (num2 + index2) % shardCluster.clusterObjects.Length;
                    GameObject gameObject = SpawnManager.SpawnDebris(shardCluster.clusterObjects[index3].gameObject, position, Quaternion.identity);
                    tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
                    if ((UnityEngine.Object) this.sprite.attachParent != (UnityEngine.Object) null && (UnityEngine.Object) component != (UnityEngine.Object) null)
                    {
                        component.attachParent = this.sprite.attachParent;
                        component.HeightOffGround = this.sprite.HeightOffGround;
                    }
                    gameObject.GetComponent<DebrisObject>().Trigger(startingForce, 1f);
                }
            }
        }
    }

