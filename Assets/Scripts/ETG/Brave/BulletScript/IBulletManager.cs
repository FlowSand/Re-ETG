// Decompiled with JetBrains decompiler
// Type: Brave.BulletScript.IBulletManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Brave.BulletScript
{
  public interface IBulletManager
  {
    Vector2 PlayerPosition();

    Vector2 PlayerVelocity();

    float TimeScale { get; set; }

    void BulletSpawnedHandler(Bullet bullet);

    void RemoveBullet(Bullet bullet);

    void DestroyBullet(Bullet deadBullet, bool suppressInAirEffects);

    Vector2 TransformOffset(Vector2 parentPos, string transform);

    float GetTransformRotation(string transform);

    Animation GetUnityAnimation();
  }
}
