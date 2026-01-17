// Decompiled with JetBrains decompiler
// Type: BashelliskRandomShots1
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
    [InspectorDropdownName("Bosses/Bashellisk/RandomShots1")]
    public class BashelliskRandomShots1 : Script
    {
      public int NumBullets = 5;
      public float BulletSpeed = 10f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BashelliskRandomShots1.<Top>c__Iterator0()
        {
          $this = this
        };
      }
    }

}
