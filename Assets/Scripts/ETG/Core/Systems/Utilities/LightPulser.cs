using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class LightPulser : MonoBehaviour
    {
        public bool flicker;
        public float pulseSpeed = 40f;
        public float waitTime = 0.05f;
        public float normalRange = 3.33f;
        public float flickerRange = 0.5f;
        private ShadowSystem m_sl;

        private void Start()
        {
            if (!this.flicker)
                return;
            this.StartCoroutine("Flicker");
        }

        public void AssignShadowSystem(ShadowSystem ss) => this.m_sl = ss;

        [DebuggerHidden]
        private IEnumerator Flicker()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new LightPulser__Flickerc__Iterator0()
            {
                _this = this
            };
        }

        private void Update()
        {
            if (this.flicker)
                return;
            if ((Object) this.m_sl != (Object) null)
                this.m_sl.uLightRange = this.flickerRange + Mathf.PingPong(UnityEngine.Time.time * this.pulseSpeed, this.normalRange - this.flickerRange);
            else
                this.GetComponent<Light>().range = this.flickerRange + Mathf.PingPong(UnityEngine.Time.time * this.pulseSpeed, this.normalRange - this.flickerRange);
        }
    }

