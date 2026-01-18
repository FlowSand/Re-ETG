using System;

using FullInspector;
using UnityEngine;

using Dungeonator;

#nullable disable

[InspectorDropdownName("Bosses/Meduzi/UnderwaterBehavior")]
public class MeduziUnderwaterBehavior : BasicAttackBehavior
    {
        public bool AttackableDuringAnimation;
        public bool AvoidWalls;
        public MeduziUnderwaterBehavior.StartingDirection startingDirection;
        public float GoneTime = 1f;
        [InspectorCategory("Attack")]
        public BulletScriptSelector disappearBulletScript;
        [InspectorCategory("Attack")]
        public BulletScriptSelector reappearInBulletScript;
        [InspectorCategory("Visuals")]
        public string disappearAnim = "teleport_out";
        [InspectorCategory("Visuals")]
        public string reappearAnim = "teleport_in";
        [InspectorCategory("Visuals")]
        public bool requiresTransparency;
        [InspectorCategory("Visuals")]
        public MeduziUnderwaterBehavior.ShadowSupport shadowSupport;
        [InspectorCategory("Visuals")]
        [InspectorShowIf("ShowShadowAnimationNames")]
        public string shadowDisappearAnim;
        [InspectorCategory("Visuals")]
        [InspectorShowIf("ShowShadowAnimationNames")]
        public string shadowReappearAnim;
        public GameObject crawlSprite;
        public tk2dSpriteAnimator crawlAnimator;
        public float crawlSpeed = 8f;
        public float crawlTurnTime = 1f;
        private tk2dBaseSprite m_shadowSprite;
        private Shader m_cachedShader;
        private GoopDoer m_goopDoer;
        private float m_timer;
        private bool m_shouldFire;
        private float m_direction;
        private float m_angularVelocity;
        private MeduziUnderwaterBehavior.UnderwaterState m_state;

        private bool ShowShadowAnimationNames()
        {
            return this.shadowSupport == MeduziUnderwaterBehavior.ShadowSupport.Animate;
        }

        public override void Start()
        {
            base.Start();
            if (this.disappearBulletScript != null && !this.disappearBulletScript.IsNull || this.reappearInBulletScript != null && !this.reappearInBulletScript.IsNull)
                this.m_aiActor.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
            this.crawlAnimator.sprite.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE;
            this.crawlAnimator.renderer.material.SetFloat("_ReflectionYOffset", 1000f);
            this.m_goopDoer = this.m_aiActor.GetComponent<GoopDoer>();
            this.m_aiActor.specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.RoomMovementRestrictor);
        }

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_timer);
        }

        public override BehaviorResult Update()
        {
            int num = (int) base.Update();
            if ((UnityEngine.Object) this.m_shadowSprite == (UnityEngine.Object) null)
                this.m_shadowSprite = this.m_aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
            if (!this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
                return BehaviorResult.Continue;
            this.State = MeduziUnderwaterBehavior.UnderwaterState.Disappear;
            this.m_aiActor.healthHaver.minimumHealth = 1f;
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num = (int) base.ContinuousUpdate();
            if (this.State == MeduziUnderwaterBehavior.UnderwaterState.Disappear)
            {
                if (this.shadowSupport == MeduziUnderwaterBehavior.ShadowSupport.Fade)
                    this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f - this.m_aiAnimator.CurrentClipProgress);
                if (!this.m_aiAnimator.IsPlaying(this.disappearAnim))
                    this.State = (double) this.GoneTime <= 0.0 ? MeduziUnderwaterBehavior.UnderwaterState.Reappear : MeduziUnderwaterBehavior.UnderwaterState.Gone;
            }
            else if (this.State == MeduziUnderwaterBehavior.UnderwaterState.Gone)
            {
                float target = (double) this.m_aiActor.BehaviorVelocity.magnitude != 0.0 ? this.m_aiActor.BehaviorVelocity.ToAngle() : 0.0f;
                if ((bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
                    target = (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.Ground) - this.m_aiActor.specRigidbody.UnitCenter).ToAngle();
                this.m_direction = Mathf.SmoothDampAngle(this.m_direction, target, ref this.m_angularVelocity, this.crawlTurnTime);
                this.crawlSprite.transform.rotation = Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.QuantizeFloat(this.m_direction, 11.25f));
                this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_direction, this.crawlSpeed);
                if ((double) this.m_timer <= 0.0)
                    this.State = MeduziUnderwaterBehavior.UnderwaterState.Reappear;
            }
            else if (this.State == MeduziUnderwaterBehavior.UnderwaterState.Reappear)
            {
                if (this.shadowSupport == MeduziUnderwaterBehavior.ShadowSupport.Fade)
                    this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(this.m_aiAnimator.CurrentClipProgress);
                if ((bool) (UnityEngine.Object) this.m_aiShooter)
                    this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (MeduziUnderwaterBehavior));
                if (!this.m_aiAnimator.IsPlaying(this.reappearAnim))
                {
                    this.State = MeduziUnderwaterBehavior.UnderwaterState.None;
                    return ContinuousBehaviorResult.Finished;
                }
            }
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.m_aiActor.healthHaver.minimumHealth = 0.0f;
            if (this.requiresTransparency && (bool) (UnityEngine.Object) this.m_cachedShader)
            {
                this.m_aiActor.sprite.usesOverrideMaterial = false;
                this.m_aiActor.renderer.material.shader = this.m_cachedShader;
                this.m_cachedShader = (Shader) null;
            }
            this.m_aiActor.sprite.renderer.enabled = true;
            if ((bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
                this.m_aiActor.knockbackDoer.SetImmobile(false, "teleport");
            this.m_aiActor.specRigidbody.CollideWithOthers = true;
            this.m_aiActor.IsGone = false;
            if ((bool) (UnityEngine.Object) this.m_aiShooter)
                this.m_aiShooter.ToggleGunAndHandRenderers(true, nameof (MeduziUnderwaterBehavior));
            this.m_aiAnimator.EndAnimationIf(this.disappearAnim);
            this.m_aiAnimator.EndAnimationIf(this.reappearAnim);
            if (this.shadowSupport == MeduziUnderwaterBehavior.ShadowSupport.Fade)
                this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f);
            else if (this.shadowSupport == MeduziUnderwaterBehavior.ShadowSupport.Animate)
            {
                tk2dSpriteAnimationClip clipByName = this.m_shadowSprite.spriteAnimator.GetClipByName(this.shadowReappearAnim);
                this.m_shadowSprite.spriteAnimator.Play(clipByName, (float) (clipByName.frames.Length - 1), clipByName.fps);
            }
            this.crawlSprite.SetActive(false);
            this.m_aiActor.BehaviorOverridesVelocity = false;
            SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, true);
            this.m_goopDoer.enabled = false;
            this.m_state = MeduziUnderwaterBehavior.UnderwaterState.None;
            this.m_updateEveryFrame = false;
            this.UpdateCooldowns();
        }

        public override void OnActorPreDeath()
        {
            this.m_aiActor.specRigidbody.MovementRestrictor -= new SpeculativeRigidbody.MovementRestrictorDelegate(this.RoomMovementRestrictor);
            base.OnActorPreDeath();
        }

        public override bool IsOverridable() => false;

        public void AnimationEventTriggered(
            tk2dSpriteAnimator animator,
            tk2dSpriteAnimationClip clip,
            int frame)
        {
            if (!this.m_shouldFire || !(clip.GetFrame(frame).eventInfo == "fire"))
                return;
            if (this.State == MeduziUnderwaterBehavior.UnderwaterState.Reappear)
                SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.reappearInBulletScript);
            else if (this.State == MeduziUnderwaterBehavior.UnderwaterState.Disappear)
                SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.disappearBulletScript);
            this.m_shouldFire = false;
        }

        private void RoomMovementRestrictor(
            SpeculativeRigidbody specRigidbody,
            IntVector2 prevPixelOffset,
            IntVector2 pixelOffset,
            ref bool validLocation)
        {
            if (!validLocation)
                return;
            IntVector2 intVector2 = pixelOffset - prevPixelOffset;
            CellArea area = this.m_aiActor.ParentRoom.area;
            if (intVector2.x < 0)
            {
                if (specRigidbody.PixelColliders[0].MinX + pixelOffset.x >= area.basePosition.x * 16)
                    return;
                validLocation = false;
            }
            else if (intVector2.x > 0)
            {
                if (specRigidbody.PixelColliders[0].MaxX + pixelOffset.x <= (area.basePosition.x + area.dimensions.x) * 16 - 1)
                    return;
                validLocation = false;
            }
            else if (intVector2.y < 0)
            {
                if (specRigidbody.PixelColliders[0].MinY + pixelOffset.y >= area.basePosition.y * 16)
                    return;
                validLocation = false;
            }
            else
            {
                if (intVector2.y <= 0 || specRigidbody.PixelColliders[0].MaxY + pixelOffset.y <= (area.basePosition.y + area.dimensions.y) * 16 - 1)
                    return;
                validLocation = false;
            }
        }

        private MeduziUnderwaterBehavior.UnderwaterState State
        {
            get => this.m_state;
            set
            {
                this.EndState(this.m_state);
                this.m_state = value;
                this.BeginState(this.m_state);
            }
        }

        private void BeginState(MeduziUnderwaterBehavior.UnderwaterState state)
        {
            switch (state)
            {
                case MeduziUnderwaterBehavior.UnderwaterState.Disappear:
                    if (this.disappearBulletScript != null && !this.disappearBulletScript.IsNull)
                        this.m_shouldFire = true;
                    if (this.requiresTransparency)
                    {
                        this.m_cachedShader = this.m_aiActor.renderer.material.shader;
                        this.m_aiActor.sprite.usesOverrideMaterial = true;
                        this.m_aiActor.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
                    }
                    this.m_aiAnimator.PlayUntilCancelled(this.disappearAnim, true);
                    if (this.shadowSupport == MeduziUnderwaterBehavior.ShadowSupport.Animate)
                        this.m_shadowSprite.spriteAnimator.PlayAndForceTime(this.shadowDisappearAnim, this.m_aiAnimator.CurrentClipLength);
                    this.m_aiActor.ClearPath();
                    if (!this.AttackableDuringAnimation)
                    {
                        this.m_aiActor.specRigidbody.CollideWithOthers = false;
                        this.m_aiActor.IsGone = true;
                    }
                    if (!(bool) (UnityEngine.Object) this.m_aiShooter)
                        break;
                    this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (MeduziUnderwaterBehavior));
                    break;
                case MeduziUnderwaterBehavior.UnderwaterState.Gone:
                    this.m_timer = this.GoneTime;
                    this.m_aiActor.specRigidbody.CollideWithOthers = false;
                    this.m_aiActor.IsGone = true;
                    this.m_aiActor.sprite.renderer.enabled = false;
                    this.crawlSprite.transform.rotation = Quaternion.identity;
                    this.crawlSprite.SetActive(true);
                    this.crawlAnimator.Play(this.crawlAnimator.DefaultClip, 0.0f, this.crawlAnimator.DefaultClip.fps);
                    this.m_direction = this.startingDirection != MeduziUnderwaterBehavior.StartingDirection.Player || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody ? UnityEngine.Random.Range(0.0f, 360f) : (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.Ground) - this.m_aiActor.specRigidbody.UnitCenter).ToAngle();
                    this.m_angularVelocity = 0.0f;
                    this.crawlSprite.transform.rotation = Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.QuantizeFloat(this.m_direction, 11.25f));
                    this.m_aiActor.BehaviorOverridesVelocity = true;
                    this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_direction, this.crawlSpeed);
                    this.m_goopDoer.enabled = true;
                    break;
                case MeduziUnderwaterBehavior.UnderwaterState.Reappear:
                    if (this.reappearInBulletScript != null && !this.reappearInBulletScript.IsNull)
                        this.m_shouldFire = true;
                    this.m_aiAnimator.PlayUntilFinished(this.reappearAnim, true);
                    if (this.shadowSupport == MeduziUnderwaterBehavior.ShadowSupport.Animate)
                        this.m_shadowSprite.spriteAnimator.PlayAndForceTime(this.shadowReappearAnim, this.m_aiAnimator.CurrentClipLength);
                    this.m_shadowSprite.renderer.enabled = true;
                    if (this.AttackableDuringAnimation)
                    {
                        this.m_aiActor.specRigidbody.CollideWithOthers = true;
                        this.m_aiActor.IsGone = false;
                    }
                    this.m_aiActor.sprite.renderer.enabled = true;
                    if ((bool) (UnityEngine.Object) this.m_aiShooter)
                        this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (MeduziUnderwaterBehavior));
                    SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, true);
                    break;
            }
        }

        private void EndState(MeduziUnderwaterBehavior.UnderwaterState state)
        {
            switch (state)
            {
                case MeduziUnderwaterBehavior.UnderwaterState.Disappear:
                    this.m_shadowSprite.renderer.enabled = false;
                    SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, false);
                    if (this.disappearBulletScript == null || this.disappearBulletScript.IsNull || !this.m_shouldFire)
                        break;
                    SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.disappearBulletScript);
                    this.m_shouldFire = false;
                    break;
                case MeduziUnderwaterBehavior.UnderwaterState.Gone:
                    this.crawlSprite.SetActive(false);
                    this.m_aiActor.BehaviorOverridesVelocity = false;
                    this.m_goopDoer.enabled = false;
                    break;
                case MeduziUnderwaterBehavior.UnderwaterState.Reappear:
                    if (this.requiresTransparency)
                    {
                        this.m_aiActor.sprite.usesOverrideMaterial = false;
                        this.m_aiActor.renderer.material.shader = this.m_cachedShader;
                    }
                    if (this.shadowSupport == MeduziUnderwaterBehavior.ShadowSupport.Fade)
                        this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f);
                    this.m_aiActor.specRigidbody.CollideWithOthers = true;
                    this.m_aiActor.IsGone = false;
                    if ((bool) (UnityEngine.Object) this.m_aiShooter)
                        this.m_aiShooter.ToggleGunAndHandRenderers(true, nameof (MeduziUnderwaterBehavior));
                    if (this.reappearInBulletScript == null || this.reappearInBulletScript.IsNull || !this.m_shouldFire)
                        break;
                    SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.reappearInBulletScript);
                    this.m_shouldFire = false;
                    break;
            }
        }

        public enum ShadowSupport
        {
            None,
            Fade,
            Animate,
        }

        public enum StartingDirection
        {
            Player,
            RandomAwayFromWalls,
        }

        private enum UnderwaterState
        {
            None,
            Disappear,
            Gone,
            Reappear,
        }
    }

