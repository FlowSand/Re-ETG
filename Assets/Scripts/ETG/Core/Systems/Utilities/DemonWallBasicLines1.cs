// Decompiled with JetBrains decompiler
// Type: DemonWallBasicLines1
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
    [InspectorDropdownName("Bosses/DemonWall/BasicLines1")]
    public class DemonWallBasicLines1 : Script
    {
      public static string[][] shootPoints = new string[3][]
      {
        new string[3]{ "sad bullet", "blobulon", "dopey bullet" },
        new string[3]{ "left eye", "right eye", "crashed bullet" },
        new string[4]
        {
          "sideways bullet",
          "shotgun bullet",
          "cultist",
          "angry bullet"
        }
      };
      public const int NumBursts = 10;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DemonWallBasicLines1.<Top>c__Iterator0()
        {
          $this = this
        };
      }

      private void FireLine(string transform, int wallLine)
      {
        for (int index1 = 0; index1 < 5; ++index1)
        {
          for (int index2 = 0; index2 < 3; ++index2)
            this.Fire(new Offset(transform), new Brave.BulletScript.Direction(-90f), new Brave.BulletScript.Speed((float) (9.0 - (double) index1 * 1.5)), (Bullet) new DemonWallBasicLines1.LineBullet(index1 == 0, (float) (index2 - 1)));
        }
        if (wallLine == 0)
          return;
        Vector2 offset = wallLine >= 0 ? new Vector2(23.75f, 3f) : new Vector2(0.5f, 3f);
        for (int index = 0; index < 5; ++index)
          this.Fire(new Offset(offset, transform: string.Empty), new Brave.BulletScript.Direction(-90f), new Brave.BulletScript.Speed((float) (9.0 - (double) index * 1.5)), (Bullet) new DemonWallBasicLines1.LineBullet(true, 0.0f));
      }

      private void FireCrossBullets(string transform, float angle)
      {
        for (int index = 0; index < 2; ++index)
          this.Fire(new Offset(transform), new Brave.BulletScript.Direction(angle + (float) Random.Range(-30, 30)), new Brave.BulletScript.Speed((float) (2 + 5 * index)), (Bullet) new SpeedChangingBullet("wave", 7f, 90, suppressVfx: index != 3));
      }

      public class LineBullet : Bullet
      {
        private float m_horizontalSign;

        public LineBullet(bool doVfx, float horizontalSign)
          : base("line")
        {
          this.SuppressVfx = !doVfx;
          this.m_horizontalSign = horizontalSign;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new DemonWallBasicLines1.LineBullet.<Top>c__Iterator0()
          {
            $this = this
          };
        }
      }
    }

}
