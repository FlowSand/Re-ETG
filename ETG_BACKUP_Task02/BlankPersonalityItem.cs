// Decompiled with JetBrains decompiler
// Type: BlankPersonalityItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class BlankPersonalityItem : PassiveItem
{
  [Range(0.0f, 100f)]
  public float ReturnAmmoToAllGunsPercentage = 5f;

  public override void Pickup(PlayerController player)
  {
    base.Pickup(player);
    this.m_owner = player;
    player.OnReceivedDamage += new Action<PlayerController>(this.HandleDamageReceived);
  }

  private void HandleDamageReceived(PlayerController source)
  {
    source.ForceBlank();
    if ((double) this.ReturnAmmoToAllGunsPercentage <= 0.0 || source.inventory == null || source.inventory.AllGuns == null)
      return;
    for (int index = 0; index < source.inventory.AllGuns.Count; ++index)
    {
      Gun allGun = source.inventory.AllGuns[index];
      if (!allGun.InfiniteAmmo && allGun.CanGainAmmo)
        allGun.GainAmmo(Mathf.CeilToInt((float) allGun.AdjustedMaxAmmo * 0.01f * this.ReturnAmmoToAllGunsPercentage));
    }
  }

  protected override void DisableEffect(PlayerController disablingPlayer)
  {
    if ((bool) (UnityEngine.Object) disablingPlayer)
      disablingPlayer.OnReceivedDamage -= new Action<PlayerController>(this.HandleDamageReceived);
    base.DisableEffect(disablingPlayer);
  }
}
