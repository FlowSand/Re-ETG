using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class RobotArmDumbwaiterController : BraveBehaviour, IPlayerInteractable
  {
    public GameObject RobotArmObject;

    public static void HandlePuzzleSetup(GameObject RobotArmPrefab)
    {
      if (GameStatsManager.Instance.GetFlag(GungeonFlags.META_SHOP_DELIVERED_ROBOT_ARM))
        return;
      if (GameStatsManager.Instance.CurrentRobotArmFloor <= 0 || GameStatsManager.Instance.CurrentRobotArmFloor > 5)
        GameStatsManager.Instance.CurrentRobotArmFloor = 5;
      if (GameManager.Instance.Dungeon.tileIndices.tilesetId != GameStatsManager.Instance.GetCurrentRobotArmTileset() || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)
        return;
      List<PickupObject> requiredObjects = new List<PickupObject>();
      requiredObjects.Add(RobotArmPrefab.GetComponent<PickupObject>());
      if (requiredObjects.Count <= 0)
        return;
      GameManager.Instance.Dungeon.data.DistributeComplexSecretPuzzleItems(requiredObjects, (RoomHandler) null, true);
    }

    private void Start() => RobotArmDumbwaiterController.HandlePuzzleSetup(this.RobotArmObject);

    public void OnEnteredRange(PlayerController interactor)
    {
      if (!(bool) (Object) this)
        return;
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 0.1f);
      this.sprite.UpdateZDepth();
    }

    public void OnExitRange(PlayerController interactor)
    {
      if (!(bool) (Object) this)
        return;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
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
      bool flag = false;
      for (int index = 0; index < player.additionalItems.Count; ++index)
      {
        if (player.additionalItems[index] is RobotArmItem)
          flag = true;
      }
      if (!flag)
        return;
      this.OnExitRange(player);
      for (int index = 0; index < player.additionalItems.Count; ++index)
      {
        if (player.additionalItems[index] is RobotArmItem)
        {
          player.UsePuzzleItem(player.additionalItems[index]);
          break;
        }
      }
      --GameStatsManager.Instance.CurrentRobotArmFloor;
      if (GameStatsManager.Instance.CurrentRobotArmFloor != 0)
        return;
      GameStatsManager.Instance.SetFlag(GungeonFlags.META_SHOP_DELIVERED_ROBOT_ARM, true);
    }

    public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
    {
      shouldBeFlipped = false;
      return string.Empty;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

