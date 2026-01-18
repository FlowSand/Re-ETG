// Decompiled with JetBrains decompiler
// Type: PlayerOrbitalItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class PlayerOrbitalItem : PassiveItem
  {
    public PlayerOrbital OrbitalPrefab;
    public PlayerOrbitalFollower OrbitalFollowerPrefab;
    public bool HasUpgradeSynergy;
    [LongNumericEnum]
    public CustomSynergyType UpgradeSynergy;
    public GameObject UpgradeOrbitalPrefab;
    public GameObject UpgradeOrbitalFollowerPrefab;
    public bool CanBeMimicked;
    [Header("Random Stuff, probably for Ioun Stones")]
    public DamageTypeModifier[] modifiers;
    public DamageTypeModifier[] synergyModifiers;
    public bool BreaksUponContact;
    public bool BreaksUponOwnerDamage;
    public GameObject BreakVFX;
    protected GameObject m_extantOrbital;
    protected bool m_synergyUpgradeActive;

    public static GameObject CreateOrbital(
      PlayerController owner,
      GameObject targetOrbitalPrefab,
      bool isFollower,
      PlayerOrbitalItem sourceItem = null)
    {
      GameObject orbital = UnityEngine.Object.Instantiate<GameObject>(targetOrbitalPrefab, owner.transform.position, Quaternion.identity);
      if (!isFollower)
      {
        PlayerOrbital component = orbital.GetComponent<PlayerOrbital>();
        component.Initialize(owner);
        component.SourceItem = sourceItem;
      }
      else
      {
        PlayerOrbitalFollower component = orbital.GetComponent<PlayerOrbitalFollower>();
        if ((bool) (UnityEngine.Object) component)
          component.Initialize(owner);
      }
      return orbital;
    }

    private void CreateOrbital(PlayerController owner)
    {
      GameObject targetOrbitalPrefab = !((UnityEngine.Object) this.OrbitalPrefab != (UnityEngine.Object) null) ? this.OrbitalFollowerPrefab.gameObject : this.OrbitalPrefab.gameObject;
      if (this.HasUpgradeSynergy && this.m_synergyUpgradeActive)
        targetOrbitalPrefab = !((UnityEngine.Object) this.UpgradeOrbitalPrefab != (UnityEngine.Object) null) ? this.UpgradeOrbitalFollowerPrefab.gameObject : this.UpgradeOrbitalPrefab.gameObject;
      this.m_extantOrbital = PlayerOrbitalItem.CreateOrbital(owner, targetOrbitalPrefab, (UnityEngine.Object) this.OrbitalFollowerPrefab != (UnityEngine.Object) null, this);
      if (this.BreaksUponContact && (bool) (UnityEngine.Object) this.m_extantOrbital)
      {
        SpeculativeRigidbody component = this.m_extantOrbital.GetComponent<SpeculativeRigidbody>();
        if ((bool) (UnityEngine.Object) component)
          component.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleBreakOnCollision);
      }
      if (!this.BreaksUponOwnerDamage || !(bool) (UnityEngine.Object) owner)
        return;
      owner.OnReceivedDamage += new Action<PlayerController>(this.HandleBreakOnOwnerDamage);
    }

    private void HandleBreakOnOwnerDamage(PlayerController arg1)
    {
      if (!(bool) (UnityEngine.Object) this)
        return;
      if ((bool) (UnityEngine.Object) this.BreakVFX && (bool) (UnityEngine.Object) this.m_extantOrbital && (bool) (UnityEngine.Object) this.m_extantOrbital.GetComponentInChildren<tk2dSprite>())
        SpawnManager.SpawnVFX(this.BreakVFX, this.m_extantOrbital.GetComponentInChildren<tk2dSprite>().WorldCenter.ToVector3ZisY(), Quaternion.identity);
      if ((bool) (UnityEngine.Object) this.m_owner)
      {
        this.m_owner.RemovePassiveItem(this.PickupObjectId);
        this.m_owner.OnReceivedDamage -= new Action<PlayerController>(this.HandleBreakOnOwnerDamage);
      }
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    private void HandleBreakOnCollision(CollisionData rigidbodyCollision)
    {
      if ((bool) (UnityEngine.Object) this.m_owner)
        this.m_owner.RemovePassiveItem(this.PickupObjectId);
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    public void DecoupleOrbital()
    {
      this.m_extantOrbital = (GameObject) null;
      if (!this.BreaksUponOwnerDamage || !(bool) (UnityEngine.Object) this.m_owner)
        return;
      this.m_owner.OnReceivedDamage -= new Action<PlayerController>(this.HandleBreakOnOwnerDamage);
    }

    private void DestroyOrbital()
    {
      if (!(bool) (UnityEngine.Object) this.m_extantOrbital)
        return;
      if (this.BreaksUponOwnerDamage && (bool) (UnityEngine.Object) this.m_owner)
        this.m_owner.OnReceivedDamage -= new Action<PlayerController>(this.HandleBreakOnOwnerDamage);
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantOrbital.gameObject);
      this.m_extantOrbital = (GameObject) null;
    }

    protected override void Update()
    {
      base.Update();
      if (!this.HasUpgradeSynergy)
        return;
      if (this.m_synergyUpgradeActive && (!(bool) (UnityEngine.Object) this.m_owner || !this.m_owner.HasActiveBonusSynergy(this.UpgradeSynergy)))
      {
        if ((bool) (UnityEngine.Object) this.m_owner)
        {
          for (int index = 0; index < this.synergyModifiers.Length; ++index)
            this.m_owner.healthHaver.damageTypeModifiers.Remove(this.synergyModifiers[index]);
        }
        this.m_synergyUpgradeActive = false;
        this.DestroyOrbital();
        if (!(bool) (UnityEngine.Object) this.m_owner)
          return;
        this.CreateOrbital(this.m_owner);
      }
      else
      {
        if (this.m_synergyUpgradeActive || !(bool) (UnityEngine.Object) this.m_owner || !this.m_owner.HasActiveBonusSynergy(this.UpgradeSynergy))
          return;
        this.m_synergyUpgradeActive = true;
        this.DestroyOrbital();
        if ((bool) (UnityEngine.Object) this.m_owner)
          this.CreateOrbital(this.m_owner);
        for (int index = 0; index < this.synergyModifiers.Length; ++index)
          this.m_owner.healthHaver.damageTypeModifiers.Add(this.synergyModifiers[index]);
      }
    }

    public override void Pickup(PlayerController player)
    {
      base.Pickup(player);
      player.OnNewFloorLoaded += new Action<PlayerController>(this.HandleNewFloor);
      for (int index = 0; index < this.modifiers.Length; ++index)
        player.healthHaver.damageTypeModifiers.Add(this.modifiers[index]);
      this.CreateOrbital(player);
    }

    private void HandleNewFloor(PlayerController obj)
    {
      this.DestroyOrbital();
      this.CreateOrbital(obj);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      this.DestroyOrbital();
      player.OnNewFloorLoaded -= new Action<PlayerController>(this.HandleNewFloor);
      for (int index = 0; index < this.modifiers.Length; ++index)
        player.healthHaver.damageTypeModifiers.Remove(this.modifiers[index]);
      for (int index = 0; index < this.synergyModifiers.Length; ++index)
        player.healthHaver.damageTypeModifiers.Remove(this.synergyModifiers[index]);
      return base.Drop(player);
    }

    protected override void OnDestroy()
    {
      if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
      {
        this.m_owner.OnNewFloorLoaded -= new Action<PlayerController>(this.HandleNewFloor);
        for (int index = 0; index < this.modifiers.Length; ++index)
          this.m_owner.healthHaver.damageTypeModifiers.Remove(this.modifiers[index]);
        for (int index = 0; index < this.synergyModifiers.Length; ++index)
          this.m_owner.healthHaver.damageTypeModifiers.Remove(this.synergyModifiers[index]);
        this.m_owner.OnReceivedDamage -= new Action<PlayerController>(this.HandleBreakOnOwnerDamage);
      }
      this.DestroyOrbital();
      base.OnDestroy();
    }
  }

