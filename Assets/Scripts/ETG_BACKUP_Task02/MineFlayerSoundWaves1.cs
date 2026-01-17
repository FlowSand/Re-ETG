// Decompiled with JetBrains decompiler
// Type: MineFlayerSoundWaves1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/MineFlayer/SoundWaves1")]
public class MineFlayerSoundWaves1 : Script
{
  private const int NumWaves = 5;
  private const int NumBullets = 18;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new MineFlayerSoundWaves1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private class ReflectBullet : Bullet
  {
    private int m_ticksLeft = -1;

    public ReflectBullet()
      : base("bounce")
    {
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MineFlayerSoundWaves1.ReflectBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }

    private void OnTileCollision(CollisionData tilecollision) => this.Reflect();

    private void Reflect()
    {
      this.Speed = 8f;
      this.Direction += 180f + Random.Range(-10f, 10f);
      this.Velocity = BraveMathCollege.DegreesToVector(this.Direction, this.Speed);
      PhysicsEngine.PostSliceVelocity = new Vector2?(this.Velocity);
      this.m_ticksLeft = (int) ((double) this.Tick * 1.5);
      if ((bool) (Object) this.Projectile.TrailRendererController)
        this.Projectile.TrailRendererController.Stop();
      this.Projectile.BulletScriptSettings.surviveTileCollisions = false;
      this.Projectile.specRigidbody.OnTileCollision -= new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision);
    }

    public override void OnBulletDestruction(
      Bullet.DestroyType destroyType,
      SpeculativeRigidbody hitRigidbody,
      bool preventSpawningProjectiles)
    {
      if (!(bool) (Object) this.Projectile)
        return;
      this.Projectile.specRigidbody.OnTileCollision -= new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision);
    }
  }
}
