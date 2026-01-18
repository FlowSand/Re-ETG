using System;
using System.Collections;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

public class ComplexProjectileModifier : PassiveItem
    {
        public float ActivationChance = 1f;
        public bool NormalizeAcrossFireRate;
        [ShowInInspectorIf("NormalizeAcrossFireRate", false)]
        public float ActivationsPerSecond = 1f;
        [ShowInInspectorIf("NormalizeAcrossFireRate", false)]
        public float MinActivationChance = 0.05f;
        public bool UsesAlternateActivationChanceInBossRooms;
        [ShowInInspectorIf("UsesAlternateActivationChanceInBossRooms", false)]
        public float BossActivationsPerSecond = 1f;
        public bool AddsChainLightning;
        [ShowInInspectorIf("AddsChainLightning", false)]
        public GameObject ChainLightningVFX;
        [ShowInInspectorIf("AddsChainLightning", false)]
        public CoreDamageTypes ChainLightningDamageTypes;
        [ShowInInspectorIf("AddsChainLightning", false)]
        public float ChainLightningMaxLinkDistance = 15f;
        [ShowInInspectorIf("AddsChainLightning", false)]
        public float ChainLightningDamagePerHit = 6f;
        [ShowInInspectorIf("AddsChainLightning", false)]
        public float ChainLightningDamageCooldown = 1f;
        [ShowInInspectorIf("AddsChainLightning", false)]
        public GameObject ChainLightningDispersalParticles;
        [ShowInInspectorIf("AddsChainLightning", false)]
        public float ChainLightningDispersalDensity;
        [ShowInInspectorIf("AddsChainLightning", false)]
        public float ChainLightningDispersalMinCoherence;
        [ShowInInspectorIf("AddsChainLightning", false)]
        public float ChainLightningDispersalMaxCoherence;
        public bool AddsExplosino;
        [ShowInInspectorIf("AddsExplosino", false)]
        public ExplosionData ExplosionData;
        public bool UsesChanceForAdditionalProjectile;
        [Header("Adds Spawned Projectiles")]
        public bool AddsSpawnProjectileModifier;
        [ShowInInspectorIf("AddsSpawnProjectileModifier", false)]
        public bool SpawnProjectileInheritsApperance;
        [ShowInInspectorIf("AddsSpawnProjectileModifier", false)]
        public float SpawnProjectileScaleModifier = 1f;
        [ShowInInspectorIf("AddsSpawnProjectileModifier", false)]
        public int NumberToSpawnOnCollision = 3;
        [ShowInInspectorIf("AddsSpawnProjectileModifier", false)]
        public Projectile CollisionSpawnProjectile;
        [ShowInInspectorIf("AddsSpawnProjectileModifier", false)]
        public bool ScaleSpawnsByFireRate;
        [ShowInInspectorIf("ScaleSpawnsByFireRate", false)]
        public int MinFlakSpawns = 2;
        [ShowInInspectorIf("ScaleSpawnsByFireRate", false)]
        public int MaxFlakSpawns = 8;
        [ShowInInspectorIf("ScaleSpawnsByFireRate", false)]
        public float MinFlakFireRate = 0.25f;
        [ShowInInspectorIf("ScaleSpawnsByFireRate", false)]
        public float MaxFlakFireRate = 2f;
        [Header("Adds Chance To Blank")]
        public bool AddsChanceToBlank;
        [ShowInInspectorIf("AddsChanceToBlank", false)]
        public float BlankRadius = 5f;
        [Header("Adds Trailed Spawns")]
        public bool AddsTrailedSpawn;
        [ShowInInspectorIf("AddsTrailedSpawn", false)]
        public GameObject TrailedObjectToSpawn;
        [ShowInInspectorIf("AddsTrailedSpawn", false)]
        public float TrailedObjectSpawnDistance = 1f;
        [Header("Critical")]
        public bool AddsCriticalChance;
        [ShowInInspectorIf("AddsCriticalChance", false)]
        public Projectile CriticalProjectile;
        [Header("Devolver")]
        [Space(20f)]
        public bool AddsDevolverModifier;
        [ShowInInspectorIf("AddsDevolverModifier", false)]
        public DevolverModifier DevolverSourceModifier;
        [Header("Hungry Bullets")]
        public bool AddsHungryBullets;
        [ShowInInspectorIf("AddsHungryBullets", false)]
        public float HungryRadius = 1.5f;
        [ShowInInspectorIf("AddsHungryBullets", false)]
        public float DamagePercentGainPerSnack = 0.25f;
        [ShowInInspectorIf("AddsHungryBullets", false)]
        public float HungryMaxMultiplier = 3f;
        [ShowInInspectorIf("AddsHungryBullets", false)]
        public int MaximumBulletsEaten = 10;
        [Header("Katana Bullets")]
        public bool AddsLinearChainExplosionOnKill;
        [ShowInInspectorIf("AddsLinearChainExplosionOnKill", false)]
        public float LCEChainDuration = 1f;
        [ShowInInspectorIf("AddsLinearChainExplosionOnKill", false)]
        public float LCEChainDistance = 10f;
        [ShowInInspectorIf("AddsLinearChainExplosionOnKill", false)]
        public int LCEChainNumExplosions = 5;
        [ShowInInspectorIf("AddsLinearChainExplosionOnKill", false)]
        public GameObject LCEChainTargetSprite;
        [ShowInInspectorIf("AddsLinearChainExplosionOnKill", false)]
        public ExplosionData LinearChainExplosionData;
        private PlayerController m_player;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            this.m_player = player;
            base.Pickup(player);
            player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
            player.PostProcessBeam += new Action<BeamController>(this.PostProcessBeam);
            player.PostProcessBeamChanceTick += new Action<BeamController>(this.PostProcessBeamChanceTick);
            if (!this.AddsCriticalChance)
                return;
            player.OnPreFireProjectileModifier += new Func<Gun, Projectile, Projectile>(this.HandlePreFireProjectileModifier);
        }

        private void PostProcessBeam(BeamController obj)
        {
            if (!this.AddsLinearChainExplosionOnKill || !(bool) (UnityEngine.Object) obj || !(bool) (UnityEngine.Object) obj.projectile)
                return;
            obj.projectile.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleLinearChainBeamHitEnemy);
        }

        [DebuggerHidden]
        private IEnumerator HandleChainExplosion(
            SpeculativeRigidbody enemySRB,
            Vector2 startPosition,
            Vector2 direction)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ComplexProjectileModifier__HandleChainExplosionc__Iterator0()
            {
                startPosition = startPosition,
                direction = direction,
                enemySRB = enemySRB,
                _this = this
            };
        }

        private bool ValidExplosionPosition(Vector2 pos)
        {
            IntVector2 intVector2 = pos.ToIntVector2(VectorConversions.Floor);
            return GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2) && GameManager.Instance.Dungeon.data[intVector2].type != CellType.WALL;
        }

        private Projectile HandlePreFireProjectileModifier(Gun sourceGun, Projectile sourceProjectile)
        {
            if (this.AddsCriticalChance)
            {
                float num1 = this.ActivationChance;
                if (this.NormalizeAcrossFireRate && (bool) (UnityEngine.Object) sourceGun)
                {
                    float num2 = 1f / sourceGun.DefaultModule.cooldownTime;
                    if ((UnityEngine.Object) sourceGun.Volley != (UnityEngine.Object) null && sourceGun.Volley.UsesShotgunStyleVelocityRandomizer)
                        num2 *= (float) Mathf.Max(1, sourceGun.Volley.projectiles.Count);
                    num1 = Mathf.Max(this.MinActivationChance, Mathf.Clamp01(this.ActivationsPerSecond / num2));
                }
                if ((bool) (UnityEngine.Object) this.Owner && this.Owner.HasActiveBonusSynergy(CustomSynergyType.VORPAL_BLADE))
                    num1 *= 0.25f;
                if ((double) UnityEngine.Random.value < (double) num1)
                    return this.CriticalProjectile;
            }
            return sourceProjectile;
        }

        private void PostProcessProjectile(Projectile obj, float effectChanceScalar)
        {
            float num1 = this.ActivationChance;
            Gun currentGun = !(bool) (UnityEngine.Object) this.m_player ? (Gun) null : this.m_player.CurrentGun;
            if (this.NormalizeAcrossFireRate && (bool) (UnityEngine.Object) currentGun)
            {
                float num2 = 1f / currentGun.DefaultModule.cooldownTime;
                if (this.AddsChanceToBlank && (UnityEngine.Object) currentGun.Volley != (UnityEngine.Object) null && currentGun.Volley.UsesShotgunStyleVelocityRandomizer)
                    num2 *= (float) currentGun.Volley.projectiles.Count;
                float b = Mathf.Clamp01(this.ActivationsPerSecond / num2);
                if (this.UsesAlternateActivationChanceInBossRooms && (bool) (UnityEngine.Object) this.m_player && this.m_player.CurrentRoom != null && this.m_player.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS)
                    b = Mathf.Clamp01(this.BossActivationsPerSecond / num2);
                num1 = Mathf.Max(this.MinActivationChance, b);
            }
            if (this.UsesChanceForAdditionalProjectile && (bool) (UnityEngine.Object) this.m_player && this.m_player.HasActiveBonusSynergy(CustomSynergyType.SHADOW_BACKUP) && (double) Vector2.Dot((Vector2) obj.transform.right, this.m_player.unadjustedAimPoint.XY() - this.m_player.CenterPosition) < -0.75)
                num1 = 1f;
            if ((double) UnityEngine.Random.value >= (double) num1)
                return;
            if (this.AddsChainLightning)
            {
                ChainLightningModifier orAddComponent = obj.gameObject.GetOrAddComponent<ChainLightningModifier>();
                orAddComponent.LinkVFXPrefab = this.ChainLightningVFX;
                orAddComponent.damageTypes = this.ChainLightningDamageTypes;
                orAddComponent.maximumLinkDistance = this.ChainLightningMaxLinkDistance;
                orAddComponent.damagePerHit = this.ChainLightningDamagePerHit;
                orAddComponent.damageCooldown = this.ChainLightningDamageCooldown;
                if ((bool) (UnityEngine.Object) this.m_player && this.m_player.HasActiveBonusSynergy(CustomSynergyType.TESLA_UNBOUND))
                {
                    orAddComponent.maximumLinkDistance *= 3f;
                    orAddComponent.CanChainToAnyProjectile = true;
                }
                if ((UnityEngine.Object) this.ChainLightningDispersalParticles != (UnityEngine.Object) null)
                {
                    orAddComponent.UsesDispersalParticles = true;
                    orAddComponent.DispersalParticleSystemPrefab = this.ChainLightningDispersalParticles;
                    orAddComponent.DispersalDensity = this.ChainLightningDispersalDensity;
                    orAddComponent.DispersalMinCoherency = this.ChainLightningDispersalMinCoherence;
                    orAddComponent.DispersalMaxCoherency = this.ChainLightningDispersalMaxCoherence;
                }
                else
                    orAddComponent.UsesDispersalParticles = false;
            }
            if (this.AddsExplosino && !(bool) (UnityEngine.Object) obj.gameObject.GetComponent<ExplosiveModifier>())
            {
                ExplosiveModifier explosiveModifier = obj.gameObject.AddComponent<ExplosiveModifier>();
                explosiveModifier.doExplosion = true;
                explosiveModifier.explosionData = this.ExplosionData;
            }
            if (this.UsesChanceForAdditionalProjectile)
                this.Owner.SpawnShadowBullet(obj, true);
            if (this.AddsSpawnProjectileModifier && !(bool) (UnityEngine.Object) obj.gameObject.GetComponent<SpawnProjModifier>())
            {
                SpawnProjModifier spawnProjModifier = obj.gameObject.AddComponent<SpawnProjModifier>();
                spawnProjModifier.SpawnedProjectilesInheritAppearance = this.SpawnProjectileInheritsApperance;
                spawnProjModifier.SpawnedProjectileScaleModifier = this.SpawnProjectileScaleModifier;
                spawnProjModifier.SpawnedProjectilesInheritData = true;
                spawnProjModifier.spawnProjectilesOnCollision = true;
                spawnProjModifier.spawnProjecitlesOnDieInAir = true;
                spawnProjModifier.doOverrideObjectCollisionSpawnStyle = true;
                spawnProjModifier.startAngle = UnityEngine.Random.Range(0, 180);
                int num3 = this.NumberToSpawnOnCollision;
                if (this.ScaleSpawnsByFireRate && (bool) (UnityEngine.Object) currentGun)
                {
                    float num4 = 1f / currentGun.DefaultModule.cooldownTime;
                    if (currentGun.Volley.projectiles.Count > 2)
                    {
                        int num5 = 0;
                        for (int index = 0; index < currentGun.Volley.projectiles.Count; ++index)
                        {
                            if (currentGun.Volley.projectiles[index] != null && currentGun.Volley.projectiles[index].mirror)
                                num5 += 2;
                            else
                                ++num5;
                        }
                        num4 = Mathf.Lerp(this.MinFlakFireRate, this.MaxFlakFireRate, (float) num5 / 5f);
                    }
                    num3 = Mathf.RoundToInt(Mathf.Lerp((float) this.MinFlakSpawns, (float) this.MaxFlakSpawns * 1f, Mathf.InverseLerp(this.MaxFlakFireRate, this.MinFlakFireRate, num4)));
                }
                if (obj.SpawnedFromOtherPlayerProjectile)
                    num3 = 2;
                spawnProjModifier.numberToSpawnOnCollison = num3;
                spawnProjModifier.projectileToSpawnOnCollision = this.CollisionSpawnProjectile;
                spawnProjModifier.collisionSpawnStyle = SpawnProjModifier.CollisionSpawnStyle.FLAK_BURST;
            }
            else if (this.AddsSpawnProjectileModifier)
                obj.gameObject.GetComponent<SpawnProjModifier>().PostprocessSpawnedProjectiles = true;
            if (this.AddsTrailedSpawn)
                obj.StartCoroutine(this.HandleTrailedSpawn(obj));
            if (this.AddsDevolverModifier && !(bool) (UnityEngine.Object) obj.gameObject.GetComponent<DevolverModifier>())
            {
                DevolverModifier devolverModifier = obj.gameObject.AddComponent<DevolverModifier>();
                devolverModifier.chanceToDevolve = this.DevolverSourceModifier.chanceToDevolve;
                devolverModifier.DevolverHierarchy = this.DevolverSourceModifier.DevolverHierarchy;
                devolverModifier.EnemyGuidsToIgnore = this.DevolverSourceModifier.EnemyGuidsToIgnore;
            }
            if (this.AddsHungryBullets && !(bool) (UnityEngine.Object) obj.gameObject.GetComponent<HungryProjectileModifier>())
            {
                HungryProjectileModifier projectileModifier = obj.gameObject.AddComponent<HungryProjectileModifier>();
                projectileModifier.HungryRadius = this.HungryRadius;
                projectileModifier.DamagePercentGainPerSnack = this.DamagePercentGainPerSnack;
                projectileModifier.MaxMultiplier = this.HungryMaxMultiplier;
                projectileModifier.MaximumBulletsEaten = this.MaximumBulletsEaten;
            }
            if (this.AddsLinearChainExplosionOnKill)
                obj.OnWillKillEnemy += new Action<Projectile, SpeculativeRigidbody>(this.HandleWillKillEnemy);
            if (!(bool) (UnityEngine.Object) this.m_player || !this.AddsChanceToBlank)
                return;
            obj.OnDestruction += new Action<Projectile>(this.HandleBlankOnDestruction);
        }

        private void HandleLinearChainBeamHitEnemy(
            Projectile sourceProjectile,
            SpeculativeRigidbody enemy,
            bool fatal)
        {
            if (!this.AddsLinearChainExplosionOnKill || !(bool) (UnityEngine.Object) enemy || !fatal)
                return;
            Vector2 vector2_1 = !(bool) (UnityEngine.Object) enemy.aiActor ? enemy.transform.position.XY() : enemy.aiActor.CenterPosition;
            UnityEngine.Debug.LogError((object) vector2_1);
            Vector2 vector2_2 = !(bool) (UnityEngine.Object) sourceProjectile ? (!(bool) (UnityEngine.Object) enemy.healthHaver ? BraveMathCollege.DegreesToVector(this.Owner.FacingDirection) : enemy.healthHaver.lastIncurredDamageDirection) : sourceProjectile.LastVelocity.normalized;
            if ((bool) (UnityEngine.Object) sourceProjectile)
            {
                BasicBeamController component = sourceProjectile.GetComponent<BasicBeamController>();
                if ((bool) (UnityEngine.Object) component)
                    vector2_2 = component.Direction.normalized;
            }
            if ((double) vector2_2.magnitude < 0.05000000074505806)
                vector2_2 = UnityEngine.Random.insideUnitCircle.normalized;
            GameManager.Instance.Dungeon.StartCoroutine(this.HandleChainExplosion(enemy, vector2_1, vector2_2.normalized));
        }

        private void HandleWillKillEnemy(Projectile sourceProjectile, SpeculativeRigidbody enemy)
        {
            if (!this.AddsLinearChainExplosionOnKill || !(bool) (UnityEngine.Object) enemy)
                return;
            Vector2 vector2_1 = !(bool) (UnityEngine.Object) enemy.aiActor ? enemy.transform.position.XY() : enemy.aiActor.CenterPosition;
            UnityEngine.Debug.LogError((object) vector2_1);
            Vector2 vector2_2 = !(bool) (UnityEngine.Object) sourceProjectile ? (!(bool) (UnityEngine.Object) enemy.healthHaver ? BraveMathCollege.DegreesToVector(this.Owner.FacingDirection) : enemy.healthHaver.lastIncurredDamageDirection) : sourceProjectile.LastVelocity.normalized;
            if ((double) vector2_2.magnitude < 0.05000000074505806)
                vector2_2 = UnityEngine.Random.insideUnitCircle.normalized;
            GameManager.Instance.Dungeon.StartCoroutine(this.HandleChainExplosion(enemy, vector2_1, vector2_2.normalized));
        }

        private void DoTrailedSpawns(
            Projectile p,
            ref Vector2 lastSpawnedPosition,
            ref float lastElapsedDistance)
        {
            float magnitude = (p.transform.position.XY() - lastSpawnedPosition).magnitude;
            if ((double) magnitude <= (double) this.TrailedObjectSpawnDistance)
                return;
            Vector2 vector2_1 = p.transform.position.XY() - lastSpawnedPosition;
            while ((double) magnitude > (double) this.TrailedObjectSpawnDistance)
            {
                magnitude -= this.TrailedObjectSpawnDistance;
                lastSpawnedPosition += vector2_1.normalized * this.TrailedObjectSpawnDistance;
                Vector2 vector2_2 = new Vector2(-0.5f, -1f) + UnityEngine.Random.insideUnitCircle * 0.25f;
                SpawnManager.SpawnVFX(this.TrailedObjectToSpawn, (Vector3) (lastSpawnedPosition + vector2_2), Quaternion.identity);
                Exploder.DoRadialDamage(5f, (Vector3) (lastSpawnedPosition + vector2_2), 0.5f, false, true);
            }
            lastElapsedDistance = p.GetElapsedDistance();
        }

        [DebuggerHidden]
        private IEnumerator HandleTrailedSpawn(Projectile p)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ComplexProjectileModifier__HandleTrailedSpawnc__Iterator1()
            {
                p = p,
                _this = this
            };
        }

        private void HandleBlankOnDestruction(Projectile obj)
        {
            if (!(bool) (UnityEngine.Object) this.m_player || !(bool) (UnityEngine.Object) obj)
                return;
            this.DoMicroBlank(!(bool) (UnityEngine.Object) obj.specRigidbody ? obj.transform.position.XY() : obj.specRigidbody.UnitCenter);
        }

        private void DoMicroBlank(Vector2 center)
        {
            GameObject silencerVFX = (GameObject) ResourceCache.Acquire("Global VFX/BlankVFX_Ghost");
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", this.gameObject);
            SilencerInstance silencerInstance = new GameObject("silencer").AddComponent<SilencerInstance>();
            float additionalTimeAtMaxRadius = 0.25f;
            silencerInstance.TriggerSilencer(center, 20f, this.BlankRadius, silencerVFX, 0.0f, 3f, 3f, 3f, 30f, 3f, additionalTimeAtMaxRadius, this.m_player);
        }

        private void PostProcessBeamChanceTick(BeamController beamController)
        {
            if ((double) UnityEngine.Random.value >= (double) this.ActivationChance)
                return;
            beamController.ChanceBasedShadowBullet = true;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            this.m_player = (PlayerController) null;
            debrisObject.GetComponent<ComplexProjectileModifier>().m_pickedUpThisRun = true;
            player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
            player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
            player.PostProcessBeamChanceTick -= new Action<BeamController>(this.PostProcessBeamChanceTick);
            player.OnPreFireProjectileModifier -= new Func<Gun, Projectile, Projectile>(this.HandlePreFireProjectileModifier);
            return debrisObject;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!(bool) (UnityEngine.Object) this.m_player)
                return;
            this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
            this.m_player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
            this.m_player.PostProcessBeamChanceTick -= new Action<BeamController>(this.PostProcessBeamChanceTick);
            this.m_player.OnPreFireProjectileModifier -= new Func<Gun, Projectile, Projectile>(this.HandlePreFireProjectileModifier);
        }
    }

