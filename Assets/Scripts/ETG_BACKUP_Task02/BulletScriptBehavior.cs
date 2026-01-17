// Decompiled with JetBrains decompiler
// Type: BulletScriptBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using UnityEngine;

#nullable disable
public class BulletScriptBehavior : BraveBehaviour
{
  public Bullet bullet;
  private int m_firstFrame = -1;

  public void Initialize(Bullet newBullet)
  {
    this.bullet = newBullet;
    this.m_firstFrame = -1;
    this.enabled = true;
    if ((bool) (Object) this.projectile)
    {
      this.bullet.AutoRotation = this.projectile.shouldRotate;
      this.projectile.braveBulletScript = this;
    }
    this.Update();
    this.m_firstFrame = UnityEngine.Time.frameCount;
  }

  public void Update()
  {
    if (this.m_firstFrame == UnityEngine.Time.frameCount || this.bullet == null)
      return;
    this.bullet.FrameUpdate();
    if (this.bullet == null)
      return;
    SpeculativeRigidbody specRigidbody = this.specRigidbody;
    if (this.bullet.DisableMotion)
    {
      if ((bool) (Object) specRigidbody)
        specRigidbody.Velocity = Vector2.zero;
    }
    else if ((bool) (Object) specRigidbody)
    {
      float deltaTime = BraveTime.DeltaTime;
      Vector2 predictedPosition = this.bullet.PredictedPosition;
      Vector2 unitPosition = specRigidbody.Position.UnitPosition;
      specRigidbody.Velocity.x = (predictedPosition.x - unitPosition.x) / deltaTime;
      specRigidbody.Velocity.y = (predictedPosition.y - unitPosition.y) / deltaTime;
    }
    else
      this.transform.position = (Vector3) this.bullet.PredictedPosition;
    if (!this.bullet.AutoRotation)
      return;
    this.transform.rotation = Quaternion.identity;
    this.transform.Rotate(0.0f, 0.0f, this.bullet.Direction);
  }

  protected override void OnDestroy() => base.OnDestroy();

  public void RemoveBullet()
  {
    if (this.bullet == null)
      return;
    this.bullet.OnForceRemoved();
    this.bullet.BulletManager.RemoveBullet(this.bullet);
  }

  public virtual void HandleBulletDestruction(
    Bullet.DestroyType destroyType,
    SpeculativeRigidbody hitRigidbody,
    bool allowProjectileSpawns)
  {
    if (this.bullet == null)
      return;
    if (destroyType != Bullet.DestroyType.DieInAir)
      this.bullet.Position = this.bullet.Projectile.specRigidbody.UnitCenter;
    this.bullet.HandleBulletDestruction(destroyType, hitRigidbody, allowProjectileSpawns);
  }

  public void OnDespawned() => this.bullet = (Bullet) null;
}
