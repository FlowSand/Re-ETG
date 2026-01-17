// Decompiled with JetBrains decompiler
// Type: BaseShopController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class BaseShopController : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
      private const bool c_allowShopkeepBossFloor = false;
      public BaseShopController.AdditionalShopType baseShopType;
      public bool FoyerMetaShopForcedTiers;
      public bool IsBeetleMerchant;
      public GameObject ExampleBlueprintPrefab;
      [Header("Spawn Group 1")]
      public GenericLootTable shopItems;
      public Transform[] spawnPositions;
      [Header("Spawn Group 2")]
      public GenericLootTable shopItemsGroup2;
      public Transform[] spawnPositionsGroup2;
      public float spawnGroupTwoItem1Chance = 0.5f;
      public float spawnGroupTwoItem2Chance = 0.5f;
      public float spawnGroupTwoItem3Chance = 0.5f;
      [Header("Other Settings")]
      public PlayMakerFSM shopkeepFSM;
      public GameObject shopItemShadowPrefab;
      public GameObject cat;
      public GameObject OptionalMinimapIcon;
      public float ShopCostModifier = 1f;
      [LongEnum]
      public GungeonFlags FlagToSetOnEncounter;
      private OverridableBool m_capableOfBeingStolenFrom = new OverridableBool(false);
      private int m_numberThingsPurchased;
      private static bool m_hasLockedShopProcedurally;
      private bool m_hasBeenEntered;
      private int m_numberOfFirstTypeItems;
      protected bool PreventTeleportingPlayerAway;
      protected List<GameObject> m_shopItems;
      protected List<ShopItemController> m_itemControllers;
      protected RoomHandler m_room;
      protected TalkDoerLite m_shopkeep;
      private FakeGameActorEffectHandler m_fakeEffectHandler;
      [NonSerialized]
      public bool BeetleExhausted;
      private bool m_onLastStockBeetle;
      protected BaseShopController.ShopState m_state;
      protected bool firstTime = true;
      protected int m_hitCount;
      protected float m_timeSinceLastHit = 10f;
      protected float m_preTeleportTimer;
      protected bool m_wasCaughtStealing;
      private float m_stealChance = 1f;
      private int m_itemsStolen;
      private static float s_mainShopkeepStealChance = 1f;
      private static int s_mainShopkeepItemsStolen;
      private static bool s_emptyFutureShops;

      protected bool IsMainShopkeep => (bool) (UnityEngine.Object) this.cat;

      public float StealChance
      {
        get => this.IsMainShopkeep ? BaseShopController.s_mainShopkeepStealChance : this.m_stealChance;
        protected set
        {
          if (this.IsMainShopkeep)
            BaseShopController.s_mainShopkeepStealChance = value;
          else
            this.m_stealChance = value;
        }
      }

      public int ItemsStolen
      {
        get => this.IsMainShopkeep ? BaseShopController.s_mainShopkeepItemsStolen : this.m_itemsStolen;
        protected set
        {
          if (this.IsMainShopkeep)
            BaseShopController.s_mainShopkeepItemsStolen = value;
          else
            this.m_itemsStolen = value;
        }
      }

      public Vector2 CenterPosition
      {
        get
        {
          if ((bool) (UnityEngine.Object) this.specRigidbody && this.specRigidbody.HitboxPixelCollider != null)
            return this.specRigidbody.HitboxPixelCollider.UnitCenter;
          return (bool) (UnityEngine.Object) this.sprite ? this.sprite.WorldCenter : this.transform.position.XY();
        }
      }

      public bool IsCapableOfBeingStolenFrom => this.m_capableOfBeingStolenFrom.Value;

      public void SetCapableOfBeingStolenFrom(bool value, string reason, float? duration = null)
      {
        this.m_capableOfBeingStolenFrom.SetOverride(reason, value, duration);
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          GameManager.Instance.AllPlayers[index].ForceRefreshInteractable = true;
      }

      public bool WasCaughtStealing => this.m_wasCaughtStealing;

      [DebuggerHidden]
      protected IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BaseShopController__Startc__Iterator0()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleDelayedFoyerInitialization()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BaseShopController__HandleDelayedFoyerInitializationc__Iterator1()
        {
          _this = this
        };
      }

      private void Update()
      {
        if ((this.State == BaseShopController.ShopState.Default || this.State == BaseShopController.ShopState.GunDrawn) && (UnityEngine.Object) SuperReaperController.Instance != (UnityEngine.Object) null)
        {
          IntVector2 intVector2 = SuperReaperController.Instance.sprite.WorldCenter.ToIntVector2(VectorConversions.Floor);
          if (GameManager.Instance.Dungeon.data.CheckInBounds(intVector2))
          {
            CellData cellData = GameManager.Instance.Dungeon.data[intVector2];
            if (cellData != null && cellData.parentRoom == this.m_room)
            {
              this.PreventTeleportingPlayerAway = true;
              this.State = BaseShopController.ShopState.TeleportAway;
            }
          }
        }
        if (this.baseShopType == BaseShopController.AdditionalShopType.FOYER_META && this.m_itemControllers != null && this.IsBeetleMerchant && !this.BeetleExhausted)
        {
          bool flag = true;
          for (int index = 0; index < this.m_itemControllers.Count; ++index)
          {
            if (!this.m_itemControllers[index].Acquired)
              flag = false;
          }
          if (flag)
          {
            this.m_shopkeep.ShopStockStatus = !this.m_onLastStockBeetle ? Tribool.Ready : Tribool.Complete;
            if (this.m_onLastStockBeetle)
              GameStatsManager.Instance.SetFlag(GungeonFlags.BLUEPRINTBEETLE_GOLDIES, true);
            this.BeetleExhausted = true;
            GameStatsManager.Instance.SetFlag(GungeonFlags.SHOP_BEETLE_ACTIVE, false);
            GameStatsManager.Instance.AccumulatedBeetleMerchantChance = 0.0f;
            GameStatsManager.Instance.AccumulatedUsedBeetleMerchantChance = 0.0f;
          }
        }
        this.m_timeSinceLastHit += BraveTime.DeltaTime;
        if (this.State == BaseShopController.ShopState.Default || this.State == BaseShopController.ShopState.GunDrawn)
        {
          if (this.IsMainShopkeep)
          {
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
              PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
              if ((bool) (UnityEngine.Object) allPlayer && allPlayer.healthHaver.IsAlive && allPlayer.CurrentRoom == this.m_room && allPlayer.IsFiring)
                this.PlayerFired();
            }
          }
        }
        else if (this.State == BaseShopController.ShopState.PreTeleportAway)
        {
          if (this.m_shopkeep.IsTalking)
            EndConversation.ForceEndConversation(this.m_shopkeep);
          this.m_preTeleportTimer += BraveTime.DeltaTime;
          if ((double) this.m_preTeleportTimer > 2.0)
            this.State = BaseShopController.ShopState.TeleportAway;
        }
        else if (this.State == BaseShopController.ShopState.TeleportAway)
        {
          if (this.m_shopkeep.IsTalking)
            EndConversation.ForceEndConversation(this.m_shopkeep);
          if (!this.m_shopkeep.aiAnimator.IsPlaying("button"))
          {
            foreach (PlayerController allPlayer in GameManager.Instance.AllPlayers)
            {
              if ((bool) (UnityEngine.Object) allPlayer && allPlayer.CurrentRoom != null && allPlayer.CurrentRoom != this.m_room && allPlayer.CurrentRoom.IsSealed)
                this.PreventTeleportingPlayerAway = true;
            }
            if (!this.PreventTeleportingPlayerAway)
            {
              PlayerController bestActivePlayer = GameManager.Instance.BestActivePlayer;
              if (bestActivePlayer.CurrentRoom == this.m_room)
              {
                bestActivePlayer.EscapeRoom(PlayerController.EscapeSealedRoomStyle.TELEPORTER, false);
                int num = (int) AkSoundEngine.PostEvent("Play_OBJ_teleport_depart_01", bestActivePlayer.gameObject);
              }
              if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
              {
                PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(bestActivePlayer);
                if ((bool) (UnityEngine.Object) otherPlayer && otherPlayer.healthHaver.IsAlive)
                  otherPlayer.ReuniteWithOtherPlayer(bestActivePlayer);
              }
            }
            this.State = BaseShopController.ShopState.Gone;
          }
        }
        else if (this.State == BaseShopController.ShopState.Hostile)
        {
          bool flag = false;
          for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          {
            PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
            if ((bool) (UnityEngine.Object) allPlayer && allPlayer.healthHaver.IsAlive && allPlayer.CurrentRoom == this.m_room)
              flag = true;
          }
          if (!flag)
          {
            this.State = BaseShopController.ShopState.Gone;
            return;
          }
          this.m_preTeleportTimer += BraveTime.DeltaTime;
          if ((double) this.m_preTeleportTimer > 10.0)
            this.State = BaseShopController.ShopState.TeleportAway;
        }
        else if (this.State == BaseShopController.ShopState.RefuseService)
        {
          bool flag = false;
          for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          {
            PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
            if ((bool) (UnityEngine.Object) allPlayer && allPlayer.healthHaver.IsAlive && allPlayer.CurrentRoom == this.m_room)
              flag = true;
          }
          if (!flag)
            this.State = BaseShopController.ShopState.Gone;
        }
        if (!this.m_capableOfBeingStolenFrom.UpdateTimers(BraveTime.DeltaTime))
          return;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          GameManager.Instance.AllPlayers[index].ForceRefreshInteractable = true;
      }

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.m_shopkeep && (bool) (UnityEngine.Object) this.m_shopkeep.bulletBank)
          this.m_shopkeep.bulletBank.OnProjectileCreated -= new Action<Projectile>(this.OnProjectileCreated);
        StaticReferenceManager.AllShops.Remove(this);
        base.OnDestroy();
      }

      public virtual void NotifyFailedPurchase(ShopItemController itemController)
      {
        if (!((UnityEngine.Object) this.shopkeepFSM != (UnityEngine.Object) null))
          return;
        FsmObject fsmObject = this.shopkeepFSM.FsmVariables.FindFsmObject("referencedItem");
        if (fsmObject != null)
          fsmObject.Value = (UnityEngine.Object) itemController;
        this.shopkeepFSM.SendEvent("failedPurchase");
      }

      public virtual void PurchaseItem(ShopItemController item, bool actualPurchase = true, bool allowSign = true)
      {
        float num1 = -1f;
        if ((bool) (UnityEngine.Object) item && (bool) (UnityEngine.Object) item.sprite)
          num1 = item.sprite.HeightOffGround;
        if (actualPurchase)
        {
          if (this.baseShopType == BaseShopController.AdditionalShopType.TRUCK)
          {
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.MERCHANT_PURCHASES_TRUCK, 1f);
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.MONEY_SPENT_AT_TRUCK_SHOP, (float) item.ModifiedPrice);
          }
          if (this.baseShopType == BaseShopController.AdditionalShopType.GOOP)
          {
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.MERCHANT_PURCHASES_GOOP, 1f);
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.MONEY_SPENT_AT_GOOP_SHOP, (float) item.ModifiedPrice);
          }
          if (this.baseShopType == BaseShopController.AdditionalShopType.CURSE)
          {
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.MERCHANT_PURCHASES_CURSE, 1f);
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.MONEY_SPENT_AT_CURSE_SHOP, (float) item.ModifiedPrice);
            item.LastInteractingPlayer.ownerlessStatModifiers.Add(new StatModifier()
            {
              amount = 2.5f,
              modifyType = StatModifier.ModifyMethod.ADDITIVE,
              statToBoost = PlayerStats.StatType.Curse
            });
            item.LastInteractingPlayer.stats.RecalculateStats(item.LastInteractingPlayer);
          }
          if (this.baseShopType == BaseShopController.AdditionalShopType.BLANK)
          {
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.MERCHANT_PURCHASES_BLANK, 1f);
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.MONEY_SPENT_AT_BLANK_SHOP, (float) item.ModifiedPrice);
          }
          if (this.baseShopType == BaseShopController.AdditionalShopType.KEY)
          {
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.MERCHANT_PURCHASES_KEY, 1f);
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.MONEY_SPENT_AT_KEY_SHOP, (float) item.ModifiedPrice);
          }
          if ((UnityEngine.Object) this.shopkeepFSM != (UnityEngine.Object) null && this.baseShopType != BaseShopController.AdditionalShopType.RESRAT_SHORTCUT)
          {
            FsmObject fsmObject = this.shopkeepFSM.FsmVariables.FindFsmObject("referencedItem");
            if (fsmObject != null)
              fsmObject.Value = (UnityEngine.Object) item;
            this.shopkeepFSM.SendEvent("succeedPurchase");
          }
        }
        if (!item.item.PersistsOnPurchase)
        {
          if (allowSign)
          {
            GameObject gameObject = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Global Prefabs/Sign_SoldOut"));
            tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
            component.PlaceAtPositionByAnchor((Vector3) item.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
            gameObject.transform.position = gameObject.transform.position.Quantize(1f / 16f);
            component.HeightOffGround = num1;
            component.UpdateZDepth();
          }
          tk2dBaseSprite component1 = ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"))).GetComponent<tk2dBaseSprite>();
          component1.PlaceAtPositionByAnchor(item.sprite.WorldCenter.ToVector3ZUp(), tk2dBaseSprite.Anchor.MiddleCenter);
          component1.transform.position = component1.transform.position.Quantize(1f / 16f);
          component1.HeightOffGround = 5f;
          component1.UpdateZDepth();
          this.m_room.DeregisterInteractable((IPlayerInteractable) item);
          UnityEngine.Object.Destroy((UnityEngine.Object) item.gameObject);
        }
        if (this.baseShopType != BaseShopController.AdditionalShopType.RESRAT_SHORTCUT)
          return;
        ++this.m_numberThingsPurchased;
        int num2;
        switch (GameManager.Instance.Dungeon.tileIndices.tilesetId)
        {
          case GlobalDungeonData.ValidTilesets.GUNGEON:
            num2 = 1;
            break;
          case GlobalDungeonData.ValidTilesets.MINEGEON:
            num2 = 1;
            break;
          case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
            num2 = 2;
            break;
          case GlobalDungeonData.ValidTilesets.FORGEGEON:
            num2 = 3;
            break;
          default:
            num2 = 1;
            break;
        }
        if (this.m_numberThingsPurchased < num2)
          return;
        for (int index = 0; index < this.m_itemControllers.Count; ++index)
        {
          if (!this.m_itemControllers[index].Acquired)
            this.m_itemControllers[index].ForceOutOfStock();
        }
        if (!((UnityEngine.Object) this.shopkeepFSM != (UnityEngine.Object) null))
          return;
        FsmObject fsmObject1 = this.shopkeepFSM.FsmVariables.FindFsmObject("referencedItem");
        if (fsmObject1 != null)
          fsmObject1.Value = (UnityEngine.Object) item;
        this.shopkeepFSM.SendEvent("succeedPurchase");
        this.m_shopkeep.IsInteractable = false;
      }

      public void NotifyStealSucceeded()
      {
        ++this.ItemsStolen;
        if (this.IsMainShopkeep)
          this.StealChance = this.ItemsStolen > 1 ? 0.1f : 0.5f;
        else
          this.StealChance = 0.1f;
      }

      public void NotifyStealFailed()
      {
        this.shopkeepFSM.SendEvent("caughtStealing");
        this.m_capableOfBeingStolenFrom.ClearOverrides();
        if ((bool) (UnityEngine.Object) this.m_fakeEffectHandler)
          this.m_fakeEffectHandler.RemoveAllEffects();
        this.State = !this.IsMainShopkeep ? BaseShopController.ShopState.RefuseService : BaseShopController.ShopState.PreTeleportAway;
        this.m_wasCaughtStealing = true;
      }

      public bool AttemptToSteal() => (double) UnityEngine.Random.value <= (double) this.StealChance;

      public static bool HasLockedShopProcedurally
      {
        get => BaseShopController.m_hasLockedShopProcedurally;
        set => BaseShopController.m_hasLockedShopProcedurally = value;
      }

      public virtual void ConfigureOnPlacement(RoomHandler room)
      {
        if (this.baseShopType != BaseShopController.AdditionalShopType.RESRAT_SHORTCUT)
          room.IsShop = true;
        this.m_room = room;
        if ((UnityEngine.Object) this.OptionalMinimapIcon != (UnityEngine.Object) null)
          Minimap.Instance.RegisterRoomIcon(this.m_room, this.OptionalMinimapIcon);
        room.Entered += new RoomHandler.OnEnteredEventHandler(this.HandleEnter);
        bool isSeeded = GameManager.Instance.IsSeeded;
        if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON || this.baseShopType != BaseShopController.AdditionalShopType.BLANK && this.baseShopType != BaseShopController.AdditionalShopType.CURSE && this.baseShopType != BaseShopController.AdditionalShopType.GOOP && this.baseShopType != BaseShopController.AdditionalShopType.TRUCK || room.connectedRooms.Count != 1)
          ;
      }

      private PickupObject GetRandomLockedPaydayItem()
      {
        GenericLootTable itemsLootTable = GameManager.Instance.RewardManager.ItemsLootTable;
        List<PickupObject> pickupObjectList = new List<PickupObject>();
        for (int index = 0; index < itemsLootTable.defaultItemDrops.elements.Count; ++index)
        {
          WeightedGameObject element = itemsLootTable.defaultItemDrops.elements[index];
          if ((bool) (UnityEngine.Object) element.gameObject)
          {
            PickupObject component = element.gameObject.GetComponent<PickupObject>();
            if ((bool) (UnityEngine.Object) component)
            {
              switch (component)
              {
                case BankMaskItem _:
                case BankBagItem _:
                case PaydayDrillItem _:
                  EncounterTrackable encounterTrackable = component.encounterTrackable;
                  if ((bool) (UnityEngine.Object) encounterTrackable && !encounterTrackable.PrerequisitesMet())
                  {
                    pickupObjectList.Add(component);
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            }
          }
        }
        return pickupObjectList.Count <= 0 ? (PickupObject) null : pickupObjectList[UnityEngine.Random.Range(0, pickupObjectList.Count)];
      }

      private void HandleEnter(PlayerController p)
      {
        if (!this.m_hasBeenEntered && this.baseShopType == BaseShopController.AdditionalShopType.NONE)
        {
          GlobalDungeonData.ValidTilesets tilesetId = GameManager.Instance.Dungeon.tileIndices.tilesetId;
          this.ReinitializeFirstItemToKey();
        }
        this.m_hasBeenEntered = true;
        if (this.FlagToSetOnEncounter == GungeonFlags.NONE)
          return;
        GameStatsManager.Instance.SetFlag(this.FlagToSetOnEncounter, true);
      }

      private void OnProjectileCreated(Projectile projectile)
      {
        projectile.OwnerName = StringTableManager.GetEnemiesString("#JUSTICE_ENCNAME");
      }

      private BaseShopController.ShopState State
      {
        get => this.m_state;
        set
        {
          this.EndState(this.m_state);
          this.m_state = value;
          this.BeginState(this.m_state);
        }
      }

      private void BeginState(BaseShopController.ShopState state)
      {
        switch (state)
        {
          case BaseShopController.ShopState.GunDrawn:
            for (int index = 0; index < this.m_itemControllers.Count; ++index)
            {
              if ((bool) (UnityEngine.Object) this.m_itemControllers[index])
                this.m_itemControllers[index].CurrentPrice *= 2;
            }
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
              PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
              if ((bool) (UnityEngine.Object) allPlayer && allPlayer.healthHaver.IsAlive)
                allPlayer.ForceRefreshInteractable = true;
            }
            break;
          case BaseShopController.ShopState.Hostile:
            this.shopkeepFSM.enabled = false;
            this.m_shopkeep.IsInteractable = false;
            this.LockItems();
            BaseShopController.s_emptyFutureShops = true;
            this.m_shopkeep.behaviorSpeculator.enabled = true;
            this.m_shopkeep.bulletBank.OnProjectileCreated += new Action<Projectile>(this.OnProjectileCreated);
            break;
          case BaseShopController.ShopState.PreTeleportAway:
            this.m_preTeleportTimer = 0.0f;
            this.m_shopkeep.IsInteractable = false;
            this.LockItems();
            BaseShopController.s_emptyFutureShops = true;
            break;
          case BaseShopController.ShopState.TeleportAway:
            if (this.IsMainShopkeep)
            {
              this.shopkeepFSM.enabled = false;
              this.m_shopkeep.IsInteractable = false;
              this.m_shopkeep.behaviorSpeculator.InterruptAndDisable();
            }
            this.m_shopkeep.aiAnimator.PlayUntilCancelled("button");
            SpriteOutlineManager.RemoveOutlineFromSprite(this.m_shopkeep.sprite);
            this.m_shopkeep.sprite.HeightOffGround = 0.0f;
            this.m_shopkeep.sprite.UpdateZDepth();
            break;
          case BaseShopController.ShopState.Gone:
            if (this.IsMainShopkeep)
            {
              this.shopkeepFSM.enabled = false;
              this.m_shopkeep.IsInteractable = false;
              this.m_shopkeep.behaviorSpeculator.InterruptAndDisable();
              if (this.m_shopkeep.spriteAnimator.CurrentClip.name != "button_hit")
                this.m_shopkeep.aiAnimator.PlayUntilCancelled("hide");
              this.m_shopkeep.specRigidbody.enabled = false;
              UnityEngine.Object.Destroy((UnityEngine.Object) this.m_shopkeep.ultraFortunesFavor);
              this.m_shopkeep.RegenerateCache();
              if ((bool) (UnityEngine.Object) this.cat)
              {
                tk2dBaseSprite component1 = this.cat.GetComponent<tk2dBaseSprite>();
                tk2dBaseSprite component2 = ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"))).GetComponent<tk2dBaseSprite>();
                component2.PlaceAtPositionByAnchor(component1.WorldCenter.ToVector3ZUp(), tk2dBaseSprite.Anchor.MiddleCenter);
                component2.transform.position = component2.transform.position.Quantize(1f / 16f);
                component2.HeightOffGround = 10f;
                component2.UpdateZDepth();
                this.cat.SetActive(false);
              }
            }
            for (int index = 0; index < this.m_itemControllers.Count; ++index)
            {
              if ((bool) (UnityEngine.Object) this.m_itemControllers[index])
              {
                ShopItemController itemController = this.m_itemControllers[index];
                bool flag = false;
                if (itemController.item is BankMaskItem || itemController.item is BankBagItem || itemController.item is PaydayDrillItem)
                {
                  EncounterTrackable encounterTrackable = itemController.item.encounterTrackable;
                  if ((bool) (UnityEngine.Object) encounterTrackable && !encounterTrackable.PrerequisitesMet())
                  {
                    flag = true;
                    itemController.Locked = false;
                    itemController.OverridePrice = new int?(0);
                    if (itemController.SetsFlagOnSteal)
                      GameStatsManager.Instance.SetFlag(itemController.FlagToSetOnSteal, true);
                  }
                }
                if (!flag)
                  this.m_itemControllers[index].ForceOutOfStock();
              }
            }
            break;
          case BaseShopController.ShopState.RefuseService:
            this.LockItems();
            this.m_shopkeep.SuppressRoomEnterExitEvents = true;
            break;
        }
      }

      private void EndState(BaseShopController.ShopState state)
      {
      }

      private void PlayerFired()
      {
        if ((double) this.m_timeSinceLastHit <= 2.0)
          return;
        ++this.m_hitCount;
        this.m_timeSinceLastHit = 0.0f;
        if (this.m_state == BaseShopController.ShopState.Default)
        {
          if (this.m_hitCount <= 1)
          {
            this.shopkeepFSM.SendEvent("betrayalWarning");
          }
          else
          {
            this.shopkeepFSM.SendEvent("betrayal");
            this.State = BaseShopController.ShopState.GunDrawn;
          }
        }
        else
        {
          if (this.m_state != BaseShopController.ShopState.GunDrawn)
            return;
          this.State = BaseShopController.ShopState.Hostile;
        }
      }

      public void ReinitializeFirstItemToKey()
      {
        if (this.baseShopType != BaseShopController.AdditionalShopType.NONE)
          return;
        for (int index = 0; index < this.m_itemControllers.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.m_itemControllers[index] && (bool) (UnityEngine.Object) this.m_itemControllers[index].item && (bool) (UnityEngine.Object) this.m_itemControllers[index].item.GetComponent<KeyBulletPickup>())
            return;
        }
        int index1 = UnityEngine.Random.Range(0, this.m_numberOfFirstTypeItems);
        if (index1 < 0)
          index1 = 0;
        if (index1 >= this.m_shopItems.Count || index1 >= this.m_itemControllers.Count || !(bool) (UnityEngine.Object) this.m_shopItems[index1] || !(bool) (UnityEngine.Object) this.m_itemControllers[index1])
          index1 = 0;
        if (!(bool) (UnityEngine.Object) this.m_shopItems[index1] || !(bool) (UnityEngine.Object) this.m_itemControllers[index1])
          return;
        GameObject gameObject = (GameObject) null;
        for (int index2 = 0; index2 < this.shopItems.defaultItemDrops.elements.Count; ++index2)
        {
          if ((bool) (UnityEngine.Object) this.shopItems.defaultItemDrops.elements[index2].gameObject && (bool) (UnityEngine.Object) this.shopItems.defaultItemDrops.elements[index2].gameObject.GetComponent<KeyBulletPickup>())
            gameObject = this.shopItems.defaultItemDrops.elements[index2].gameObject;
        }
        this.m_shopItems[index1] = gameObject;
        this.m_itemControllers[index1].Initialize(gameObject.GetComponent<PickupObject>(), this);
      }

      protected virtual void DoSetup()
      {
        this.m_shopItems = new List<GameObject>();
        List<int> intList = new List<int>();
        Func<GameObject, float, float> weightModifier = (Func<GameObject, float, float>) null;
        if (SecretHandshakeItem.NumActive > 0)
          weightModifier = (Func<GameObject, float, float>) ((prefabObject, sourceWeight) =>
          {
            PickupObject component = prefabObject.GetComponent<PickupObject>();
            float num = sourceWeight;
            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            {
              int quality = (int) component.quality;
              num *= (float) (1.0 + (double) quality / 10.0);
            }
            return num;
          });
        System.Random safeRandom = (System.Random) null;
        if (this.baseShopType == BaseShopController.AdditionalShopType.RESRAT_SHORTCUT)
        {
          if (GameStatsManager.Instance.CurrentResRatShopSeed < 0)
            GameStatsManager.Instance.CurrentResRatShopSeed = UnityEngine.Random.Range(1, 1000000);
          safeRandom = new System.Random(GameStatsManager.Instance.CurrentResRatShopSeed);
        }
        bool flag1 = GameStatsManager.Instance.IsRainbowRun && (this.baseShopType == BaseShopController.AdditionalShopType.BLANK || this.baseShopType == BaseShopController.AdditionalShopType.CURSE || this.baseShopType == BaseShopController.AdditionalShopType.GOOP || this.baseShopType == BaseShopController.AdditionalShopType.KEY || this.baseShopType == BaseShopController.AdditionalShopType.TRUCK);
        for (int index1 = 0; index1 < this.spawnPositions.Length; ++index1)
        {
          if (flag1)
            this.m_shopItems.Add((GameObject) null);
          else if (this.baseShopType == BaseShopController.AdditionalShopType.RESRAT_SHORTCUT)
            this.m_shopItems.Add(GameManager.Instance.RewardManager.GetShopItemResourcefulRatStyle(this.m_shopItems, safeRandom));
          else if (this.baseShopType == BaseShopController.AdditionalShopType.FOYER_META && (UnityEngine.Object) this.ExampleBlueprintPrefab != (UnityEngine.Object) null)
          {
            if (this.FoyerMetaShopForcedTiers)
            {
              List<WeightedGameObject> compiledRawItems = this.shopItems.GetCompiledRawItems();
              int num = 0;
              bool flag2 = true;
              while (flag2)
              {
                for (int index2 = num; index2 < num + this.spawnPositions.Length; ++index2)
                {
                  if (index2 >= compiledRawItems.Count)
                  {
                    flag2 = false;
                    break;
                  }
                  if (!compiledRawItems[index2].gameObject.GetComponent<PickupObject>().encounterTrackable.PrerequisitesMet())
                  {
                    flag2 = false;
                    break;
                  }
                }
                if (flag2)
                  num += this.spawnPositions.Length;
              }
              if (num >= compiledRawItems.Count - this.spawnPositions.Length)
                this.m_onLastStockBeetle = true;
              for (int index3 = num; index3 < num + this.spawnPositions.Length; ++index3)
              {
                if (index3 >= compiledRawItems.Count)
                {
                  this.m_shopItems.Add((GameObject) null);
                  intList.Add(1);
                }
                else
                {
                  GameObject gameObject = compiledRawItems[index3].gameObject;
                  PickupObject component = gameObject.GetComponent<PickupObject>();
                  if (this.m_shopItems.Contains(gameObject) || component.encounterTrackable.PrerequisitesMet())
                  {
                    this.m_shopItems.Add((GameObject) null);
                    intList.Add(1);
                  }
                  else
                  {
                    this.m_shopItems.Add(gameObject);
                    intList.Add(Mathf.RoundToInt(compiledRawItems[index3].weight));
                  }
                }
              }
            }
            else
            {
              List<WeightedGameObject> compiledRawItems = this.shopItems.GetCompiledRawItems();
              GameObject gameObject1 = (GameObject) null;
              for (int index4 = 0; index4 < compiledRawItems.Count; ++index4)
              {
                GameObject gameObject2 = compiledRawItems[index4].gameObject;
                PickupObject component = gameObject2.GetComponent<PickupObject>();
                if (!this.m_shopItems.Contains(gameObject2) && !component.encounterTrackable.PrerequisitesMet())
                {
                  gameObject1 = gameObject2;
                  intList.Add(Mathf.RoundToInt(compiledRawItems[index4].weight));
                  break;
                }
              }
              this.m_shopItems.Add(gameObject1);
              if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
                intList.Add(1);
            }
          }
          else
          {
            GameObject gameObject = this.shopItems.SubshopSelectByWeightWithoutDuplicatesFullPrereqs(this.m_shopItems, weightModifier, 1, GameManager.Instance.IsSeeded);
            this.m_shopItems.Add(gameObject);
            if ((bool) (UnityEngine.Object) gameObject)
              ++this.m_numberOfFirstTypeItems;
          }
        }
        this.m_itemControllers = new List<ShopItemController>();
        for (int index5 = 0; index5 < this.spawnPositions.Length; ++index5)
        {
          Transform spawnPosition = this.spawnPositions[index5];
          if (!flag1 && !((UnityEngine.Object) this.m_shopItems[index5] == (UnityEngine.Object) null))
          {
            PickupObject component1 = this.m_shopItems[index5].GetComponent<PickupObject>();
            if (!((UnityEngine.Object) component1 == (UnityEngine.Object) null))
            {
              GameObject gameObject3 = new GameObject("Shop item " + index5.ToString());
              Transform transform = gameObject3.transform;
              transform.parent = spawnPosition;
              transform.localPosition = Vector3.zero;
              EncounterTrackable component2 = component1.GetComponent<EncounterTrackable>();
              if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
                GameManager.Instance.ExtantShopTrackableGuids.Add(component2.EncounterGuid);
              ShopItemController shopItemController = gameObject3.AddComponent<ShopItemController>();
              this.AssignItemFacing(spawnPosition, shopItemController);
              if (!this.m_room.IsRegistered((IPlayerInteractable) shopItemController))
                this.m_room.RegisterInteractable((IPlayerInteractable) shopItemController);
              if (this.baseShopType == BaseShopController.AdditionalShopType.FOYER_META && (UnityEngine.Object) this.ExampleBlueprintPrefab != (UnityEngine.Object) null)
              {
                GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(this.ExampleBlueprintPrefab, new Vector3(150f, -50f, -100f), Quaternion.identity);
                ItemBlueprintItem component3 = gameObject4.GetComponent<ItemBlueprintItem>();
                EncounterTrackable component4 = gameObject4.GetComponent<EncounterTrackable>();
                component4.journalData.PrimaryDisplayName = component1.encounterTrackable.journalData.PrimaryDisplayName;
                component4.journalData.NotificationPanelDescription = component1.encounterTrackable.journalData.NotificationPanelDescription;
                component4.journalData.AmmonomiconFullEntry = component1.encounterTrackable.journalData.AmmonomiconFullEntry;
                component4.journalData.AmmonomiconSprite = component1.encounterTrackable.journalData.AmmonomiconSprite;
                component4.DoNotificationOnEncounter = false;
                component3.UsesCustomCost = true;
                component3.CustomCost = intList[index5];
                GungeonFlags gungeonFlags = GungeonFlags.NONE;
                for (int index6 = 0; index6 < component1.encounterTrackable.prerequisites.Length; ++index6)
                {
                  if (component1.encounterTrackable.prerequisites[index6].prerequisiteType == DungeonPrerequisite.PrerequisiteType.FLAG)
                    gungeonFlags = component1.encounterTrackable.prerequisites[index6].saveFlagToCheck;
                }
                component3.SaveFlagToSetOnAcquisition = gungeonFlags;
                component3.HologramIconSpriteName = component4.journalData.AmmonomiconSprite;
                shopItemController.Initialize((PickupObject) component3, this);
                gameObject4.SetActive(false);
              }
              else
                shopItemController.Initialize(component1, this);
              this.m_itemControllers.Add(shopItemController);
            }
          }
        }
        bool flag3 = false;
        if ((UnityEngine.Object) this.shopItemsGroup2 != (UnityEngine.Object) null && this.spawnPositionsGroup2.Length > 0)
        {
          int count = this.m_shopItems.Count;
          for (int index = 0; index < this.spawnPositionsGroup2.Length; ++index)
          {
            if (flag1)
            {
              this.m_shopItems.Add((GameObject) null);
            }
            else
            {
              float num = this.spawnGroupTwoItem1Chance;
              switch (index)
              {
                case 1:
                  num = this.spawnGroupTwoItem2Chance;
                  break;
                case 2:
                  num = this.spawnGroupTwoItem3Chance;
                  break;
              }
              bool isSeeded = GameManager.Instance.IsSeeded;
              if ((!isSeeded ? (double) UnityEngine.Random.value : (double) BraveRandom.GenerationRandomValue()) < (double) num)
              {
                if (this.baseShopType == BaseShopController.AdditionalShopType.BLACKSMITH)
                {
                  if (!GameStatsManager.Instance.IsRainbowRun)
                  {
                    if ((!isSeeded ? (double) UnityEngine.Random.value : (double) BraveRandom.GenerationRandomValue()) > 0.5)
                      this.m_shopItems.Add(this.shopItemsGroup2.SelectByWeightWithoutDuplicatesFullPrereqs(this.m_shopItems, useSeedRandom: GameManager.Instance.IsSeeded));
                    else
                      this.m_shopItems.Add(GameManager.Instance.RewardManager.GetRewardObjectShopStyle(GameManager.Instance.PrimaryPlayer, true, excludedObjects: this.m_shopItems));
                  }
                  else
                    this.m_shopItems.Add((GameObject) null);
                }
                else
                {
                  float rewardWithPickup = GameManager.Instance.RewardManager.CurrentRewardData.ReplaceFirstRewardWithPickup;
                  if (!flag3 && (!isSeeded ? (double) UnityEngine.Random.value : (double) BraveRandom.GenerationRandomValue()) < (double) rewardWithPickup)
                  {
                    flag3 = true;
                    this.m_shopItems.Add(this.shopItems.SelectByWeightWithoutDuplicatesFullPrereqs(this.m_shopItems, weightModifier, GameManager.Instance.IsSeeded));
                  }
                  else if (!GameStatsManager.Instance.IsRainbowRun)
                    this.m_shopItems.Add(GameManager.Instance.RewardManager.GetRewardObjectShopStyle(GameManager.Instance.PrimaryPlayer, excludedObjects: this.m_shopItems));
                  else
                    this.m_shopItems.Add((GameObject) null);
                }
              }
              else
                this.m_shopItems.Add((GameObject) null);
            }
          }
          bool flag4 = GameStatsManager.Instance.GetFlag(GungeonFlags.ACHIEVEMENT_BIGGEST_WALLET) || (double) UnityEngine.Random.value < 0.05000000074505806;
          if (this.baseShopType == BaseShopController.AdditionalShopType.NONE && flag4 && !flag1)
          {
            PickupObject lockedPaydayItem = this.GetRandomLockedPaydayItem();
            if ((bool) (UnityEngine.Object) lockedPaydayItem)
            {
              if (this.m_shopItems.Count - count < this.spawnPositionsGroup2.Length)
                this.m_shopItems.Add(lockedPaydayItem.gameObject);
              else
                this.m_shopItems[UnityEngine.Random.Range(count, this.m_shopItems.Count)] = lockedPaydayItem.gameObject;
            }
          }
          for (int index = 0; index < this.spawnPositionsGroup2.Length; ++index)
          {
            Transform spawnTransform = this.spawnPositionsGroup2[index];
            if (!flag1 && !((UnityEngine.Object) this.m_shopItems[count + index] == (UnityEngine.Object) null))
            {
              PickupObject component5 = this.m_shopItems[count + index].GetComponent<PickupObject>();
              if (!((UnityEngine.Object) component5 == (UnityEngine.Object) null))
              {
                GameObject gameObject = new GameObject("Shop 2 item " + index.ToString());
                Transform transform = gameObject.transform;
                transform.parent = spawnTransform;
                transform.localPosition = Vector3.zero;
                EncounterTrackable component6 = component5.GetComponent<EncounterTrackable>();
                if ((UnityEngine.Object) component6 != (UnityEngine.Object) null)
                  GameManager.Instance.ExtantShopTrackableGuids.Add(component6.EncounterGuid);
                ShopItemController shopItemController = gameObject.AddComponent<ShopItemController>();
                this.AssignItemFacing(spawnTransform, shopItemController);
                if (!this.m_room.IsRegistered((IPlayerInteractable) shopItemController))
                  this.m_room.RegisterInteractable((IPlayerInteractable) shopItemController);
                shopItemController.Initialize(component5, this);
                this.m_itemControllers.Add(shopItemController);
              }
            }
          }
        }
        if (this.baseShopType == BaseShopController.AdditionalShopType.NONE || this.baseShopType == BaseShopController.AdditionalShopType.BLACKSMITH || this.baseShopType == BaseShopController.AdditionalShopType.FOYER_META)
        {
          List<ShopSubsidiaryZone> componentsInRoom = this.m_room.GetComponentsInRoom<ShopSubsidiaryZone>();
          for (int index = 0; index < componentsInRoom.Count; ++index)
            componentsInRoom[index].HandleSetup(this, this.m_room, this.m_shopItems, this.m_itemControllers);
        }
        for (int index = 0; index < this.m_itemControllers.Count; ++index)
        {
          if (this.baseShopType == BaseShopController.AdditionalShopType.KEY)
            this.m_itemControllers[index].CurrencyType = ShopItemController.ShopCurrencyType.KEYS;
          if (this.baseShopType == BaseShopController.AdditionalShopType.FOYER_META)
            this.m_itemControllers[index].CurrencyType = ShopItemController.ShopCurrencyType.META_CURRENCY;
        }
      }

      private void AssignItemFacing(Transform spawnTransform, ShopItemController itemController)
      {
        if (this.baseShopType == BaseShopController.AdditionalShopType.FOYER_META)
          itemController.UseOmnidirectionalItemFacing = true;
        else if (spawnTransform.name.Contains("SIDE") || spawnTransform.name.Contains("EAST"))
          itemController.itemFacing = DungeonData.Direction.EAST;
        else if (spawnTransform.name.Contains("WEST"))
        {
          itemController.itemFacing = DungeonData.Direction.WEST;
        }
        else
        {
          if (!spawnTransform.name.Contains("NORTH"))
            return;
          itemController.itemFacing = DungeonData.Direction.NORTH;
        }
      }

      private void LockItems()
      {
        for (int index = 0; index < this.m_itemControllers.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.m_itemControllers[index])
            this.m_itemControllers[index].Locked = true;
        }
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
          if ((bool) (UnityEngine.Object) allPlayer && allPlayer.healthHaver.IsAlive)
            allPlayer.ForceRefreshInteractable = true;
        }
      }

      public static MidGameStaticShopData GetStaticShopDataForMidGameSave()
      {
        return new MidGameStaticShopData()
        {
          MainShopkeepStealChance = BaseShopController.s_mainShopkeepStealChance,
          MainShopkeepItemsStolen = BaseShopController.s_mainShopkeepItemsStolen,
          EmptyFutureShops = BaseShopController.s_emptyFutureShops,
          HasDroppedSerJunkan = Chest.HasDroppedSerJunkanThisSession
        };
      }

      public static void LoadFromMidGameSave(MidGameStaticShopData ssd)
      {
        BaseShopController.s_mainShopkeepStealChance = ssd.MainShopkeepStealChance;
        BaseShopController.s_mainShopkeepItemsStolen = ssd.MainShopkeepItemsStolen;
        BaseShopController.s_emptyFutureShops = ssd.EmptyFutureShops;
        Chest.HasDroppedSerJunkanThisSession = ssd.HasDroppedSerJunkan;
      }

      public static void ClearStaticMemory()
      {
        BaseShopController.s_mainShopkeepItemsStolen = 0;
        BaseShopController.s_mainShopkeepStealChance = 1f;
        BaseShopController.s_emptyFutureShops = false;
      }

      public enum AdditionalShopType
      {
        NONE,
        GOOP,
        BLANK,
        KEY,
        CURSE,
        TRUCK,
        FOYER_META,
        BLACKSMITH,
        RESRAT_SHORTCUT,
      }

      protected enum ShopState
      {
        Default,
        GunDrawn,
        Hostile,
        PreTeleportAway,
        TeleportAway,
        Gone,
        RefuseService,
      }
    }

}
