using HutongGames.PlayMaker.Actions;
using InControl;
using UnityEngine;

#nullable disable

public class FoyerCharacterSelectFlag : BraveBehaviour
  {
    public GameObject OverheadElement;
    public string CharacterPrefabPath;
    public bool IsCoopCharacter;
    public bool IsEevee;
    public bool IsGunslinger;
    public DungeonPrerequisite[] prerequisites;
    public tk2dSpriteAnimation AltCostumeLibrary;
    public string AltCostumeClipName;
    private dfControl m_extantOverheadUIElement;
    private bool m_active = true;
    private bool m_isAlternateCostume;

    public bool PrerequisitesFulfilled()
    {
      bool flag = true;
      for (int index = 0; index < this.prerequisites.Length; ++index)
      {
        if (!this.prerequisites[index].CheckConditionsFulfilled())
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    public bool CanBeSelected()
    {
      return (!this.IsEevee || (double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.META_CURRENCY) >= 5.0) && (!this.IsGunslinger || (double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.META_CURRENCY) >= 7.0);
    }

    private void EnsureStartingEquipmentEncountered()
    {
      if (!this.PrerequisitesFulfilled() || string.IsNullOrEmpty(this.CharacterPrefabPath))
        return;
      GameObject gameObject = (GameObject) BraveResources.Load(this.CharacterPrefabPath);
      if (!(bool) (Object) gameObject)
        return;
      PlayerController component = gameObject.GetComponent<PlayerController>();
      if (!(bool) (Object) component)
        return;
      if (component.startingGunIds != null)
      {
        for (int index = 0; index < component.startingGunIds.Count; ++index)
        {
          Gun byId = PickupObjectDatabase.GetById(component.startingGunIds[index]) as Gun;
          if ((bool) (Object) byId && (bool) (Object) byId.encounterTrackable)
            GameStatsManager.Instance.HandleEncounteredObjectRaw(byId.encounterTrackable.EncounterGuid);
        }
      }
      if (component.startingActiveItemIds != null)
      {
        for (int index = 0; index < component.startingActiveItemIds.Count; ++index)
        {
          PlayerItem byId = PickupObjectDatabase.GetById(component.startingActiveItemIds[index]) as PlayerItem;
          if ((bool) (Object) byId && (bool) (Object) byId.encounterTrackable)
            GameStatsManager.Instance.HandleEncounteredObjectRaw(byId.encounterTrackable.EncounterGuid);
        }
      }
      if (component.startingPassiveItemIds == null)
        return;
      for (int index = 0; index < component.startingPassiveItemIds.Count; ++index)
      {
        PlayerItem byId = PickupObjectDatabase.GetById(component.startingPassiveItemIds[index]) as PlayerItem;
        if ((bool) (Object) byId && (bool) (Object) byId.encounterTrackable)
          GameStatsManager.Instance.HandleEncounteredObjectRaw(byId.encounterTrackable.EncounterGuid);
      }
    }

    public void Start() => this.EnsureStartingEquipmentEncountered();

    private void ToggleSelf(bool activate)
    {
      this.m_active = activate;
      this.specRigidbody.enabled = activate;
      this.renderer.enabled = activate;
      this.talkDoer.IsInteractable = activate;
      this.talkDoer.ShowOutlines = activate;
      SetNpcVisibility.SetVisible(this.talkDoer, activate);
      SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, activate);
    }

    private void Update()
    {
      if (!this.IsCoopCharacter)
        return;
      if (this.m_active && InputManager.Devices.Count == 0)
      {
        this.ToggleSelf(false);
      }
      else
      {
        if (this.m_active || InputManager.Devices.Count <= 0)
          return;
        this.ToggleSelf(true);
      }
    }

    public void ToggleOverheadElementVisibility(bool value)
    {
      if (!(bool) (Object) this.m_extantOverheadUIElement || this.m_extantOverheadUIElement.IsVisible == value)
        return;
      this.m_extantOverheadUIElement.IsVisible = value;
      FoyerInfoPanelController component1 = this.m_extantOverheadUIElement.GetComponent<FoyerInfoPanelController>();
      if (!(bool) (Object) component1.arrow || component1.arrow.transform.childCount <= 0)
        return;
      MeshRenderer component2 = component1.arrow.transform.GetChild(0).GetComponent<MeshRenderer>();
      if (!(bool) (Object) component2)
        return;
      component2.enabled = value;
    }

    public bool IsAlternateCostume => this.m_isAlternateCostume;

    public void ChangeToAlternateCostume()
    {
      if ((Object) this.AltCostumeLibrary != (Object) null && !this.m_isAlternateCostume)
      {
        CharacterSelectIdleDoer component = this.GetComponent<CharacterSelectIdleDoer>();
        if ((bool) (Object) component)
          component.enabled = false;
        this.m_isAlternateCostume = true;
        tk2dSpriteAnimation library = this.spriteAnimator.Library;
        this.spriteAnimator.Library = this.AltCostumeLibrary;
        this.spriteAnimator.Play(this.AltCostumeClipName);
        this.AltCostumeLibrary = library;
      }
      else
      {
        if (!((Object) this.AltCostumeLibrary != (Object) null))
          return;
        this.m_isAlternateCostume = false;
        tk2dSpriteAnimation library = this.spriteAnimator.Library;
        this.spriteAnimator.Library = this.AltCostumeLibrary;
        this.spriteAnimator.Play("select_idle");
        this.AltCostumeLibrary = library;
      }
    }

    public FoyerInfoPanelController CreateOverheadElement()
    {
      this.m_extantOverheadUIElement = GameUIRoot.Instance.Manager.AddPrefab(this.OverheadElement);
      FoyerInfoPanelController component = this.m_extantOverheadUIElement.GetComponent<FoyerInfoPanelController>();
      if ((bool) (Object) component)
      {
        component.followTransform = this.transform;
        component.offset = new Vector3(0.75f, 1.625f, 0.0f);
      }
      return component;
    }

    private void OnDisable() => this.ClearOverheadElement();

    public void OnCoopChangedCallback()
    {
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
      {
        this.gameObject.SetActive(false);
        this.GetComponent<SpeculativeRigidbody>().enabled = false;
      }
      else
      {
        this.gameObject.SetActive(true);
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
        this.specRigidbody.enabled = true;
        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.specRigidbody);
        this.GetComponent<CharacterSelectIdleDoer>().enabled = true;
      }
    }

    public void OnSelectedCharacterCallback(PlayerController newCharacter)
    {
      Debug.Log((object) $"{newCharacter.name}|{(object) newCharacter.characterIdentity} <====");
      if (newCharacter.gameObject.name.Contains(this.CharacterPrefabPath))
      {
        this.gameObject.SetActive(false);
        this.GetComponent<SpeculativeRigidbody>().enabled = false;
        if (this.IsEevee)
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.META_CURRENCY, -5f);
        if (!this.IsGunslinger)
          return;
        GameStatsManager.Instance.RegisterStatChange(TrackedStats.META_CURRENCY, -7f);
      }
      else
      {
        if (this.gameObject.activeSelf)
          return;
        this.gameObject.SetActive(true);
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
        this.specRigidbody.enabled = true;
        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.specRigidbody);
        if (this.m_isAlternateCostume)
          return;
        this.GetComponent<CharacterSelectIdleDoer>().enabled = true;
      }
    }

    public void ClearOverheadElement()
    {
      if (!((Object) this.m_extantOverheadUIElement != (Object) null))
        return;
      Object.Destroy((Object) this.m_extantOverheadUIElement.gameObject);
      this.m_extantOverheadUIElement = (dfControl) null;
    }
  }

