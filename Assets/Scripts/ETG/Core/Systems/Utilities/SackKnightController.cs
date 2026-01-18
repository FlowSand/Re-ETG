using Dungeonator;
using UnityEngine;

#nullable disable

public class SackKnightController : CompanionController
  {
    public const bool c_usesJunkNotArmor = true;
    public SackKnightController.SackKnightPhase CurrentForm;

    public static bool HasJunkan()
    {
      if (GameManager.HasInstance && (bool) (Object) GameManager.Instance.PrimaryPlayer)
      {
        for (int index = 0; index < GameManager.Instance.PrimaryPlayer.passiveItems.Count; ++index)
        {
          PassiveItem passiveItem = GameManager.Instance.PrimaryPlayer.passiveItems[index];
          if (passiveItem is CompanionItem && (bool) (Object) (passiveItem as CompanionItem).ExtantCompanion && (bool) (Object) (passiveItem as CompanionItem).ExtantCompanion.GetComponent<SackKnightController>())
            return true;
        }
      }
      if (GameManager.HasInstance && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && (bool) (Object) GameManager.Instance.SecondaryPlayer)
      {
        for (int index = 0; index < GameManager.Instance.SecondaryPlayer.passiveItems.Count; ++index)
        {
          PassiveItem passiveItem = GameManager.Instance.SecondaryPlayer.passiveItems[index];
          if (passiveItem is CompanionItem && (bool) (Object) (passiveItem as CompanionItem).ExtantCompanion && (bool) (Object) (passiveItem as CompanionItem).ExtantCompanion.GetComponent<SackKnightController>())
            return true;
        }
      }
      return false;
    }

    protected override void HandleRoomCleared(PlayerController callingPlayer)
    {
      if (this.CurrentForm < SackKnightController.SackKnightPhase.KNIGHT || callingPlayer.CurrentRoom.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS || callingPlayer.CurrentRoom.area.PrototypeRoomBossSubcategory == PrototypeDungeonRoom.RoomBossSubCategory.MINI_BOSS)
        return;
      GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_SER_JUNKAN_UNLOCKED, true);
    }

    public override void Update()
    {
      if (!(bool) (Object) GameManager.Instance || GameManager.Instance.IsLoadingLevel)
        return;
      if ((bool) (Object) this.m_owner && !Chest.HasDroppedSerJunkanThisSession)
        Chest.HasDroppedSerJunkanThisSession = true;
      this.UpdateAnimationNamesBasedOnSacks();
      if (this.HasStealthMode && (bool) (Object) this.m_owner)
      {
        if (this.m_owner.IsStealthed && !this.m_isStealthed)
        {
          this.m_isStealthed = true;
          this.aiAnimator.IdleAnimation.AnimNames[0] = "sst_dis_idle_right";
          this.aiAnimator.IdleAnimation.AnimNames[1] = "sst_dis_idle_left";
          this.aiAnimator.MoveAnimation.AnimNames[0] = "sst_dis_move_right";
          this.aiAnimator.MoveAnimation.AnimNames[1] = "sst_dis_move_left";
        }
        else if (!this.m_owner.IsStealthed && this.m_isStealthed)
        {
          this.m_isStealthed = false;
          this.aiAnimator.IdleAnimation.AnimNames[0] = "sst_idle_right";
          this.aiAnimator.IdleAnimation.AnimNames[1] = "sst_idle_left";
          this.aiAnimator.MoveAnimation.AnimNames[0] = "sst_move_right";
          this.aiAnimator.MoveAnimation.AnimNames[1] = "sst_move_left";
        }
      }
      IntVector2 intVector2 = this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
      if (!GameManager.Instance.Dungeon.data.CheckInBounds(intVector2))
        return;
      CellData cellData = GameManager.Instance.Dungeon.data[intVector2];
      if (cellData == null)
        return;
      this.m_lastCellType = cellData.type;
    }

    private void UpdateAnimationNamesBasedOnSacks()
    {
      if (!(bool) (Object) this.m_owner)
        return;
      int num = 0;
      bool flag = false;
      for (int index = 0; index < this.m_owner.passiveItems.Count; ++index)
      {
        if (this.m_owner.passiveItems[index] is BasicStatPickup)
        {
          BasicStatPickup passiveItem = this.m_owner.passiveItems[index] as BasicStatPickup;
          if (passiveItem.IsJunk)
            ++num;
          if (passiveItem.IsJunk && passiveItem.GivesCurrency)
            flag = true;
        }
      }
      AIAnimator aiAnimator = this.aiAnimator;
      if (flag)
      {
        if (this.CurrentForm != SackKnightController.SackKnightPhase.MECHAJUNKAN)
        {
          this.specRigidbody.PixelColliders[0].ManualOffsetX = 30;
          this.specRigidbody.PixelColliders[0].ManualOffsetY = 3;
          this.specRigidbody.PixelColliders[0].ManualWidth = 17;
          this.specRigidbody.PixelColliders[0].ManualHeight = 16 /*0x10*/;
          this.specRigidbody.PixelColliders[1].ManualOffsetX = 30;
          this.specRigidbody.PixelColliders[1].ManualOffsetY = 3;
          this.specRigidbody.PixelColliders[1].ManualWidth = 17;
          this.specRigidbody.PixelColliders[1].ManualHeight = 28;
          this.specRigidbody.PixelColliders[0].Regenerate(this.transform);
          this.specRigidbody.PixelColliders[1].Regenerate(this.transform);
          this.specRigidbody.Reinitialize();
          this.aiActor.ShadowObject.transform.position = (Vector3) this.specRigidbody.UnitBottomCenter;
        }
        this.CurrentForm = SackKnightController.SackKnightPhase.MECHAJUNKAN;
        aiAnimator.IdleAnimation.AnimNames[0] = "junk_g_idle_right";
        aiAnimator.IdleAnimation.AnimNames[1] = "junk_g_idle_left";
        aiAnimator.MoveAnimation.AnimNames[0] = "junk_g_move_right";
        aiAnimator.MoveAnimation.AnimNames[1] = "junk_g_move_left";
        aiAnimator.TalkAnimation.AnimNames[0] = "junk_g_talk_right";
        aiAnimator.TalkAnimation.AnimNames[1] = "junk_g_talk_left";
        aiAnimator.OtherAnimations[0].anim.AnimNames[0] = "junk_g_sword_right";
        aiAnimator.OtherAnimations[0].anim.AnimNames[1] = "junk_g_sword_left";
      }
      else if (num < 1)
      {
        this.CurrentForm = SackKnightController.SackKnightPhase.PEASANT;
        aiAnimator.IdleAnimation.AnimNames[0] = "junk_idle_right";
        aiAnimator.IdleAnimation.AnimNames[1] = "junk_idle_left";
        aiAnimator.MoveAnimation.AnimNames[0] = "junk_move_right";
        aiAnimator.MoveAnimation.AnimNames[1] = "junk_move_left";
        aiAnimator.TalkAnimation.AnimNames[0] = "junk_talk_right";
        aiAnimator.TalkAnimation.AnimNames[1] = "junk_talk_left";
        aiAnimator.OtherAnimations[0].anim.AnimNames[0] = "junk_attack_right";
        aiAnimator.OtherAnimations[0].anim.AnimNames[1] = "junk_attack_left";
      }
      else
      {
        switch (num)
        {
          case 1:
            this.CurrentForm = SackKnightController.SackKnightPhase.SQUIRE;
            aiAnimator.IdleAnimation.AnimNames[0] = "junk_h_idle_right";
            aiAnimator.IdleAnimation.AnimNames[1] = "junk_h_idle_left";
            aiAnimator.MoveAnimation.AnimNames[0] = "junk_h_move_right";
            aiAnimator.MoveAnimation.AnimNames[1] = "junk_h_move_left";
            aiAnimator.TalkAnimation.AnimNames[0] = "junk_h_talk_right";
            aiAnimator.TalkAnimation.AnimNames[1] = "junk_h_talk_left";
            aiAnimator.OtherAnimations[0].anim.AnimNames[0] = "junk_h_attack_right";
            aiAnimator.OtherAnimations[0].anim.AnimNames[1] = "junk_h_attack_left";
            break;
          case 2:
            GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_GOLD_JUNK, true);
            this.CurrentForm = SackKnightController.SackKnightPhase.HEDGE_KNIGHT;
            aiAnimator.IdleAnimation.AnimNames[0] = "junk_sh_idle_right";
            aiAnimator.IdleAnimation.AnimNames[1] = "junk_sh_idle_left";
            aiAnimator.MoveAnimation.AnimNames[0] = "junk_sh_move_right";
            aiAnimator.MoveAnimation.AnimNames[1] = "junk_sh_move_left";
            aiAnimator.TalkAnimation.AnimNames[0] = "junk_sh_talk_right";
            aiAnimator.TalkAnimation.AnimNames[1] = "junk_sh_talk_left";
            aiAnimator.OtherAnimations[0].anim.AnimNames[0] = "junk_sh_attack_right";
            aiAnimator.OtherAnimations[0].anim.AnimNames[1] = "junk_sh_attack_left";
            break;
          case 3:
            this.CurrentForm = SackKnightController.SackKnightPhase.KNIGHT;
            aiAnimator.IdleAnimation.AnimNames[0] = "junk_shs_idle_right";
            aiAnimator.IdleAnimation.AnimNames[1] = "junk_shs_idle_left";
            aiAnimator.MoveAnimation.AnimNames[0] = "junk_shs_move_right";
            aiAnimator.MoveAnimation.AnimNames[1] = "junk_shs_move_left";
            aiAnimator.TalkAnimation.AnimNames[0] = "junk_shs_talk_right";
            aiAnimator.TalkAnimation.AnimNames[1] = "junk_shs_talk_left";
            aiAnimator.OtherAnimations[0].anim.AnimNames[0] = "junk_shs_attack_right";
            aiAnimator.OtherAnimations[0].anim.AnimNames[1] = "junk_shs_attack_left";
            break;
          case 4:
            this.CurrentForm = SackKnightController.SackKnightPhase.KNIGHT_LIEUTENANT;
            aiAnimator.IdleAnimation.AnimNames[0] = "junk_shsp_idle_right";
            aiAnimator.IdleAnimation.AnimNames[1] = "junk_shsp_idle_left";
            aiAnimator.MoveAnimation.AnimNames[0] = "junk_shsp_move_right";
            aiAnimator.MoveAnimation.AnimNames[1] = "junk_shsp_move_left";
            aiAnimator.TalkAnimation.AnimNames[0] = "junk_shsp_talk_right";
            aiAnimator.TalkAnimation.AnimNames[1] = "junk_shsp_talk_left";
            aiAnimator.OtherAnimations[0].anim.AnimNames[0] = "junk_shsp_attack_right";
            aiAnimator.OtherAnimations[0].anim.AnimNames[1] = "junk_shsp_attack_left";
            break;
          case 5:
            this.CurrentForm = SackKnightController.SackKnightPhase.KNIGHT_COMMANDER;
            aiAnimator.IdleAnimation.AnimNames[0] = "junk_shspc_idle_right";
            aiAnimator.IdleAnimation.AnimNames[1] = "junk_shspc_idle_left";
            aiAnimator.MoveAnimation.AnimNames[0] = "junk_shspc_move_right";
            aiAnimator.MoveAnimation.AnimNames[1] = "junk_shspc_move_left";
            aiAnimator.TalkAnimation.AnimNames[0] = "junk_shspc_talk_right";
            aiAnimator.TalkAnimation.AnimNames[1] = "junk_shspc_talk_left";
            aiAnimator.OtherAnimations[0].anim.AnimNames[0] = "junk_shspc_attack_right";
            aiAnimator.OtherAnimations[0].anim.AnimNames[1] = "junk_shspc_attack_left";
            break;
          case 6:
            this.CurrentForm = SackKnightController.SackKnightPhase.HOLY_KNIGHT;
            aiAnimator.IdleAnimation.AnimNames[0] = "junk_shspcg_idle_right";
            aiAnimator.IdleAnimation.AnimNames[1] = "junk_shspcg_idle_left";
            aiAnimator.MoveAnimation.AnimNames[0] = "junk_shspcg_move_right";
            aiAnimator.MoveAnimation.AnimNames[1] = "junk_shspcg_move_left";
            aiAnimator.TalkAnimation.AnimNames[0] = "junk_shspcg_talk_right";
            aiAnimator.TalkAnimation.AnimNames[1] = "junk_shspcg_talk_left";
            aiAnimator.OtherAnimations[0].anim.AnimNames[0] = "junk_shspcg_attack_right";
            aiAnimator.OtherAnimations[0].anim.AnimNames[1] = "junk_shspcg_attack_left";
            break;
          default:
            if (num > 6)
            {
              this.CurrentForm = SackKnightController.SackKnightPhase.ANGELIC_KNIGHT;
              GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_SER_JUNKAN_MAXLVL, true);
              aiAnimator.IdleAnimation.AnimNames[0] = "junk_a_idle_right";
              aiAnimator.IdleAnimation.AnimNames[1] = "junk_a_idle_left";
              aiAnimator.MoveAnimation.AnimNames[0] = "junk_a_idle_right";
              aiAnimator.MoveAnimation.AnimNames[1] = "junk_a_idle_left";
              aiAnimator.TalkAnimation.AnimNames[0] = "junk_a_talk_right";
              aiAnimator.TalkAnimation.AnimNames[1] = "junk_a_talk_left";
              aiAnimator.OtherAnimations[0].anim.AnimNames[0] = "junk_a_attack_right";
              aiAnimator.OtherAnimations[0].anim.AnimNames[1] = "junk_a_attack_left";
              if (!this.aiActor.IsFlying)
              {
                this.aiActor.SetIsFlying(true, "angel", modifyPathing: true);
                break;
              }
              break;
            }
            break;
        }
      }
      if (this.CurrentForm == SackKnightController.SackKnightPhase.ANGELIC_KNIGHT || !this.aiActor.IsFlying)
        return;
      this.aiActor.SetIsFlying(false, "angel", modifyPathing: true);
    }

    public enum SackKnightPhase
    {
      PEASANT,
      SQUIRE,
      HEDGE_KNIGHT,
      KNIGHT,
      KNIGHT_LIEUTENANT,
      KNIGHT_COMMANDER,
      HOLY_KNIGHT,
      ANGELIC_KNIGHT,
      MECHAJUNKAN,
    }
  }

