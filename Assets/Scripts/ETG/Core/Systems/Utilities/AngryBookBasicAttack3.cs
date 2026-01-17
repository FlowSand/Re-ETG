// Decompiled with JetBrains decompiler
// Type: AngryBookBasicAttack3
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
    [InspectorDropdownName("AngryBook/BasicAttack3")]
    public class AngryBookBasicAttack3 : Script
    {
      public int LineBullets = 6;
      public int EdgeBullets = 4;
      public int CircleBullets = 6;
      public int StemBullets = 6;
      public const float Height = 2f;
      public const float Width = 1.5f;
      public const float CircleRadius = 0.5f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AngryBookBasicAttack3.<Top>c__Iterator0()
        {
          $this = this
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
          return (IEnumerator) new AngryBookBasicAttack3.DefaultBullet.<Top>c__Iterator0()
          {
            $this = this
          };
        }
      }
    }

}
