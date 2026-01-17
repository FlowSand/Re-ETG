// Decompiled with JetBrains decompiler
// Type: AngryBookBasicAttack1
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
    [InspectorDropdownName("AngryBook/BasicAttack1")]
    public class AngryBookBasicAttack1 : Script
    {
      public int CircleBullets = 20;
      public int LineBullets = 12;
      public const float CircleRadius = 1.3f;
      public const float LineHalfDist = 1.6f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AngryBookBasicAttack1__Topc__Iterator0()
        {
          _this = this
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
          return (IEnumerator) new AngryBookBasicAttack1.DefaultBullet__Topc__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
