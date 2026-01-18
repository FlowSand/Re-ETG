using UnityEngine;

#nullable disable

    public static class dfRectExtensions
    {
        public static RectOffset ConstrainPadding(this RectOffset borders)
        {
            if (borders == null)
                return new RectOffset();
            borders.left = Mathf.Max(0, borders.left);
            borders.right = Mathf.Max(0, borders.right);
            borders.top = Mathf.Max(0, borders.top);
            borders.bottom = Mathf.Max(0, borders.bottom);
            return borders;
        }

        public static bool IsEmpty(this Rect rect)
        {
            return (double) rect.xMin == (double) rect.xMax || (double) rect.yMin == (double) rect.yMax;
        }

        public static Rect Intersection(this Rect a, Rect b)
        {
            if (!a.Intersects(b))
                return new Rect();
            float xmin = Mathf.Max(a.xMin, b.xMin);
            float xmax = Mathf.Min(a.xMax, b.xMax);
            float ymin = Mathf.Max(a.yMin, b.yMin);
            float ymax = Mathf.Min(a.yMax, b.yMax);
            return Rect.MinMaxRect(xmin, ymin, xmax, ymax);
        }

        public static Rect Union(this Rect a, Rect b)
        {
            float xmin = Mathf.Min(a.xMin, b.xMin);
            float xmax = Mathf.Max(a.xMax, b.xMax);
            float ymin = Mathf.Min(a.yMin, b.yMin);
            float ymax = Mathf.Max(a.yMax, b.yMax);
            return Rect.MinMaxRect(xmin, ymin, xmax, ymax);
        }

        public static bool Contains(this Rect rect, Rect other)
        {
            bool flag1 = (double) rect.x <= (double) other.x;
            bool flag2 = (double) rect.x + (double) rect.width >= (double) other.x + (double) other.width;
            bool flag3 = (double) rect.yMin <= (double) other.yMin;
            bool flag4 = (double) rect.y + (double) rect.height >= (double) other.y + (double) other.height;
            return flag1 && flag2 && flag3 && flag4;
        }

        public static bool Intersects(this Rect rect, Rect other)
        {
            return (double) rect.xMax >= (double) other.xMin && (double) rect.yMax >= (double) other.yMin && (double) rect.xMin <= (double) other.xMax && (double) rect.yMin <= (double) other.yMax;
        }

        public static Rect RoundToInt(this Rect rect)
        {
            return new Rect((float) Mathf.RoundToInt(rect.x), (float) Mathf.RoundToInt(rect.y), (float) Mathf.RoundToInt(rect.width), (float) Mathf.RoundToInt(rect.height));
        }

        public static string Debug(this Rect rect)
        {
            return $"[{rect.xMin},{rect.yMin},{rect.xMax},{rect.yMax}]";
        }
    }

