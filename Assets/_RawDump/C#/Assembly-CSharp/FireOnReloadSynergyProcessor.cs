// Decompiled with JetBrains decompiler
// Type: FireOnReloadSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class FireOnReloadSynergyProcessor : MonoBehaviour
{
  public bool RequiresNoSynergy;
  [LongNumericEnum]
  public CustomSynergyType SynergyToCheck;
  public bool OnlyOnEmptyClip;
  public bool DoesRadialBurst = true;
  public RadialBurstInterface RadialBurstSettings;
  public bool DoesDirectedBurst;
  public DirectedBurstInterface DirectedBurstSettings;
  public string SwitchGroup;
  public string SFX;
  private Gun m_gun;
  private PassiveItem m_item;

  private void Awake()
  {
    Gun component = this.GetComponent<Gun>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
    {
      component.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.HandleReloaded);
    }
    else
    {
      this.m_item = this.GetComponent<PassiveItem>();
      if (!((UnityEngine.Object) this.m_item != (UnityEngine.Object) null))
        return;
      this.m_item.OnPickedUp += new Action<PlayerController>(this.Hookup);
    }
  }

  private void Hookup(PlayerController acquiringPlayer)
  {
    acquiringPlayer.OnReloadPressed += new Action<PlayerController, Gun>(this.HandleReloadedPlayer);
  }

  private void HandleReloadedPlayer(PlayerController usingPlayer, Gun usedGun)
  {
    if (!(bool) (UnityEngine.Object) this.m_item || !(bool) (UnityEngine.Object) this.m_item.Owner)
      usingPlayer.OnReloadPressed -= new Action<PlayerController, Gun>(this.HandleReloadedPlayer);
    else
      this.HandleReloaded(usingPlayer, usedGun, false);
  }

  private void HandleReloaded(PlayerController usingPlayer, Gun usedGun, bool manual)
  {
    if (this.OnlyOnEmptyClip && usedGun.ClipShotsRemaining > 0 || !usedGun.IsReloading || !(bool) (UnityEngine.Object) usingPlayer || !this.RequiresNoSynergy && !usingPlayer.HasActiveBonusSynergy(this.SynergyToCheck) || (bool) (UnityEngine.Object) usedGun && usedGun.HasFiredReloadSynergy)
      return;
    usedGun.HasFiredReloadSynergy = true;
    if (this.DoesRadialBurst)
    {
      int num1 = (int) AkSoundEngine.SetSwitch("WPN_Guns", this.SwitchGroup, this.gameObject);
      int num2 = (int) AkSoundEngine.PostEvent(this.SFX, this.gameObject);
      this.RadialBurstSettings.DoBurst(usingPlayer);
      int num3 = (int) AkSoundEngine.SetSwitch("WPN_Guns", usedGun.gunSwitchGroup, this.gameObject);
    }
    if (!this.DoesDirectedBurst)
      return;
    int num4 = (int) AkSoundEngine.SetSwitch("WPN_Guns", this.SwitchGroup, this.gameObject);
    int num5 = (int) AkSoundEngine.PostEvent(this.SFX, this.gameObject);
    this.DirectedBurstSettings.DoBurst(usingPlayer, usedGun.CurrentAngle);
    int num6 = (int) AkSoundEngine.SetSwitch("WPN_Guns", usedGun.gunSwitchGroup, this.gameObject);
  }
}
