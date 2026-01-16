// Decompiled with JetBrains decompiler
// Type: TeleporterPrototypeItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

#nullable disable
public class TeleporterPrototypeItem : PlayerItem
{
  public float ChanceToGoToSpecialRoom = 0.1f;
  public float ChanceToGoToEyeballRoom = 0.01f;
  public float ChanceToGoToNextFloor = 0.01f;
  public float ChanceToGoToSecretFloor = 0.01f;
  public float ChanceToGoToBossFoyer = 0.01f;
  [Header("Synergies")]
  public GameObject TelefragVFXPrefab;
  private float LastCooldownModifier = 1f;

  public override bool CanBeUsed(PlayerController user)
  {
    return (bool) (Object) user && !user.IsInMinecart && GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.RATGEON && (user.CurrentRoom == null || !user.CurrentRoom.CompletelyPreventLeaving && (!((Object) GameManager.Instance.Dungeon != (Object) null) || GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.HELLGEON || user.CurrentRoom == null || user.CurrentRoom.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS) && (!user.CurrentRoom.IsSealed || (double) Mathf.Abs(user.RealtimeEnteredCurrentRoom - UnityEngine.Time.realtimeSinceStartup) >= 0.5) && user.CurrentRoom.CanBeEscaped()) && base.CanBeUsed(user);
  }

  protected void TelefragRandomEnemy(RoomHandler room)
  {
    AIActor randomActiveEnemy = room.GetRandomActiveEnemy();
    if (!randomActiveEnemy.IsNormalEnemy || !(bool) (Object) randomActiveEnemy.healthHaver || randomActiveEnemy.healthHaver.IsBoss)
      return;
    Vector2 vector2_1 = !(bool) (Object) randomActiveEnemy.specRigidbody ? randomActiveEnemy.sprite.WorldBottomLeft : randomActiveEnemy.specRigidbody.UnitBottomLeft;
    Vector2 vector2_2 = !(bool) (Object) randomActiveEnemy.specRigidbody ? randomActiveEnemy.sprite.WorldTopRight : randomActiveEnemy.specRigidbody.UnitTopRight;
    Object.Instantiate<GameObject>(this.TelefragVFXPrefab, randomActiveEnemy.CenterPosition.ToVector3ZisY(), Quaternion.identity);
    randomActiveEnemy.healthHaver.ApplyDamage(100000f, Vector2.zero, "Telefrag", ignoreInvulnerabilityFrames: true);
  }

  protected void TelefragRoom(RoomHandler room)
  {
    Pixelator.Instance.FadeToColor(0.25f, Color.white, true);
    List<AIActor> activeEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
    for (int index = 0; index < activeEnemies.Count; ++index)
    {
      if (activeEnemies[index].IsNormalEnemy && (bool) (Object) activeEnemies[index].healthHaver && !activeEnemies[index].healthHaver.IsBoss)
      {
        Vector2 vector2_1 = !(bool) (Object) activeEnemies[index].specRigidbody ? activeEnemies[index].sprite.WorldBottomLeft : activeEnemies[index].specRigidbody.UnitBottomLeft;
        Vector2 vector2_2 = !(bool) (Object) activeEnemies[index].specRigidbody ? activeEnemies[index].sprite.WorldTopRight : activeEnemies[index].specRigidbody.UnitTopRight;
        Object.Instantiate<GameObject>(this.TelefragVFXPrefab, activeEnemies[index].CenterPosition.ToVector3ZisY(), Quaternion.identity);
        activeEnemies[index].healthHaver.ApplyDamage(100000f, Vector2.zero, "Telefrag", ignoreInvulnerabilityFrames: true);
      }
    }
  }

