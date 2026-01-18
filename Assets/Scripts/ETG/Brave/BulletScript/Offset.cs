using UnityEngine;

#nullable disable
namespace Brave.BulletScript
{
    public class Offset : IFireParam
    {
        public float x;
        public float y;
        public string transform;
        public float rotation;
        public DirectionType directionType;
        private Vector2? m_overridePosition;

        public Offset(float x = 0.0f, float y = 0.0f, float rotation = 0.0f, string transform = "", DirectionType directionType = DirectionType.Absolute)
        {
            this.x = x;
            this.y = y;
            this.rotation = rotation;
            this.transform = transform;
            this.directionType = directionType;
        }

        public Offset(Vector2 offset, float rotation = 0.0f, string transform = "", DirectionType directionType = DirectionType.Absolute)
        {
            this.x = offset.x;
            this.y = offset.y;
            this.rotation = rotation;
            this.transform = transform;
            this.directionType = directionType;
        }

        public Offset(string transform)
        {
            this.x = 0.0f;
            this.y = 0.0f;
            this.rotation = 0.0f;
            this.transform = transform;
            this.directionType = DirectionType.Relative;
        }

        public Vector2 GetPosition(Bullet bullet)
        {
            if (this.m_overridePosition.HasValue)
                return this.m_overridePosition.Value;
            Vector2 vector2 = bullet.Position;
            if (!string.IsNullOrEmpty(this.transform))
                vector2 = bullet.BulletManager.TransformOffset(bullet.Position, this.transform);
            Vector2 v = new Vector2(this.x, this.y);
            if ((double) this.rotation != 0.0)
                v = v.Rotate(this.rotation);
            if (this.directionType != DirectionType.Absolute)
            {
                if (this.directionType == DirectionType.Relative)
                    v = v.Rotate(bullet.Direction);
                else
                    Debug.LogError((object) "Cannot use DirectionType {0} in an Offset instance.");
            }
            return vector2 + v;
        }

        public float? GetDirection(Bullet bullet)
        {
            return string.IsNullOrEmpty(this.transform) ? new float?() : new float?(bullet.BulletManager.GetTransformRotation(this.transform));
        }

        public string GetTransformValue() => this.transform;

        public static Offset OverridePosition(Vector2 overridePosition)
        {
            return new Offset(0.0f, 0.0f, 0.0f, string.Empty, DirectionType.Absolute)
            {
                m_overridePosition = new Vector2?(overridePosition)
            };
        }
    }
}
