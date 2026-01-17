// Decompiled with JetBrains decompiler
// Type: ExoticSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class ExoticSynergyProcessor : MonoBehaviour
{
  [LongNumericEnum]
  public CustomSynergyType RequiredSynergy;
  public bool SnapsToAngleMultiple;
  public float AngleMultiple = 90f;
  public bool HasChanceToGainAmmo;
  public float ChanceToGainAmmo;
  public bool SetsFlying;
  public bool SetsGoopReloadFree;
  private Gun m_gun;
  private PlayerController m_cachedPlayer;

  public void Awake()
  {
    this.m_gun = this.GetComponent<Gun>();
    if (!this.HasChanceToGainAmmo)
      return;
    this.m_gun.PostProcessProjectile += new Action<Projectile>(this.HandleGainAmmo);
  }

  private void HandleGainAmmo(Projectile obj)
  {
    if (!(bool) (UnityEngine.Object) this.m_gun || !this.m_gun.OwnerHasSynergy(this.RequiredSynergy) || (double) UnityEngine.Random.value >= (double) this.ChanceToGainAmmo)
      return;
    this.m_gun.GainAmmo(1);
  }

  public void Update()
  {
    if (this.SnapsToAngleMultiple && (bool) (UnityEngine.Object) this.m_gun)
    {
      if (this.m_gun.OwnerHasSynergy(this.RequiredSynergy))
      {
        this.m_gun.preventRotation = true;
        this.m_gun.OverrideAngleSnap = new float?(this.AngleMultiple);
      }
      else
      {
        this.m_gun.preventRotation = false;
        this.m_gun.OverrideAngleSnap = new float?();
      }
    }
    if (this.SetsGoopReloadFree && (bool) (UnityEngine.Object) this.m_gun)
      this.m_gun.GoopReloadsFree = this.m_gun.OwnerHasSynergy(this.RequiredSynergy);
    if (!this.SetsFlying)
      return;
    if ((bool) (UnityEngine.Object) this.m_gun && this.m_gun.OwnerHasSynergy(this.RequiredSynergy))
    {
      if ((bool) (UnityEngine.Object) this.m_cachedPlayer)
        return;
      this.m_cachedPlayer = this.m_gun.CurrentOwner as PlayerController;
      this.m_cachedPlayer.SetIsFlying(true, "synergy flight");
      this.m_cachedPlayer.AdditionalCanDodgeRollWhileFlying.AddOverride("synergy flight");
    }
    else
    {
      if (!(bool) (UnityEngine.Object) this.m_cachedPlayer)
        return;
      this.m_cachedPlayer.AdditionalCanDodgeRollWhileFlying.RemoveOverride("synergy flight");
      this.m_cachedPlayer.SetIsFlying(false, "synergy flight");
      this.m_cachedPlayer = (PlayerController) null;
    }
  }

  private void OnDisable()
  {
    if (!(bool) (UnityEngine.Object) this.m_cachedPlayer)
      return;
    this.m_cachedPlayer.AdditionalCanDodgeRollWhileFlying.RemoveOverride("synergy flight");
    this.m_cachedPlayer.SetIsFlying(false, "synergy flight");
    this.m_cachedPlayer = (PlayerController) null;
  }

  private void OnDestroy()
  {
    if (!(bool) (UnityEngine.Object) this.m_cachedPlayer)
      return;
    this.m_cachedPlayer.AdditionalCanDodgeRollWhileFlying.RemoveOverride("synergy flight");
    this.m_cachedPlayer.SetIsFlying(false, "synergy flight");
    this.m_cachedPlayer = (PlayerController) null;
  }
}
