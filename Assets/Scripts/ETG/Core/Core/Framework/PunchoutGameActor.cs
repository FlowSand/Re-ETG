using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

    public abstract class PunchoutGameActor : BraveBehaviour
    {
        public dfSprite HealthBarUI;
        public dfSprite[] StarsUI;
        public PunchoutGameActor Opponent;
[NonSerialized]
        public float Health = 100f;
        private IEnumerator m_flashCoroutine;
        private PunchoutGameActor.State m_state;
        private bool m_isFlashing;
        protected List<Material> materialsToFlash = new List<Material>();
        protected List<Material> outlineMaterialsToFlash = new List<Material>();
        protected List<Material> materialsToEnableBrightnessClampOn = new List<Material>();
        protected List<Color> sourceColors = new List<Color>();
        private Vector2 m_cameraVelocity;
        private Vector2 m_cameraTarget;
        private float m_cameraTime;

        public int Stars { get; set; }

        public PunchoutGameActor.State LastHitBy { get; set; }

        public int CurrentFrame => this.spriteAnimator.CurrentFrame;

        public float CurrentFrameFloat
        {
            get => (float) this.spriteAnimator.CurrentFrame + this.spriteAnimator.clipTime % 1f;
        }

        public Vector2 CameraOffset { get; set; }

        public bool IsYellow { get; set; }

        public bool IsFarAway => this.state != null && this.state.IsFarAway();

        public abstract bool IsDead { get; }

        public virtual void Start()
        {
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f);
        }

        public virtual void ManualUpdate()
        {
            if (this is PunchoutPlayerController && (double) this.Health <= 0.0 && !(this.state is PunchoutPlayerController.DeathState))
            {
                this.Health = 100f;
                (this as PunchoutPlayerController).UpdateUI();
            }
            if ((bool) (UnityEngine.Object) this.HealthBarUI)
                this.HealthBarUI.FillAmount = Mathf.Max(0.0f, this.Health / 100f);
            for (int index = 0; index < this.StarsUI.Length; ++index)
                this.StarsUI[index].IsVisible = index < this.Stars;
            this.CameraOffset = Vector2.SmoothDamp(this.CameraOffset, this.m_cameraTarget, ref this.m_cameraVelocity, this.m_cameraTime, 100f, BraveTime.DeltaTime);
        }

        public virtual void Hit(bool isLeft, float damage, int starsUsed = 0, bool skipProcessing = false)
        {
        }

        public void Play(string name)
        {
            this.aiAnimator.FacingDirection = 90f;
            this.aiAnimator.PlayUntilFinished(name);
        }

        public void Play(string name, bool isLeft)
        {
            this.aiAnimator.FacingDirection = !isLeft ? 0.0f : 180f;
            this.aiAnimator.PlayUntilFinished(name);
        }

        public void FlashDamage(float flashTime = 0.04f)
        {
            this.StopFlash();
            this.m_flashCoroutine = this.FlashColor(Color.white, flashTime);
            this.StartCoroutine(this.m_flashCoroutine);
        }

        public void FlashWarn(float flashFrames)
        {
            float flashTime = flashFrames / this.spriteAnimator.ClipFps;
            this.StopFlash();
            this.IsYellow = true;
            this.m_flashCoroutine = this.FlashColor(Color.yellow, flashTime);
            this.StartCoroutine(this.m_flashCoroutine);
            int num = (int) AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Flash_01", this.gameObject);
        }

        public void PulseColor(Color overrideColor, float flashFrames)
        {
            float flashTime = flashFrames / this.spriteAnimator.ClipFps;
            this.StopFlash();
            this.m_flashCoroutine = this.FlashColor(overrideColor, flashTime, true);
            this.StartCoroutine(this.m_flashCoroutine);
        }

