using System;
using System.Collections.Generic;

using UnityEngine;

using DaikonForge.Editor;

#nullable disable
namespace DaikonForge.Tween.Components
{
    [InspectorGroupOrder(new string[] {"General", "Animation", "Looping", "Tweens"})]
    [AddComponentMenu("Daikon Forge/Tween/Group")]
    public class TweenComponentGroup : TweenComponentBase
    {
        [SerializeField]
        [Inspector("General", 1, Label = "Mode")]
        protected TweenGroupMode groupMode;
        [Inspector("Tweens", 0, Label = "Tweens")]
        [SerializeField]
        protected List<TweenPlayableComponent> tweens = new List<TweenPlayableComponent>();
        protected TweenGroup group;

        public override TweenBase BaseTween
        {
            get
            {
                this.configureTween();
                return (TweenBase) this.group;
            }
        }

        public List<TweenPlayableComponent> Tweens => this.tweens;

        public TweenGroupMode GroupMode
        {
            get => this.groupMode;
            set
            {
                if (value == this.groupMode)
                    return;
                this.groupMode = value;
                this.Stop();
            }
        }

        public override TweenState State => this.group == null ? TweenState.Stopped : this.group.State;

        public virtual void OnApplicationQuit() => this.cleanup();

        public override void OnDisable()
        {
            base.OnDisable();
            this.cleanup();
        }

        public override void Play()
        {
            if (this.State != TweenState.Stopped)
                this.Stop();
            this.configureTween();
            this.validateTweenConfiguration();
            this.group.Play();
        }

        public override void Stop()
        {
            if (!this.IsPlaying)
                return;
            this.validateTweenConfiguration();
            this.group.Stop();
        }

        public override void Pause()
        {
            if (!this.IsPlaying)
                return;
            this.validateTweenConfiguration();
            this.group.Pause();
        }

        public override void Resume()
        {
            if (!this.IsPaused)
                return;
            this.validateTweenConfiguration();
            this.group.Resume();
        }

        public override void Rewind()
        {
            this.validateTweenConfiguration();
            this.group.Rewind();
        }

        public override void FastForward()
        {
            this.validateTweenConfiguration();
            this.group.FastForward();
        }

        protected void cleanup()
        {
            if (this.group == null)
                return;
            this.group.Stop();
            this.group.Release();
            this.group = (TweenGroup) null;
        }

        protected void validateTweenConfiguration()
        {
            this.loopCount = Mathf.Max(0, this.loopCount);
            if (this.loopType != TweenLoopType.None && this.loopType != TweenLoopType.Loop)
                throw new ArgumentException("LoopType may only be one of the following values: TweenLoopType.None, TweenLoopType.Loop");
        }

        protected void configureTween()
        {
            if (this.group == null)
                this.group = (TweenGroup) new TweenGroup().OnStarted((TweenCallback) (x => this.onStarted())).OnStopped((TweenCallback) (x => this.onStopped())).OnPaused((TweenCallback) (x => this.onPaused())).OnResumed((TweenCallback) (x => this.onResumed())).OnLoopCompleted((TweenCallback) (x => this.onLoopCompleted())).OnCompleted((TweenCallback) (x => this.onCompleted()));
            this.group.ClearTweens().SetMode(this.groupMode).SetDelay(this.startDelay).SetLoopType(this.loopType).SetLoopCount(this.loopCount);
            for (int index = 0; index < this.tweens.Count; ++index)
            {
                TweenPlayableComponent tween = this.tweens[index];
                if ((UnityEngine.Object) tween != (UnityEngine.Object) null)
                {
                    tween.AutoRun = false;
                    TweenBase baseTween = tween.BaseTween;
                    if (baseTween == null)
                        Debug.LogError((object) "Base tween not set", (UnityEngine.Object) tween);
                    else
                        this.group.AppendTween(baseTween);
                }
            }
        }
    }
}