  protected override void DoEffect(PlayerController user)
  {
    if (user.CurrentRoom != null && user.CurrentRoom.CompletelyPreventLeaving)
      return;
    int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_teleport_depart_01", this.gameObject);
    RoomHandler targetRoom = (RoomHandler) null;
    GlobalDungeonData.ValidTilesets tilesetId = GameManager.Instance.Dungeon.tileIndices.tilesetId;
    float num2 = Random.value;
    bool flag = user.HasActiveBonusSynergy(CustomSynergyType.DOUBLE_TELEPORTERS);
    this.LastCooldownModifier = 1f;
    if ((double) num2 < (double) this.ChanceToGoToNextFloor)
    {
      if (tilesetId != GlobalDungeonData.ValidTilesets.FORGEGEON && tilesetId != GlobalDungeonData.ValidTilesets.HELLGEON)
      {
        this.PlayTeleporterEffect(user);
        Pixelator.Instance.FadeToBlack(0.5f);
        GameManager.Instance.DelayedLoadNextLevel(0.5f);
      }
    }
    else if ((double) num2 < (double) this.ChanceToGoToNextFloor + (double) this.ChanceToGoToEyeballRoom && !user.IsInCombat)
    {
      this.PlayTeleporterEffect(user);
      this.StartCoroutine(this.HandleCreepyEyeWarp(user));
      int num3 = (int) AkSoundEngine.PostEvent("Play_OBJ_teleport_depart_01", this.gameObject);
      if (flag)
        this.LastCooldownModifier = 0.5f;
    }
    else if ((double) num2 < (double) this.ChanceToGoToNextFloor + (double) this.ChanceToGoToEyeballRoom + (double) this.ChanceToGoToSpecialRoom)
    {
      List<int> intList = Enumerable.Range(0, GameManager.Instance.Dungeon.data.rooms.Count).ToList<int>().Shuffle<int>();
      for (int index = 0; index < GameManager.Instance.Dungeon.data.rooms.Count; ++index)
      {
        RoomHandler room = GameManager.Instance.Dungeon.data.rooms[intList[index]];
        if (room.IsSecretRoom)
          targetRoom = room;
      }
      if (targetRoom == null)
      {
        for (int index = 0; index < GameManager.Instance.Dungeon.data.rooms.Count; ++index)
        {
          RoomHandler room = GameManager.Instance.Dungeon.data.rooms[intList[index]];
          if (room.IsShop || room.IsSecretRoom || room.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.REWARD)
          {
            if (room.IsSecretRoom)
              room.secretRoomManager.HandleDoorBrokenOpen(room.secretRoomManager.doorObjects[0]);
            targetRoom = room;
            break;
          }
        }
      }
      if (flag)
        this.LastCooldownModifier = 0.5f;
    }
    else if ((double) num2 < (double) this.ChanceToGoToNextFloor + (double) this.ChanceToGoToEyeballRoom + (double) this.ChanceToGoToSpecialRoom + (double) this.ChanceToGoToBossFoyer)
    {
      RoomHandler bossFoyer = ChestTeleporterItem.FindBossFoyer();
      if (bossFoyer != null)
        targetRoom = bossFoyer;
      if (flag)
        this.LastCooldownModifier = 0.5f;
    }
    else if ((double) num2 < (double) this.ChanceToGoToNextFloor + (double) this.ChanceToGoToEyeballRoom + (double) this.ChanceToGoToSpecialRoom + (double) this.ChanceToGoToSecretFloor && (tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON || tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON))
    {
      switch (tilesetId)
      {
        case GlobalDungeonData.ValidTilesets.GUNGEON:
          this.PlayTeleporterEffect(user);
          Pixelator.Instance.FadeToBlack(0.5f);
          GameManager.DoMidgameSave(GlobalDungeonData.ValidTilesets.CATHEDRALGEON);
          GameManager.Instance.DelayedLoadCustomLevel(0.5f, "tt_cathedral");
          break;
        case GlobalDungeonData.ValidTilesets.CASTLEGEON:
          this.PlayTeleporterEffect(user);
          Pixelator.Instance.FadeToBlack(0.5f);
          GameManager.DoMidgameSave(GlobalDungeonData.ValidTilesets.CATHEDRALGEON);
          GameManager.Instance.DelayedLoadCustomLevel(0.5f, "tt_sewer");
          break;
      }
    }
    else
    {
      List<int> intList = Enumerable.Range(0, GameManager.Instance.Dungeon.data.rooms.Count).ToList<int>().Shuffle<int>();
      for (int index = 0; index < GameManager.Instance.Dungeon.data.rooms.Count; ++index)
      {
        RoomHandler room = GameManager.Instance.Dungeon.data.rooms[intList[index]];
        if (room.area.PrototypeRoomNormalSubcategory != PrototypeDungeonRoom.RoomNormalSubCategory.TRAP && room.IsStandardRoom && room.EverHadEnemies || room.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.REWARD || room.IsShop)
        {
          targetRoom = room;
          break;
        }
      }
    }
    if (targetRoom == null)
      return;
    user.EscapeRoom(PlayerController.EscapeSealedRoomStyle.TELEPORTER, true, targetRoom);
    if (targetRoom.IsSecretRoom && (Object) targetRoom.secretRoomManager != (Object) null && targetRoom.secretRoomManager.doorObjects.Count > 0)
      targetRoom.secretRoomManager.doorObjects[0].BreakOpen();
    bool allEnemies = flag;
    if ((targetRoom.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.NORMAL || targetRoom.area.PrototypeRoomNormalSubcategory != PrototypeDungeonRoom.RoomNormalSubCategory.COMBAT) && !targetRoom.area.IsProceduralRoom)
      return;
    user.StartCoroutine(this.HandleTelefragDelay(targetRoom, allEnemies));
  }

  protected override void AfterCooldownApplied(PlayerController user)
  {
    if ((double) this.LastCooldownModifier >= 1.0)
      return;
    base.AfterCooldownApplied(user);
    this.DidDamage(user, this.CurrentDamageCooldown * (1f - this.LastCooldownModifier));
  }

  [DebuggerHidden]
  private IEnumerator HandleTelefragDelay(RoomHandler targetRoom, bool allEnemies)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new TeleporterPrototypeItem.\u003CHandleTelefragDelay\u003Ec__Iterator0()
    {
      targetRoom = targetRoom,
      allEnemies = allEnemies,
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator HandleCreepyEyeWarp(PlayerController interactor)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new TeleporterPrototypeItem.\u003CHandleCreepyEyeWarp\u003Ec__Iterator1()
    {
      interactor = interactor
    };
  }

  private void PlayTeleporterEffect(PlayerController p)
  {
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      if (!GameManager.Instance.AllPlayers[index].IsGhost)
      {
        GameManager.Instance.AllPlayers[index].healthHaver.TriggerInvulnerabilityPeriod(1f);
        GameManager.Instance.AllPlayers[index].knockbackDoer.TriggerTemporaryKnockbackInvulnerability(1f);
      }
    }
    GameObject original = (GameObject) ResourceCache.Acquire("Global VFX/VFX_Teleport_Beam");
    if (!((Object) original != (Object) null))
      return;
    GameObject gameObject = Object.Instantiate<GameObject>(original);
    gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor((Vector3) (p.specRigidbody.UnitBottomCenter + new Vector2(0.0f, -0.5f)), tk2dBaseSprite.Anchor.LowerCenter);
    gameObject.transform.position = gameObject.transform.position.Quantize(1f / 16f);
    gameObject.GetComponent<tk2dBaseSprite>().UpdateZDepth();
  }
}
