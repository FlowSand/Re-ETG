// Decompiled with JetBrains decompiler
// Type: MetaShopController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class MetaShopController : ShopController, IPlaceConfigurable
  {
    public TalkDoerLite Witchkeeper;
    public Transform WitchStandPoint;
    public Transform WitchSleepPoint;
    public Transform WitchChairPoint;
    public float ChanceToBeAsleep = 0.5f;
    public HologramDoer Hologramophone;
    public GameObject ExampleBlueprintPrefab;
    public GameObject ExampleBlueprintPrefabItem;
    [SerializeField]
    public List<MetaShopTier> metaShopTiers;

    protected override void Start() => base.Start();

    public override void OnRoomEnter(PlayerController p)
    {
      if (this.firstTime)
      {
        this.firstTime = false;
        this.OnInitialRoomEnter();
      }
      else
        this.OnSequentialRoomEnter();
    }

    public override void OnRoomExit()
    {
    }

    protected override void OnInitialRoomEnter()
    {
      float num = UnityEngine.Random.value;
      if (!GameStatsManager.Instance.GetFlag(GungeonFlags.META_SHOP_RECEIVED_ROBOT_ARM_REWARD) && GameStatsManager.Instance.CurrentRobotArmFloor == 0)
        num = this.ChanceToBeAsleep - 0.01f;
      if (!GameStatsManager.Instance.GetFlag(GungeonFlags.META_SHOP_EVER_TALKED))
        num = this.ChanceToBeAsleep - 0.01f;
      string str = !GameStatsManager.Instance.GetFlag(GungeonFlags.META_SHOP_RECEIVED_ROBOT_ARM_REWARD) ? "idle" : "idle_hand";
      if ((double) num < (double) this.ChanceToBeAsleep)
      {
        if ((double) num < (double) this.ChanceToBeAsleep / 2.0)
        {
          this.Witchkeeper.transform.position = this.WitchChairPoint.position;
          this.Witchkeeper.specRigidbody.Reinitialize();
          this.Witchkeeper.SendPlaymakerEvent("SetChairMode");
          str += "_mask";
        }
        else
        {
          this.Witchkeeper.transform.position = this.WitchSleepPoint.position;
          this.Witchkeeper.specRigidbody.Reinitialize();
          this.Witchkeeper.SendPlaymakerEvent("SetShopMode");
        }
      }
      else
      {
        this.Witchkeeper.transform.position = this.WitchStandPoint.position;
        this.Witchkeeper.specRigidbody.Reinitialize();
        this.Witchkeeper.SendPlaymakerEvent("SetStandMode");
      }
      FsmString fsmString = this.speakerAnimator.playmakerFsm.FsmVariables.GetFsmString("idleAnim");
      fsmString.Value = str;
      this.speakerAnimator.Play(fsmString.Value);
    }

    protected override void OnSequentialRoomEnter()
    {
    }

    protected override void GunsmithTalk(string message)
    {
      TextBoxManager.ShowTextBox(this.speechPoint.position, this.speechPoint, 5f, message, "shopkeep", false);
      this.speakerAnimator.aiAnimator.PlayForDuration(this.defaultTalkAction, 2.5f);
    }

    public override void OnBetrayalWarning()
    {
    }

    public override void PullGun()
    {
    }

    public override void NotifyFailedPurchase(ShopItemController itemController)
    {
      if ((double) UnityEngine.Random.value < 0.75)
        this.speakerAnimator.SendPlaymakerEvent("playerHasNoMoney");
      else
        this.Witchkeeper.SendPlaymakerEvent("playerHasNoMoney");
    }

    public override void ReturnToIdle(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
    {
      animator.Play(this.defaultIdleAction);
      animator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(((ShopController) this).ReturnToIdle);
    }

    public override void PurchaseItem(ShopItemController item, bool actualPurchase = true, bool allowSign = true)
    {
      if (actualPurchase && !GameManager.Instance.IsSelectingCharacter)
      {
        if ((double) UnityEngine.Random.value < 0.75)
          this.speakerAnimator.SendPlaymakerEvent("playerPaid");
        else
          this.Witchkeeper.SendPlaymakerEvent("playerPaid");
        GameStatsManager.Instance.RegisterStatChange(TrackedStats.MERCHANT_PURCHASES_META, 1f);
        GameStatsManager.Instance.RegisterStatChange(TrackedStats.MONEY_SPENT_AT_CURSE_SHOP, (float) item.ModifiedPrice);
      }
      if (item.item.PersistsOnPurchase)
        return;
      this.m_room.DeregisterInteractable((IPlayerInteractable) item);
      if (allowSign)
      {
        GameObject gameObject = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Global Prefabs/Sign_SoldOut"));
        gameObject.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor((Vector3) item.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
        gameObject.transform.position = gameObject.transform.position.Quantize(1f / 16f);
        GameObject original = (GameObject) null;
        if ((UnityEngine.Object) this.shopItemShadowPrefab != (UnityEngine.Object) null)
          original = this.shopItemShadowPrefab;
        tk2dBaseSprite component1 = gameObject.GetComponent<tk2dBaseSprite>();
        if ((UnityEngine.Object) original != (UnityEngine.Object) null)
        {
          tk2dBaseSprite component2 = UnityEngine.Object.Instantiate<GameObject>(original).GetComponent<tk2dBaseSprite>();
          component1.AttachRenderer(component2);
          component2.transform.parent = component1.transform;
          component2.transform.localPosition = new Vector3(0.0f, 1f / 16f, 0.0f);
          component2.HeightOffGround = -1f / 16f;
        }
        gameObject.GetComponent<tk2dBaseSprite>().UpdateZDepth();
      }
      UnityEngine.Object.Destroy((UnityEngine.Object) item.gameObject);
    }

    public override void ConfigureOnPlacement(RoomHandler room) => base.ConfigureOnPlacement(room);

    protected MetaShopTier GetCurrentTier()
    {
      for (int index = 0; index < this.metaShopTiers.Count; ++index)
      {
        if (!GameStatsManager.Instance.GetFlag(this.GetFlagFromTargetItem(this.metaShopTiers[index].itemId1)) || !GameStatsManager.Instance.GetFlag(this.GetFlagFromTargetItem(this.metaShopTiers[index].itemId2)) || !GameStatsManager.Instance.GetFlag(this.GetFlagFromTargetItem(this.metaShopTiers[index].itemId3)))
          return this.metaShopTiers[index];
      }
      return this.metaShopTiers[this.metaShopTiers.Count - 1];
    }

    protected MetaShopTier GetProximateTier()
    {
      for (int index = 0; index < this.metaShopTiers.Count - 1; ++index)
      {
        if (!GameStatsManager.Instance.GetFlag(this.GetFlagFromTargetItem(this.metaShopTiers[index].itemId1)) || !GameStatsManager.Instance.GetFlag(this.GetFlagFromTargetItem(this.metaShopTiers[index].itemId2)) || !GameStatsManager.Instance.GetFlag(this.GetFlagFromTargetItem(this.metaShopTiers[index].itemId3)))
          return this.metaShopTiers[index + 1];
      }
      return (MetaShopTier) null;
    }

    protected GungeonFlags GetFlagFromTargetItem(int shopItemId)
    {
      GungeonFlags flagFromTargetItem = GungeonFlags.NONE;
      PickupObject byId = PickupObjectDatabase.GetById(shopItemId);
      for (int index = 0; index < byId.encounterTrackable.prerequisites.Length; ++index)
      {
        if (byId.encounterTrackable.prerequisites[index].prerequisiteType == DungeonPrerequisite.PrerequisiteType.FLAG)
          flagFromTargetItem = byId.encounterTrackable.prerequisites[index].saveFlagToCheck;
      }
      return flagFromTargetItem;
    }

    protected override void DoSetup()
    {
      this.m_shopItems = new List<GameObject>();
      this.m_room.Entered += new RoomHandler.OnEnteredEventHandler(((ShopController) this).OnRoomEnter);
      this.m_room.Exited += new RoomHandler.OnExitedEventHandler(((ShopController) this).OnRoomExit);
      MetaShopTier currentTier = this.GetCurrentTier();
      MetaShopTier proximateTier = this.GetProximateTier();
      this.m_itemControllers = new List<ShopItemController>();
      int num1 = 0;
      if (currentTier != null)
      {
        if (currentTier.itemId1 >= 0)
          this.m_shopItems.Add(PickupObjectDatabase.GetById(currentTier.itemId1).gameObject);
        if (currentTier.itemId2 >= 0)
          this.m_shopItems.Add(PickupObjectDatabase.GetById(currentTier.itemId2).gameObject);
        if (currentTier.itemId3 >= 0)
          this.m_shopItems.Add(PickupObjectDatabase.GetById(currentTier.itemId3).gameObject);
        for (int index = 0; index < this.spawnPositions.Length; ++index)
        {
          Transform spawnPosition = this.spawnPositions[index];
          if (index < this.m_shopItems.Count && !((UnityEngine.Object) this.m_shopItems[index] == (UnityEngine.Object) null))
          {
            PickupObject component1 = this.m_shopItems[index].GetComponent<PickupObject>();
            GameObject original = this.ExampleBlueprintPrefab;
            if (!(component1 is Gun))
              original = this.ExampleBlueprintPrefabItem;
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, new Vector3(150f, -50f, -100f), Quaternion.identity);
            ItemBlueprintItem component2 = gameObject.GetComponent<ItemBlueprintItem>();
            EncounterTrackable component3 = gameObject.GetComponent<EncounterTrackable>();
            component3.journalData.PrimaryDisplayName = component1.encounterTrackable.journalData.PrimaryDisplayName;
            component3.journalData.NotificationPanelDescription = component1.encounterTrackable.journalData.NotificationPanelDescription;
            component3.journalData.AmmonomiconFullEntry = component1.encounterTrackable.journalData.AmmonomiconFullEntry;
            component3.journalData.AmmonomiconSprite = component1.encounterTrackable.journalData.AmmonomiconSprite;
            component3.DoNotificationOnEncounter = false;
            component2.UsesCustomCost = true;
            int num2 = this.metaShopTiers.IndexOf(currentTier) + 1;
            if (currentTier.overrideTierCost > 0)
              num2 = currentTier.overrideTierCost;
            if (index == 0 && currentTier.overrideItem1Cost > 0)
              num2 = currentTier.overrideItem1Cost;
            if (index == 1 && currentTier.overrideItem2Cost > 0)
              num2 = currentTier.overrideItem2Cost;
            if (index == 2 && currentTier.overrideItem3Cost > 0)
              num2 = currentTier.overrideItem3Cost;
            component2.CustomCost = num2;
            GungeonFlags flagFromTargetItem = this.GetFlagFromTargetItem(component1.PickupObjectId);
            component2.SaveFlagToSetOnAcquisition = flagFromTargetItem;
            component2.HologramIconSpriteName = component3.journalData.AmmonomiconSprite;
            this.HandleItemPlacement(spawnPosition, (PickupObject) component2);
            gameObject.SetActive(false);
            if (GameStatsManager.Instance.GetFlag(component2.SaveFlagToSetOnAcquisition))
              this.m_itemControllers[index].ForceOutOfStock();
            else
              ++num1;
          }
        }
      }
      if (proximateTier != null)
      {
        this.m_shopItems.Clear();
        if (proximateTier.itemId1 >= 0)
          this.m_shopItems.Add(PickupObjectDatabase.GetById(proximateTier.itemId1).gameObject);
        if (proximateTier.itemId2 >= 0)
          this.m_shopItems.Add(PickupObjectDatabase.GetById(proximateTier.itemId2).gameObject);
        if (proximateTier.itemId3 >= 0)
          this.m_shopItems.Add(PickupObjectDatabase.GetById(proximateTier.itemId3).gameObject);
        for (int index = 0; index < this.spawnPositionsGroup2.Length; ++index)
        {
          Transform spawnTransform = this.spawnPositionsGroup2[index];
          if (index < this.m_shopItems.Count && !((UnityEngine.Object) this.m_shopItems[index] == (UnityEngine.Object) null))
          {
            PickupObject component4 = this.m_shopItems[index].GetComponent<PickupObject>();
            GameObject original = this.ExampleBlueprintPrefab;
            if (!(component4 is Gun))
              original = this.ExampleBlueprintPrefabItem;
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, new Vector3(150f, -50f, -100f), Quaternion.identity);
            ItemBlueprintItem component5 = gameObject.GetComponent<ItemBlueprintItem>();
            EncounterTrackable component6 = gameObject.GetComponent<EncounterTrackable>();
            component6.journalData.PrimaryDisplayName = component4.encounterTrackable.journalData.PrimaryDisplayName;
            component6.journalData.NotificationPanelDescription = component4.encounterTrackable.journalData.NotificationPanelDescription;
            component6.journalData.AmmonomiconFullEntry = component4.encounterTrackable.journalData.AmmonomiconFullEntry;
            component6.journalData.AmmonomiconSprite = component4.encounterTrackable.journalData.AmmonomiconSprite;
            component6.DoNotificationOnEncounter = false;
            component5.UsesCustomCost = true;
            int num3 = this.metaShopTiers.IndexOf(proximateTier) + 1;
            if (proximateTier.overrideTierCost > 0)
              num3 = proximateTier.overrideTierCost;
            if (index == 0 && proximateTier.overrideItem1Cost > 0)
              num3 = proximateTier.overrideItem1Cost;
            if (index == 1 && proximateTier.overrideItem2Cost > 0)
              num3 = proximateTier.overrideItem2Cost;
            if (index == 2 && proximateTier.overrideItem3Cost > 0)
              num3 = proximateTier.overrideItem3Cost;
            component5.CustomCost = num3;
            GungeonFlags flagFromTargetItem = this.GetFlagFromTargetItem(component4.PickupObjectId);
            component5.SaveFlagToSetOnAcquisition = flagFromTargetItem;
            component5.HologramIconSpriteName = component6.journalData.AmmonomiconSprite;
            this.HandleItemPlacement(spawnTransform, (PickupObject) component5);
            gameObject.SetActive(false);
            if (GameStatsManager.Instance.GetFlag(component5.SaveFlagToSetOnAcquisition))
              this.m_itemControllers[this.m_itemControllers.Count - 1].ForceOutOfStock();
            else
              ++num1;
          }
        }
      }
      if (GameManager.Instance.platformInterface != null && num1 == 0 && proximateTier == null)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.SPEND_META_CURRENCY);
      for (int index = 0; index < this.m_itemControllers.Count; ++index)
      {
        this.m_itemControllers[index].sprite.IsPerpendicular = true;
        this.m_itemControllers[index].sprite.UpdateZDepth();
        this.m_itemControllers[index].CurrencyType = ShopItemController.ShopCurrencyType.META_CURRENCY;
      }
    }

    private void HandleItemPlacement(Transform spawnTransform, PickupObject shopItem)
    {
      GameObject gameObject = new GameObject("Shop item " + Array.IndexOf<Transform>(this.spawnPositions, spawnTransform).ToString());
      Transform transform = gameObject.transform;
      transform.parent = spawnTransform;
      transform.localPosition = Vector3.zero;
      EncounterTrackable component = shopItem.GetComponent<EncounterTrackable>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        GameManager.Instance.ExtantShopTrackableGuids.Add(component.EncounterGuid);
      ShopItemController ixable = gameObject.AddComponent<ShopItemController>();
      if (spawnTransform.name.Contains("SIDE") || spawnTransform.name.Contains("EAST"))
        ixable.itemFacing = DungeonData.Direction.EAST;
      else if (spawnTransform.name.Contains("WEST"))
        ixable.itemFacing = DungeonData.Direction.WEST;
      else if (spawnTransform.name.Contains("NORTH"))
        ixable.itemFacing = DungeonData.Direction.NORTH;
      if (!this.m_room.IsRegistered((IPlayerInteractable) ixable))
        this.m_room.RegisterInteractable((IPlayerInteractable) ixable);
      ixable.Initialize(shopItem, (ShopController) this);
      transform.localPosition += new Vector3(1f / 16f, 0.0f, 0.0f);
      this.m_itemControllers.Add(ixable);
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

