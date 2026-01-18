// Decompiled with JetBrains decompiler
// Type: BirdEggVomit2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bird/EggVomit2")]
public class BirdEggVomit2 : Script
  {
    private const int ClusterBullets = 0;
    private const float ClusterRotation = 150f;
    private const float ClusterRadius = 0.5f;
    private const float ClusterRadiusSpeed = 2f;
    private const int InnerBullets = 12;
    private const int InnerBounceTime = 30;

    protected override IEnumerator Top()
    {
      float num = BraveMathCollege.ClampAngle360(this.Direction);
      this.Fire(new Brave.BulletScript.Direction((double) num <= 90.0 || (double) num > 180.0 ? 20f : 160f), new Brave.BulletScript.Speed(2f), (Bullet) new BirdEggVomit2.EggBullet());
      return (IEnumerator) null;
    }

    public class EggBullet : Bullet
    {
      private bool spawnedBursts;

      public EggBullet()
        : base("egg")
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BirdEggVomit2.EggBullet__Topc__Iterator0()
        {
          _this = this
        };
      }

      public override void OnBulletDestruction(
        Bullet.DestroyType destroyType,
        SpeculativeRigidbody hitRigidbody,
        bool preventSpawningProjectiles)
      {
        if (this.spawnedBursts || preventSpawningProjectiles)
          return;
        this.SpawnBursts();
      }

      private void SpawnBursts()
      {
        float num = float.PositiveInfinity;
        for (int index = 0; index < 0; ++index)
          this.Fire(new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(9f), (Bullet) new BirdEggVomit2.ClusterBullet((float) index * num));
        for (int index = 0; index < 12; ++index)
          this.Fire(new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(9f), (Bullet) new BirdEggVomit2.InnerBullet());
        this.spawnedBursts = true;
      }
    }

    public class ClusterBullet : Bullet
    {
      private float clusterAngle;

      public ClusterBullet(float clusterAngle)
        : base()
      {
        this.clusterAngle = clusterAngle;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BirdEggVomit2.ClusterBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

    public class InnerBullet : Bullet
    {
      public InnerBullet()
        : base()
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BirdEggVomit2.InnerBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

