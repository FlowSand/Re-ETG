// Decompiled with JetBrains decompiler
// Type: BossStatuesDirectionalWaveAllSimple
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/BossStatues/DirectionalWaveAllSimple")]
    public class BossStatuesDirectionalWaveAllSimple : Script
    {
      protected override IEnumerator Top()
      {
        this.Fire(new Offset("top 0"), new Brave.BulletScript.Direction(100f), new Brave.BulletScript.Speed(9f), (Bullet) new BossStatuesDirectionalWaveAllSimple.EggBullet());
        this.Fire(new Offset("top 1"), new Brave.BulletScript.Direction(90f), new Brave.BulletScript.Speed(9f), (Bullet) new BossStatuesDirectionalWaveAllSimple.EggBullet());
        this.Fire(new Offset("top 2"), new Brave.BulletScript.Direction(80f), new Brave.BulletScript.Speed(9f), (Bullet) new BossStatuesDirectionalWaveAllSimple.EggBullet());
        this.Fire(new Offset("right 0"), new Brave.BulletScript.Direction(10f), new Brave.BulletScript.Speed(9f), (Bullet) new BossStatuesDirectionalWaveAllSimple.EggBullet());
        this.Fire(new Offset("right 1"), new Brave.BulletScript.Direction(), new Brave.BulletScript.Speed(9f), (Bullet) new BossStatuesDirectionalWaveAllSimple.EggBullet());
        this.Fire(new Offset("right 2"), new Brave.BulletScript.Direction(-10f), new Brave.BulletScript.Speed(9f), (Bullet) new BossStatuesDirectionalWaveAllSimple.EggBullet());
        this.Fire(new Offset("bottom 0"), new Brave.BulletScript.Direction(-80f), new Brave.BulletScript.Speed(9f), (Bullet) new BossStatuesDirectionalWaveAllSimple.EggBullet());
        this.Fire(new Offset("bottom 1"), new Brave.BulletScript.Direction(-90f), new Brave.BulletScript.Speed(9f), (Bullet) new BossStatuesDirectionalWaveAllSimple.EggBullet());
        this.Fire(new Offset("bottom 2"), new Brave.BulletScript.Direction(-100f), new Brave.BulletScript.Speed(9f), (Bullet) new BossStatuesDirectionalWaveAllSimple.EggBullet());
        this.Fire(new Offset("left 0"), new Brave.BulletScript.Direction(190f), new Brave.BulletScript.Speed(9f), (Bullet) new BossStatuesDirectionalWaveAllSimple.EggBullet());
        this.Fire(new Offset("left 1"), new Brave.BulletScript.Direction(180f), new Brave.BulletScript.Speed(9f), (Bullet) new BossStatuesDirectionalWaveAllSimple.EggBullet());
        this.Fire(new Offset("left 2"), new Brave.BulletScript.Direction(170f), new Brave.BulletScript.Speed(9f), (Bullet) new BossStatuesDirectionalWaveAllSimple.EggBullet());
        BossStatuesChaos1.AntiCornerShot((Script) this);
        return (IEnumerator) null;
      }

      public class EggBullet : Bullet
      {
        public EggBullet()
          : base("egg")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BossStatuesDirectionalWaveAllSimple.EggBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
