using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class BroController : BraveBehaviour
    {
        public string enrageAnim;
        public float enrageAnimTime = 1f;
        public Color enrageColor;
        public GameObject enrageVfx;
        public Transform enrageVfxTransform;
        public GameObject overheadVfx;
        public float postEnrageMoveSpeed = -1f;
        public float enrageHealToPercent = 0.5f;
        private bool m_shouldEnrage;
        private float m_cachedSpawnProbability = 0.1f;
        private bool m_isEnraged;
        private GameObject m_overheadVfxInstance;
        private float m_overheadVfxTimer;
        private float m_particleCounter;
        private tk2dBaseSprite m_enrageVfx;

        public static void ClearPerLevelData() => StaticReferenceManager.AllBros.Clear();

        public static BroController GetOtherBro(AIActor me) => BroController.GetOtherBro(me.gameObject);

        public static BroController GetOtherBro(GameObject me)
        {
            BroController broController = (BroController) null;
            bool flag = false;
            List<BroController> allBros = StaticReferenceManager.AllBros;
            for (int index = 0; index < allBros.Count; ++index)
            {
                if ((bool) (Object) allBros[index])
                {
                    if ((Object) me == (Object) allBros[index].gameObject)
                        flag = true;
                    else
                        broController = allBros[index];
                }
            }
            if (!flag)
                UnityEngine.Debug.LogWarning((object) $"Searched for a bro, but didn't find myself ({me.name})", (Object) me);
            return (bool) (Object) broController ? broController : (BroController) null;
        }

        public void Awake() => StaticReferenceManager.AllBros.Add(this);

        public void Update()
        {
            if (!this.healthHaver.IsDead && this.m_shouldEnrage && this.behaviorSpeculator.IsInterruptable)
            {
                this.m_shouldEnrage = false;
                this.behaviorSpeculator.InterruptAndDisable();
                this.aiActor.ClearPath();
                this.StartCoroutine(this.EnrageCR());
            }
            if (!this.m_isEnraged)
                return;
            this.m_overheadVfxTimer += BraveTime.DeltaTime;
            if ((bool) (Object) this.m_overheadVfxInstance && (double) this.m_overheadVfxTimer > 1.5)
            {
                this.m_overheadVfxInstance.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("rage_face_vfx_out");
                this.m_overheadVfxInstance = (GameObject) null;
            }
            if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW || !(bool) (Object) this.aiActor || this.aiActor.IsGone)
                return;
            this.m_particleCounter += BraveTime.DeltaTime * 40f;
            if ((double) this.m_particleCounter <= 1.0)
                return;
            int num = Mathf.FloorToInt(this.m_particleCounter);
            this.m_particleCounter %= 1f;
            GlobalSparksDoer.DoRandomParticleBurst(num, this.aiActor.sprite.WorldBottomLeft.ToVector3ZisY(), this.aiActor.sprite.WorldTopRight.ToVector3ZisY(), Vector3.up, 90f, 0.5f, systemType: GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
        }

        protected override void OnDestroy()
        {
            StaticReferenceManager.AllBros.Remove(this);
            base.OnDestroy();
        }

        public void Enrage() => this.m_shouldEnrage = true;

        [DebuggerHidden]
        private IEnumerator EnrageCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BroController__EnrageCRc__Iterator0()
            {
                _this = this
            };
        }

        private void ProcessAttackGroup(AttackBehaviorGroup attackGroup)
        {
            for (int index = 0; index < attackGroup.AttackBehaviors.Count; ++index)
            {
                AttackBehaviorGroup.AttackGroupItem attackBehavior = attackGroup.AttackBehaviors[index];
                if (attackBehavior.Behavior is AttackBehaviorGroup)
                    this.ProcessAttackGroup(attackBehavior.Behavior as AttackBehaviorGroup);
                else if (attackBehavior.Behavior is ShootGunBehavior)
                {
                    ShootGunBehavior behavior = attackBehavior.Behavior as ShootGunBehavior;
                    if (behavior.WeaponType == WeaponType.AIShooterProjectile)
                        attackBehavior.Probability = 0.0f;
                    else if (behavior.WeaponType == WeaponType.BulletScript)
                    {
                        attackBehavior.Probability = 1f;
                        behavior.StopDuringAttack = false;
                    }
                }
                else if (attackBehavior.Behavior is SpawnReinforcementsBehavior)
                {
                    if ((double) attackBehavior.Probability > 0.0)
                    {
                        this.m_cachedSpawnProbability = attackBehavior.Probability;
                        attackBehavior.Probability = 0.0f;
                    }
                    else
                        attackBehavior.Probability = this.m_cachedSpawnProbability;
                }
                else if (attackBehavior.Behavior is ShootBehavior)
                    attackBehavior.Probability = (double) attackBehavior.Probability <= 0.0 ? 1f : 0.0f;
            }
        }

        private void OnPreDeath(Vector2 finalDeathDir)
        {
            if (!(bool) (Object) this.m_enrageVfx)
                return;
            SpawnManager.Despawn(this.m_enrageVfx.gameObject);
        }
    }

