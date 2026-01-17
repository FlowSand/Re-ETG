// Decompiled with JetBrains decompiler
// Type: BulletShotgunManRedBasicAttack1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("BulletShotgunMan/RedBasicAttack1")]
    public class BulletShotgunManRedBasicAttack1 : Script
    {
      protected override IEnumerator Top()
      {
        for (int index = -2; index <= 2; ++index)
          this.Fire(new Brave.BulletScript.Direction((float) (index * 6), Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(5f), (Bullet) null);
        return (IEnumerator) null;
      }
    }

}
