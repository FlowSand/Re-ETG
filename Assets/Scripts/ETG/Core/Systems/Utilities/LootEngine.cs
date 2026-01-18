using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

    public static class LootEngine
    {
        private const float HIGH_AMMO_THRESHOLD = 0.9f;
        private const float LOW_AMMO_THRESHOLD = 0.1f;
        private const float AMMO_DROP_CHANCE_REDUCTION_FACTOR = 0.05f;

        public static void ClearPerLevelData()
        {
            StaticReferenceManager.WeaponChestsSpawnedOnFloor = 0;
            StaticReferenceManager.ItemChestsSpawnedOnFloor = 0;
            StaticReferenceManager.DChestsSpawnedOnFloor = 0;
        }

        public static void SpawnHealth(
            Vector2 centerPoint,
            int halfHearts,
            Vector2? direction,
            float startingZForce = 4f,
            float startingHeight = 0.05f)
        {
            int num;
            for (num = halfHearts; num >= 2; num -= 2)
                LootEngine.SpawnItem(GameManager.Instance.RewardManager.FullHeartPrefab.gameObject, (Vector3) centerPoint, !direction.HasValue ? Vector2.up : direction.Value, startingZForce);
            for (; num >= 1; --num)
                LootEngine.SpawnItem(GameManager.Instance.RewardManager.HalfHeartPrefab.gameObject, (Vector3) centerPoint, !direction.HasValue ? Vector2.up : direction.Value, startingZForce);
        }

        public static GameObject SpawnBowlerNote(
            GameObject note,
            Vector2 position,
            RoomHandler parentRoom,
            bool doPoof = false)
        {
            GameObject gObj = UnityEngine.Object.Instantiate<GameObject>(note, position.ToVector3ZisY(), Quaternion.identity);
            if ((bool) (UnityEngine.Object) gObj)
            {
                foreach (IPlayerInteractable interfacesInChild in gObj.GetInterfacesInChildren<IPlayerInteractable>())
                    parentRoom.RegisterInteractable(interfacesInChild);
            }
            if (doPoof)
            {
                tk2dBaseSprite component = ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"))).GetComponent<tk2dBaseSprite>();
                component.PlaceAtPositionByAnchor(position.ToVector3ZUp() + new Vector3(0.5f, 0.75f, 0.0f), tk2dBaseSprite.Anchor.MiddleCenter);
                component.HeightOffGround = 5f;
                component.UpdateZDepth();
            }
            return gObj;
        }

        public static void SpawnCurrency(
            Vector2 centerPoint,
            int amountToDrop,
            bool isMetaCurrency,
            Vector2? direction,
            float? angleVariance,
            float startingZForce = 4f,
            float startingHeight = 0.05f)
        {
            if (!isMetaCurrency && PassiveItem.IsFlagSetAtAll(typeof (BankBagItem)))
                amountToDrop *= 2;
            List<GameObject> currencyToDrop = GameManager.Instance.Dungeon.sharedSettingsPrefab.GetCurrencyToDrop(amountToDrop, isMetaCurrency);
            float num = 360f / (float) currencyToDrop.Count;
            if (angleVariance.HasValue)
                num = angleVariance.Value * 2f / (float) currencyToDrop.Count;
            Vector3 vector3 = Vector3.up;
            if (direction.HasValue && angleVariance.HasValue)
                vector3 = Quaternion.Euler(0.0f, 0.0f, -angleVariance.Value) * (Vector3) direction.Value;
            else if (direction.HasValue)
                vector3 = direction.Value.ToVector3ZUp();
            for (int index = 0; index < currencyToDrop.Count; ++index)
            {
                Vector3 vector = Quaternion.Euler(0.0f, 0.0f, num * (float) index) * vector3 * 2f;
                DebrisObject orAddComponent = SpawnManager.SpawnDebris(currencyToDrop[index], centerPoint.ToVector3ZUp(centerPoint.y), Quaternion.identity).GetOrAddComponent<DebrisObject>();
                orAddComponent.shouldUseSRBMotion = true;
                orAddComponent.angularVelocity = 0.0f;
                orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                orAddComponent.Trigger(vector.WithZ(startingZForce), startingHeight);
                orAddComponent.canRotate = false;
            }
        }

        public static void SpawnCurrencyManual(Vector2 centerPoint, int amountToDrop)
        {
            List<GameObject> currencyToDrop = GameManager.Instance.Dungeon.sharedSettingsPrefab.GetCurrencyToDrop(amountToDrop, randomAmounts: true);
            float num = 360f / (float) currencyToDrop.Count;
            Vector3 up = Vector3.up;
            List<CurrencyPickup> coins = new List<CurrencyPickup>();
            for (int index = 0; index < currencyToDrop.Count; ++index)
            {
                Vector3 vector = Quaternion.Euler(0.0f, 0.0f, num * (float) index) * up * 2f;
                GameObject gameObject = SpawnManager.SpawnDebris(currencyToDrop[index], centerPoint.ToVector3ZUp(centerPoint.y), Quaternion.identity);
                CurrencyPickup component1 = gameObject.GetComponent<CurrencyPickup>();
                component1.PreventPickup = true;
                coins.Add(component1);
                PickupMover component2 = gameObject.GetComponent<PickupMover>();
                if ((bool) (UnityEngine.Object) component2)
                    component2.enabled = false;
                DebrisObject orAddComponent = gameObject.GetOrAddComponent<DebrisObject>();
                orAddComponent.OnGrounded += (Action<DebrisObject>) (sourceDebris =>
                {
                    sourceDebris.GetComponent<CurrencyPickup>().PreventPickup = false;
                    sourceDebris.OnGrounded = (Action<DebrisObject>) null;
                });
                orAddComponent.shouldUseSRBMotion = true;
                orAddComponent.angularVelocity = 0.0f;
                orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                orAddComponent.Trigger(vector.WithZ(2f) * UnityEngine.Random.Range(1.5f, 2.125f), 0.05f);
                orAddComponent.canRotate = false;
            }
            GameManager.Instance.Dungeon.StartCoroutine(LootEngine.HandleManualCoinSpawnLifespan(coins));
        }

[DebuggerHidden]
        private static IEnumerator HandleManualCoinSpawnLifespan(List<CurrencyPickup> coins)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new LootEngine__HandleManualCoinSpawnLifespanc__Iterator0()
            {
                coins = coins
            };
        }

        public static void SpawnCurrency(Vector2 centerPoint, int amountToDrop, bool isMetaCurrency = false)
        {
            LootEngine.SpawnCurrency(centerPoint, amountToDrop, isMetaCurrency, new Vector2?(), new float?());
        }

        public static bool DoAmmoClipCheck(
            FloorRewardData currentRewardData,
            out LootEngine.AmmoDropType AmmoToDrop)
        {
            bool flag = LootEngine.DoAmmoClipCheck(currentRewardData.FloorChanceToDropAmmo);
            AmmoToDrop = LootEngine.AmmoDropType.DEFAULT_AMMO;
            if (!flag)
                return false;
            AmmoToDrop = (double) UnityEngine.Random.value >= (double) currentRewardData.FloorChanceForSpreadAmmo ? LootEngine.AmmoDropType.DEFAULT_AMMO : LootEngine.AmmoDropType.SPREAD_AMMO;
            return true;
        }

        public static bool DoAmmoClipCheck(float baseAmmoDropChance)
        {
            float num1 = baseAmmoDropChance;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                num1 *= GameManager.Instance.RewardManager.CoopAmmoChanceModifier;
            float num2 = 1f + Mathf.Clamp01((float) PlayerStats.GetTotalCurse() / 10f) / 2f;
            float num3 = num1 * num2;
            if (GameManager.Instance.AllPlayers != null)
            {
                float num4 = 0.0f;
                for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
                {
                    if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index1] && GameManager.Instance.AllPlayers[index1].inventory != null && GameManager.Instance.AllPlayers[index1].inventory.AllGuns != null)
                    {
                        for (int index2 = 0; index2 < GameManager.Instance.AllPlayers[index1].inventory.AllGuns.Count; ++index2)
                        {
                            Gun allGun = GameManager.Instance.AllPlayers[index1].inventory.AllGuns[index2];
                            if ((bool) (UnityEngine.Object) allGun && !allGun.InfiniteAmmo)
                                num4 = 1f;
                        }
                    }
                }
                num3 *= num4;
            }
            return (double) UnityEngine.Random.value < (double) num3;
        }

        private static void PostprocessGunSpawn(Gun spawnedGun)
        {
            spawnedGun.gameObject.SetActive(true);
            if (!GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_HAS_BEEN_PEDESTAL_MIMICKED) || GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.NONE || (double) UnityEngine.Random.value >= (double) GameManager.Instance.RewardManager.GunMimicMimicGunChance)
                return;
            spawnedGun.gameObject.AddComponent<MimicGunMimicModifier>();
        }

        private static void PostprocessItemSpawn(DebrisObject item)
        {
            tk2dSpriteAnimator component1 = item.gameObject.GetComponent<tk2dSpriteAnimator>();
            if ((UnityEngine.Object) item.GetComponent<CurrencyPickup>() == (UnityEngine.Object) null && !(bool) (UnityEngine.Object) item.GetComponent<BulletThatCanKillThePast>() && ((UnityEngine.Object) component1 == (UnityEngine.Object) null || !component1.playAutomatically))
                item.gameObject.GetOrAddComponent<SquishyBounceWiggler>();
            PlayerItem component2 = item.GetComponent<PlayerItem>();
            if ((UnityEngine.Object) component2 != (UnityEngine.Object) null && !RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) component2))
                RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) component2);
            PassiveItem component3 = item.GetComponent<PassiveItem>();
            if ((UnityEngine.Object) component3 != (UnityEngine.Object) null && !RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) component3))
                RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) component3);
            AmmoPickup component4 = item.GetComponent<AmmoPickup>();
            if ((UnityEngine.Object) component4 != (UnityEngine.Object) null && !RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) component4))
                RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) component4);
            HealthPickup component5 = item.GetComponent<HealthPickup>();
            if ((UnityEngine.Object) component5 != (UnityEngine.Object) null && !RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) component5))
                RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) component5);
            DebrisObject debrisObject = item;
            Action<DebrisObject> onGrounded = debrisObject.OnGrounded;
            // ISSUE: reference to a compiler-generated field
            if (LootEngine._f__mg_cache0 == null)
            {
                // ISSUE: reference to a compiler-generated field
                LootEngine._f__mg_cache0 = new Action<DebrisObject>(LootEngine.PostprocessItemSpawn);
            }
            // ISSUE: reference to a compiler-generated field
            Action<DebrisObject> fMgCache0 = LootEngine._f__mg_cache0;
            debrisObject.OnGrounded = onGrounded - fMgCache0;
        }

        private static DebrisObject SpawnInternal(
            GameObject spawnedItem,
            Vector3 spawnPosition,
            Vector2 spawnDirection,
            float force,
            bool invalidUntilGrounded = true,
            bool doDefaultItemPoof = false,
            bool disablePostprocessing = false,
            bool disableHeightBoost = false)
        {
            Vector3 vector = spawnDirection.ToVector3ZUp().normalized * force;
            Gun component1 = spawnedItem.GetComponent<Gun>();
            if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
            {
                LootEngine.PostprocessGunSpawn(component1);
                DebrisObject debrisObject = component1.DropGun(2f);
                if (doDefaultItemPoof)
                {
                    tk2dBaseSprite component2 = ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"))).GetComponent<tk2dBaseSprite>();
                    component2.PlaceAtPositionByAnchor(debrisObject.sprite.WorldCenter.ToVector3ZUp() + new Vector3(0.0f, 0.5f, 0.0f), tk2dBaseSprite.Anchor.MiddleCenter);
                    component2.HeightOffGround = 5f;
                    component2.UpdateZDepth();
                }
                return debrisObject;
            }
            DebrisObject orAddComponent = spawnedItem.GetOrAddComponent<DebrisObject>();
            if (!disablePostprocessing)
            {
                DebrisObject debrisObject = orAddComponent;
                Action<DebrisObject> onGrounded = debrisObject.OnGrounded;
                // ISSUE: reference to a compiler-generated field
                if (LootEngine._f__mg_cache1 == null)
                {
                    // ISSUE: reference to a compiler-generated field
                    LootEngine._f__mg_cache1 = new Action<DebrisObject>(LootEngine.PostprocessItemSpawn);
                }
                // ISSUE: reference to a compiler-generated field
                Action<DebrisObject> fMgCache1 = LootEngine._f__mg_cache1;
                debrisObject.OnGrounded = onGrounded + fMgCache1;
            }
            orAddComponent.additionalHeightBoost = !disableHeightBoost ? 1.5f : 0.0f;
            orAddComponent.shouldUseSRBMotion = true;
            orAddComponent.angularVelocity = 0.0f;
            orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
            orAddComponent.sprite.UpdateZDepth();
            orAddComponent.Trigger(vector.WithZ(2f), !disableHeightBoost ? 0.5f : 0.0f);
            orAddComponent.canRotate = false;
            if (invalidUntilGrounded && (UnityEngine.Object) orAddComponent.specRigidbody != (UnityEngine.Object) null)
            {
                orAddComponent.specRigidbody.CollideWithOthers = false;
                DebrisObject debrisObject = orAddComponent;
                Action<DebrisObject> onTouchedGround = debrisObject.OnTouchedGround;
                // ISSUE: reference to a compiler-generated field
                if (LootEngine._f__mg_cache2 == null)
                {
                    // ISSUE: reference to a compiler-generated field
                    LootEngine._f__mg_cache2 = new Action<DebrisObject>(LootEngine.BecomeViableItem);
                }
                // ISSUE: reference to a compiler-generated field
                Action<DebrisObject> fMgCache2 = LootEngine._f__mg_cache2;
                debrisObject.OnTouchedGround = onTouchedGround + fMgCache2;
            }
            orAddComponent.AssignFinalWorldDepth(-0.5f);
            if (doDefaultItemPoof)
            {
                tk2dBaseSprite component3 = ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"))).GetComponent<tk2dBaseSprite>();
                component3.PlaceAtPositionByAnchor(orAddComponent.sprite.WorldCenter.ToVector3ZUp() + new Vector3(0.0f, 0.5f, 0.0f), tk2dBaseSprite.Anchor.MiddleCenter);
                component3.HeightOffGround = 5f;
                component3.UpdateZDepth();
            }
            return orAddComponent;
        }

        public static void DoDefaultSynergyPoof(Vector2 worldPosition, bool ignoreTimeScale = false)
        {
            tk2dBaseSprite component1 = ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Synergy_Poof_001"))).GetComponent<tk2dBaseSprite>();
            component1.PlaceAtPositionByAnchor(worldPosition.ToVector3ZUp(), tk2dBaseSprite.Anchor.MiddleCenter);
            component1.HeightOffGround = 5f;
            component1.UpdateZDepth();
            if (!ignoreTimeScale)
                return;
            tk2dSpriteAnimator component2 = component1.GetComponent<tk2dSpriteAnimator>();
            if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null))
                return;
            component2.ignoreTimeScale = true;
            component2.alwaysUpdateOffscreen = true;
        }

        public static void DoDefaultPurplePoof(Vector2 worldPosition, bool ignoreTimeScale = false)
        {
            tk2dBaseSprite component1 = ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Purple_Smoke_001"))).GetComponent<tk2dBaseSprite>();
            component1.PlaceAtPositionByAnchor(worldPosition.ToVector3ZUp(), tk2dBaseSprite.Anchor.MiddleCenter);
            component1.HeightOffGround = 5f;
            component1.UpdateZDepth();
            if (!ignoreTimeScale)
                return;
            tk2dSpriteAnimator component2 = component1.GetComponent<tk2dSpriteAnimator>();
            if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null))
                return;
            component2.ignoreTimeScale = true;
            component2.alwaysUpdateOffscreen = true;
        }

        public static void DoDefaultItemPoof(Vector2 worldPosition, bool ignoreTimeScale = false, bool muteAudio = false)
        {
            tk2dBaseSprite component1 = ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"))).GetComponent<tk2dBaseSprite>();
            component1.PlaceAtPositionByAnchor(worldPosition.ToVector3ZUp(), tk2dBaseSprite.Anchor.MiddleCenter);
            component1.HeightOffGround = 5f;
            component1.UpdateZDepth();
            if (!ignoreTimeScale && !muteAudio)
                return;
            tk2dSpriteAnimator component2 = component1.GetComponent<tk2dSpriteAnimator>();
            if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null))
                return;
            if (ignoreTimeScale)
            {
                component2.ignoreTimeScale = true;
                component2.alwaysUpdateOffscreen = true;
            }
            if (!muteAudio)
                return;
            component2.MuteAudio = true;
        }

        public static DebrisObject DropItemWithoutInstantiating(
            GameObject item,
            Vector3 spawnPosition,
            Vector2 spawnDirection,
            float force,
            bool invalidUntilGrounded = true,
            bool doDefaultItemPoof = false,
            bool disablePostprocessing = false,
            bool disableHeightBoost = false)
        {
            if ((UnityEngine.Object) item.GetComponent<DebrisObject>() != (UnityEngine.Object) null)
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) item.GetComponent<DebrisObject>());
            item.GetComponent<Renderer>().enabled = true;
            item.transform.parent = (Transform) null;
            item.transform.position = spawnPosition;
            item.transform.rotation = Quaternion.identity;
            return LootEngine.SpawnInternal(item, spawnPosition, spawnDirection, force, invalidUntilGrounded, doDefaultItemPoof, disablePostprocessing, disableHeightBoost);
        }

        public static DebrisObject SpawnItem(
            GameObject item,
            Vector3 spawnPosition,
            Vector2 spawnDirection,
            float force,
            bool invalidUntilGrounded = true,
            bool doDefaultItemPoof = false,
            bool disableHeightBoost = false)
        {
            if (GameStatsManager.HasInstance && GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_AMMONOMICON_COMPLETE))
            {
                PickupObject component = item.GetComponent<PickupObject>();
                if ((bool) (UnityEngine.Object) component && component.PickupObjectId == GlobalItemIds.UnfinishedGun)
                    item = PickupObjectDatabase.GetById(GlobalItemIds.FinishedGun).gameObject;
            }
            return LootEngine.SpawnInternal(UnityEngine.Object.Instantiate<GameObject>(item, spawnPosition, Quaternion.identity), spawnPosition, spawnDirection, force, invalidUntilGrounded, doDefaultItemPoof, disableHeightBoost: disableHeightBoost);
        }

        public static void DelayedSpawnItem(
            float delay,
            GameObject item,
            Vector3 spawnPosition,
            Vector2 spawnDirection,
            float force,
            bool invalidUntilGrounded = true,
            bool doDefaultItemPoof = false,
            bool disableHeightBoost = false)
        {
            GameManager.Instance.StartCoroutine(LootEngine.DelayedSpawnItem_CR(delay, item, spawnPosition, spawnDirection, force, invalidUntilGrounded, doDefaultItemPoof, disableHeightBoost));
        }

