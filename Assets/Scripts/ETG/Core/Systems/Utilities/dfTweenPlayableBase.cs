using UnityEngine;

#nullable disable

    public abstract class dfTweenPlayableBase : MonoBehaviour
    {
        public abstract string TweenName { get; set; }

        public abstract bool IsPlaying { get; }

        public abstract void Play();

        public abstract void Stop();

        public abstract void Reset();

        public void Enable() => this.enabled = true;

        public void Disable() => this.enabled = false;

        public override string ToString() => $"{this.TweenName} - {base.ToString()}";
    }

