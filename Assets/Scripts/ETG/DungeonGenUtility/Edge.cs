using UnityEngine;

#nullable disable
namespace DungeonGenUtility
{
    public class Edge
    {
        private Vector2 v0;
        private Vector2 v1;

        public Edge(Vector2 vert0, Vector2 vert1)
        {
            this.v0 = vert0;
            this.v1 = vert1;
        }

        private float Length => Vector2.Distance(this.v0, this.v1);
    }
}
