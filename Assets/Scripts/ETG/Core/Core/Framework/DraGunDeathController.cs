using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class DraGunDeathController : BraveBehaviour
    {
        public List<GameObject> explosionVfx;
        public float explosionMidDelay = 0.3f;
        public int explosionCount = 10;
        public GameObject skullDebris;
        public GameObject fingerDebris;
        public GameObject neckDebris;
        private DraGunController m_dragunController;
        private tk2dSpriteAnimator m_deathDummy;

        public void Awake()
        {
            this.m_dragunController = this.GetComponent<DraGunController>();
            this.m_deathDummy = this.transform.Find("DeathDummy").GetComponent<tk2dSpriteAnimator>();
        }

        public void Start()
        {
            this.healthHaver.ManualDeathHandling = true;
            this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
            this.healthHaver.OverrideKillCamTime = new float?(6.25f);
        }

        protected override void OnDestroy() => base.OnDestroy();

        private void OnBossDeath(Vector2 dir)
        {
            this.behaviorSpeculator.enabled = false;
            GameManager.Instance.StartCoroutine(this.OnDeathExplosionsCR());
            int num = (int) AkSoundEngine.PostEvent("Play_BOSS_dragun_thunder_01", this.gameObject);
        }

        [DebuggerHidden]
        private IEnumerator LerpCrackEmission(float startVal, float targetVal, float duration)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunDeathController__LerpCrackEmissionc__Iterator0()
            {
                duration = duration,
                startVal = startVal,
                targetVal = targetVal,
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator LerpCrackColor(Color targetVal, float duration)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunDeathController__LerpCrackColorc__Iterator1()
            {
                duration = duration,
                targetVal = targetVal,
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator OnDeathExplosionsCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunDeathController__OnDeathExplosionsCRc__Iterator2()
            {
                _this = this
            };
        }

        private void SpawnBones(GameObject bonePrefab, int count, Vector2 min, Vector2 max)
        {
            Vector2 min1 = this.aiActor.ParentRoom.area.basePosition.ToVector2() + min + new Vector2(0.0f, (float) DraGunRoomPlaceable.HallHeight);
            Vector2 max1 = this.aiActor.ParentRoom.area.basePosition.ToVector2() + this.aiActor.ParentRoom.area.dimensions.ToVector2() + max;
            for (int index = 0; index < count; ++index)
            {
                Vector2 position = BraveUtility.RandomVector2(min1, max1);
                GameObject gameObject = SpawnManager.SpawnVFX(bonePrefab, (Vector3) position, Quaternion.identity);
                if ((bool) (UnityEngine.Object) gameObject)
                {
                    gameObject.transform.parent = SpawnManager.Instance.VFX;
                    DebrisObject orAddComponent = gameObject.GetOrAddComponent<DebrisObject>();
                    orAddComponent.decayOnBounce = 0.5f;
                    orAddComponent.bounceCount = 1;
                    orAddComponent.canRotate = true;
                    Vector2 vector2 = BraveMathCollege.DegreesToVector(UnityEngine.Random.Range(-80f, -100f)) * UnityEngine.Random.Range(0.1f, 3f);
                    Vector3 startingForce = new Vector3(vector2.x, (double) vector2.y >= 0.0 ? 0.0f : vector2.y, (double) vector2.y <= 0.0 ? 0.0f : vector2.y);
                    if ((bool) (UnityEngine.Object) orAddComponent.minorBreakable)
                        orAddComponent.minorBreakable.enabled = true;
                    orAddComponent.Trigger(startingForce, UnityEngine.Random.Range(1f, 2f));
                    if ((bool) (UnityEngine.Object) orAddComponent.specRigidbody)
                        orAddComponent.OnGrounded += new Action<DebrisObject>(this.HandleComplexDebris);
                }
            }
        }

        private void HandleComplexDebris(DebrisObject debrisObject)
        {
            GameManager.Instance.StartCoroutine(this.DelayedSpriteFixer(debrisObject.sprite));
            SpeculativeRigidbody specRigidbody = debrisObject.specRigidbody;
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(specRigidbody);
            UnityEngine.Object.Destroy((UnityEngine.Object) debrisObject);
            specRigidbody.RegenerateCache();
        }

        [DebuggerHidden]
        private IEnumerator DelayedSpriteFixer(tk2dBaseSprite sprite)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunDeathController__DelayedSpriteFixerc__Iterator3()
            {
                sprite = sprite
            };
        }
    }

