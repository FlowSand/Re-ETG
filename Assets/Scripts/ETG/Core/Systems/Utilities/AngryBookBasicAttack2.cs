// Decompiled with JetBrains decompiler
// Type: AngryBookBasicAttack2
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
    [InspectorDropdownName("AngryBook/BasicAttack2")]
    public class AngryBookBasicAttack2 : Script
    {
      public int LineBullets = 10;
      public const float Height = 2.5f;
      public const float Width = 1.9f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AngryBookBasicAttack2.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public class DefaultBullet : Bullet
      {
        public int spawnTime;

        public DefaultBullet(int spawnTime)
          : base()
        {
          this.spawnTime = spawnTime;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new AngryBookBasicAttack2.DefaultBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
