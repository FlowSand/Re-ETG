// Decompiled with JetBrains decompiler
// Type: EnemyBulletSpeedItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class EnemyBulletSpeedItem : PassiveItem
  {
    public float percentageSpeedReduction = 0.25f;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      base.Pickup(player);
      Projectile.BaseEnemyBulletSpeedMultiplier *= Mathf.Clamp01(1f - this.percentageSpeedReduction);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      debrisObject.GetComponent<EnemyBulletSpeedItem>().m_pickedUpThisRun = true;
      Projectile.BaseEnemyBulletSpeedMultiplier /= Mathf.Clamp01(1f - this.percentageSpeedReduction);
      return debrisObject;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

