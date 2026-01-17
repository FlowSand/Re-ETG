// Decompiled with JetBrains decompiler
// Type: SpectreGroupShot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class SpectreGroupShot : Script
    {
      private const int NumBullets = 4;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SpectreGroupShot.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private void FireFrom(string transform)
      {
        float aimDirection = this.GetAimDirection(transform, (float) Random.Range(0, 2), 8f);
        Vector2 unit = PhysicsEngine.PixelToUnit(new IntVector2(4, 0));
        this.Fire(new Offset(unit, transform: transform), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(8f), new Bullet("eyeBullet"));
        this.Fire(new Offset(-unit, transform: transform), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(8f), new Bullet("eyeBullet"));
      }
    }

}
