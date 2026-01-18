using UnityEngine;

#nullable disable

public class SimpleLightIntensityCurve : MonoBehaviour
    {
        public float Duration = 1f;
        public float MinIntensity;
        public float MaxIntensity = 1f;
        [CurveRange(0.0f, 0.0f, 1f, 1f)]
        public AnimationCurve Curve;
        protected Light m_light;
        protected float m_elapsed;

        private void Start()
        {
            this.m_light = this.GetComponent<Light>();
            this.m_light.intensity = this.Curve.Evaluate(0.0f) * (this.MaxIntensity - this.MinIntensity) + this.MinIntensity;
        }

        private void Update()
        {
            this.m_elapsed += BraveTime.DeltaTime;
            if ((double) this.m_elapsed < (double) this.Duration)
            {
                this.m_light.intensity = this.Curve.Evaluate(this.m_elapsed / this.Duration) * (this.MaxIntensity - this.MinIntensity) + this.MinIntensity;
            }
            else
            {
                this.m_light.intensity = this.Curve.Evaluate(1f) * (this.MaxIntensity - this.MinIntensity) + this.MinIntensity;
                Object.Destroy((Object) this);
            }
        }
    }

