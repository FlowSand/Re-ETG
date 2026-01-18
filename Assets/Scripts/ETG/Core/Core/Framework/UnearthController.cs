using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class UnearthController : BraveBehaviour
    {
        public string triggerAnim;
        public List<GameObject> dirtVfx;
        public int dirtCount;
        public List<GameObject> dustVfx;
        public float dustMidDelay = 0.05f;
        public Vector2 dustOffset;
        public Vector2 dustDimensions;
        private UnearthController.UnearthState m_state;

        private void Update()
        {
            if (this.m_state == UnearthController.UnearthState.Idle)
            {
                if (!this.aiAnimator.IsPlaying(this.triggerAnim))
                    return;
                this.m_state = UnearthController.UnearthState.Unearth;
                this.StartCoroutine(this.DirtCR());
                this.StartCoroutine(this.PuffCR());
            }
            else
            {
                if (this.m_state != UnearthController.UnearthState.Unearth || this.aiAnimator.IsPlaying(this.triggerAnim))
                    return;
                this.m_state = UnearthController.UnearthState.Finished;
            }
        }

        protected override void OnDestroy() => base.OnDestroy();

        [DebuggerHidden]
        private IEnumerator DirtCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new UnearthController__DirtCRc__Iterator0()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator PuffCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new UnearthController__PuffCRc__Iterator1()
            {
                _this = this
            };
        }

        private enum UnearthState
        {
            Idle,
            Unearth,
            Finished,
        }
    }

