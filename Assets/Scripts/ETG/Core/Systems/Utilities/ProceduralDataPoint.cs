using System;

using UnityEngine;

#nullable disable

[Serializable]
public class ProceduralDataPoint
    {
        public float minValue;
        public float maxValue;
        public AnimationCurve distribution;
        private const float INTEGRATION_STEP = 0.01f;

        public ProceduralDataPoint(float min, float max)
        {
            this.minValue = min;
            this.maxValue = max;
            this.distribution = new AnimationCurve();
        }

        private float FullIntegortion()
        {
            float num = 0.0f;
            for (float time = 0.01f; (double) time <= 1.0; time += 0.01f)
                num += 0.01f * this.distribution.Evaluate(time);
            return num;
        }

        private float PartialIntegortion(float target)
        {
            float num1 = this.FullIntegortion();
            float num2 = 0.0f;
            for (float time = 0.01f; (double) time <= 1.0; time += 0.01f)
            {
                num2 += 0.01f * this.distribution.Evaluate(time);
                if ((double) num2 / (double) num1 > (double) target)
                    return time;
            }
            return 1f;
        }

        public float GetSpecificValue(float p)
        {
            return this.minValue + this.PartialIntegortion(p) * (this.maxValue - this.minValue);
        }

        public int GetSpecificIntValue(float p) => Mathf.RoundToInt(this.GetSpecificValue(p));

        public float GetRandomValue() => this.GetSpecificValue(UnityEngine.Random.value);

        public int GetRandomIntValue() => Mathf.RoundToInt(this.GetRandomValue());
    }

