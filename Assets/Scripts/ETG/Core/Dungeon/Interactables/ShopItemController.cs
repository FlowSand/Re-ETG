using System;
using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

public class ShopItemController : BraveBehaviour, IPlayerInteractable
    {
        public PickupObject item;
        public bool UseOmnidirectionalItemFacing;
        public DungeonData.Direction itemFacing = DungeonData.Direction.SOUTH;
        [NonSerialized]
        public PlayerController LastInteractingPlayer;
        public ShopItemController.ShopCurrencyType CurrencyType;
        public bool PrecludeAllDiscounts;
        public int CurrentPrice = -1;
        [NonSerialized]
        public int? OverridePrice;
        [NonSerialized]
        public bool SetsFlagOnSteal;
        [NonSerialized]
        public GungeonFlags FlagToSetOnSteal;
        [NonSerialized]
        public bool IsResourcefulRatKey;
        private bool pickedUp;
        private ShopController m_parentShop;
        private BaseShopController m_baseParentShop;
        private float THRESHOLD_CUTOFF_PRIMARY = 3f;
        private float THRESHOLD_CUTOFF_SECONDARY = 2f;
        [NonSerialized]
        private GameObject m_shadowObject;

        public bool Locked { get; set; }

        public int ModifiedPrice
        {
            get
            {
                if ((bool) (UnityEngine.Object) this.m_baseParentShop && this.m_baseParentShop.baseShopType == BaseShopController.AdditionalShopType.RESRAT_SHORTCUT)
                    return 0;
                if (this.IsResourcefulRatKey)
                {
                    int num = 1000 - Mathf.RoundToInt(GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.AMOUNT_PAID_FOR_RAT_KEY));
                    return num <= 0 ? this.CurrentPrice : num;
                }
                if (this.CurrencyType == ShopItemController.ShopCurrencyType.META_CURRENCY || this.CurrencyType == ShopItemController.ShopCurrencyType.KEYS)
                    return this.CurrentPrice;
                if (this.OverridePrice.HasValue)
                    return this.OverridePrice.Value;
                if (this.PrecludeAllDiscounts)
                    return this.CurrentPrice;
                float statValue = GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier);
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && (bool) (UnityEngine.Object) GameManager.Instance.SecondaryPlayer)
                    statValue *= GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.GlobalPriceMultiplier);
                GameLevelDefinition loadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
                float num1 = loadedLevelDefinition == null ? 1f : loadedLevelDefinition.priceMultiplier;
                float num2 = 1f;
                if ((UnityEngine.Object) this.m_baseParentShop != (UnityEngine.Object) null && (double) this.m_baseParentShop.ShopCostModifier != 1.0)
                    num2 *= this.m_baseParentShop.ShopCostModifier;
                if (this.m_baseParentShop.GetAbsoluteParentRoom().area.PrototypeRoomName.Contains("Black Market"))
                    num2 *= 0.5f;
                return Mathf.RoundToInt((float) this.CurrentPrice * statValue * num1 * num2);
            }
        }

        public bool Acquired => this.pickedUp;

        public void Initialize(PickupObject i, BaseShopController parent)
        {
            this.m_baseParentShop = parent;
            this.InitializeInternal(i);
            if (parent.baseShopType == BaseShopController.AdditionalShopType.NONE)
                return;
            this.sprite.depthUsesTrimmedBounds = true;
            this.sprite.HeightOffGround = -1.25f;
            this.sprite.UpdateZDepth();
        }

        public void Initialize(PickupObject i, ShopController parent)
        {
            this.m_parentShop = parent;
            this.InitializeInternal(i);
        }

        private void InitializeInternal(PickupObject i)
        {
            this.item = i;
            if (i is SpecialKeyItem && (i as SpecialKeyItem).keyType == SpecialKeyItem.SpecialKeyType.RESOURCEFUL_RAT_LAIR)
                this.IsResourcefulRatKey = true;
            if ((bool) (UnityEngine.Object) this.item && (bool) (UnityEngine.Object) this.item.encounterTrackable)
                GameStatsManager.Instance.SingleIncrementDifferentiator(this.item.encounterTrackable);
            this.CurrentPrice = this.item.PurchasePrice;
            if ((UnityEngine.Object) this.m_baseParentShop != (UnityEngine.Object) null && this.m_baseParentShop.baseShopType == BaseShopController.AdditionalShopType.KEY)
            {
                this.CurrentPrice = 1;
                if (this.item.quality == PickupObject.ItemQuality.A)
                    this.CurrentPrice = 2;
                if (this.item.quality == PickupObject.ItemQuality.S)
                    this.CurrentPrice = 3;
            }
            if ((UnityEngine.Object) this.m_baseParentShop != (UnityEngine.Object) null && this.m_baseParentShop.baseShopType == BaseShopController.AdditionalShopType.NONE && (this.item is BankMaskItem || this.item is BankBagItem || this.item is PaydayDrillItem))
            {
                EncounterTrackable encounterTrackable = this.item.encounterTrackable;
                if ((bool) (UnityEngine.Object) encounterTrackable && !encounterTrackable.PrerequisitesMet())
                {
                    if (this.item is BankMaskItem)
                    {
                        this.SetsFlagOnSteal = true;
                        this.FlagToSetOnSteal = GungeonFlags.ITEMSPECIFIC_STOLE_BANKMASK;
                    }
                    else if (this.item is BankBagItem)
                    {
                        this.SetsFlagOnSteal = true;
                        this.FlagToSetOnSteal = GungeonFlags.ITEMSPECIFIC_STOLE_BANKBAG;
                    }
                    else if (this.item is PaydayDrillItem)
                    {
                        this.SetsFlagOnSteal = true;
                        this.FlagToSetOnSteal = GungeonFlags.ITEMSPECIFIC_STOLE_DRILL;
                    }
                    this.OverridePrice = new int?(9999);
                }
            }
            this.gameObject.AddComponent<tk2dSprite>();
            tk2dSprite tk2dSprite = i.GetComponent<tk2dSprite>();
            if ((UnityEngine.Object) tk2dSprite == (UnityEngine.Object) null)
                tk2dSprite = i.GetComponentInChildren<tk2dSprite>();
            this.sprite.SetSprite(tk2dSprite.Collection, tk2dSprite.spriteId);
            this.sprite.IsPerpendicular = true;
            if (this.UseOmnidirectionalItemFacing)
                this.sprite.IsPerpendicular = false;
            this.sprite.HeightOffGround = 1f;
            if ((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null)
            {
                if (this.m_parentShop is MetaShopController)
                {
                    this.UseOmnidirectionalItemFacing = true;
                    this.sprite.IsPerpendicular = false;
                }
                this.sprite.HeightOffGround += this.m_parentShop.ItemHeightOffGroundModifier;
            }
            else if (this.m_baseParentShop.baseShopType == BaseShopController.AdditionalShopType.BLACKSMITH)
                this.UseOmnidirectionalItemFacing = true;
            else if (this.m_baseParentShop.baseShopType == BaseShopController.AdditionalShopType.TRUCK || this.m_baseParentShop.baseShopType == BaseShopController.AdditionalShopType.GOOP || this.m_baseParentShop.baseShopType == BaseShopController.AdditionalShopType.CURSE || this.m_baseParentShop.baseShopType == BaseShopController.AdditionalShopType.BLANK || this.m_baseParentShop.baseShopType == BaseShopController.AdditionalShopType.KEY || this.m_baseParentShop.baseShopType == BaseShopController.AdditionalShopType.RESRAT_SHORTCUT)
                this.UseOmnidirectionalItemFacing = true;
            this.sprite.PlaceAtPositionByAnchor(this.transform.parent.position, tk2dBaseSprite.Anchor.MiddleCenter);
            this.sprite.transform.position = this.sprite.transform.position.Quantize(1f / 16f);
            DepthLookupManager.ProcessRenderer(this.sprite.renderer);
            tk2dSprite componentInParent = this.transform.parent.gameObject.GetComponentInParent<tk2dSprite>();
            if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null)
                componentInParent.AttachRenderer(this.sprite);
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f, 0.05f);
            GameObject original = (GameObject) null;
            if ((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null && (UnityEngine.Object) this.m_parentShop.shopItemShadowPrefab != (UnityEngine.Object) null)
                original = this.m_parentShop.shopItemShadowPrefab;
            if ((UnityEngine.Object) this.m_baseParentShop != (UnityEngine.Object) null && (UnityEngine.Object) this.m_baseParentShop.shopItemShadowPrefab != (UnityEngine.Object) null)
                original = this.m_baseParentShop.shopItemShadowPrefab;
            if ((UnityEngine.Object) original != (UnityEngine.Object) null)
            {
                if (!(bool) (UnityEngine.Object) this.m_shadowObject)
                    this.m_shadowObject = UnityEngine.Object.Instantiate<GameObject>(original);
                tk2dBaseSprite component = this.m_shadowObject.GetComponent<tk2dBaseSprite>();
                component.PlaceAtPositionByAnchor((Vector3) this.sprite.WorldBottomCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                component.transform.position = component.transform.position.Quantize(1f / 16f);
                this.sprite.AttachRenderer(component);
                component.transform.parent = this.sprite.transform;
                component.HeightOffGround = -0.5f;
                if (this.m_parentShop is MetaShopController)
                    component.HeightOffGround = -1f / 16f;
            }
            this.sprite.UpdateZDepth();
            SpeculativeRigidbody orAddComponent = this.gameObject.GetOrAddComponent<SpeculativeRigidbody>();
            orAddComponent.PixelColliders = new List<PixelCollider>();
            PixelCollider pixelCollider = new PixelCollider()
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Circle,
                CollisionLayer = CollisionLayer.HighObstacle,
                ManualDiameter = 14
            };
            Vector2 vector2 = this.sprite.WorldCenter - this.transform.position.XY();
            pixelCollider.ManualOffsetX = PhysicsEngine.UnitToPixel(vector2.x) - 7;
            pixelCollider.ManualOffsetY = PhysicsEngine.UnitToPixel(vector2.y) - 7;
            orAddComponent.PixelColliders.Add(pixelCollider);
            orAddComponent.Initialize();
            orAddComponent.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate) null;
            orAddComponent.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.ItemOnPreRigidbodyCollision);
            this.RegenerateCache();
            if (GameManager.Instance.IsFoyer || !(this.item is Gun) || !GameManager.Instance.PrimaryPlayer.CharacterUsesRandomGuns)
                return;
            this.ForceOutOfStock();
        }

        private void ItemOnPreRigidbodyCollision(
            SpeculativeRigidbody myRigidbody,
            PixelCollider myPixelCollider,
            SpeculativeRigidbody otherRigidbody,
            PixelCollider otherPixelCollider)
        {
            if ((bool) (UnityEngine.Object) otherRigidbody && otherRigidbody.PrimaryPixelCollider != null && otherRigidbody.PrimaryPixelCollider.CollisionLayer == CollisionLayer.Projectile)
                return;
            PhysicsEngine.SkipCollision = true;
        }

        private void Update()
        {
            if (!(bool) (UnityEngine.Object) this.m_baseParentShop || this.m_baseParentShop.baseShopType != BaseShopController.AdditionalShopType.CURSE || this.pickedUp || !(bool) (UnityEngine.Object) this.sprite)
                return;
            PickupObject.HandlePickupCurseParticles(this.sprite, 1f);
        }

        protected override void OnDestroy()
        {
            if ((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null && this.m_parentShop is MetaShopController)
            {
                MetaShopController parentShop = this.m_parentShop as MetaShopController;
                if ((bool) (UnityEngine.Object) parentShop.Hologramophone && this.item is ItemBlueprintItem)
                    parentShop.Hologramophone.HideSprite(this.gameObject);
            }
            base.OnDestroy();
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!(bool) (UnityEngine.Object) this)
                return;
            SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
            Vector3 offset = new Vector3(this.sprite.GetBounds().max.x + 3f / 16f, this.sprite.GetBounds().min.y, 0.0f);
            EncounterTrackable component = this.item.GetComponent<EncounterTrackable>();
            string str1 = !((UnityEngine.Object) component != (UnityEngine.Object) null) ? this.item.DisplayName : component.journalData.GetPrimaryDisplayName();
            string str2 = this.ModifiedPrice.ToString();
            if ((UnityEngine.Object) this.m_baseParentShop != (UnityEngine.Object) null)
                str2 = this.m_baseParentShop.baseShopType != BaseShopController.AdditionalShopType.FOYER_META ? (this.m_baseParentShop.baseShopType != BaseShopController.AdditionalShopType.CURSE ? (this.m_baseParentShop.baseShopType != BaseShopController.AdditionalShopType.RESRAT_SHORTCUT ? (this.m_baseParentShop.baseShopType != BaseShopController.AdditionalShopType.KEY ? str2 + "[sprite \"ui_coin\"]" : str2 + "[sprite \"ui_key\"]") : "0[sprite \"ui_coin\"]?") : str2 + "[sprite \"ui_coin\"]?") : str2 + "[sprite \"hbux_text_icon\"]";
            if ((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null && this.m_parentShop is MetaShopController)
            {
                str2 += "[sprite \"hbux_text_icon\"]";
                MetaShopController parentShop = this.m_parentShop as MetaShopController;
                if ((bool) (UnityEngine.Object) parentShop.Hologramophone && this.item is ItemBlueprintItem)
                {
                    ItemBlueprintItem itemBlueprintItem = this.item as ItemBlueprintItem;
                    tk2dSpriteCollectionData encounterIconCollection = AmmonomiconController.ForceInstance.EncounterIconCollection;
                    parentShop.Hologramophone.ChangeToSprite(this.gameObject, encounterIconCollection, encounterIconCollection.GetSpriteIdByName(itemBlueprintItem.HologramIconSpriteName));
                }
            }
            string text = (bool) (UnityEngine.Object) this.m_baseParentShop && this.m_baseParentShop.IsCapableOfBeingStolenFrom || interactor.IsCapableOfStealing ? $"[color red]{str1}: {str2} {StringTableManager.GetString("#STEAL")}[/color]" : $"{str1}: {str2}";
            if (this.IsResourcefulRatKey)
            {
                int num1 = Mathf.RoundToInt(GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.AMOUNT_PAID_FOR_RAT_KEY));
                if (num1 < 1000)
                {
                    int num2 = Mathf.Min(interactor.carriedConsumables.Currency, 1000 - num1);
                    if (num2 > 0)
                        text = $"{text}[color green] (-{num2.ToString()})[/color]";
                }
            }
            dfLabel componentInChildren = GameUIRoot.Instance.RegisterDefaultLabel(this.transform, offset, text).GetComponentInChildren<dfLabel>();
            componentInChildren.ColorizeSymbols = false;
            componentInChildren.ProcessMarkup = true;
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!(bool) (UnityEngine.Object) this)
                return;
            SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f, 0.05f);
            GameUIRoot.Instance.DeregisterDefaultLabel(this.transform);
            if (!((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null) || !(this.m_parentShop is MetaShopController))
                return;
            MetaShopController parentShop = this.m_parentShop as MetaShopController;
            if (!(bool) (UnityEngine.Object) parentShop.Hologramophone || !(this.item is ItemBlueprintItem))
                return;
            parentShop.Hologramophone.HideSprite(this.gameObject);
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!(bool) (UnityEngine.Object) this || this.Locked)
                return 1000f;
            if (this.UseOmnidirectionalItemFacing)
            {
                Bounds bounds = this.sprite.GetBounds();
                return BraveMathCollege.DistToRectangle(point, (Vector2) (bounds.min + this.transform.position), (Vector2) bounds.size);
            }
            if (this.itemFacing == DungeonData.Direction.EAST)
            {
                Bounds bounds = this.sprite.GetBounds();
                bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
                Vector2 vector2 = bounds.center.XY();
                float num1 = vector2.x - point.x;
                float num2 = Mathf.Abs(point.y - vector2.y);
                return (double) num1 > 0.0 || (double) num1 < -(double) this.THRESHOLD_CUTOFF_PRIMARY || (double) num2 > (double) this.THRESHOLD_CUTOFF_SECONDARY ? 1000f : num2;
            }
            if (this.itemFacing == DungeonData.Direction.NORTH)
            {
                Bounds bounds = this.sprite.GetBounds();
                bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
                Vector2 vector2 = bounds.center.XY();
                float num3 = Mathf.Abs(point.x - vector2.x);
                float num4 = vector2.y - point.y;
                return (double) num4 > (double) bounds.extents.y || (double) num4 < -(double) this.THRESHOLD_CUTOFF_PRIMARY || (double) num3 > (double) this.THRESHOLD_CUTOFF_SECONDARY ? 1000f : num3;
            }
            if (this.itemFacing == DungeonData.Direction.WEST)
            {
                Bounds bounds = this.sprite.GetBounds();
                bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
                Vector2 vector2 = bounds.center.XY();
                float num5 = vector2.x - point.x;
                float num6 = Mathf.Abs(point.y - vector2.y);
                return (double) num5 < 0.0 || (double) num5 > (double) this.THRESHOLD_CUTOFF_PRIMARY || (double) num6 > (double) this.THRESHOLD_CUTOFF_SECONDARY ? 1000f : num6;
            }
            Bounds bounds1 = this.sprite.GetBounds();
            bounds1.SetMinMax(bounds1.min + this.transform.position, bounds1.max + this.transform.position);
            Vector2 vector2_1 = bounds1.center.XY();
            float num7 = Mathf.Abs(point.x - vector2_1.x);
            float num8 = vector2_1.y - point.y;
            return (double) num8 < (double) bounds1.extents.y || (double) num8 > (double) this.THRESHOLD_CUTOFF_PRIMARY || (double) num7 > (double) this.THRESHOLD_CUTOFF_SECONDARY ? 1000f : num7;
        }

        public float GetOverrideMaxDistance() => -1f;

        private bool ShouldSteal(PlayerController player)
        {
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                return false;
            return this.m_baseParentShop.IsCapableOfBeingStolenFrom || player.IsCapableOfStealing;
        }

        public void Interact(PlayerController player)
        {
            if ((bool) (UnityEngine.Object) this.item && this.item is HealthPickup)
            {
                if ((double) (this.item as HealthPickup).healAmount > 0.0 && (this.item as HealthPickup).armorAmount <= 0 && (double) player.healthHaver.GetCurrentHealthPercentage() >= 1.0)
                    return;
            }
            else if ((bool) (UnityEngine.Object) this.item && this.item is AmmoPickup && ((UnityEngine.Object) player.CurrentGun == (UnityEngine.Object) null || player.CurrentGun.ammo == player.CurrentGun.AdjustedMaxAmmo || !player.CurrentGun.CanGainAmmo || player.CurrentGun.InfiniteAmmo))
            {
                GameUIRoot.Instance.InformNeedsReload(player, new Vector3(player.specRigidbody.UnitCenter.x - player.transform.position.x, 1.25f, 0.0f), 1f, "#RELOAD_FULL");
                return;
            }
            this.LastInteractingPlayer = player;
            if (this.CurrencyType == ShopItemController.ShopCurrencyType.COINS || this.CurrencyType == ShopItemController.ShopCurrencyType.BLANKS || this.CurrencyType == ShopItemController.ShopCurrencyType.KEYS)
            {
                bool flag1 = false;
                bool flag2 = true;
                if (this.ShouldSteal(player))
                {
                    flag1 = this.m_baseParentShop.AttemptToSteal();
                    flag2 = false;
                    if (!flag1)
                    {
                        player.DidUnstealthyAction();
                        this.m_baseParentShop.NotifyStealFailed();
                        return;
                    }
                }
                if (flag2)
                {
                    bool flag3 = false;
                    if (this.CurrencyType == ShopItemController.ShopCurrencyType.COINS || this.CurrencyType == ShopItemController.ShopCurrencyType.BLANKS)
                        flag3 = player.carriedConsumables.Currency >= this.ModifiedPrice;
                    else if (this.CurrencyType == ShopItemController.ShopCurrencyType.KEYS)
                        flag3 = player.carriedConsumables.KeyBullets >= this.ModifiedPrice;
                    if (this.IsResourcefulRatKey)
                    {
                        if (!flag3)
                        {
                            if (Mathf.RoundToInt(GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.AMOUNT_PAID_FOR_RAT_KEY)) >= 1000)
                            {
                                int num = (int) AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", this.gameObject);
                                if ((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null)
                                    this.m_parentShop.NotifyFailedPurchase(this);
                                if (!((UnityEngine.Object) this.m_baseParentShop != (UnityEngine.Object) null))
                                    return;
                                this.m_baseParentShop.NotifyFailedPurchase(this);
                                return;
                            }
                            if (player.carriedConsumables.Currency > 0)
                            {
                                GameStatsManager.Instance.RegisterStatChange(TrackedStats.AMOUNT_PAID_FOR_RAT_KEY, (float) player.carriedConsumables.Currency);
                                player.carriedConsumables.Currency = 0;
                                this.OnExitRange(player);
                                this.OnEnteredRange(player);
                                return;
                            }
                            int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", this.gameObject);
                            if ((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null)
                                this.m_parentShop.NotifyFailedPurchase(this);
                            if (!((UnityEngine.Object) this.m_baseParentShop != (UnityEngine.Object) null))
                                return;
                            this.m_baseParentShop.NotifyFailedPurchase(this);
                            return;
                        }
                        player.carriedConsumables.Currency -= this.ModifiedPrice;
                        GameStatsManager.Instance.RegisterStatChange(TrackedStats.AMOUNT_PAID_FOR_RAT_KEY, (float) this.ModifiedPrice);
                        flag2 = false;
                    }
                    else if (!flag3)
                    {
                        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", this.gameObject);
                        if ((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null)
                            this.m_parentShop.NotifyFailedPurchase(this);
                        if (!((UnityEngine.Object) this.m_baseParentShop != (UnityEngine.Object) null))
                            return;
                        this.m_baseParentShop.NotifyFailedPurchase(this);
                        return;
                    }
                }
                if (this.pickedUp)
                    return;
                this.pickedUp = !this.item.PersistsOnPurchase;
                LootEngine.GivePrefabToPlayer(this.item.gameObject, player);
                if (flag2)
                {
                    if (this.CurrencyType == ShopItemController.ShopCurrencyType.COINS || this.CurrencyType == ShopItemController.ShopCurrencyType.BLANKS)
                        player.carriedConsumables.Currency -= this.ModifiedPrice;
                    else if (this.CurrencyType == ShopItemController.ShopCurrencyType.KEYS)
                        player.carriedConsumables.KeyBullets -= this.ModifiedPrice;
                }
                if ((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null)
                    this.m_parentShop.PurchaseItem(this, !flag1);
                if ((UnityEngine.Object) this.m_baseParentShop != (UnityEngine.Object) null)
                    this.m_baseParentShop.PurchaseItem(this, !flag1);
                if (flag1)
                {
                    player.ownerlessStatModifiers.Add(new StatModifier()
                    {
                        statToBoost = PlayerStats.StatType.Curse,
                        amount = 1f,
                        modifyType = StatModifier.ModifyMethod.ADDITIVE
                    });
                    player.stats.RecalculateStats(player);
                    player.HandleItemStolen(this);
                    this.m_baseParentShop.NotifyStealSucceeded();
                    player.IsThief = true;
                    GameStatsManager.Instance.RegisterStatChange(TrackedStats.MERCHANT_ITEMS_STOLEN, 1f);
                    if (this.SetsFlagOnSteal)
                        GameStatsManager.Instance.SetFlag(this.FlagToSetOnSteal, true);
                }
                else
                {
                    if (this.CurrencyType == ShopItemController.ShopCurrencyType.BLANKS)
                        ++player.Blanks;
                    player.HandleItemPurchased(this);
                }
                if (!this.item.PersistsOnPurchase)
                    GameUIRoot.Instance.DeregisterDefaultLabel(this.transform);
                int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_item_purchase_01", this.gameObject);
            }
            else
            {
                if (this.CurrencyType != ShopItemController.ShopCurrencyType.META_CURRENCY)
                    return;
                int num3 = Mathf.RoundToInt(GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.META_CURRENCY));
                if (num3 < this.ModifiedPrice)
                {
                    int num4 = (int) AkSoundEngine.PostEvent("Play_OBJ_purchase_unable_01", this.gameObject);
                    if ((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null)
                        this.m_parentShop.NotifyFailedPurchase(this);
                    if (!((UnityEngine.Object) this.m_baseParentShop != (UnityEngine.Object) null))
                        return;
                    this.m_baseParentShop.NotifyFailedPurchase(this);
                }
                else
                {
                    if (this.pickedUp)
                        return;
                    this.pickedUp = !this.item.PersistsOnPurchase;
                    GameStatsManager.Instance.ClearStatValueGlobal(TrackedStats.META_CURRENCY);
                    GameStatsManager.Instance.SetStat(TrackedStats.META_CURRENCY, (float) (num3 - this.ModifiedPrice));
                    GameStatsManager.Instance.RegisterStatChange(TrackedStats.META_CURRENCY_SPENT_AT_META_SHOP, (float) this.ModifiedPrice);
                    LootEngine.GivePrefabToPlayer(this.item.gameObject, player);
                    if ((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null)
                        this.m_parentShop.PurchaseItem(this);
                    if ((UnityEngine.Object) this.m_baseParentShop != (UnityEngine.Object) null)
                        this.m_baseParentShop.PurchaseItem(this);
                    player.HandleItemPurchased(this);
                    if (!this.item.PersistsOnPurchase)
                        GameUIRoot.Instance.DeregisterDefaultLabel(this.transform);
                    int num5 = (int) AkSoundEngine.PostEvent("Play_OBJ_item_purchase_01", this.gameObject);
                }
            }
        }

        public void ForceSteal(PlayerController player)
        {
            this.pickedUp = true;
            LootEngine.GivePrefabToPlayer(this.item.gameObject, player);
            if ((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null)
                this.m_parentShop.PurchaseItem(this, false, false);
            if ((UnityEngine.Object) this.m_baseParentShop != (UnityEngine.Object) null)
                this.m_baseParentShop.PurchaseItem(this, false, false);
            player.ownerlessStatModifiers.Add(new StatModifier()
            {
                statToBoost = PlayerStats.StatType.Curse,
                amount = 1f,
                modifyType = StatModifier.ModifyMethod.ADDITIVE
            });
            player.stats.RecalculateStats(player);
            player.HandleItemStolen(this);
            this.m_baseParentShop.NotifyStealSucceeded();
            player.IsThief = true;
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.MERCHANT_ITEMS_STOLEN, 1f);
            if (!this.m_baseParentShop.AttemptToSteal())
            {
                player.DidUnstealthyAction();
                this.m_baseParentShop.NotifyStealFailed();
            }
            GameUIRoot.Instance.DeregisterDefaultLabel(this.transform);
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_item_purchase_01", this.gameObject);
        }

        public void ForceOutOfStock()
        {
            this.pickedUp = true;
            if ((UnityEngine.Object) this.m_parentShop != (UnityEngine.Object) null)
                this.m_parentShop.PurchaseItem(this, false);
            if ((UnityEngine.Object) this.m_baseParentShop != (UnityEngine.Object) null)
                this.m_baseParentShop.PurchaseItem(this, false);
            GameUIRoot.Instance.DeregisterDefaultLabel(this.transform);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public enum ShopCurrencyType
        {
            COINS,
            META_CURRENCY,
            KEYS,
            BLANKS,
        }
    }

