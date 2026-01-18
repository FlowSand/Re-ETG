using UnityEngine;

#nullable disable

public class ActiveKnockbackData
    {
        public Vector2 initialKnockback;
        public Vector2 knockback;
        public float elapsedTime;
        public float curveTime;
        public AnimationCurve curveFalloff;
        public GameObject sourceObject;
        public bool immutable;

        public ActiveKnockbackData(Vector2 k, float t, bool i)
        {
            this.knockback = k;
            this.initialKnockback = k;
            this.curveTime = t;
            this.immutable = i;
        }

        public ActiveKnockbackData(Vector2 k, AnimationCurve curve, float t, bool i)
        {
            this.knockback = k;
            this.initialKnockback = k;
            this.curveFalloff = curve;
            this.curveTime = t;
            this.immutable = i;
        }
    }

