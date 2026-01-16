// Decompiled with JetBrains decompiler
// Type: ShopController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class ShopController : DungeonPlaceableBehaviour, IPlaceConfigurable
{
  [Header("Spawn Group 1")]
  public GenericLootTable shopItems;
  public Transform[] spawnPositions;
  [Header("Spawn Group 2")]
  public GenericLootTable shopItemsGroup2;
  public Transform[] spawnPositionsGroup2;
  [Header("Other Settings")]
  public tk2dSpriteAnimator speakerAnimator;
  public Transform speechPoint;
  public float ItemHeightOffGroundModifier;
  public GameObject shopItemShadowPrefab;
  protected List<GameObject> m_shopItems;
  protected List<ShopItemController> m_itemControllers;
  protected bool firstTime = true;
  [NonSerialized]
  public int StolenCount;
  protected string defaultTalkAction = "talk";
  protected string defaultIdleAction = "idle";
  protected RoomHandler m_room;

  public RoomHandler Room => this.m_room;

  protected virtual void Start() => this.DoSetup();

  public virtual void OnRoomEnter(PlayerController p)
  {
    if (p.IsStealthed)
      return;
    if (this.firstTime)
    {
      this.firstTime = false;
      this.OnInitialRoomEnter();
    }
    else
      this.OnSequentialRoomEnter();
  }

  public virtual void OnRoomExit()
  {
    if ((bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer && GameManager.Instance.PrimaryPlayer.IsStealthed)
      return;
    this.GunsmithTalk(StringTableManager.GetString("#SHOP_EXIT"));
  }

  protected virtual void OnInitialRoomEnter()
  {
    this.GunsmithTalk(StringTableManager.GetString("#SHOP_ENTER"));
  }

  protected virtual void OnSequentialRoomEnter()
  {
    this.GunsmithTalk(StringTableManager.GetString("#SHOP_REENTER"));
  }

  protected virtual void GunsmithTalk(string message)
  {
    TextBoxManager.ShowTextBox(this.speechPoint.position, this.speechPoint, 5f, message, "shopkeep", false);
    this.speakerAnimator.PlayForDuration(this.defaultTalkAction, 2.5f, this.defaultIdleAction);
  }

  public virtual void OnBetrayalWarning()
  {
    this.speakerAnimator.PlayForDuration("scold", 1f, this.defaultIdleAction);
    TextBoxManager.ShowTextBox(this.speechPoint.position, this.speechPoint, 5f, StringTableManager.GetString("#SHOPKEEP_BETRAYAL_WARNING"), "shopkeep", false);
  }

  public virtual void PullGun()
  {
    this.speakerAnimator.Play("gun");
    this.defaultIdleAction = "gun_idle";
    this.defaultTalkAction = "gun_talk";
    TextBoxManager.ShowTextBox(this.speechPoint.position, this.speechPoint, 5f, StringTableManager.GetString("#SHOPKEEP_ANGRYTOWN"), "shopkeep", false);
    for (int index = 0; index < this.m_itemControllers.Count; ++index)
      this.m_itemControllers[index].CurrentPrice *= 2;
    TalkDoer component = this.speakerAnimator.GetComponent<TalkDoer>();
    component.modules[0].stringKeys[0] = "#SHOPKEEP_ANGRY_CHAT";
    component.modules[0].usesAnimation = true;
    component.modules[0].animationDuration = 2.5f;
    component.modules[0].animationName = "gun_talk";
    component.defaultSpeechAnimName = "gun_talk";
    component.fallbackAnimName = "gun_idle";
  }

  public virtual void NotifyFailedPurchase(ShopItemController itemController)
  {
    TextBoxManager.ShowTextBox(this.speechPoint.position, this.speechPoint, 5f, StringTableManager.GetString("#SHOP_NOMONEY"), "shopkeep", false);
    if (this.defaultIdleAction == "idle")
      this.speakerAnimator.PlayForDuration("shake", 2.5f, this.defaultIdleAction);
    else
      this.speakerAnimator.PlayForDuration("scold", 1f, this.defaultIdleAction);
  }

  public virtual void ReturnToIdle(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
  {
    animator.Play(this.defaultIdleAction);
    animator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.ReturnToIdle);
  }

  public virtual void PurchaseItem(ShopItemController item, bool actualPurchase = true, bool allowSign = true)
  {
    if (actualPurchase)
    {
      if (this.defaultIdleAction == "gun_idle")
      {
        this.GunsmithTalk(StringTableManager.GetString("#SHOPKEEP_PURCHASE_ANGRY"));
      }
      else
      {
        this.GunsmithTalk(StringTableManager.GetString("#SHOP_PURCHASE"));
        this.speakerAnimator.PlayForDuration("nod", 1.5f, this.defaultIdleAction);
      }
    }
    if (!item.item.PersistsOnPurchase)
    {
      this.m_room.DeregisterInteractable((IPlayerInteractable) item);
      if (allowSign)
      {
        GameObject gameObject = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Global Prefabs/Sign_SoldOut"));
        gameObject.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor((Vector3) item.sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
        gameObject.transform.position = gameObject.transform.position.Quantize(1f / 16f);
      }
    }
    UnityEngine.Object.Destroy((UnityEngine.Object) item.gameObject);
  }

  public virtual void ConfigureOnPlacement(RoomHandler room)
  {
    room.IsShop = true;
    this.m_room = room;
  }

  protected virtual void DoSetup()
  {
    this.m_shopItems = new List<GameObject>();
    this.m_room.Entered += new RoomHandler.OnEnteredEventHandler(this.OnRoomEnter);
    this.m_room.Exited += new RoomHandler.OnExitedEventHandler(this.OnRoomExit);
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
    for (int index = 0; index < this.spawnPositions.Length; ++index)
      this.m_shopItems.Add(this.shopItems.SelectByWeightWithoutDuplicatesFullPrereqs(this.m_shopItems, weightModifier));
    this.m_itemControllers = new List<ShopItemController>();
    for (int index = 0; index < this.spawnPositions.Length; ++index)
    {
      Transform spawnPosition = this.spawnPositions[index];
      if (!((UnityEngine.Object) this.m_shopItems[index] == (UnityEngine.Object) null))
      {
        PickupObject component1 = this.m_shopItems[index].GetComponent<PickupObject>();
        if (!((UnityEngine.Object) component1 == (UnityEngine.Object) null))
        {
          GameObject gameObject = new GameObject("Shop item " + index.ToString());
          Transform transform = gameObject.transform;
          transform.parent = spawnPosition;
          transform.localPosition = Vector3.zero;
          EncounterTrackable component2 = component1.GetComponent<EncounterTrackable>();
          if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
            GameManager.Instance.ExtantShopTrackableGuids.Add(component2.EncounterGuid);
          ShopItemController ixable = gameObject.AddComponent<ShopItemController>();
          if (spawnPosition.name.Contains("SIDE") || spawnPosition.name.Contains("EAST"))
            ixable.itemFacing = DungeonData.Direction.EAST;
          else if (spawnPosition.name.Contains("WEST"))
            ixable.itemFacing = DungeonData.Direction.WEST;
          else if (spawnPosition.name.Contains("NORTH"))
            ixable.itemFacing = DungeonData.Direction.NORTH;
          if (!this.m_room.IsRegistered((IPlayerInteractable) ixable))
            this.m_room.RegisterInteractable((IPlayerInteractable) ixable);
          ixable.Initialize(component1, this);
          this.m_itemControllers.Add(ixable);
        }
      }
    }
    if ((UnityEngine.Object) this.shopItemsGroup2 != (UnityEngine.Object) null && this.spawnPositionsGroup2.Length > 0)
    {
      int count = this.m_shopItems.Count;
      for (int index = 0; index < this.spawnPositionsGroup2.Length; ++index)
      {
        if ((double) UnityEngine.Random.value < 1.0 - (double) index * 0.25)
          this.m_shopItems.Add(GameManager.Instance.RewardManager.GetRewardObjectShopStyle(GameManager.Instance.PrimaryPlayer, excludedObjects: this.m_shopItems));
        else
          this.m_shopItems.Add((GameObject) null);
      }
      for (int index = 0; index < this.spawnPositionsGroup2.Length; ++index)
      {
        Transform transform1 = this.spawnPositionsGroup2[index];
        if (!((UnityEngine.Object) this.m_shopItems[count + index] == (UnityEngine.Object) null))
        {
          PickupObject component3 = this.m_shopItems[count + index].GetComponent<PickupObject>();
          if (!((UnityEngine.Object) component3 == (UnityEngine.Object) null))
          {
            GameObject gameObject = new GameObject("Shop 2 item " + index.ToString());
            Transform transform2 = gameObject.transform;
            transform2.parent = transform1;
            transform2.localPosition = Vector3.zero;
            EncounterTrackable component4 = component3.GetComponent<EncounterTrackable>();
            if ((UnityEngine.Object) component4 != (UnityEngine.Object) null)
              GameManager.Instance.ExtantShopTrackableGuids.Add(component4.EncounterGuid);
            ShopItemController ixable = gameObject.AddComponent<ShopItemController>();
            if (transform1.name.Contains("SIDE") || transform1.name.Contains("EAST"))
              ixable.itemFacing = DungeonData.Direction.EAST;
            else if (transform1.name.Contains("WEST"))
              ixable.itemFacing = DungeonData.Direction.WEST;
            else if (transform1.name.Contains("NORTH"))
              ixable.itemFacing = DungeonData.Direction.NORTH;
            if (!this.m_room.IsRegistered((IPlayerInteractable) ixable))
              this.m_room.RegisterInteractable((IPlayerInteractable) ixable);
            ixable.Initialize(component3, this);
            this.m_itemControllers.Add(ixable);
          }
        }
      }
    }
    List<ShopSubsidiaryZone> componentsInRoom = this.m_room.GetComponentsInRoom<ShopSubsidiaryZone>();
    for (int index = 0; index < componentsInRoom.Count; ++index)
      componentsInRoom[index].HandleSetup(this, this.m_room, this.m_shopItems, this.m_itemControllers);
    TalkDoer component5 = this.speakerAnimator.GetComponent<TalkDoer>();
    if (!((UnityEngine.Object) component5 != (UnityEngine.Object) null))
      return;
    component5.usesCustomBetrayalLogic = true;
    component5.OnBetrayalWarning += new System.Action(this.OnBetrayalWarning);
    component5.OnBetrayal += new System.Action(this.PullGun);
  }

  protected override void OnDestroy() => base.OnDestroy();
}