[DebuggerHidden]
        private static IEnumerator DelayedSpawnItem_CR(
            float delay,
            GameObject item,
            Vector3 spawnPosition,
            Vector2 spawnDirection,
            float force,
            bool invalidUntilGrounded = true,
            bool doDefaultItemPoof = false,
            bool disableHeightBoost = false)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new LootEngine__DelayedSpawnItem_CRc__Iterator1()
            {
                delay = delay,
                item = item,
                spawnPosition = spawnPosition,
                spawnDirection = spawnDirection,
                force = force,
                invalidUntilGrounded = invalidUntilGrounded,
                doDefaultItemPoof = doDefaultItemPoof,
                disableHeightBoost = disableHeightBoost
            };
        }

        public static void GivePrefabToPlayer(GameObject item, PlayerController player)
        {
            Gun component1 = item.GetComponent<Gun>();
            if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
            {
                EncounterTrackable component2 = component1.GetComponent<EncounterTrackable>();
                if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
                    component2.HandleEncounter();
                if (player.CharacterUsesRandomGuns)
                    player.ChangeToRandomGun();
                else
                    player.inventory.AddGunToInventory(component1, true);
            }
            else
            {
                PickupObject component3 = UnityEngine.Object.Instantiate<GameObject>(item, Vector3.zero, Quaternion.identity).GetComponent<PickupObject>();
                if ((UnityEngine.Object) component3 != (UnityEngine.Object) null)
                {
                    if (component3 is PlayerItem)
                        (component3 as PlayerItem).ForceAsExtant = true;
                    component3.Pickup(player);
                }
                else
                    UnityEngine.Debug.LogError((object) $"Failed in giving item to player; item {item.name} is not a pickupObject.");
            }
        }

        public static Gun TryGiveGunToPlayer(GameObject item, PlayerController player, bool attemptForce = false)
        {
            Gun g = item.GetComponent<Gun>();
            if (!((UnityEngine.Object) g != (UnityEngine.Object) null))
                return (Gun) null;
            if (player.inventory.AllGuns.Count >= player.inventory.maxGuns && !attemptForce)
            {
                Gun gun = player.inventory.AllGuns.Find((Predicate<Gun>) (g2 => g2.PickupObjectId == g.PickupObjectId));
                if ((UnityEngine.Object) gun == (UnityEngine.Object) null || gun.CurrentAmmo >= gun.AdjustedMaxAmmo)
                {
                    LootEngine.SpewLoot(item, (Vector3) player.specRigidbody.UnitCenter);
                    return (Gun) null;
                }
            }
            EncounterTrackable component = g.GetComponent<EncounterTrackable>();
            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
                component.HandleEncounter();
            return player.inventory.AddGunToInventory(g, true);
        }

        public static bool TryGivePrefabToPlayer(
            GameObject item,
            PlayerController player,
            bool attemptForce = false)
        {
            Gun g = item.GetComponent<Gun>();
            if ((UnityEngine.Object) g != (UnityEngine.Object) null)
            {
                if (player.inventory.AllGuns.Count >= player.inventory.maxGuns && !attemptForce)
                {
                    Gun gun = player.inventory.AllGuns.Find((Predicate<Gun>) (g2 => g2.PickupObjectId == g.PickupObjectId));
                    if ((UnityEngine.Object) gun == (UnityEngine.Object) null || gun.CurrentAmmo >= gun.AdjustedMaxAmmo)
                    {
                        LootEngine.SpewLoot(item, (Vector3) player.specRigidbody.UnitCenter);
                        return false;
                    }
                }
                EncounterTrackable component = g.GetComponent<EncounterTrackable>();
                if ((UnityEngine.Object) component != (UnityEngine.Object) null)
                    component.HandleEncounter();
                player.inventory.AddGunToInventory(g, true);
                return true;
            }
            if ((bool) (UnityEngine.Object) item.GetComponent<PlayerItem>() && player.activeItems.Count >= player.maxActiveItemsHeld && !attemptForce)
            {
                LootEngine.SpewLoot(item, (Vector3) player.specRigidbody.UnitCenter);
                return false;
            }
            PickupObject component1 = UnityEngine.Object.Instantiate<GameObject>(item, Vector3.zero, Quaternion.identity).GetComponent<PickupObject>();
            if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
            {
                UnityEngine.Debug.LogError((object) $"Failed in giving item to player; item {item.name} is not a pickupObject.");
                return false;
            }
            if (component1 is PlayerItem)
                (component1 as PlayerItem).ForceAsExtant = true;
            component1.Pickup(player);
            return true;
        }

        private static void BecomeViableItem(DebrisObject debris)
        {
            debris.specRigidbody.CollideWithOthers = true;
        }

        public static DebrisObject SpewLoot(GameObject itemToSpawn, Vector3 spawnPosition)
        {
            Vector3 vector3 = Quaternion.Euler(0.0f, 0.0f, 0.0f) * Vector3.down * 2f;
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_AMMONOMICON_COMPLETE))
            {
                PickupObject component = itemToSpawn.GetComponent<PickupObject>();
                if ((bool) (UnityEngine.Object) component && component.PickupObjectId == GlobalItemIds.UnfinishedGun)
                    itemToSpawn = PickupObjectDatabase.GetById(GlobalItemIds.FinishedGun).gameObject;
            }
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(itemToSpawn, spawnPosition, Quaternion.identity);
            Gun component1 = gameObject.GetComponent<Gun>();
            DebrisObject debrisObject1;
            if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
            {
                LootEngine.PostprocessGunSpawn(component1);
                debrisObject1 = component1.DropGun(2f);
            }
            else
            {
                DebrisObject orAddComponent = gameObject.GetOrAddComponent<DebrisObject>();
                if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
                {
                    DebrisObject debrisObject2 = orAddComponent;
                    Action<DebrisObject> onGrounded = debrisObject2.OnGrounded;
                    // ISSUE: reference to a compiler-generated field
                    if (LootEngine._f__mg_cache3 == null)
                    {
                        // ISSUE: reference to a compiler-generated field
                        LootEngine._f__mg_cache3 = new Action<DebrisObject>(LootEngine.PostprocessItemSpawn);
                    }
                    // ISSUE: reference to a compiler-generated field
                    Action<DebrisObject> fMgCache3 = LootEngine._f__mg_cache3;
                    debrisObject2.OnGrounded = onGrounded + fMgCache3;
                }
                orAddComponent.FlagAsPickup();
                orAddComponent.additionalHeightBoost = 1.5f;
                orAddComponent.shouldUseSRBMotion = true;
                orAddComponent.angularVelocity = 0.0f;
                orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                orAddComponent.sprite.UpdateZDepth();
                orAddComponent.AssignFinalWorldDepth(-0.5f);
                orAddComponent.Trigger(Vector3.zero, 0.5f);
                orAddComponent.canRotate = false;
                debrisObject1 = orAddComponent;
            }
            return debrisObject1;
        }

        public static List<DebrisObject> SpewLoot(List<GameObject> itemsToSpawn, Vector3 spawnPosition)
        {
            List<DebrisObject> debrisObjectList = new List<DebrisObject>();
            float num1 = itemsToSpawn.Count != 8 ? 0.0f : 22.5f;
            float num2 = 360f / (float) itemsToSpawn.Count;
            for (int index = 0; index < itemsToSpawn.Count; ++index)
            {
                Vector3 vector = Quaternion.Euler(0.0f, 0.0f, num1 + num2 * (float) index) * Vector3.down * 2f;
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(itemsToSpawn[index], spawnPosition, Quaternion.identity);
                Gun component = gameObject.GetComponent<Gun>();
                if ((UnityEngine.Object) component != (UnityEngine.Object) null)
                {
                    LootEngine.PostprocessGunSpawn(component);
                    debrisObjectList.Add(component.DropGun(2f));
                }
                else
                {
                    DebrisObject orAddComponent = gameObject.GetOrAddComponent<DebrisObject>();
                    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
                    {
                        DebrisObject debrisObject = orAddComponent;
                        Action<DebrisObject> onGrounded = debrisObject.OnGrounded;
                        // ISSUE: reference to a compiler-generated field
                        if (LootEngine._f__mg_cache4 == null)
                        {
                            // ISSUE: reference to a compiler-generated field
                            LootEngine._f__mg_cache4 = new Action<DebrisObject>(LootEngine.PostprocessItemSpawn);
                        }
                        // ISSUE: reference to a compiler-generated field
                        Action<DebrisObject> fMgCache4 = LootEngine._f__mg_cache4;
                        debrisObject.OnGrounded = onGrounded + fMgCache4;
                    }
                    orAddComponent.additionalHeightBoost = 1.5f;
                    orAddComponent.shouldUseSRBMotion = true;
                    orAddComponent.angularVelocity = 0.0f;
                    orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
                    orAddComponent.sprite.UpdateZDepth();
                    orAddComponent.AssignFinalWorldDepth(-0.5f);
                    orAddComponent.Trigger(vector.WithZ(2f), 0.5f);
                    orAddComponent.canRotate = false;
                    debrisObjectList.Add(orAddComponent);
                }
            }
            for (int index = 0; index < debrisObjectList.Count; ++index)
            {
                if ((bool) (UnityEngine.Object) debrisObjectList[index].sprite)
                    debrisObjectList[index].sprite.UpdateZDepth();
            }
            return debrisObjectList;
        }

        private static PickupObject.ItemQuality GetRandomItemTier()
        {
            return (PickupObject.ItemQuality) UnityEngine.Random.Range(0, 6);
        }

        public static List<T> GetItemsOfQualityFromList<T>(
            List<T> validObjects,
            PickupObject.ItemQuality quality)
            where T : PickupObject
        {
            List<T> ofQualityFromList = new List<T>();
            for (int index = 0; index < validObjects.Count; ++index)
            {
                if (validObjects[index].quality == quality)
                    ofQualityFromList.Add(validObjects[index]);
            }
            return ofQualityFromList;
        }

        public static T GetItemOfTypeAndQuality<T>(
            PickupObject.ItemQuality itemQuality,
            GenericLootTable lootTable,
            bool anyQuality = false)
            where T : PickupObject
        {
            List<T> validObjects = new List<T>();
            if ((UnityEngine.Object) lootTable != (UnityEngine.Object) null)
            {
                List<WeightedGameObject> compiledRawItems = lootTable.GetCompiledRawItems();
                for (int index = 0; index < compiledRawItems.Count; ++index)
                {
                    if (!((UnityEngine.Object) compiledRawItems[index].gameObject == (UnityEngine.Object) null))
                    {
                        T component1 = compiledRawItems[index].gameObject.GetComponent<T>();
                        if ((UnityEngine.Object) component1 != (UnityEngine.Object) null && component1.PrerequisitesMet())
                        {
                            EncounterTrackable component2 = component1.GetComponent<EncounterTrackable>();
                            if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null) || GameStatsManager.Instance.QueryEncounterableDifferentiator(component2) <= 0)
                                validObjects.Add(component1);
                        }
                    }
                }
            }
            else
            {
                for (int index = 0; index < PickupObjectDatabase.Instance.Objects.Count; ++index)
                {
                    T obj = PickupObjectDatabase.Instance.Objects[index] as T;
                    if (!((object) obj is ContentTeaserGun) && !((object) obj is ContentTeaserItem) && (UnityEngine.Object) obj != (UnityEngine.Object) null && obj.PrerequisitesMet())
                    {
                        EncounterTrackable component = obj.GetComponent<EncounterTrackable>();
                        if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || GameStatsManager.Instance.QueryEncounterableDifferentiator(component) <= 0)
                            validObjects.Add(obj);
                    }
                }
            }
            if (validObjects.Count == 0)
                return (T) null;
            if (anyQuality)
            {
                if (validObjects.Count > 0)
                    return validObjects[UnityEngine.Random.Range(0, validObjects.Count)];
            }
            else
            {
                for (; itemQuality >= PickupObject.ItemQuality.COMMON; --itemQuality)
                {
                    List<T> ofQualityFromList = LootEngine.GetItemsOfQualityFromList<T>(validObjects, itemQuality);
                    if (ofQualityFromList.Count > 0)
                        return ofQualityFromList[UnityEngine.Random.Range(0, ofQualityFromList.Count)];
                }
            }
            return (T) null;
        }

public enum AmmoDropType
        {
            DEFAULT_AMMO,
            SPREAD_AMMO,
        }
    }

