using UnityEngine;

#nullable disable
namespace BraveDynamicTree
{
    public struct b2AABB
    {
        public Vector2 lowerBound;
        public Vector2 upperBound;

        public b2AABB(float lowX, float lowY, float upperX, float upperY)
        {
            this.lowerBound.x = lowX;
            this.lowerBound.y = lowY;
            this.upperBound.x = upperX;
            this.upperBound.y = upperY;
        }

        public b2AABB(Vector2 lowerBound, Vector2 upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        public bool IsValid()
        {
            Vector2 vector2 = this.upperBound - this.lowerBound;
            return (double) vector2.x >= 0.0 && (double) vector2.y >= 0.0;
        }

        public Vector2 GetCenter() => 0.5f * (this.lowerBound + this.upperBound);

        public Vector2 GetExtents() => 0.5f * (this.upperBound - this.lowerBound);

        public float GetPerimeter()
        {
            return (float) (2.0 * ((double) (this.upperBound.x - this.lowerBound.x) + (double) (this.upperBound.y - this.lowerBound.y)));
        }

        public void Combine(b2AABB aabb)
        {
            this.lowerBound = Vector2.Min(this.lowerBound, aabb.lowerBound);
            this.upperBound = Vector2.Max(this.upperBound, aabb.upperBound);
        }

        public void Combine(b2AABB aabb1, b2AABB aabb2)
        {
            this.lowerBound = Vector2.Min(aabb1.lowerBound, aabb2.lowerBound);
            this.upperBound = Vector2.Max(aabb1.upperBound, aabb2.upperBound);
        }

        public bool Contains(b2AABB aabb)
        {
            return (double) this.lowerBound.x <= (double) aabb.lowerBound.x && (double) this.lowerBound.y <= (double) aabb.lowerBound.y && (double) aabb.upperBound.x <= (double) this.upperBound.x && (double) aabb.upperBound.y <= (double) this.upperBound.y;
        }

        public bool RayCast(ref b2RayCastOutput output, b2RayCastInput input)
        {
            float num1 = float.MinValue;
            float a = float.MaxValue;
            Vector2 p1 = input.p1;
            Vector2 vector = input.p2 - input.p1;
            Vector2 vector2 = vector.Abs();
            Vector2 zero = Vector2.zero;
            for (int index = 0; index < 2; ++index)
            {
                if ((double) vector2[index] < 1.4012984643248171E-45)
                {
                    if ((double) p1[index] < (double) this.lowerBound[index] || (double) this.upperBound[index] < (double) p1[index])
                        return false;
                }
                else
                {
                    float num2 = 1f / vector[index];
                    float num3 = (this.lowerBound[index] - p1[index]) * num2;
                    float b = (this.upperBound[index] - p1[index]) * num2;
                    float num4 = -1f;
                    if ((double) num3 > (double) b)
                    {
                        float num5 = num3;
                        num3 = b;
                        b = num5;
                        num4 = 1f;
                    }
                    if ((double) num3 > (double) num1)
                    {
                        zero = Vector2.zero;
                        zero[index] = num4;
                        num1 = num3;
                    }
                    a = Mathf.Min(a, b);
                    if ((double) num1 > (double) a)
                        return false;
                }
            }
            if ((double) num1 < 0.0 || (double) input.maxFraction < (double) num1)
                return false;
            output.fraction = num1;
            output.normal = zero;
            return true;
        }

        public static bool b2TestOverlap(ref b2AABB a, ref b2AABB b)
        {
            return (double) b.lowerBound.x <= (double) a.upperBound.x && (double) a.lowerBound.x <= (double) b.upperBound.x && (double) b.lowerBound.y <= (double) a.upperBound.y && (double) a.lowerBound.y <= (double) b.upperBound.y;
        }
    }
}
