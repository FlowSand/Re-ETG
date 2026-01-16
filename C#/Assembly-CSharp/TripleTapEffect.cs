// Decompiled with JetBrains decompiler
// Type: TripleTapEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class TripleTapEffect : MonoBehaviour
{
  public int RequiredSequentialShots = 3;
  public int AmmoToGain = 1;
  private int m_shotCounter;
  private AIActor m_companion;
  private PlayerController m_player;
  private Dictionary<float, int> m_slicesFired = new Dictionary<float, int>();

  private void Start()
  {
    this.m_companion = this.GetComponent<AIActor>();
    PlayerController companionOwner = this.m_companion.CompanionOwner;
    if (!(bool) (UnityEngine.Object) companionOwner)
      return;
    this.m_player = companionOwner;
    this.m_player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
  }

  private void PostProcessProjectile(Projectile sourceProjectile, float effectChanceScalar)
  {
    if ((double) sourceProjectile.PlayerProjectileSourceGameTimeslice == -1.0)
      return;
    if (this.m_slicesFired.ContainsKey(sourceProjectile.PlayerProjectileSourceGameTimeslice))
      this.m_slicesFired[sourceProjectile.PlayerProjectileSourceGameTimeslice] = this.m_slicesFired[sourceProjectile.PlayerProjectileSourceGameTimeslice] + 1;
    else
      this.m_slicesFired.Add(sourceProjectile.PlayerProjectileSourceGameTimeslice, 1);
    sourceProjectile.OnDestruction += new Action<Projectile>(this.HandleProjectileDestruction);
  }

  private void HandleProjectileDestruction(Projectile source)
  {
    if ((double) source.PlayerProjectileSourceGameTimeslice == -1.0 || !this.m_slicesFired.ContainsKey(source.PlayerProjectileSourceGameTimeslice) || !(bool) (UnityEngine.Object) this.m_player || !(bool) (UnityEngine.Object) source)
      return;
    if (source.HasImpactedEnemy)
    {
      this.m_slicesFired.Remove(source.PlayerProjectileSourceGameTimeslice);
      if (this.m_player.HasActiveBonusSynergy(CustomSynergyType.GET_IT_ITS_BOWLING))
        this.m_shotCounter = Mathf.Min(this.RequiredSequentialShots, this.m_shotCounter + source.NumberHealthHaversHit);
      else
        ++this.m_shotCounter;
      if (this.m_shotCounter < this.RequiredSequentialShots)
        return;
      this.m_shotCounter -= this.RequiredSequentialShots;
      if (!(bool) (UnityEngine.Object) source.PossibleSourceGun || source.PossibleSourceGun.InfiniteAmmo || !source.PossibleSourceGun.CanGainAmmo)
        return;
      source.PossibleSourceGun.GainAmmo(this.AmmoToGain);
    }
    else
    {
      this.m_slicesFired[source.PlayerProjectileSourceGameTimeslice] = this.m_slicesFired[source.PlayerProjectileSourceGameTimeslice] - 1;
      if (this.m_slicesFired[source.PlayerProjectileSourceGameTimeslice] != 0)
        return;
      this.m_shotCounter = 0;
    }
  }

  private void OnDestroy()
  {
    if (!(bool) (UnityEngine.Object) this.m_player)
      return;
    this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
  }
}
