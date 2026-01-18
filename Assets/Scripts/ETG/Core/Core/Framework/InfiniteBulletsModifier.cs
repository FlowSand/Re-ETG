// Decompiled with JetBrains decompiler
// Type: InfiniteBulletsModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class InfiniteBulletsModifier : BraveBehaviour
  {
    public void Start()
    {
      this.projectile.OnDestruction += new Action<Projectile>(this.HandleDestruction);
    }

    private void HandleDestruction(Projectile p)
    {
      if (p.HasImpactedEnemy || !(bool) (UnityEngine.Object) p.PossibleSourceGun || !p.PossibleSourceGun.gameObject.activeSelf)
        return;
      p.PossibleSourceGun.GainAmmo(1);
      p.PossibleSourceGun.ForceFireProjectile(p);
    }
  }

