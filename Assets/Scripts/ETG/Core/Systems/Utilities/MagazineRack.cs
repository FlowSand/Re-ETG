// Decompiled with JetBrains decompiler
// Type: MagazineRack
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class MagazineRack : MonoBehaviour
  {
    public float Duration = 10f;
    public float Radius = 5f;
    private bool m_radialIndicatorActive;
    private HeatIndicatorController m_radialIndicator;
    private int m_p1MaxGunAmmoThisFrame = 1000;
    private int m_p1GunIDThisFrame = -1;
    private int m_p2MaxGunAmmoThisFrame = 1000;
    private int m_p2GunIDThisFrame = -1;

    [DebuggerHidden]
    public IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MagazineRack__Startc__Iterator0()
      {
        _this = this
      };
    }

    private void Update()
    {
      if (Dungeon.IsGenerating || GameManager.Instance.IsLoadingLevel)
        return;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
        float radius = this.Radius;
        if (allPlayer.HasActiveBonusSynergy(CustomSynergyType.MAGAZINE_CLIPS))
          radius *= 2f;
        if ((bool) (Object) allPlayer && (double) Vector2.Distance(allPlayer.CenterPosition, this.transform.position.XY()) < (double) radius)
        {
          if (index == 0 && (bool) (Object) allPlayer.CurrentGun)
          {
            this.m_p1MaxGunAmmoThisFrame = allPlayer.CurrentGun.CurrentAmmo;
            this.m_p1GunIDThisFrame = allPlayer.CurrentGun.PickupObjectId;
          }
          if (index == 1 && (bool) (Object) allPlayer.CurrentGun)
          {
            this.m_p2MaxGunAmmoThisFrame = allPlayer.CurrentGun.CurrentAmmo;
            this.m_p2GunIDThisFrame = allPlayer.CurrentGun.PickupObjectId;
          }
          allPlayer.InfiniteAmmo.SetOverride(nameof (MagazineRack), true);
          allPlayer.OnlyFinalProjectiles.SetOverride(nameof (MagazineRack), allPlayer.HasActiveBonusSynergy(CustomSynergyType.JUNK_MAIL));
        }
        else if ((bool) (Object) allPlayer)
        {
          allPlayer.InfiniteAmmo.SetOverride(nameof (MagazineRack), false);
          allPlayer.OnlyFinalProjectiles.SetOverride(nameof (MagazineRack), false);
        }
      }
    }

    private void LateUpdate()
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
        if ((bool) (Object) allPlayer && (bool) (Object) allPlayer.CurrentGun && allPlayer.InfiniteAmmo.HasOverride(nameof (MagazineRack)))
        {
          int b = index != 0 ? this.m_p2MaxGunAmmoThisFrame : this.m_p1MaxGunAmmoThisFrame;
          int num = index != 0 ? this.m_p2GunIDThisFrame : this.m_p1GunIDThisFrame;
          if (!allPlayer.CurrentGun.RequiresFundsToShoot && allPlayer.CurrentGun.CurrentAmmo < b && allPlayer.CurrentGun.PickupObjectId == num)
            allPlayer.CurrentGun.ammo = Mathf.Min(allPlayer.CurrentGun.AdjustedMaxAmmo, b);
        }
        if (index == 0 && (bool) (Object) allPlayer.CurrentGun)
          this.m_p1MaxGunAmmoThisFrame = allPlayer.CurrentGun.CurrentAmmo;
        if (index == 1 && (bool) (Object) allPlayer.CurrentGun)
          this.m_p2MaxGunAmmoThisFrame = allPlayer.CurrentGun.CurrentAmmo;
      }
    }

    private void OnDestroy()
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
        if ((bool) (Object) allPlayer)
        {
          allPlayer.InfiniteAmmo.SetOverride(nameof (MagazineRack), false);
          allPlayer.OnlyFinalProjectiles.SetOverride(nameof (MagazineRack), false);
        }
      }
      this.UnhandleRadialIndicator();
    }

    private void HandleRadialIndicator()
    {
      if (this.m_radialIndicatorActive)
        return;
      this.m_radialIndicatorActive = true;
      this.m_radialIndicator = ((GameObject) Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), this.transform.position, Quaternion.identity, this.transform)).GetComponent<HeatIndicatorController>();
      UnityEngine.Debug.LogError((object) "setting color and fire");
      this.m_radialIndicator.CurrentColor = Color.white;
      this.m_radialIndicator.IsFire = false;
      float radius = this.Radius;
      int count = -1;
      if (PlayerController.AnyoneHasActiveBonusSynergy(CustomSynergyType.MAGAZINE_CLIPS, out count))
        radius *= 2f;
      this.m_radialIndicator.CurrentRadius = radius;
    }

    private void UnhandleRadialIndicator()
    {
      if (!this.m_radialIndicatorActive)
        return;
      this.m_radialIndicatorActive = false;
      if ((bool) (Object) this.m_radialIndicator)
        this.m_radialIndicator.EndEffect();
      this.m_radialIndicator = (HeatIndicatorController) null;
    }
  }

