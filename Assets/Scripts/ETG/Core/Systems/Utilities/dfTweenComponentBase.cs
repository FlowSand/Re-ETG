using System;

using UnityEngine;

#nullable disable

[Serializable]
    public abstract class dfTweenComponentBase : dfTweenPlayableBase
    {
[SerializeField]
        protected string tweenName = string.Empty;
[SerializeField]
        protected dfComponentMemberInfo target;
[SerializeField]
        protected dfEasingType easingType;
[SerializeField]
        protected AnimationCurve animCurve = new AnimationCurve(new Keyframe[2]
        {
            new Keyframe(0.0f, 0.0f, 0.0f, 1f),
            new Keyframe(1f, 1f, 1f, 0.0f)
        });
[SerializeField]
        protected float length = 1f;
[SerializeField]
        protected bool syncStartWhenRun;
[SerializeField]
        protected bool startValueIsOffset;
[SerializeField]
        protected bool syncEndWhenRun;
[SerializeField]
        protected bool endValueIsOffset;
[SerializeField]
        protected dfTweenLoopType loopType;
[SerializeField]
        protected bool autoRun;
[SerializeField]
        protected bool skipToEndOnStop;
[SerializeField]
        protected float delayBeforeStarting;
        protected dfTweenState state;
        protected dfEasingFunctions.EasingFunction easingFunction;
        protected dfObservableProperty boundProperty;
        protected bool wasAutoStarted;

        public override string TweenName
        {
            get
            {
                if (this.tweenName == null)
                    this.tweenName = base.ToString();
                return this.tweenName;
            }
            set => this.tweenName = value;
        }

        public dfComponentMemberInfo Target
        {
            get => this.target;
            set => this.target = value;
        }

        public AnimationCurve AnimationCurve
        {
            get => this.animCurve;
            set => this.animCurve = value;
        }

        public float Length
        {
            get => this.length;
            set => this.length = Mathf.Max(0.0f, value);
        }

        public float StartDelay
        {
            get => this.delayBeforeStarting;
            set => this.delayBeforeStarting = value;
        }

        public dfEasingType Function
        {
            get => this.easingType;
            set
            {
                this.easingType = value;
                if (this.state == dfTweenState.Stopped)
                    return;
                this.Stop();
                this.Play();
            }
        }

        public dfTweenLoopType LoopType
        {
            get => this.loopType;
            set
            {
                this.loopType = value;
                if (this.state == dfTweenState.Stopped)
                    return;
                this.Stop();
                this.Play();
            }
        }

        public bool SyncStartValueWhenRun
        {
            get => this.syncStartWhenRun;
            set => this.syncStartWhenRun = value;
        }

        public bool StartValueIsOffset
        {
            get => this.startValueIsOffset;
            set => this.startValueIsOffset = value;
        }

        public bool SyncEndValueWhenRun
        {
            get => this.syncEndWhenRun;
            set => this.syncEndWhenRun = value;
        }

        public bool EndValueIsOffset
        {
            get => this.endValueIsOffset;
            set => this.endValueIsOffset = value;
        }

        public bool AutoRun
        {
            get => this.autoRun;
            set => this.autoRun = value;
        }

        public override bool IsPlaying => this.enabled && this.state != dfTweenState.Stopped;

        public bool IsPaused
        {
            get => this.state == dfTweenState.Paused;
            set
            {
                bool flag = this.state == dfTweenState.Paused;
                if (value == flag || this.state == dfTweenState.Stopped)
                    return;
                this.state = !value ? dfTweenState.Playing : dfTweenState.Paused;
                if (value)
                    this.onPaused();
                else
                    this.onResumed();
            }
        }

        protected internal abstract void onPaused();

        protected internal abstract void onResumed();

        protected internal abstract void onStarted();

        protected internal abstract void onStopped();

        protected internal abstract void onReset();

        protected internal abstract void onCompleted();

        public void Start()
        {
            if (!this.autoRun || this.wasAutoStarted)
                return;
            this.wasAutoStarted = true;
            this.Play();
        }

        public void OnDisable()
        {
            this.Stop();
            this.wasAutoStarted = false;
        }

        public override string ToString()
        {
            return this.Target != null && this.Target.IsValid ? $"{this.TweenName} ({this.target.Component.name}.{this.target.MemberName})" : this.TweenName;
        }
    }

