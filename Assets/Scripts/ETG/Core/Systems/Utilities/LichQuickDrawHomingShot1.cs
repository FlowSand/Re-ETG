// Decompiled with JetBrains decompiler
// Type: LichQuickDrawHomingShot1
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
    [InspectorDropdownName("Bosses/Lich/QuickDrawHomingShot1")]
    public class LichQuickDrawHomingShot1 : Script
    {
      protected override IEnumerator Top()
      {
        this.Fire(new Brave.BulletScript.Direction(this.GetAimDirection(1f, 16f)), new Brave.BulletScript.Speed(16f), (Bullet) new LichQuickDrawHomingShot1.FastHomingShot());
        return (IEnumerator) null;
      }

      public class FastHomingShot : Bullet
      {
        public FastHomingShot()
          : base("quickHoming")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new LichQuickDrawHomingShot1.FastHomingShot.<Top>c__Iterator0()
          {
            $this = this
          };
        }
      }
    }

}
