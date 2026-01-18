using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Tweens/Color")]
public class dfTweenColor : dfTweenComponent<Color>
    {
        public override Color offset(Color lhs, Color rhs) => lhs + rhs;

        public override Color evaluate(Color startValue, Color endValue, float time)
        {
            Vector4 vector4_1 = (Vector4) startValue;
            Vector4 vector4_2 = (Vector4) endValue;
            return (Color) new Vector4(dfTweenComponent<Color>.Lerp(vector4_1.x, vector4_2.x, time), dfTweenComponent<Color>.Lerp(vector4_1.y, vector4_2.y, time), dfTweenComponent<Color>.Lerp(vector4_1.z, vector4_2.z, time), dfTweenComponent<Color>.Lerp(vector4_1.w, vector4_2.w, time));
        }
    }

