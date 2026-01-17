// Decompiled with JetBrains decompiler
// Type: BulletShotgunManDeathBurst1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("BulletShotgunMan/DeathBurst1")]
    public class BulletShotgunManDeathBurst1 : Script
    {
      protected override IEnumerator Top()
      {
        for (int index = 0; index <= 6; ++index)
          this.Fire(new Brave.BulletScript.Direction((float) (index * 60)), new Brave.BulletScript.Speed(6.5f), new Bullet("flashybullet"));
        return (IEnumerator) null;
      }
    }

}
