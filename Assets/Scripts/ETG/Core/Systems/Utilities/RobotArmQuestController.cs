// Decompiled with JetBrains decompiler
// Type: RobotArmQuestController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Tween;
using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

  public static class RobotArmQuestController
  {
    public static void HandlePuzzleSetup()
    {
      if ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_CLEARED_FORGE) == 0.0)
        return;
      PickupObject byId1 = PickupObjectDatabase.GetById(GlobalItemIds.RobotArm);
      PickupObject byId2 = PickupObjectDatabase.GetById(GlobalItemIds.RobotBalloons);
      List<PickupObject> requiredObjects = new List<PickupObject>();
      if (!GameStatsManager.Instance.GetFlag(GungeonFlags.META_SHOP_DELIVERED_ROBOT_ARM))
      {
        if (GameStatsManager.Instance.CurrentRobotArmFloor < 0 || GameStatsManager.Instance.CurrentRobotArmFloor > 5)
          GameStatsManager.Instance.CurrentRobotArmFloor = 5;
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER && GameStatsManager.Instance.CurrentRobotArmFloor == 0)
        {
          RoomHandler entrance = GameManager.Instance.Dungeon.data.Entrance;
          if (entrance != null)
          {
            IntVector2 intVector2 = new IntVector2(29, 62);
            DungeonPlaceableUtility.InstantiateDungeonPlaceable(BraveResources.Load("Global Prefabs/Global Items/RobotArmPlaceable") as GameObject, entrance, intVector2 - entrance.area.basePosition, false);
          }
        }
        else if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GameStatsManager.Instance.GetCurrentRobotArmTileset())
        {
          if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)
          {
            BaseShopController[] objectsOfType = Object.FindObjectsOfType<BaseShopController>();
            RoomHandler targetRoom = (RoomHandler) null;
            Transform transform = (Transform) null;
            for (int index = 0; index < objectsOfType.Length; ++index)
            {
              if (objectsOfType[index].name.Contains("Blacksmith"))
              {
                targetRoom = objectsOfType[index].GetAbsoluteParentRoom();
                transform = objectsOfType[index].transform.Find("ArmPoint");
                break;
              }
            }
            if (targetRoom != null)
            {
              bool success = false;
              IntVector2 zero = IntVector2.Zero;
              IntVector2 intVector2;
              if ((Object) transform != (Object) null)
              {
                success = true;
                intVector2 = transform.position.IntXY();
              }
              else
                intVector2 = targetRoom.GetCenteredVisibleClearSpot(2, 2, out success, true);
              if (success)
              {
                DungeonPlaceableUtility.InstantiateDungeonPlaceable(BraveResources.Load("Global Prefabs/Global Items/RobotArmPlaceable") as GameObject, targetRoom, intVector2 - targetRoom.area.basePosition, false);
                if (GameStatsManager.Instance.GetFlag(GungeonFlags.META_SHOP_EVER_SEEN_ROBOT_ARM))
                  requiredObjects.Add(byId2.GetComponent<PickupObject>());
              }
            }
          }
          else
          {
            requiredObjects.Add(byId1);
            requiredObjects.Add(byId2);
          }
        }
      }
      if (requiredObjects.Count <= 0)
        return;
      GameManager.Instance.Dungeon.data.DistributeComplexSecretPuzzleItems(requiredObjects, (RoomHandler) null, true);
    }

    public static void CombineBalloonsWithArm(
      PickupObject balloonsObject,
      PickupObject armObject,
      PlayerController relevantPlayer)
    {
      relevantPlayer.UsePuzzleItem(balloonsObject);
      relevantPlayer.UsePuzzleItem(armObject);
      if ((bool) (Object) balloonsObject)
        Object.Destroy((Object) balloonsObject.gameObject);
      if ((bool) (Object) armObject)
        Object.Destroy((Object) armObject.gameObject);
      BalloonAttachmentDoer objectOfType = Object.FindObjectOfType<BalloonAttachmentDoer>();
      if ((bool) (Object) objectOfType)
        Object.Destroy((Object) objectOfType.gameObject);
      GameObject gameObject = (GameObject) Object.Instantiate(BraveResources.Load("Global VFX/VFX_BalloonArmLift"));
      gameObject.transform.position = relevantPlayer.SpriteBottomCenter;
      DaikonForge.Tween.Tween<Vector3> tween = gameObject.transform.TweenMoveTo(gameObject.transform.position + new Vector3(0.0f, 20f, 0.0f));
      AnimationCurve sourceCurve = gameObject.GetComponent<SimpleAnimationCurveHolder>().curve;
      tween.Easing = (TweenEasingCallback) (a => sourceCurve.Evaluate(a));
      tween.Duration = 4.5f;
      tween.Play();
      --GameStatsManager.Instance.CurrentRobotArmFloor;
      GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.GetString("#METASHOP_ARM_UP_ONE_LEVEL_HEADER"), StringTableManager.GetString("#METASHOP_ARM_UP_ONE_LEVEL_BODY"), gameObject.GetComponent<tk2dBaseSprite>().Collection, gameObject.GetComponent<tk2dBaseSprite>().spriteId, UINotificationController.NotificationColor.GOLD);
    }
  }

