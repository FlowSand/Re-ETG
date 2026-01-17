// Decompiled with JetBrains decompiler
// Type: BossStatuesChaos1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/BossStatues/Chaos1")]
    public class BossStatuesChaos1 : Script
    {
      protected override IEnumerator Top()
      {
        this.Fire(new Offset("top 1"), new Brave.BulletScript.Direction(90f), new Brave.BulletScript.Speed(7.5f), (Bullet) new BossStatuesChaos1.EggBullet());
        this.Fire(new Offset("top 1"), new Brave.BulletScript.Direction(90f), new Brave.BulletScript.Speed(6f), (Bullet) new BossStatuesChaos1.EggBullet());
        this.Fire(new Offset("right 1"), new Brave.BulletScript.Direction(), new Brave.BulletScript.Speed(7.5f), (Bullet) new BossStatuesChaos1.EggBullet());
        this.Fire(new Offset("right 1"), new Brave.BulletScript.Direction(), new Brave.BulletScript.Speed(6f), (Bullet) new BossStatuesChaos1.EggBullet());
        this.Fire(new Offset("bottom 1"), new Brave.BulletScript.Direction(-90f), new Brave.BulletScript.Speed(7.5f), (Bullet) new BossStatuesChaos1.EggBullet());
        this.Fire(new Offset("bottom 1"), new Brave.BulletScript.Direction(-90f), new Brave.BulletScript.Speed(6f), (Bullet) new BossStatuesChaos1.EggBullet());
        this.Fire(new Offset("left 1"), new Brave.BulletScript.Direction(180f), new Brave.BulletScript.Speed(7.5f), (Bullet) new BossStatuesChaos1.EggBullet());
        this.Fire(new Offset("left 1"), new Brave.BulletScript.Direction(180f), new Brave.BulletScript.Speed(6f), (Bullet) new BossStatuesChaos1.EggBullet());
        BossStatuesChaos1.AntiCornerShot((Script) this);
        return (IEnumerator) null;
      }

      public static void AntiCornerShot(Script parentScript)
      {
        if ((double) Random.value > 0.33000001311302185)
          return;
        float aimDirection = parentScript.AimDirection;
        string transform = "top 1";
        switch (BraveMathCollege.AngleToQuadrant(aimDirection))
        {
          case 0:
            transform = "top 1";
            break;
          case 1:
            transform = "right 1";
            break;
          case 2:
            transform = "bottom 1";
            break;
          case 3:
            transform = "left 1";
            break;
        }
        parentScript.Fire(new Offset(transform), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(7.5f), new Bullet("egg"));
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
          return (IEnumerator) new BossStatuesChaos1.EggBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
