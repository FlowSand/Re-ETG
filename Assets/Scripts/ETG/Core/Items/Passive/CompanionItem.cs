using Dungeonator;
using System;
using UnityEngine;

#nullable disable

public class CompanionItem : PassiveItem
  {
    [EnemyIdentifier]
    public string CompanionGuid;
    public bool UsesAlternatePastPrefab;
    [EnemyIdentifier]
    [ShowInInspectorIf("UsesAlternatePastPrefab", false)]
    public string CompanionPastGuid;
    public CompanionTransformSynergy[] Synergies;
    [NonSerialized]
    public bool PreventRespawnOnFloorLoad;
    [Header("For Pig Synergy")]
    public bool HasGunTransformationSacrificeSynergy;
    [ShowInInspectorIf("HasGunTransformationSacrificeSynergy", false)]
    public CustomSynergyType GunTransformationSacrificeSynergy;
    [PickupIdentifier]
    [ShowInInspectorIf("HasGunTransformationSacrificeSynergy", false)]
    public int SacrificeGunID;
    [ShowInInspectorIf("HasGunTransformationSacrificeSynergy", false)]
    public float SacrificeGunDuration = 30f;
    [NonSerialized]
    public bool BabyGoodMimicOrbitalOverridden;
    [NonSerialized]
    public PlayerOrbitalItem OverridePlayerOrbitalItem;
    private int m_lastActiveSynergyTransformation = -1;
    private GameObject m_extantCompanion;

    public GameObject ExtantCompanion => this.m_extantCompanion;

    private void CreateCompanion(PlayerController owner)
    {
      if (this.PreventRespawnOnFloorLoad)
        return;
      if (this.BabyGoodMimicOrbitalOverridden)
      {
        this.m_extantCompanion = PlayerOrbitalItem.CreateOrbital(owner, !(bool) (UnityEngine.Object) this.OverridePlayerOrbitalItem.OrbitalFollowerPrefab ? this.OverridePlayerOrbitalItem.OrbitalPrefab.gameObject : this.OverridePlayerOrbitalItem.OrbitalFollowerPrefab.gameObject, (bool) (UnityEngine.Object) this.OverridePlayerOrbitalItem.OrbitalFollowerPrefab);
      }
      else
      {
        string guid = this.CompanionGuid;
        this.m_lastActiveSynergyTransformation = -1;
        if (this.UsesAlternatePastPrefab && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST)
          guid = this.CompanionPastGuid;
        else if (this.Synergies.Length > 0)
        {
          for (int index = 0; index < this.Synergies.Length; ++index)
          {
            if (owner.HasActiveBonusSynergy(this.Synergies[index].RequiredSynergy))
            {
              guid = this.Synergies[index].SynergyCompanionGuid;
              this.m_lastActiveSynergyTransformation = index;
            }
          }
        }
        AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
        Vector3 position = owner.transform.position;
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
          position += new Vector3(1.125f, -5f / 16f, 0.0f);
        this.m_extantCompanion = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, position, Quaternion.identity);
        CompanionController orAddComponent = this.m_extantCompanion.GetOrAddComponent<CompanionController>();
        orAddComponent.Initialize(owner);
        if ((bool) (UnityEngine.Object) orAddComponent.specRigidbody)
          PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent.specRigidbody);
        if (orAddComponent.companionID != CompanionController.CompanionIdentifier.BABY_GOOD_MIMIC)
          return;
        GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_GOT_BABY_MIMIC, true);
      }
    }

    public void ForceCompanionRegeneration(PlayerController owner, Vector2? overridePosition)
    {
      bool flag = false;
      Vector2 vector = Vector2.zero;
      if ((bool) (UnityEngine.Object) this.m_extantCompanion)
      {
        flag = true;
        vector = this.m_extantCompanion.transform.position.XY();
      }
      if (overridePosition.HasValue)
      {
        flag = true;
        vector = overridePosition.Value;
      }
      this.DestroyCompanion();
      this.CreateCompanion(owner);
      if (!(bool) (UnityEngine.Object) this.m_extantCompanion || !flag)
        return;
      this.m_extantCompanion.transform.position = vector.ToVector3ZisY();
      SpeculativeRigidbody component = this.m_extantCompanion.GetComponent<SpeculativeRigidbody>();
      if (!(bool) (UnityEngine.Object) component)
        return;
      component.Reinitialize();
    }

    public void ForceDisconnectCompanion() => this.m_extantCompanion = (GameObject) null;

    private void DestroyCompanion()
    {
      if (!(bool) (UnityEngine.Object) this.m_extantCompanion)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantCompanion);
      this.m_extantCompanion = (GameObject) null;
    }

    protected override void Update()
    {
      base.Update();
      if (Dungeon.IsGenerating || !(bool) (UnityEngine.Object) this.m_owner || this.Synergies.Length <= 0 || this.UsesAlternatePastPrefab && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST)
        return;
      bool flag = false;
      for (int index = this.Synergies.Length - 1; index >= 0; --index)
      {
        if (this.m_owner.HasActiveBonusSynergy(this.Synergies[index].RequiredSynergy))
        {
          if (this.m_lastActiveSynergyTransformation != index)
          {
            this.DestroyCompanion();
            this.CreateCompanion(this.m_owner);
          }
          flag = true;
          break;
        }
      }
      if (flag || this.m_lastActiveSynergyTransformation == -1)
        return;
      this.DestroyCompanion();
      this.CreateCompanion(this.m_owner);
    }

    public override void Pickup(PlayerController player)
    {
      base.Pickup(player);
      player.OnNewFloorLoaded += new Action<PlayerController>(this.HandleNewFloor);
      this.CreateCompanion(player);
    }

    private void HandleNewFloor(PlayerController obj)
    {
      this.DestroyCompanion();
      if (this.PreventRespawnOnFloorLoad)
        return;
      this.CreateCompanion(obj);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      this.DestroyCompanion();
      player.OnNewFloorLoaded -= new Action<PlayerController>(this.HandleNewFloor);
      return base.Drop(player);
    }

    protected override void OnDestroy()
    {
      if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
        this.m_owner.OnNewFloorLoaded -= new Action<PlayerController>(this.HandleNewFloor);
      this.DestroyCompanion();
      base.OnDestroy();
    }
  }

