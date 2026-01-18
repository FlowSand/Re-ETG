using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class InfinilichIntroDoer : SpecificIntroDoer
    {
        [Header("Shell Sucking")]
        public float radius = 15f;
        public float gravityForce = 200f;
        public float destroyRadius = 1f;
        private bool m_isFinished;
        private tk2dBaseSprite m_shadowSprite;
        private bool m_isWorldModified;
        private EndTimesNebulaController m_endTimesNebulaController;
        private float m_radiusSquared;

        public void Awake()
        {
            this.GetComponentInChildren<BulletLimbController>().HideBullets = true;
            RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
            if (absoluteRoom == null)
                return;
            absoluteRoom.AdditionalRoomState = RoomHandler.CustomRoomState.LICH_PHASE_THREE;
        }

        protected override void OnDestroy()
        {
            this.ModifyWorld(false);
            base.OnDestroy();
        }

        public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
        {
            this.aiAnimator.PlayUntilCancelled("preintro");
            this.m_shadowSprite = this.aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
            this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(0.0f);
        }

        public override void StartIntro(List<tk2dSpriteAnimator> animators)
        {
            Minimap.Instance.TemporarilyPreventMinimap = false;
            this.StartCoroutine(this.DoIntro());
        }

        public override Vector2? OverrideOutroPosition
        {
            get
            {
                GameManager.Instance.MainCameraController.controllerCamera.isTransitioning = false;
                return new Vector2?();
            }
        }

        [DebuggerHidden]
        public IEnumerator DoIntro()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new InfinilichIntroDoer__DoIntroc__Iterator0()
            {
                _this = this
            };
        }

        public override void EndIntro()
        {
            this.StopAllCoroutines();
            this.aiAnimator.EndAnimationIf("preintro");
            this.aiAnimator.EndAnimationIf("intro");
            this.GetComponentInChildren<BulletLimbController>().HideBullets = false;
            int num = (int) AkSoundEngine.PostEvent("Play_MUS_Lich_Phase_03", this.gameObject);
        }

        public override bool IsIntroFinished => this.m_isFinished;

        public void ModifyWorld(bool value)
        {
            if (!GameManager.HasInstance || value == this.m_isWorldModified)
                return;
            if (value)
            {
                if (!(bool) (Object) this.m_endTimesNebulaController)
                    this.m_endTimesNebulaController = Object.FindObjectOfType<EndTimesNebulaController>();
                if ((bool) (Object) this.m_endTimesNebulaController)
                    this.m_endTimesNebulaController.BecomeActive();
            }
            else if ((bool) (Object) this.m_endTimesNebulaController)
                this.m_endTimesNebulaController.BecomeInactive(false);
            this.m_isWorldModified = value;
        }

        private bool AdjustDebrisVelocity(DebrisObject debris)
        {
            if (debris.IsPickupObject || (Object) debris.GetComponent<BlackHoleDoer>() != (Object) null || !debris.name.Contains("shell", true))
                return false;
            float f = Vector2.SqrMagnitude(debris.sprite.WorldCenter - this.specRigidbody.UnitCenter);
            if ((double) f > (double) this.m_radiusSquared)
                return false;
            float currentDistance = Mathf.Sqrt(f);
            if ((double) currentDistance < (double) this.destroyRadius)
            {
                Object.Destroy((Object) debris.gameObject);
                return true;
            }
            Vector2 accelerationForRigidbody = this.GetFrameAccelerationForRigidbody(debris.sprite.WorldCenter, currentDistance, this.gravityForce);
            float num = Mathf.Clamp(GameManager.INVARIANT_DELTA_TIME, 0.0f, 0.02f);
            if (debris.HasBeenTriggered)
                debris.ApplyVelocity(accelerationForRigidbody * num);
            else if ((double) currentDistance < (double) this.radius / 2.0)
                debris.Trigger((Vector3) (accelerationForRigidbody * num), 0.5f);
            return true;
        }

        private Vector2 GetFrameAccelerationForRigidbody(
            Vector2 unitCenter,
            float currentDistance,
            float g)
        {
            float num1 = Mathf.Clamp01((float) (1.0 - (double) currentDistance / (double) this.radius));
            float num2 = g * num1 * num1;
            return (this.specRigidbody.UnitCenter - unitCenter).normalized * num2;
        }
    }

