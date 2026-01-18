using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace Dungeonator
{
    public class RoomHandlerBoundingPolygon
    {
        private List<Vector2> m_points;

        public RoomHandlerBoundingPolygon(List<Vector2> points) => this.m_points = points;

        public RoomHandlerBoundingPolygon(List<Vector2> points, float inset)
        {
            this.m_points = new List<Vector2>();
            for (int index = 0; index < points.Count; ++index)
            {
                Vector2 point1 = points[(index + points.Count - 1) % points.Count];
                Vector2 point2 = points[index];
                Vector2 point3 = points[(index + 1) % points.Count];
                Vector2 vector2_1 = (point2 - point1).normalized;
                vector2_1 = new Vector2(vector2_1.y, -vector2_1.x);
                Vector2 vector2_2 = (point3 - point2).normalized;
                vector2_2 = new Vector2(vector2_2.y, -vector2_2.x);
                Vector2 normalized = ((vector2_1 + vector2_2) / 2f).normalized;
                this.m_points.Add(point2 + normalized * inset);
            }
        }

        public bool ContainsPoint(Vector2 point)
        {
            int num = this.m_points.Count - 1;
            int index1 = this.m_points.Count - 1;
            bool flag = false;
            for (int index2 = 0; index2 < this.m_points.Count; ++index2)
            {
                if (((double) this.m_points[index2].y < (double) point.y && (double) this.m_points[index1].y >= (double) point.y || (double) this.m_points[index1].y < (double) point.y && (double) this.m_points[index2].y >= (double) point.y) && ((double) this.m_points[index2].x <= (double) point.x || (double) this.m_points[index1].x <= (double) point.x))
                    flag ^= (double) this.m_points[index2].x + ((double) point.y - (double) this.m_points[index2].y) / ((double) this.m_points[index1].y - (double) this.m_points[index2].y) * ((double) this.m_points[index1].x - (double) this.m_points[index2].x) < (double) point.x;
                index1 = index2;
            }
            return flag;
        }
    }
}
