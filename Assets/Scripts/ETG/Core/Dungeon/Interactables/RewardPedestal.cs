// Decompiled with JetBrains decompiler
// Type: RewardPedestal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class RewardPedestal : DungeonPlaceableBehaviour, IPlayerInteractable
    {
      [NonSerialized]
      public PickupObject contents;
      public bool UsesSpecificItem;
      [PickupIdentifier]
      [ShowInInspectorIf("UsesSpecificItem", true)]
      public int SpecificItemId = -1;
      [HideInInspectorIf("UsesSpecificItem", false)]
      public LootData lootTable;
      public bool UsesDelayedConfiguration;
      public Transform spawnTransform;
      [FormerlySerializedAs("spawnsHearts")]
      public bool SpawnsTertiarySet = true;
      [CheckAnimation(null)]
      public string spawnAnimName;
      public GameObject VFX_PreSpawn;
      public GameObject VFX_GroundHit;
      public float groundHitDelay = 0.73f;
      [NonSerialized]
      public bool pickedUp;
      [NonSerialized]
      public bool ReturnCoopPlayerOnLand;
      [NonSerialized]
      public bool IsBossRewardPedestal;
      private GameObject minimapIconInstance;
      private RoomHandler m_room;
      private RoomHandler m_registeredIconRoom;
      private tk2dBaseSprite m_itemDisplaySprite;
      private bool m_isMimic;
      private bool m_isMimicBreathing;
      [Header("Mimic")]
      [EnemyIdentifier]
      public string MimicGuid;
      public IntVector2 mimicOffset;
      [CheckAnimation(null)]
      public string preMimicIdleAnim;
      public float preMimicIdleAnimDelay = 3f;
      public float overrideMimicChance = -1f;
      private const float SPAWN_PUSH_RADIUS = 5f;
      private const float SPAWN_PUSH_FORCE = 22f;

      public bool OffsetTertiarySet { get; set; }

      public bool IsMimic => this.m_isMimic;

      private void Awake()
      {
        if (!(bool) (UnityEngine.Object) this.majorBreakable)
          return;
        this.majorBreakable.TemporarilyInvulnerable = true;
      }

      private void Start()
      {
        if (!this.UsesSpecificItem)
          return;
        this.m_room = this.GetAbsoluteParentRoom();
        this.HandleSpawnBehavior();
      }

      private void OnEnable()
      {
        if (!this.m_isMimic || this.m_isMimicBreathing)
          return;
        this.StartCoroutine(this.MimicIdleAnimCR());
      }

      public static RewardPedestal Spawn(RewardPedestal pedestalPrefab, IntVector2 basePosition)
      {
        RoomHandler roomFromPosition = GameManager.Instance.Dungeon.GetRoomFromPosition(basePosition);
        return RewardPedestal.Spawn(pedestalPrefab, basePosition, roomFromPosition);
      }

      public static RewardPedestal Spawn(
        RewardPedestal pedestalPrefab,
        IntVector2 basePosition,
        RoomHandler room)
      {
        if ((UnityEngine.Object) pedestalPrefab == (UnityEngine.Object) null)
          return (RewardPedestal) null;
        RewardPedestal component = UnityEngine.Object.Instantiate<GameObject>(pedestalPrefab.gameObject, basePosition.ToVector3() + new Vector3(3f / 16f, 0.0f, 0.0f), Quaternion.identity).GetComponent<RewardPedestal>();
        component.m_room = room;
        component.HandleSpawnBehavior();
        return component;
      }

      public void RegisterChestOnMinimap(RoomHandler r)
      {
        if (GameStatsManager.HasInstance && GameStatsManager.Instance.IsRainbowRun)
          return;
        this.m_registeredIconRoom = r;
        GameObject iconPrefab = BraveResources.Load("Global Prefabs/Minimap_Treasure_Icon") as GameObject;
        this.minimapIconInstance = Minimap.Instance.RegisterRoomIcon(r, iconPrefab);
      }

      public void GiveCoopPartnerBack()
      {
        PlayerController playerController = !GameManager.Instance.PrimaryPlayer.healthHaver.IsDead ? GameManager.Instance.SecondaryPlayer : GameManager.Instance.PrimaryPlayer;
        playerController.specRigidbody.enabled = true;
        playerController.gameObject.SetActive(true);
        playerController.ResurrectFromBossKill();
      }

      private void HandleSpawnBehavior()
      {
        GameManager.Instance.Dungeon.StartCoroutine(this.SpawnBehavior_CR());
      }

      [DebuggerHidden]
      private IEnumerator SpawnBehavior_CR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RewardPedestal.\u003CSpawnBehavior_CR\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      protected void DetermineContents(PlayerController player)
      {
        if ((UnityEngine.Object) this.contents == (UnityEngine.Object) null)
        {
          if (this.IsBossRewardPedestal)
          {
            if (GameStatsManager.Instance.IsRainbowRun)
            {
              LootEngine.SpawnBowlerNote(GameManager.Instance.RewardManager.BowlerNoteBoss, this.sprite.WorldCenter + new Vector2(-0.5f, -3f), this.GetAbsoluteParentRoom());
              return;
            }
            if ((UnityEngine.Object) this.lootTable.lootTable != (UnityEngine.Object) null)
            {
              this.contents = this.lootTable.lootTable.SelectByWeightWithoutDuplicatesFullPrereqs((List<GameObject>) null).GetComponent<PickupObject>();
            }
            else
            {
              if (GameManager.Instance.IsSeeded)
              {
                FloorRewardManifest manifestForCurrentFloor = GameManager.Instance.RewardManager.GetSeededManifestForCurrentFloor();
                if (manifestForCurrentFloor != null)
                  this.contents = manifestForCurrentFloor.GetNextBossReward(GameManager.Instance.RewardManager.IsBossRewardForcedGun());
              }
              if ((UnityEngine.Object) this.contents == (UnityEngine.Object) null)
                this.contents = GameManager.Instance.RewardManager.GetRewardObjectBossStyle(player).GetComponent<PickupObject>();
            }
          }
          else if (this.UsesSpecificItem)
            this.contents = PickupObjectDatabase.GetById(this.SpecificItemId);
          else if ((UnityEngine.Object) this.lootTable.lootTable == (UnityEngine.Object) null)
            this.contents = GameManager.Instance.Dungeon.baseChestContents.SelectByWeight().GetComponent<PickupObject>();
          else if (this.lootTable != null)
          {
            this.contents = this.lootTable.GetSingleItemForPlayer(player);
            if (!((UnityEngine.Object) this.contents == (UnityEngine.Object) null))
              ;
          }
        }
        if (!((UnityEngine.Object) this.m_itemDisplaySprite == (UnityEngine.Object) null))
          return;
        this.m_itemDisplaySprite = (tk2dBaseSprite) tk2dSprite.AddComponent(new GameObject("Display Sprite")
        {
          transform = {
            parent = this.spawnTransform
          }
        }, this.contents.sprite.Collection, this.contents.sprite.spriteId);
        SpriteOutlineManager.AddOutlineToSprite(this.m_itemDisplaySprite, Color.black, 0.1f, 0.05f);
        this.sprite.AttachRenderer(this.m_itemDisplaySprite);
        this.m_itemDisplaySprite.HeightOffGround = 0.25f;
        this.m_itemDisplaySprite.depthUsesTrimmedBounds = true;
        this.m_itemDisplaySprite.PlaceAtPositionByAnchor(this.spawnTransform.position, tk2dBaseSprite.Anchor.LowerCenter);
        this.m_itemDisplaySprite.transform.position = this.m_itemDisplaySprite.transform.position.Quantize(1f / 16f);
        tk2dBaseSprite component = ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"))).GetComponent<tk2dBaseSprite>();
        component.PlaceAtPositionByAnchor(this.m_itemDisplaySprite.WorldCenter.ToVector3ZUp(), tk2dBaseSprite.Anchor.MiddleCenter);
        component.HeightOffGround = 5f;
        component.UpdateZDepth();
        this.sprite.UpdateZDepth();
      }

      private void DoPickup(PlayerController player)
      {
        if (this.pickedUp)
          return;
        this.pickedUp = true;
        if (this.IsMimic && (UnityEngine.Object) this.contents != (UnityEngine.Object) null)
        {
          this.DoMimicTransformation(this.contents);
        }
        else
        {
          if ((UnityEngine.Object) this.contents != (UnityEngine.Object) null)
          {
            LootEngine.GivePrefabToPlayer(this.contents.gameObject, player);
            if (this.contents is Gun)
            {
              int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_weapon_pickup_01", this.gameObject);
            }
            else
            {
              int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_item_pickup_01", this.gameObject);
            }
            tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>((GameObject) ResourceCache.Acquire("Global VFX/VFX_Item_Pickup")).GetComponent<tk2dSprite>();
            component.PlaceAtPositionByAnchor((Vector3) this.m_itemDisplaySprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
            component.HeightOffGround = 6f;
            component.UpdateZDepth();
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_itemDisplaySprite);
          }
          if (this.m_registeredIconRoom == null)
            return;
          Minimap.Instance.DeregisterRoomIcon(this.m_registeredIconRoom, this.minimapIconInstance);
        }
      }

      private void DoMimicTransformation(PickupObject overrideDeathRewards)
      {
        if ((bool) (UnityEngine.Object) this.m_itemDisplaySprite)
        {
          tk2dBaseSprite component = ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"))).GetComponent<tk2dBaseSprite>();
          component.PlaceAtPositionByAnchor(this.m_itemDisplaySprite.WorldCenter.ToVector3ZUp(), tk2dBaseSprite.Anchor.MiddleCenter);
          component.HeightOffGround = 5f;
          component.UpdateZDepth();
        }
        this.sprite.UpdateZDepth();
        IntVector2 intVector2_1 = this.specRigidbody.UnitBottomLeft.ToIntVector2(VectorConversions.Floor);
        IntVector2 intVector2_2 = this.specRigidbody.UnitTopRight.ToIntVector2(VectorConversions.Floor);
        for (int x = intVector2_1.x; x <= intVector2_2.x; ++x)
        {
          for (int y = intVector2_1.y; y <= intVector2_2.y; ++y)
            GameManager.Instance.Dungeon.data[new IntVector2(x, y)].isOccupied = false;
        }
        if (!this.pickedUp)
        {
          this.pickedUp = true;
          this.m_room.DeregisterInteractable((IPlayerInteractable) this);
        }
        if (this.m_registeredIconRoom != null)
          Minimap.Instance.DeregisterRoomIcon(this.m_registeredIconRoom, this.minimapIconInstance);
        AIActor aiActor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(this.MimicGuid), this.transform.position.XY().ToIntVector2(VectorConversions.Floor), this.GetAbsoluteParentRoom());
        if ((UnityEngine.Object) overrideDeathRewards != (UnityEngine.Object) null)
          aiActor.AdditionalSafeItemDrops.Add(overrideDeathRewards);
        PickupObject ofTypeAndQuality = LootEngine.GetItemOfTypeAndQuality<PickupObject>(!BraveUtility.RandomBool() ? PickupObject.ItemQuality.C : PickupObject.ItemQuality.D, !BraveUtility.RandomBool() ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable);
        if ((bool) (UnityEngine.Object) ofTypeAndQuality)
          aiActor.AdditionalSafeItemDrops.Add(ofTypeAndQuality);
        aiActor.specRigidbody.Initialize();
        Vector2 unitBottomLeft1 = aiActor.specRigidbody.UnitBottomLeft;
        Vector2 unitBottomLeft2 = this.specRigidbody.UnitBottomLeft;
        aiActor.transform.position -= (Vector3) (unitBottomLeft1 - unitBottomLeft2);
        aiActor.transform.position += (Vector3) PhysicsEngine.PixelToUnit(this.mimicOffset);
        aiActor.specRigidbody.Reinitialize();
        aiActor.HasDonePlayerEnterCheck = true;
        GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_HAS_BEEN_PEDESTAL_MIMICKED, true);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }

      public void MaybeBecomeMimic()
      {
        this.m_isMimic = false;
        bool flag1 = false;
        if (string.IsNullOrEmpty(this.MimicGuid))
          return;
        bool flag2 = flag1 | GameManager.Instance.Dungeon.sharedSettingsPrefab.RandomShouldBecomePedestalMimic(this.overrideMimicChance);
        GameLevelDefinition loadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
        bool flag3 = ((flag2 ? 1 : 0) | (loadedLevelDefinition == null || loadedLevelDefinition.lastSelectedFlowEntry == null ? 0 : (loadedLevelDefinition.lastSelectedFlowEntry.levelMode == FlowLevelEntryMode.ALL_MIMICS ? 1 : 0))) != 0;
        if (this.m_room != null)
        {
          string roomName = this.m_room.GetRoomName();
          if (roomName.StartsWith("DemonWallRoom"))
            flag3 = false;
          if (roomName.StartsWith("DoubleBeholsterRoom"))
            flag3 = BraveUtility.RandomBool();
        }
        if (!flag3 || PassiveItem.IsFlagSetAtAll(typeof (MimicRingItem)))
          return;
        this.m_isMimic = true;
        if (!this.gameObject.activeInHierarchy)
          return;
        this.StartCoroutine(this.MimicIdleAnimCR());
      }

      [DebuggerHidden]
      private IEnumerator MimicIdleAnimCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RewardPedestal.\u003CMimicIdleAnimCR\u003Ec__Iterator1()
        {
          \u0024this = this
        };
      }

      private void OnDamaged(float damage)
      {
        if (!this.m_isMimic)
          return;
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.PREFIRE_ON_MIMIC);
        this.DoMimicTransformation(this.contents);
      }

      public void OnEnteredRange(PlayerController interactor)
      {
        if (!(bool) (UnityEngine.Object) this)
          return;
        if ((UnityEngine.Object) this.m_itemDisplaySprite != (UnityEngine.Object) null)
        {
          SpriteOutlineManager.RemoveOutlineFromSprite(this.m_itemDisplaySprite, true);
          SpriteOutlineManager.AddOutlineToSprite(this.m_itemDisplaySprite, Color.white, 0.1f);
        }
        this.sprite.UpdateZDepth();
      }

      public void OnExitRange(PlayerController interactor)
      {
        if (!(bool) (UnityEngine.Object) this)
          return;
        if ((UnityEngine.Object) this.m_itemDisplaySprite != (UnityEngine.Object) null)
        {
          SpriteOutlineManager.RemoveOutlineFromSprite(this.m_itemDisplaySprite, true);
          SpriteOutlineManager.AddOutlineToSprite(this.m_itemDisplaySprite, Color.black, 0.1f, 0.05f);
        }
        this.sprite.UpdateZDepth();
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        Bounds bounds = this.sprite.GetBounds();
        bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
        float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
        float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
        return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
      }

      public float GetOverrideMaxDistance() => -1f;

      public void Interact(PlayerController player)
      {
        if (this.pickedUp)
          return;
        this.m_room.DeregisterInteractable((IPlayerInteractable) this);
        if ((UnityEngine.Object) this.m_itemDisplaySprite != (UnityEngine.Object) null)
          SpriteOutlineManager.RemoveOutlineFromSprite(this.m_itemDisplaySprite);
        this.DoPickup(player);
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      public void ForceConfiguration() => this.DetermineContents(GameManager.Instance.PrimaryPlayer);

      public void ConfigureOnPlacement(RoomHandler room)
      {
        this.m_room = room;
        this.RegisterChestOnMinimap(room);
        if (this.UsesDelayedConfiguration)
          return;
        this.ForceConfiguration();
      }

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.majorBreakable)
          this.majorBreakable.OnDamaged -= new Action<float>(this.OnDamaged);
        base.OnDestroy();
      }
    }

}
