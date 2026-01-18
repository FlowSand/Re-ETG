using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class KaliberController : BraveBehaviour
    {
        private int m_headsLeft = 3;
        private float m_minHealth = 1f;
        private bool m_isTransitioning;

        public void Start()
        {
            this.m_minHealth = (float) Mathf.RoundToInt(this.healthHaver.GetMaxHealth() * 0.666f);
            this.healthHaver.minimumHealth = this.m_minHealth;
        }

        public void Update()
        {
            if (this.m_isTransitioning || (double) this.healthHaver.GetCurrentHealth() > (double) this.m_minHealth + 0.5)
                return;
            this.StartCoroutine(this.DestroyHead());
        }

        protected override void OnDestroy() => base.OnDestroy();

        [DebuggerHidden]
        private IEnumerator DestroyHead()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new KaliberController__DestroyHeadc__Iterator0()
            {
                _this = this
            };
        }
    }

