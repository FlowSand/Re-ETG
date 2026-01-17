// Decompiled with JetBrains decompiler
// Type: BulletKingDirectedFire
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public abstract class BulletKingDirectedFire : Script
    {
      public bool IsHard => this is BulletKingDirectedFireHard;

      protected void DirectedShots(float x, float y, float direction)
      {
        direction -= 90f;
        if (this.IsHard)
          direction += 15f;
        this.Fire(new Offset(x, y, transform: string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(!this.IsHard ? 12f : 16f), new Bullet("directedfire"));
        if (!this.IsHard)
          return;
        direction += 30f;
        this.Fire(new Offset(x, y, transform: string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(!this.IsHard ? 12f : 16f), new Bullet("directedfire"));
      }
    }

}
