// Decompiled with JetBrains decompiler
// Type: TutorialTurretBlankRoom
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
    [InspectorDropdownName("TutorialTurret/BlankRoom")]
    public class TutorialTurretBlankRoom : Script
    {
      public int CircleBullets = 20;
      public int LineBullets = 12;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TutorialTurretBlankRoom.<Top>c__Iterator0()
        {
          _this = this
        };
      }

      public class WarpBullet : Bullet
      {
        private bool m_doWarp;

        public WarpBullet(bool doWarp)
          : base()
        {
          this.m_doWarp = doWarp;
        }

        protected override IEnumerator Top()
        {
          if (this.m_doWarp)
          {
            TutorialTurretBlankRoom.WarpBullet warpBullet = this;
            warpBullet.Position = warpBullet.Position + new Vector2(-0.75f, 0.0f);
          }
          this.Position = this.Position.WithY(BraveMathCollege.QuantizeFloat(this.Position.y, 1f / 16f));
          return (IEnumerator) null;
        }
      }
    }

}
