using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class PlayerStats : MonoBehaviour
  {
    public int NumBlanksPerFloor = 3;
    public int NumBlanksPerFloorCoop = 2;
    public float rollDamage = 4f;
    [Header("Status Effect Things")]
    public bool UsesFireSourceEffect;
    public GameActorFireEffect OnFireSourceEffect;
    [SerializeField]
    [Header("Base Stat Values")]
    public List<float> BaseStatValues;
    [NonSerialized]
    public List<int> PreviouslyActiveSynergies;
    [NonSerialized]
    public List<CustomSynergyType> ActiveCustomSynergies = new List<CustomSynergyType>();
    protected List<float> StatValues;
    private const bool c_BonusSynergies = true;
    protected float m_magnificence;
    protected float m_floorMagnificence;

    public static int GetTotalCurse()
    {
      int val = 0;
      if ((bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer && GameManager.Instance.PrimaryPlayer.stats.StatValues != null)
        val += Mathf.FloorToInt(GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.Curse));
      if ((bool) (UnityEngine.Object) GameManager.Instance.SecondaryPlayer && GameManager.Instance.SecondaryPlayer.stats.StatValues != null)
        val += Mathf.FloorToInt(GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.Curse));
      GameStatsManager.Instance.UpdateMaximum(TrackedMaximums.HIGHEST_CURSE_LEVEL, (float) val);
      return val;
    }

    public static int GetTotalCoolness()
    {
      int totalCoolness = 0;
      if ((bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer && GameManager.Instance.PrimaryPlayer.stats.StatValues != null)
        totalCoolness += Mathf.FloorToInt(GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.Coolness));
      if ((bool) (UnityEngine.Object) GameManager.Instance.SecondaryPlayer && GameManager.Instance.SecondaryPlayer.stats.StatValues != null)
        totalCoolness += Mathf.FloorToInt(GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.Coolness));
      return totalCoolness;
    }

    public static float GetTotalEnemyProjectileSpeedMultiplier()
    {
      float projectileSpeedMultiplier = 1f;
      if ((bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer && GameManager.Instance.PrimaryPlayer.stats.StatValues != null)
        projectileSpeedMultiplier *= GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.EnemyProjectileSpeedMultiplier);
      if ((bool) (UnityEngine.Object) GameManager.Instance.SecondaryPlayer && GameManager.Instance.SecondaryPlayer.stats.StatValues != null)
        projectileSpeedMultiplier *= GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.EnemyProjectileSpeedMultiplier);
      return projectileSpeedMultiplier;
    }

    public float MovementSpeed => this.StatValues[0];

    public void CopyFrom(PlayerStats prefab)
    {
      this.NumBlanksPerFloor = prefab.NumBlanksPerFloor;
      this.NumBlanksPerFloorCoop = prefab.NumBlanksPerFloorCoop;
      this.rollDamage = prefab.rollDamage;
      this.UsesFireSourceEffect = prefab.UsesFireSourceEffect;
      this.OnFireSourceEffect = prefab.OnFireSourceEffect;
      this.BaseStatValues = new List<float>();
      for (int index = 0; index < prefab.BaseStatValues.Count; ++index)
        this.BaseStatValues.Add(prefab.BaseStatValues[index]);
    }

    public float GetBaseStatValue(PlayerStats.StatType stat) => this.BaseStatValues[(int) stat];

    public void SetBaseStatValue(PlayerStats.StatType stat, float value, PlayerController owner)
    {
      this.BaseStatValues[(int) stat] = value;
      this.RecalculateStats(owner, true);
    }

    public float GetStatValue(PlayerStats.StatType stat) => this.StatValues[(int) stat];

    public float GetStatModifier(PlayerStats.StatType stat)
    {
      if (!Application.isPlaying)
        return 1f;
      int index = (int) stat;
      return index < 0 || index >= this.StatValues.Count ? 1f : this.StatValues[index] / this.BaseStatValues[index];
    }

    public void RebuildGunVolleys(PlayerController owner)
    {
      if (owner.inventory == null || owner.inventory.AllGuns == null || owner.inventory.AllGuns.Count == 0)
        return;
      for (int index1 = 0; index1 < owner.inventory.AllGuns.Count; ++index1)
      {
        Gun allGun = owner.inventory.AllGuns[index1];
        ProjectileVolleyData modifiedVolley = allGun.modifiedVolley;
        allGun.modifiedVolley = (ProjectileVolleyData) null;
        allGun.modifiedFinalVolley = (ProjectileVolleyData) null;
        ProjectileVolleyData instance1 = ScriptableObject.CreateInstance<ProjectileVolleyData>();
        if ((UnityEngine.Object) allGun.Volley != (UnityEngine.Object) null)
        {
          instance1.InitializeFrom(allGun.Volley);
        }
        else
        {
          instance1.projectiles = new List<ProjectileModule>();
          instance1.projectiles.Add(ProjectileModule.CreateClone(allGun.singleModule));
          instance1.BeamRotationDegreesPerSecond = float.MaxValue;
        }
        this.ModVolley(owner, instance1);
        for (int index2 = 0; index2 < instance1.projectiles.Count; ++index2)
        {
          if (instance1.projectiles[index2].numberOfShotsInClip > 0)
            instance1.projectiles[index2].numberOfShotsInClip = Mathf.Max(1, instance1.projectiles[index2].numberOfShotsInClip + allGun.AdditionalClipCapacity);
        }
        if (allGun.PostProcessVolley != null)
          allGun.PostProcessVolley(instance1);
        allGun.modifiedVolley = instance1;
        if (allGun.DefaultModule.HasFinalVolleyOverride())
        {
          ProjectileVolleyData instance2 = ScriptableObject.CreateInstance<ProjectileVolleyData>();
          instance2.InitializeFrom(allGun.DefaultModule.finalVolley);
          this.ModVolley(owner, instance2);
          allGun.modifiedFinalVolley = instance2;
        }
        if ((UnityEngine.Object) allGun.rawOptionalReloadVolley != (UnityEngine.Object) null)
        {
          ProjectileVolleyData instance3 = ScriptableObject.CreateInstance<ProjectileVolleyData>();
          instance3.InitializeFrom(allGun.rawOptionalReloadVolley);
          this.ModVolley(owner, instance3);
          allGun.modifiedOptionalReloadVolley = instance3;
        }
        for (int index3 = 0; index3 < instance1.projectiles.Count; ++index3)
        {
          if (string.IsNullOrEmpty(instance1.projectiles[index3].runtimeGuid))
            instance1.projectiles[index3].runtimeGuid = Guid.NewGuid().ToString();
        }
        allGun.ReinitializeModuleData(modifiedVolley);
      }
      for (int index = 0; index < owner.passiveItems.Count; ++index)
      {
        if (owner.passiveItems[index] is FireVolleyOnRollItem)
        {
          FireVolleyOnRollItem passiveItem = owner.passiveItems[index] as FireVolleyOnRollItem;
          passiveItem.ModVolley = (ProjectileVolleyData) null;
          ProjectileVolleyData instance = ScriptableObject.CreateInstance<ProjectileVolleyData>();
          instance.InitializeFrom(passiveItem.Volley);
          this.ModVolley(owner, instance);
          passiveItem.ModVolley = instance;
        }
      }
    }

    public event Action<ProjectileVolleyData> AdditionalVolleyModifiers;

    private void ModVolley(PlayerController owner, ProjectileVolleyData volley)
    {
      for (int index = 0; index < owner.passiveItems.Count; ++index)
      {
        PassiveItem passiveItem = owner.passiveItems[index];
        if (passiveItem is GunVolleyModificationItem)
          (passiveItem as GunVolleyModificationItem).ModifyVolley(volley);
      }
      PlayerItem currentItem = owner.CurrentItem;
      if (currentItem is ActiveGunVolleyModificationItem && currentItem.IsActive)
        (currentItem as ActiveGunVolleyModificationItem).ModifyVolley(volley);
      if (this.AdditionalVolleyModifiers == null)
        return;
      this.AdditionalVolleyModifiers(volley);
    }

    private void ApplyStatModifier(
      StatModifier modifier,
      float[] statModsAdditive,
      float[] statModsMultiplic)
    {
      int statToBoost = (int) modifier.statToBoost;
      if (modifier.modifyType == StatModifier.ModifyMethod.ADDITIVE)
      {
        statModsAdditive[statToBoost] += modifier.amount;
      }
      else
      {
        if (modifier.modifyType != StatModifier.ModifyMethod.MULTIPLICATIVE)
          return;
        statModsMultiplic[statToBoost] *= modifier.amount;
      }
    }

    public void RecalculateStats(PlayerController owner, bool force = false, bool recursive = false)
    {
      this.RecalculateStatsInternal(owner);
      if (recursive || GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
        return;
      PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(owner);
      if (!(bool) (UnityEngine.Object) otherPlayer || !((UnityEngine.Object) otherPlayer.stats != (UnityEngine.Object) null))
        return;
      otherPlayer.stats.RecalculateStats(otherPlayer, force, true);
    }

    private void RecalculateSynergies(PlayerController owner)
    {
      if (this.PreviouslyActiveSynergies == null)
        this.PreviouslyActiveSynergies = new List<int>();
      this.PreviouslyActiveSynergies.Clear();
      this.PreviouslyActiveSynergies.AddRange((IEnumerable<int>) owner.ActiveExtraSynergies);
      if (!(bool) (UnityEngine.Object) GameManager.Instance || !(bool) (UnityEngine.Object) GameManager.Instance.SynergyManager || !(bool) (UnityEngine.Object) owner)
        return;
      GameManager.Instance.SynergyManager.RebuildSynergies(owner, this.PreviouslyActiveSynergies);
      bool flag = false;
      int index1 = -1;
      for (int index2 = 0; index2 < owner.ActiveExtraSynergies.Count; ++index2)
      {
        if (!GameManager.Instance.SynergyManager.synergies[owner.ActiveExtraSynergies[index2]].SuppressVFX && GameManager.Instance.SynergyManager.synergies[owner.ActiveExtraSynergies[index2]].ActivationStatus != SynergyEntry.SynergyActivation.INACTIVE && !this.PreviouslyActiveSynergies.Contains(owner.ActiveExtraSynergies[index2]))
        {
          flag = true;
          index1 = owner.ActiveExtraSynergies[index2];
          GameStatsManager.Instance.HandleEncounteredSynergy(index1);
          break;
        }
      }
      if (flag)
      {
        owner.PlayEffectOnActor((GameObject) ResourceCache.Acquire("Global VFX/VFX_Synergy"), new Vector3(0.0f, 0.5f, 0.0f));
        AdvancedSynergyEntry synergy = GameManager.Instance.SynergyManager.synergies[index1];
        if (synergy.ActivationStatus != SynergyEntry.SynergyActivation.INACTIVE && !string.IsNullOrEmpty(synergy.NameKey))
          GameUIRoot.Instance.notificationController.AttemptSynergyAttachment(synergy);
      }
      this.PreviouslyActiveSynergies.Clear();
      this.PreviouslyActiveSynergies.AddRange((IEnumerable<int>) owner.ActiveExtraSynergies);
    }

    public void RecalculateStatsInternal(PlayerController owner)
    {
      owner.DeferredStatRecalculationRequired = false;
      this.RecalculateSynergies(owner);
      int totalCurse1 = PlayerStats.GetTotalCurse();
      if (this.StatValues == null)
        this.StatValues = new List<float>();
      this.StatValues.Clear();
      for (int index = 0; index < this.BaseStatValues.Count; ++index)
        this.StatValues.Add(this.BaseStatValues[index]);
      float[] statModsAdditive = new float[this.StatValues.Count];
      float[] statModsMultiplic = new float[this.StatValues.Count];
      for (int index = 0; index < statModsMultiplic.Length; ++index)
        statModsMultiplic[index] = 1f;
      float num1 = 0.0f;
      this.ActiveCustomSynergies.Clear();
      for (int index1 = 0; index1 < owner.ActiveExtraSynergies.Count; ++index1)
      {
        AdvancedSynergyEntry synergy = GameManager.Instance.SynergyManager.synergies[owner.ActiveExtraSynergies[index1]];
        if (synergy.SynergyIsActive(GameManager.Instance.PrimaryPlayer, GameManager.Instance.SecondaryPlayer))
        {
          for (int index2 = 0; index2 < synergy.statModifiers.Count; ++index2)
          {
            StatModifier statModifier = synergy.statModifiers[index2];
            int statToBoost = (int) statModifier.statToBoost;
            if (statModifier.modifyType == StatModifier.ModifyMethod.ADDITIVE)
              statModsAdditive[statToBoost] += statModifier.amount;
            else if (statModifier.modifyType == StatModifier.ModifyMethod.MULTIPLICATIVE)
              statModsMultiplic[statToBoost] *= statModifier.amount;
          }
          for (int index3 = 0; index3 < synergy.bonusSynergies.Count; ++index3)
            this.ActiveCustomSynergies.Add(synergy.bonusSynergies[index3]);
        }
      }
      for (int index = 0; index < owner.ownerlessStatModifiers.Count; ++index)
      {
        StatModifier ownerlessStatModifier = owner.ownerlessStatModifiers[index];
        if (!ownerlessStatModifier.hasBeenOwnerlessProcessed && ownerlessStatModifier.statToBoost == PlayerStats.StatType.Health && (double) ownerlessStatModifier.amount > 0.0)
          num1 += ownerlessStatModifier.amount;
        int statToBoost = (int) ownerlessStatModifier.statToBoost;
        if (ownerlessStatModifier.modifyType == StatModifier.ModifyMethod.ADDITIVE)
          statModsAdditive[statToBoost] += ownerlessStatModifier.amount;
        else if (ownerlessStatModifier.modifyType == StatModifier.ModifyMethod.MULTIPLICATIVE)
          statModsMultiplic[statToBoost] *= ownerlessStatModifier.amount;
        ownerlessStatModifier.hasBeenOwnerlessProcessed = true;
      }
      for (int index4 = 0; index4 < owner.passiveItems.Count; ++index4)
      {
        PassiveItem passiveItem = owner.passiveItems[index4];
        if (passiveItem.passiveStatModifiers != null && passiveItem.passiveStatModifiers.Length > 0)
        {
          for (int index5 = 0; index5 < passiveItem.passiveStatModifiers.Length; ++index5)
          {
            StatModifier passiveStatModifier = passiveItem.passiveStatModifiers[index5];
            if (!passiveItem.HasBeenStatProcessed && passiveStatModifier.statToBoost == PlayerStats.StatType.Health && (double) passiveStatModifier.amount > 0.0)
              num1 += passiveStatModifier.amount;
            this.ApplyStatModifier(passiveStatModifier, statModsAdditive, statModsMultiplic);
          }
        }
        if (passiveItem is BasicStatPickup)
        {
          BasicStatPickup basicStatPickup = passiveItem as BasicStatPickup;
          for (int index6 = 0; index6 < basicStatPickup.modifiers.Count; ++index6)
          {
            StatModifier modifier = basicStatPickup.modifiers[index6];
            if (!passiveItem.HasBeenStatProcessed && modifier.statToBoost == PlayerStats.StatType.Health && (double) modifier.amount > 0.0)
              num1 += modifier.amount;
            this.ApplyStatModifier(modifier, statModsAdditive, statModsMultiplic);
          }
        }
        if (passiveItem is CoopPassiveItem && (GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER || (bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer.healthHaver && GameManager.Instance.PrimaryPlayer.healthHaver.IsDead || owner.HasActiveBonusSynergy(CustomSynergyType.THE_TRUE_HERO)))
        {
          CoopPassiveItem coopPassiveItem = passiveItem as CoopPassiveItem;
          for (int index7 = 0; index7 < coopPassiveItem.modifiers.Count; ++index7)
            this.ApplyStatModifier(coopPassiveItem.modifiers[index7], statModsAdditive, statModsMultiplic);
        }
        if (passiveItem is MetronomeItem)
        {
          float currentMultiplier = (passiveItem as MetronomeItem).GetCurrentMultiplier();
          statModsMultiplic[5] *= currentMultiplier;
        }
        passiveItem.HasBeenStatProcessed = true;
      }
      if (owner.inventory != null && owner.inventory.AllGuns != null)
      {
        if ((UnityEngine.Object) owner.inventory.CurrentGun != (UnityEngine.Object) null && owner.inventory.CurrentGun.currentGunStatModifiers != null && owner.inventory.CurrentGun.currentGunStatModifiers.Length > 0)
        {
          for (int index = 0; index < owner.inventory.CurrentGun.currentGunStatModifiers.Length; ++index)
            this.ApplyStatModifier(owner.inventory.CurrentGun.currentGunStatModifiers[index], statModsAdditive, statModsMultiplic);
        }
        for (int index8 = 0; index8 < owner.inventory.AllGuns.Count; ++index8)
        {
          if ((bool) (UnityEngine.Object) owner.inventory.AllGuns[index8] && owner.inventory.AllGuns[index8].passiveStatModifiers != null && owner.inventory.AllGuns[index8].passiveStatModifiers.Length > 0)
          {
            for (int index9 = 0; index9 < owner.inventory.AllGuns[index8].passiveStatModifiers.Length; ++index9)
              this.ApplyStatModifier(owner.inventory.AllGuns[index8].passiveStatModifiers[index9], statModsAdditive, statModsMultiplic);
          }
        }
      }
      for (int index10 = 0; index10 < owner.activeItems.Count; ++index10)
      {
        PlayerItem activeItem = owner.activeItems[index10];
        if (activeItem.passiveStatModifiers != null && activeItem.passiveStatModifiers.Length > 0)
        {
          for (int index11 = 0; index11 < activeItem.passiveStatModifiers.Length; ++index11)
          {
            StatModifier passiveStatModifier = activeItem.passiveStatModifiers[index11];
            if (!activeItem.HasBeenStatProcessed && passiveStatModifier.statToBoost == PlayerStats.StatType.Health && (double) passiveStatModifier.amount > 0.0)
              num1 += passiveStatModifier.amount;
            this.ApplyStatModifier(passiveStatModifier, statModsAdditive, statModsMultiplic);
          }
        }
        StatHolder component = activeItem.GetComponent<StatHolder>();
        if ((bool) (UnityEngine.Object) component && (!component.RequiresPlayerItemActive || activeItem.IsCurrentlyActive))
        {
          for (int index12 = 0; index12 < component.modifiers.Length; ++index12)
          {
            StatModifier modifier = component.modifiers[index12];
            if (!activeItem.HasBeenStatProcessed && modifier.statToBoost == PlayerStats.StatType.Health && (double) modifier.amount > 0.0)
              num1 += modifier.amount;
            this.ApplyStatModifier(modifier, statModsAdditive, statModsMultiplic);
          }
        }
        activeItem.HasBeenStatProcessed = true;
      }
      PlayerItem currentItem = owner.CurrentItem;
      if ((bool) (UnityEngine.Object) currentItem && currentItem is ActiveBasicStatItem && currentItem.IsActive)
      {
        ActiveBasicStatItem activeBasicStatItem = currentItem as ActiveBasicStatItem;
        for (int index = 0; index < activeBasicStatItem.modifiers.Count; ++index)
          this.ApplyStatModifier(activeBasicStatItem.modifiers[index], statModsAdditive, statModsMultiplic);
      }
      for (int index = 0; index < this.StatValues.Count; ++index)
        this.StatValues[index] = this.BaseStatValues[index] * statModsMultiplic[index] + statModsAdditive[index];
      float num2 = 0.0f;
      int num3 = !owner.AllowZeroHealthState ? 1 : 0;
      if ((double) this.StatValues[3] < (double) num3)
        this.StatValues[3] = (float) num3;
      if (owner.ForceZeroHealthState)
        this.StatValues[3] = 0.0f;
      if ((double) owner.healthHaver.GetMaxHealth() != (double) this.StatValues[3] + (double) num2)
        owner.healthHaver.SetHealthMaximum(this.StatValues[3] + num2, new float?(num1));
      owner.UpdateInventoryMaxGuns();
      owner.UpdateInventoryMaxItems();
      this.RebuildGunVolleys(owner);
      int totalCurse2 = PlayerStats.GetTotalCurse();
      if (totalCurse2 > totalCurse1 && !MidGameSaveData.IsInitializingPlayerData)
        owner.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, Vector3.zero);
      if (totalCurse2 < 10 || MidGameSaveData.IsInitializingPlayerData)
        return;
      GameManager.Instance.platformInterface.AchievementUnlock(Achievement.HAVE_MAX_CURSE);
      if (GameManager.Instance.Dungeon.CurseReaperActive)
        return;
      GameManager.Instance.Dungeon.SpawnCurseReaper();
    }

    public float Magnificence => this.m_magnificence + this.m_floorMagnificence;

    public void AddFloorMagnificence(float m) => this.m_floorMagnificence += m;

    public void ToNextLevel()
    {
      this.m_magnificence += this.m_floorMagnificence;
      this.m_floorMagnificence = 0.0f;
    }

    public enum StatType
    {
      MovementSpeed,
      RateOfFire,
      Accuracy,
      Health,
      Coolness,
      Damage,
      ProjectileSpeed,
      AdditionalGunCapacity,
      AdditionalItemCapacity,
      AmmoCapacityMultiplier,
      ReloadSpeed,
      AdditionalShotPiercing,
      KnockbackMultiplier,
      GlobalPriceMultiplier,
      Curse,
      PlayerBulletScale,
      AdditionalClipCapacityMultiplier,
      AdditionalShotBounces,
      AdditionalBlanksPerFloor,
      ShadowBulletChance,
      ThrownGunDamage,
      DodgeRollDamage,
      DamageToBosses,
      EnemyProjectileSpeedMultiplier,
      ExtremeShadowBulletChance,
      ChargeAmountMultiplier,
      RangeMultiplier,
      DodgeRollDistanceMultiplier,
      DodgeRollSpeedMultiplier,
      TarnisherClipCapacityMultiplier,
      MoneyMultiplierFromEnemies,
    }
  }

