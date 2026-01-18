using UnityEngine;

#nullable disable

public class StickyFrictionModifier
    {
        public float length;
        public float magnitude;
        public float elapsed;
        public bool usesFalloff = true;

        public StickyFrictionModifier(float l, float m, bool falloff = true)
        {
            this.length = l * GameManager.Options.StickyFrictionMultiplier;
            this.magnitude = Mathf.Clamp01(m);
            this.usesFalloff = falloff;
        }

        public float GetCurrentMagnitude()
        {
            if (!this.usesFalloff)
                return this.magnitude;
            float num = this.elapsed / this.length;
            return Mathf.Lerp(this.magnitude, 1f, Mathf.Clamp01(num * num * num));
        }
    }

