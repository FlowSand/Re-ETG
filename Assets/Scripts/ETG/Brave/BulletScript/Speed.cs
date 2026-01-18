#nullable disable
namespace Brave.BulletScript
{
  public class Speed : IFireParam
  {
    public SpeedType type;
    public float speed;

    public Speed(float speed = 0.0f, SpeedType type = SpeedType.Absolute)
    {
      this.speed = speed;
      this.type = type;
    }

    public float GetSpeed(Bullet bullet)
    {
      return this.type == SpeedType.Relative || this.type == SpeedType.Sequence ? bullet.Speed + this.speed : this.speed;
    }
  }
}
