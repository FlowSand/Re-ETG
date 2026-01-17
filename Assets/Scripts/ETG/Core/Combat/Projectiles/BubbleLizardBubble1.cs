// Decompiled with JetBrains decompiler
// Type: BubbleLizardBubble1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    [InspectorDropdownName("BubbleLizard/Bubble1")]
    public class BubbleLizardBubble1 : Script
    {
      private const float WaftXPeriod = 3f;
      private const float WaftXMagnitude = 1f;
      private const float WaftYPeriod = 1f;
      private const float WaftYMagnitude = 0.25f;
      private const int BubbleLifeTime = 960;

      protected override IEnumerator Top()
      {
        this.Fire(new Brave.BulletScript.Direction(), new Brave.BulletScript.Speed(2f), (Bullet) new BubbleLizardBubble1.BubbleBullet());
        return (IEnumerator) null;
      }

      public class BubbleBullet : Bullet
      {
        public BubbleBullet()
          : base("bubble")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BubbleLizardBubble1.BubbleBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }

        public override void OnBulletDestruction(
          Bullet.DestroyType destroyType,
          SpeculativeRigidbody hitRigidbody,
          bool preventSpawningProjectiles)
        {
          if (preventSpawningProjectiles)
            return;
          this.Fire(new Brave.BulletScript.Direction(this.GetAimDirection(1f, 14f)), new Brave.BulletScript.Speed(14f), (Bullet) null);
        }
      }
    }

}
