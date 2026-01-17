// Decompiled with JetBrains decompiler
// Type: Brave.BulletScript.Speed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
