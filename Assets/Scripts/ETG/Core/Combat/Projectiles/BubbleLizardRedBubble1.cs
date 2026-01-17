// Decompiled with JetBrains decompiler
// Type: BubbleLizardRedBubble1
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
    [InspectorDropdownName("BubbleLizard/RedBubble1")]
    public class BubbleLizardRedBubble1 : Script
    {
      private const int NumBullets = 4;
      private const float WaftXPeriod = 3f;
      private const float WaftXMagnitude = 1f;
      private const float WaftYPeriod = 1f;
      private const float WaftYMagnitude = 0.25f;
      private const int BubbleLifeTime = 960;
      private const int DumbfireTime = 300;

      protected override IEnumerator Top()
      {
        float num1 = this.RandomAngle();
        float num2 = 90f;
        for (int index = 0; index < 4; ++index)
          this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(2f), (Bullet) new BubbleLizardRedBubble1.BubbleBullet());
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
          return (IEnumerator) new BubbleLizardRedBubble1.BubbleBullet.<Top>c__Iterator0()
          {
            _this = this
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
