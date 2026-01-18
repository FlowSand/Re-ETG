// Decompiled with JetBrains decompiler
// Type: OnDamagedSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class OnDamagedSynergyProcessor : MonoBehaviour
  {
    [LongNumericEnum]
    public CustomSynergyType RequiredSynergy;
    public bool OnlyArmorDamage;
    public bool DoesRadialBurst;
    public RadialBurstInterface RadialBurst;
    public bool DoesRadialSlow;
    public RadialSlowInterface RadialSlow;
    public string OnTriggeredAudioEvent;
    private bool m_actionsLinked;
    private PlayerController m_cachedLinkedPlayer;
    private Gun m_gun;
    private PassiveItem m_item;
    private float m_cachedArmor;

    public void Awake()
    {
      this.m_gun = this.GetComponent<Gun>();
      this.m_item = this.GetComponent<PassiveItem>();
    }

    public void Update()
    {
      PlayerController owner = this.GetOwner();
      if (!this.m_actionsLinked && (bool) (UnityEngine.Object) owner)
      {
        this.m_cachedLinkedPlayer = owner;
        this.m_cachedArmor = owner.healthHaver.Armor;
        owner.OnReceivedDamage += new Action<PlayerController>(this.HandleOwnerDamaged);
        this.m_actionsLinked = true;
      }
      else if (this.m_actionsLinked && !(bool) (UnityEngine.Object) owner && (bool) (UnityEngine.Object) this.m_cachedLinkedPlayer)
      {
        this.m_cachedLinkedPlayer.OnReceivedDamage -= new Action<PlayerController>(this.HandleOwnerDamaged);
        this.m_cachedLinkedPlayer = (PlayerController) null;
        this.m_actionsLinked = false;
      }
      if (!this.m_actionsLinked || !(bool) (UnityEngine.Object) this.m_cachedLinkedPlayer)
        return;
      this.m_cachedArmor = this.m_cachedLinkedPlayer.healthHaver.Armor;
    }

    private void HandleOwnerDamaged(PlayerController sourcePlayer)
    {
      if (!sourcePlayer.HasActiveBonusSynergy(this.RequiredSynergy) || this.OnlyArmorDamage && (double) this.m_cachedArmor == (double) sourcePlayer.healthHaver.Armor)
        return;
      if (!string.IsNullOrEmpty(this.OnTriggeredAudioEvent))
      {
        int num = (int) AkSoundEngine.PostEvent(this.OnTriggeredAudioEvent, sourcePlayer.gameObject);
      }
      if (this.DoesRadialBurst)
        this.RadialBurst.DoBurst(sourcePlayer);
      if (!this.DoesRadialSlow)
        return;
      this.RadialSlow.DoRadialSlow(sourcePlayer.CenterPosition, sourcePlayer.CurrentRoom);
    }

    private PlayerController GetOwner()
    {
      if ((bool) (UnityEngine.Object) this.m_gun)
        return this.m_gun.CurrentOwner as PlayerController;
      return (bool) (UnityEngine.Object) this.m_item ? this.m_item.Owner : (PlayerController) null;
    }

    public void OnDestroy()
    {
      if (!this.m_actionsLinked || !(bool) (UnityEngine.Object) this.m_cachedLinkedPlayer)
        return;
      this.m_cachedLinkedPlayer.OnReceivedDamage -= new Action<PlayerController>(this.HandleOwnerDamaged);
      this.m_cachedLinkedPlayer = (PlayerController) null;
      this.m_actionsLinked = false;
    }
  }

