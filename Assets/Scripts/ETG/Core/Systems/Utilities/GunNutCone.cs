// Decompiled with JetBrains decompiler
// Type: GunNutCone
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
    [InspectorDropdownName("GunNut/Cone1")]
    public class GunNutCone : Script
    {
      private const int NumBulletsMainWave = 25;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GunNutCone__Topc__Iterator0()
        {
          _this = this
        };
      }

      private void FireCluster(float direction)
      {
        this.Fire(new Offset(0.5f, rotation: direction, transform: string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(12f));
        this.Fire(new Offset(0.275f, 0.25f, direction, string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(12f));
        this.Fire(new Offset(0.275f, -0.25f, direction, string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(12f));
        this.Fire(new Offset(y: 0.4f, rotation: direction, transform: string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(12f));
        this.Fire(new Offset(y: -0.4f, rotation: direction, transform: string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(12f));
      }
    }

}
