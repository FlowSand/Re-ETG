using UnityEngine;

#nullable disable
namespace Brave.BulletScript
{
    public class Direction : IFireParam
    {
        public DirectionType type;
        public float direction;
        public float maxFrameDelta;

        public Direction(float direction = 0.0f, DirectionType type = DirectionType.Absolute, float maxFrameDelta = -1f)
        {
            this.direction = direction;
            this.type = type;
            this.maxFrameDelta = maxFrameDelta;
        }

        public float GetDirection(Bullet bullet, float? overrideBaseDirection = null)
        {
            float direction = this.type != DirectionType.Aim ? (this.type == DirectionType.Relative || this.type == DirectionType.Sequence ? (!overrideBaseDirection.HasValue ? bullet.Direction : overrideBaseDirection.Value) + this.direction : this.direction) : (bullet.BulletManager.PlayerPosition() - bullet.Position).ToAngle() + this.direction;
            if ((double) this.maxFrameDelta > 0.0)
            {
                float num = BraveMathCollege.ClampAngle180(direction - bullet.Direction);
                direction = bullet.Direction + Mathf.Clamp(num, -this.maxFrameDelta, this.maxFrameDelta);
            }
            return direction;
        }
    }
}
