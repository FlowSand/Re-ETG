// Decompiled with JetBrains decompiler
// Type: ShopSubsidiaryZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class ShopSubsidiaryZone : MonoBehaviour
    {
      public GenericLootTable shopItems;
      public Transform[] spawnPositions;
      public GameObject shopItemShadowPrefab;
      public bool IsShopRoundTable;
      public bool PrecludeAllDiscounts;

      public void HandleSetup(
        ShopController controller,
        RoomHandler room,
        List<GameObject> shopItemObjects,
        List<ShopItemController> shopItemControllers)
      {
        int count = shopItemObjects.Count;
        for (int index = 0; index < this.spawnPositions.Length; ++index)
        {
          if (this.IsShopRoundTable && index == 0 && (GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON || GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON))
          {
            shopItemObjects.Add(this.shopItems.defaultItemDrops.elements[0].gameObject);
          }
          else
          {
            GameObject gameObject = this.shopItems.SelectByWeightWithoutDuplicatesFullPrereqs(shopItemObjects);
            shopItemObjects.Add(gameObject);
          }
        }
        bool flag = false;
        for (int index = 0; index < this.spawnPositions.Length; ++index)
        {
          if (!((Object) shopItemObjects[count + index] == (Object) null))
          {
            flag = true;
            Transform spawnPosition = this.spawnPositions[index];
            PickupObject component1 = shopItemObjects[count + index].GetComponent<PickupObject>();
            if (!((Object) component1 == (Object) null))
            {
              GameObject gameObject = new GameObject("Shop item " + index.ToString());
              Transform transform = gameObject.transform;
              transform.parent = spawnPosition;
              transform.localPosition = Vector3.zero;
              EncounterTrackable component2 = component1.GetComponent<EncounterTrackable>();
              if ((Object) component2 != (Object) null)
                GameManager.Instance.ExtantShopTrackableGuids.Add(component2.EncounterGuid);
              ShopItemController ixable = gameObject.AddComponent<ShopItemController>();
              ixable.PrecludeAllDiscounts = this.PrecludeAllDiscounts;
              if (spawnPosition.name.Contains("SIDE") || spawnPosition.name.Contains("EAST"))
                ixable.itemFacing = DungeonData.Direction.EAST;
              else if (spawnPosition.name.Contains("WEST"))
                ixable.itemFacing = DungeonData.Direction.WEST;
              else if (spawnPosition.name.Contains("NORTH"))
                ixable.itemFacing = DungeonData.Direction.NORTH;
              if (!room.IsRegistered((IPlayerInteractable) ixable))
                room.RegisterInteractable((IPlayerInteractable) ixable);
              ixable.Initialize(component1, controller);
              shopItemControllers.Add(ixable);
            }
          }
        }
        if (flag)
          return;
        foreach (Behaviour componentsInChild in this.GetComponentsInChildren<SpeculativeRigidbody>())
          componentsInChild.enabled = false;
        this.gameObject.SetActive(false);
      }

      public void HandleSetup(
        BaseShopController controller,
        RoomHandler room,
        List<GameObject> shopItemObjects,
        List<ShopItemController> shopItemControllers)
      {
        int count = shopItemObjects.Count;
        for (int index = 0; index < this.spawnPositions.Length; ++index)
        {
          GameObject gameObject = this.shopItems.SelectByWeightWithoutDuplicatesFullPrereqs(shopItemObjects);
          shopItemObjects.Add(gameObject);
        }
        bool flag = false;
        for (int index = 0; index < this.spawnPositions.Length; ++index)
        {
          if (!((Object) shopItemObjects[count + index] == (Object) null))
          {
            flag = true;
            Transform spawnPosition = this.spawnPositions[index];
            PickupObject component1 = shopItemObjects[count + index].GetComponent<PickupObject>();
            if (!((Object) component1 == (Object) null))
            {
              GameObject gameObject = new GameObject("Shop item " + index.ToString());
              Transform transform = gameObject.transform;
              transform.parent = spawnPosition;
              transform.localPosition = Vector3.zero;
              EncounterTrackable component2 = component1.GetComponent<EncounterTrackable>();
              if ((Object) component2 != (Object) null)
                GameManager.Instance.ExtantShopTrackableGuids.Add(component2.EncounterGuid);
              ShopItemController ixable = gameObject.AddComponent<ShopItemController>();
              ixable.PrecludeAllDiscounts = this.PrecludeAllDiscounts;
              if (spawnPosition.name.Contains("SIDE") || spawnPosition.name.Contains("EAST"))
                ixable.itemFacing = DungeonData.Direction.EAST;
              else if (spawnPosition.name.Contains("WEST"))
                ixable.itemFacing = DungeonData.Direction.WEST;
              else if (spawnPosition.name.Contains("NORTH"))
                ixable.itemFacing = DungeonData.Direction.NORTH;
              if (!room.IsRegistered((IPlayerInteractable) ixable))
                room.RegisterInteractable((IPlayerInteractable) ixable);
              ixable.Initialize(component1, controller);
              shopItemControllers.Add(ixable);
            }
          }
        }
        if (flag)
          return;
        foreach (Behaviour componentsInChild in this.GetComponentsInChildren<SpeculativeRigidbody>())
          componentsInChild.enabled = false;
        this.gameObject.SetActive(false);
      }
    }

}
