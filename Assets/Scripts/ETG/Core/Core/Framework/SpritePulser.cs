using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class SpritePulser : BraveBehaviour
    {
        public float duration = 1f;
        public float minDuration = 0.3f;
        public float maxDuration = 2.9f;
        public float metaDuration = 6f;
        public float minAlpha = 0.3f;
        public float minScale = 0.9f;
        public float maxScale = 1.1f;
        private bool m_active;

        private void Start()
        {
            if (!((Object) this.sprite == (Object) null))
                return;
            UnityEngine.Debug.LogError((object) "No sprite on SpritePulser!", (Object) this);
        }

        private void Update()
        {
            if (!this.m_active)
                return;
            float t = Mathf.SmoothStep(0.0f, 1f, Mathf.PingPong(UnityEngine.Time.realtimeSinceStartup, this.duration) / this.duration);
            this.sprite.color = this.sprite.color with
            {
                a = Mathf.Lerp(this.minAlpha, 1f, t)
            };
        }

        private void OnBecameVisible() => this.m_active = true;

        private void OnBecameInvisible() => this.m_active = false;

        [DebuggerHidden]
        private IEnumerator Pulse()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new SpritePulser__Pulsec__Iterator0()
            {
                _this = this
            };
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

