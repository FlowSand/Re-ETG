using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Pathfinding;
using UnityEngine;

using Brave.BulletScript;
using Dungeonator;

#nullable disable

public class CircleBurstChallengeModifier : ChallengeModifier
    {
        public BulletScriptSelector BulletScript;
        public GameObject tellVFX;
        public GameObject ChainLightningVFX;
        public float NearRadius = 5f;
        public float FarRadius = 9f;
        public float StartDelay = 3f;
        public float TimeBetweenWaves = 10f;
        private RoomHandler m_room;
        [NonSerialized]
        public bool Preprocessed;
        private float m_waveTimer = 5f;
        private List<ChainLightningModifier> m_clms = new List<ChainLightningModifier>();
        private ChainLightningModifier m_firstCLM;
        private ChainLightningModifier m_lastCLM;

        public override bool IsValid(RoomHandler room)
        {
            return room.Cells.Count >= 150 && !room.area.IsProceduralRoom && base.IsValid(room);
        }

        [DebuggerHidden]
        private IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new CircleBurstChallengeModifier__Startc__Iterator0()
            {
                _this = this
            };
        }

        public static IntVector2? GetAppropriateSpawnPointForChallengeBurst(
            RoomHandler room,
            float tooCloseRadius,
            float tooFarRadius)
        {
            CellValidator cellValidator = (CellValidator) (c =>
            {
                for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                {
                    float num = Vector2.Distance(c.ToCenterVector2(), GameManager.Instance.AllPlayers[index].CenterPosition);
                    if ((double) num < (double) tooCloseRadius || (double) num > (double) tooFarRadius)
                        return false;
                }
                return true;
            });
            return room.GetRandomAvailableCell(new IntVector2?(IntVector2.One), new CellTypes?(CellTypes.FLOOR | CellTypes.PIT), true, cellValidator);
        }

        private void Update()
        {
            this.m_waveTimer -= BraveTime.DeltaTime;
            if ((double) this.m_waveTimer > 0.0)
                return;
            this.Cleanup();
            IntVector2? forChallengeBurst = CircleBurstChallengeModifier.GetAppropriateSpawnPointForChallengeBurst(this.m_room, this.NearRadius, this.FarRadius);
            if (!forChallengeBurst.HasValue)
                return;
            this.m_waveTimer = this.TimeBetweenWaves;
            this.SpawnBulletScript((AIActor) null, this.BulletScript, this.GetComponent<AIBulletBank>(), forChallengeBurst.Value.ToCenterVector2(), StringTableManager.GetEnemiesString("#TRAP"));
        }

        private void OnDestroy() => this.Cleanup();

        private void Cleanup()
        {
            for (int index = this.m_clms.Count - 1; index >= 0; --index)
            {
                ChainLightningModifier clm = this.m_clms[index];
                if ((bool) (UnityEngine.Object) clm)
                {
                    clm.ForcedLinkProjectile = (Projectile) null;
                    if ((bool) (UnityEngine.Object) clm.projectile)
                        clm.projectile.ForceDestruction();
                }
            }
            this.m_clms.Clear();
        }

        private void SpawnBulletScript(
            AIActor aiActor,
            BulletScriptSelector bulletScript,
            AIBulletBank bank,
            Vector2 position,
            string ownerName)
        {
            this.StartCoroutine(this.HandleSpawnBulletScript(aiActor, bulletScript, bank, position, ownerName));
        }

        [DebuggerHidden]
        private IEnumerator HandleSpawnBulletScript(
            AIActor aiActor,
            BulletScriptSelector bulletScript,
            AIBulletBank bank,
            Vector2 position,
            string ownerName)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new CircleBurstChallengeModifier__HandleSpawnBulletScriptc__Iterator1()
            {
                position = position,
                aiActor = aiActor,
                bank = bank,
                bulletScript = bulletScript,
                ownerName = ownerName,
                _this = this
            };
        }

        private void OnBulletCreated(Bullet b, Projectile p)
        {
            ChainLightningModifier orAddComponent = p.gameObject.GetOrAddComponent<ChainLightningModifier>();
            orAddComponent.DamagesPlayers = true;
            orAddComponent.DamagesEnemies = false;
            orAddComponent.RequiresSameProjectileClass = true;
            orAddComponent.LinkVFXPrefab = this.ChainLightningVFX;
            orAddComponent.damageTypes = CoreDamageTypes.Electric;
            orAddComponent.maximumLinkDistance = 100f;
            orAddComponent.damagePerHit = 0.5f;
            orAddComponent.damageCooldown = 1f;
            orAddComponent.UsesDispersalParticles = false;
            orAddComponent.UseForcedLinkProjectile = true;
            if ((UnityEngine.Object) this.m_lastCLM != (UnityEngine.Object) null)
            {
                orAddComponent.ForcedLinkProjectile = this.m_lastCLM.projectile;
                this.m_lastCLM.BackLinkProjectile = orAddComponent.projectile;
            }
            if ((UnityEngine.Object) this.m_firstCLM == (UnityEngine.Object) null)
                this.m_firstCLM = orAddComponent;
            this.m_lastCLM = orAddComponent;
            this.m_clms.Add(orAddComponent);
        }
    }