[DebuggerHidden]
        protected IEnumerator FlashColor(Color overrideColor, float flashTime, bool roundtrip = false)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PunchoutGameActor__FlashColorc__Iterator0()
            {
                overrideColor = overrideColor,
                flashTime = flashTime,
                roundtrip = roundtrip,
                _this = this
            };
        }

        private void StopFlash()
        {
            if (this is PunchoutPlayerController && (this as PunchoutPlayerController).IsEevee)
                return;
            for (int index = 0; index < this.materialsToFlash.Count; ++index)
                this.materialsToFlash[index].SetColor("_OverrideColor", new Color(1f, 1f, 1f, 0.0f));
            for (int index = 0; index < this.outlineMaterialsToFlash.Count; ++index)
                this.outlineMaterialsToFlash[index].SetColor("_OverrideColor", this.sourceColors[index]);
            for (int index = 0; index < this.materialsToEnableBrightnessClampOn.Count; ++index)
            {
                this.materialsToEnableBrightnessClampOn[index].DisableKeyword("BRIGHTNESS_CLAMP_OFF");
                this.materialsToEnableBrightnessClampOn[index].EnableKeyword("BRIGHTNESS_CLAMP_ON");
            }
            this.m_isFlashing = false;
            this.IsYellow = false;
            if (this.m_flashCoroutine != null)
                this.StopCoroutine(this.m_flashCoroutine);
            this.m_flashCoroutine = (IEnumerator) null;
        }

        public void MoveCamera(Vector2 offset, float time)
        {
            this.m_cameraTarget = offset;
            this.m_cameraTime = time;
        }

        public PunchoutGameActor.State state
        {
            get => this.m_state;
            set
            {
                if (this.m_state != null)
                    this.m_state.Stop();
                this.m_state = value;
                if (this.m_state == null)
                    return;
                this.m_state.Actor = this;
                this.m_state.Start();
            }
        }

        public abstract class State
        {
            public bool IsLeft;
            private int m_lastReportedFrame = -1;

            public State()
            {
            }

            public State(bool isLeft) => this.IsLeft = isLeft;

            public bool IsDone { get; set; }

            public PunchoutGameActor Actor { get; set; }

            public PunchoutPlayerController ActorPlayer => (PunchoutPlayerController) this.Actor;

            public PunchoutAIActor ActorEnemy => (PunchoutAIActor) this.Actor;

            public virtual string AnimName => (string) null;

            public virtual float PunishTime => 0.0f;

            public bool WasBlocked { get; set; }

            public virtual void Start()
            {
                if (this.AnimName != null)
                    this.Actor.Play(this.AnimName, this.IsLeft);
                this.Actor.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleted);
            }

            public virtual void Update()
            {
                if (this.AnimName != null && this.Actor.aiAnimator.IsIdle())
                    this.IsDone = true;
                int currentFrame = this.Actor.spriteAnimator.CurrentFrame;
                if (currentFrame < this.m_lastReportedFrame)
                    this.m_lastReportedFrame = this.Actor.spriteAnimator.CurrentClip.wrapMode != tk2dSpriteAnimationClip.WrapMode.LoopSection ? -1 : currentFrame - 1;
                while (currentFrame > this.m_lastReportedFrame)
                {
                    ++this.m_lastReportedFrame;
                    this.OnFrame(this.m_lastReportedFrame);
                }
            }

            public virtual void Stop()
            {
                this.Actor.spriteAnimator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleted);
            }

            private void AnimationCompleted(
                tk2dSpriteAnimator tk2DSpriteAnimator,
                tk2dSpriteAnimationClip tk2DSpriteAnimationClip)
            {
                this.OnAnimationCompleted();
            }

            public virtual void OnFrame(int currentFrame)
            {
            }

            public virtual void OnHit(ref bool preventDamage, bool isLeft, int starsUsed)
            {
            }

            public virtual void OnAnimationCompleted()
            {
            }

            public virtual bool CanBeHit(bool isLeft) => true;

            public virtual bool IsFarAway() => false;

            public virtual bool ShouldInstantKO(int starsUsed) => false;
        }

public class DuckState : PunchoutGameActor.State
        {
            public override string AnimName => "duck";

            public override void Start()
            {
                base.Start();
                this.Actor.MoveCamera(new Vector2(0.0f, -0.4f), 0.2f);
            }

            public override void Stop()
            {
                base.Stop();
                this.Actor.MoveCamera(new Vector2(0.0f, 0.0f), 0.2f);
            }
        }

        public class DodgeState : PunchoutGameActor.State
        {
            public DodgeState(bool isLeft)
                : base(isLeft)
            {
            }

            public override string AnimName => "dodge";

            public override void Start()
            {
                base.Start();
                this.Actor.MoveCamera(new Vector2((float) (0.699999988079071 * (!this.IsLeft ? 1.0 : -1.0)), 0.0f), 0.15f);
            }

            public override void Stop()
            {
                base.Stop();
                this.Actor.MoveCamera(new Vector2(0.0f, 0.0f), 0.25f);
            }
        }

        public class HitState : PunchoutGameActor.State
        {
            public HitState(bool isLeft)
                : base(isLeft)
            {
            }

            public override string AnimName => "hit";
        }

        public class BlockState : PunchoutGameActor.State
        {
            public override string AnimName => "block";

            public virtual void Bonk()
            {
            }
        }

        public abstract class BasicAttackState : PunchoutGameActor.State
        {
            public BasicAttackState()
            {
            }

            public BasicAttackState(bool isLeft)
                : base(isLeft)
            {
            }

            public abstract int DamageFrame { get; }

            public abstract float Damage { get; }

            public override void OnFrame(int currentFrame)
            {
                base.OnFrame(currentFrame);
                if (currentFrame != this.DamageFrame || !this.CanHitOpponent(this.Actor.Opponent.state))
                    return;
                this.Actor.Opponent.Hit(this.IsLeft, this.Damage);
            }

            public abstract bool CanHitOpponent(PunchoutGameActor.State state);
        }

        public abstract class BasicComboState : PunchoutGameActor.State
        {
            public PunchoutGameActor.State[] States;
            private int m_index;

            public BasicComboState() => this.States = new PunchoutGameActor.State[0];

            public BasicComboState(PunchoutGameActor.State[] states) => this.States = states;

            public PunchoutGameActor.State CurrentState => this.States[this.m_index];

            public override void Start()
            {
                this.CurrentState.Actor = this.Actor;
                this.CurrentState.Start();
            }

            public override void Update()
            {
                this.CurrentState.Update();
                this.CurrentState.WasBlocked = this.WasBlocked;
                if (!this.CurrentState.IsDone)
                    return;
                this.CurrentState.Stop();
                ++this.m_index;
                if (this.m_index >= this.States.Length || this.Actor.Opponent.IsDead)
                {
                    this.IsDone = true;
                }
                else
                {
                    this.CurrentState.Actor = this.Actor;
                    this.CurrentState.Start();
                    this.WasBlocked = false;
                }
            }

            public override void OnHit(ref bool preventDamage, bool isLeft, int starsUsed)
            {
                if (this.m_index >= this.States.Length)
                    return;
                this.CurrentState.OnHit(ref preventDamage, isLeft, starsUsed);
            }

            public override bool CanBeHit(bool isLeft)
            {
                if (this.m_index >= this.States.Length)
                    return true;
                return !this.WasBlocked && this.CurrentState.CanBeHit(isLeft);
            }
        }
    }

