#nullable disable
namespace DaikonForge.Tween.Interpolation
{
    public class FloatInterpolator : Interpolator<float>
    {
        protected static FloatInterpolator singleton;

        public override float Add(float lhs, float rhs) => lhs + rhs;

        public override float Interpolate(float startValue, float endValue, float time)
        {
            return startValue + (endValue - startValue) * time;
        }

        public static Interpolator<float> Default
        {
            get
            {
                if (FloatInterpolator.singleton == null)
                    FloatInterpolator.singleton = new FloatInterpolator();
                return (Interpolator<float>) FloatInterpolator.singleton;
            }
        }
    }
}
