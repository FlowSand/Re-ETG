// Decompiled with JetBrains decompiler
// Type: BlizzbulonBasicAttack1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Blizzbulon/BasicAttack1")]
    public class BlizzbulonBasicAttack1 : Script
    {
      private const int NumBullets = 12;

      protected override IEnumerator Top()
      {
        float num = 30f;
        for (int index = 0; index < 12; ++index)
          this.Fire(new Brave.BulletScript.Direction((float) index * num), new Brave.BulletScript.Speed(6f), (Bullet) null);
        return (IEnumerator) null;
      }
    }

}
