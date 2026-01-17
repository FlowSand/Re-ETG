// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.CheckGunslingChallengeComplete
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

public class CheckGunslingChallengeComplete : BraveFsmStateAction
{
  public GunslingChallengeType ChallengeType;
  public Gun GunToUsePrefab;
  public Gun GunToUse;
  public FsmEvent SuccessEvent;
  public FsmEvent FailEvent;
  private RoomHandler m_challengeRoom;
  private TalkDoerLite m_talkDoer;
  private bool m_success = true;
  private GameObject m_extantIcon;
  private bool m_hasAlreadyRegisteredIcon;
  private bool m_hasSucceeded;
  private int gunId = -1;

  public RoomHandler ChallengeRoom
  {
    get => this.m_challengeRoom;
    set => this.m_challengeRoom = value;
  }

  public override void Awake()
  {
    base.Awake();
    this.m_talkDoer = this.Owner.GetComponent<TalkDoerLite>();
  }

  public override void OnEnter()
  {
    base.OnEnter();
    this.ChallengeType = (GunslingChallengeType) this.Fsm.Variables.GetFsmInt("ChallengeType").Value;
    this.m_challengeRoom = this.m_talkDoer.GetAbsoluteParentRoom().injectionTarget;
    this.m_challengeRoom.IsGunslingKingChallengeRoom = true;
    if (!this.m_hasAlreadyRegisteredIcon)
    {
      this.m_hasAlreadyRegisteredIcon = true;
      this.m_extantIcon = Minimap.Instance.RegisterRoomIcon(this.m_challengeRoom, ResourceCache.Acquire("Global Prefabs/Minimap_King_Icon") as GameObject, true);
    }
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      if (this.ChallengeType == GunslingChallengeType.NO_DAMAGE)
        GameManager.Instance.AllPlayers[index].healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.HandlePlayerDamagedFailed);
      if (this.ChallengeType == GunslingChallengeType.SPECIFIC_GUN)
      {
        GameManager.Instance.AllPlayers[index].PostProcessProjectile += new Action<Projectile, float>(this.HandlePlayerFiredProjectile);
        GameManager.Instance.AllPlayers[index].PostProcessBeam += new Action<BeamController>(this.HandlePlayerFiredBeam);
      }
    }
    if (this.ChallengeType != GunslingChallengeType.DAISUKE_CHALLENGES)
      return;
    ChallengeManager.ChallengeModeType = ChallengeModeType.GunslingKingTemporary;
    ChallengeManager.Instance.GunslingTargetRoom = this.m_challengeRoom;
  }

  private void Succeed()
  {
    if (this.m_hasSucceeded)
      return;
    if ((bool) (UnityEngine.Object) this.m_extantIcon)
      Minimap.Instance.DeregisterRoomIcon(this.m_challengeRoom, this.m_extantIcon);
    this.m_hasSucceeded = true;
    switch (this.ChallengeType)
    {
      case GunslingChallengeType.NO_DAMAGE:
        GameStatsManager.Instance.SetFlag(GungeonFlags.GUNSLING_KING_CHALLENGE_TYPE_ONE_COMPLETE, true);
        break;
      case GunslingChallengeType.NO_DODGE_ROLL:
        GameStatsManager.Instance.SetFlag(GungeonFlags.GUNSLING_KING_CHALLENGE_TYPE_TWO_COMPLETE, true);
        break;
      case GunslingChallengeType.SPECIFIC_GUN:
        GameStatsManager.Instance.SetFlag(GungeonFlags.GUNSLING_KING_CHALLENGE_TYPE_THREE_COMPLETE, true);
        break;
      case GunslingChallengeType.DAISUKE_CHALLENGES:
        GameStatsManager.Instance.SetFlag(GungeonFlags.GUNSLING_KING_CHALLENGE_TYPE_FOUR_COMPLETE, true);
        break;
    }
    this.GetRidOfSuppliedGun();
    int num = 0;
    if (GameStatsManager.Instance.GetFlag(GungeonFlags.GUNSLING_KING_CHALLENGE_TYPE_ONE_COMPLETE))
      ++num;
    if (GameStatsManager.Instance.GetFlag(GungeonFlags.GUNSLING_KING_CHALLENGE_TYPE_TWO_COMPLETE))
      ++num;
    if (GameStatsManager.Instance.GetFlag(GungeonFlags.GUNSLING_KING_CHALLENGE_TYPE_THREE_COMPLETE))
      ++num;
    if (GameStatsManager.Instance.GetFlag(GungeonFlags.GUNSLING_KING_CHALLENGE_TYPE_FOUR_COMPLETE))
      ++num;
    if (num >= 3)
      GameStatsManager.Instance.SetFlag(GungeonFlags.GUNSLING_KING_ACTIVE_IN_FOYER, true);
    this.InformManservantSuccess();
    this.Fsm.Event(this.SuccessEvent);
    tk2dSprite component = (ResourceCache.Acquire("Global VFX/GunslingKing_VictoryIcon") as GameObject).GetComponent<tk2dSprite>();
    GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.GetString("#GUNKING_SUCCESS_HEADER"), StringTableManager.GetString("#GUNKING_SUCCESS_BODY"), component.Collection, component.spriteId, UINotificationController.NotificationColor.GOLD);
    this.Finish();
  }

  private void Fail()
  {
    this.m_success = false;
    if ((bool) (UnityEngine.Object) this.m_extantIcon)
      Minimap.Instance.DeregisterRoomIcon(this.m_challengeRoom, this.m_extantIcon);
    this.GetRidOfSuppliedGun();
    GameStatsManager.Instance.RegisterStatChange(TrackedStats.GUNSLING_KING_CHALLENGES_FAILED, 1f);
    this.Fsm.Event(this.FailEvent);
    tk2dSprite component = (ResourceCache.Acquire("Global VFX/GunslingKing_DefeatIcon") as GameObject).GetComponent<tk2dSprite>();
    GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.GetString("#GUNKING_FAIL_HEADER"), StringTableManager.GetString("#GUNKING_FAIL_BODY"), component.Collection, component.spriteId);
    this.Finish();
  }

  private void GetRidOfSuppliedGun()
  {
    if (!((UnityEngine.Object) this.GunToUsePrefab != (UnityEngine.Object) null) || !(bool) (UnityEngine.Object) this.GunToUsePrefab.encounterTrackable)
      return;
    for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
    {
      PlayerController allPlayer = GameManager.Instance.AllPlayers[index1];
      for (int index2 = 0; index2 < allPlayer.inventory.AllGuns.Count; ++index2)
      {
        Gun allGun = allPlayer.inventory.AllGuns[index2];
        if ((bool) (UnityEngine.Object) allGun && (bool) (UnityEngine.Object) allGun.encounterTrackable && allGun.IsMinusOneGun && allGun.encounterTrackable.journalData.GetPrimaryDisplayName() == this.GunToUsePrefab.encounterTrackable.journalData.GetPrimaryDisplayName())
        {
          allPlayer.inventory.DestroyGun(allGun);
          break;
        }
      }
    }
    this.GunToUse = (Gun) null;
    this.GunToUsePrefab = (Gun) null;
  }

  private void HandlePlayerFiredBeam(BeamController obj)
  {
    if (this.gunId == -1)
      this.gunId = this.FindActionOfType<SelectGunslingGun>().SelectedObject.GetComponent<PickupObject>().PickupObjectId;
    if (!(bool) (UnityEngine.Object) obj || !(bool) (UnityEngine.Object) obj.Gun || obj.Gun.CurrentOwner is PlayerController && (obj.Gun.CurrentOwner as PlayerController).CurrentRoom != this.ChallengeRoom || obj.Gun.PickupObjectId == this.gunId)
      return;
    this.Fail();
  }

  private void HandlePlayerFiredProjectile(Projectile obj, float effectChanceScalar)
  {
    if (this.gunId == -1)
      this.gunId = this.FindActionOfType<SelectGunslingGun>().SelectedObject.GetComponent<PickupObject>().PickupObjectId;
    if (obj.Owner is PlayerController && (obj.Owner as PlayerController).CurrentRoom != this.ChallengeRoom || obj.Owner.CurrentGun.PickupObjectId == this.gunId)
      return;
    this.Fail();
  }

  public override void OnExit()
  {
    base.OnExit();
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      if (this.ChallengeType == GunslingChallengeType.NO_DAMAGE)
        GameManager.Instance.AllPlayers[index].healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.HandlePlayerDamagedFailed);
      if (this.ChallengeType == GunslingChallengeType.SPECIFIC_GUN)
      {
        GameManager.Instance.AllPlayers[index].PostProcessProjectile -= new Action<Projectile, float>(this.HandlePlayerFiredProjectile);
        GameManager.Instance.AllPlayers[index].PostProcessBeam -= new Action<BeamController>(this.HandlePlayerFiredBeam);
      }
    }
  }

  private void HandlePlayerDamagedFailed(
    float resultValue,
    float maxValue,
    CoreDamageTypes damageTypes,
    DamageCategory damageCategory,
    Vector2 damageDirection)
  {
    if (!GameManager.HasInstance)
      return;
    PlayerController primaryPlayer = GameManager.Instance.PrimaryPlayer;
    if ((bool) (UnityEngine.Object) primaryPlayer && primaryPlayer.healthHaver.IsAlive && primaryPlayer.CurrentRoom == this.m_challengeRoom)
      this.Fail();
    PlayerController secondaryPlayer = GameManager.Instance.SecondaryPlayer;
    if (!(bool) (UnityEngine.Object) secondaryPlayer || !secondaryPlayer.healthHaver.IsAlive || secondaryPlayer.CurrentRoom != this.m_challengeRoom)
      return;
    this.Fail();
  }

  public override void OnUpdate()
  {
    base.OnUpdate();
    if (this.ChallengeType != GunslingChallengeType.NO_DODGE_ROLL || !this.m_success)
      return;
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      if (GameManager.Instance.AllPlayers[index].CurrentRoom == this.m_challengeRoom && GameManager.Instance.AllPlayers[index].IsDodgeRolling)
        this.Fail();
    }
  }

  private void InformManservantSuccess()
  {
    List<TalkDoerLite> componentsAbsoluteInRoom = this.m_talkDoer.GetAbsoluteParentRoom().GetComponentsAbsoluteInRoom<TalkDoerLite>();
    for (int index1 = 0; index1 < componentsAbsoluteInRoom.Count; ++index1)
    {
      if (!((UnityEngine.Object) componentsAbsoluteInRoom[index1] == (UnityEngine.Object) this.m_talkDoer))
      {
        for (int index2 = 0; index2 < componentsAbsoluteInRoom[index1].playmakerFsms.Length; ++index2)
        {
          if (componentsAbsoluteInRoom[index1].playmakerFsms[index2].FsmName.Contains("Dungeon"))
            componentsAbsoluteInRoom[index1].playmakerFsms[index2].FsmVariables.FindFsmString("currentMode").Value = "modeQuest";
        }
      }
    }
  }

  public override void OnLateUpdate()
  {
    base.OnLateUpdate();
    if (this.m_challengeRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
      return;
    if (this.m_success)
      this.Succeed();
    else
      this.Fail();
    this.Finish();
  }
}
