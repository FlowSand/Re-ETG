// Decompiled with JetBrains decompiler
// Type: FusebombRandomRapid1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/Fusebomb/RandomBurstsRapid1")]
    public class FusebombRandomRapid1 : Script
    {
      private const int NumBullets = 8;
      private static bool s_offset;

      protected override IEnumerator Top()
      {
        float startAngle = this.RandomAngle();
        for (int i = 0; i < 8; ++i)
          this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(startAngle, 360f, 8, i)), new Brave.BulletScript.Speed(9f), (Bullet) null);
        return (IEnumerator) null;
      }
    }

}
