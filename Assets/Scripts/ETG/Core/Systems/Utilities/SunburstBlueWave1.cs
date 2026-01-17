// Decompiled with JetBrains decompiler
// Type: SunburstBlueWave1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Sunburst/BlueWave1")]
    public class SunburstBlueWave1 : Script
    {
      protected override IEnumerator Top()
      {
        float aimDirection = this.AimDirection;
        this.Fire(new Offset(y: 0.66f, rotation: aimDirection, transform: string.Empty), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(9f));
        this.Fire(new Offset(0.66f, rotation: aimDirection, transform: string.Empty), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(9f));
        this.Fire(new Offset(y: -0.66f, rotation: aimDirection, transform: string.Empty), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(9f));
        return (IEnumerator) null;
      }
    }

}
