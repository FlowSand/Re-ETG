// Decompiled with JetBrains decompiler
// Type: BulletPastRoomController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using HutongGames.PlayMaker;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Rooms
{
    public class BulletPastRoomController : MonoBehaviour
    {
      public BulletPastRoomController.BulletRoomCategory RoomIdentifier;
      public WarpPointHandler EntranceWarp;
      public WarpPointHandler ExitWarp;
      private TalkDoerLite OldBulletTalkDoer;
      public SpeculativeRigidbody OldBulletTalkTrigger;
      public TalkDoerLite AgunimPreDeathTalker;
      public TalkDoerLite AgunimPostDeathTalker;
      public tk2dSpriteAnimator AgunimFloorChunk;
      public ScreenShakeSettings AgunimPostDeathShake;
      public SpeculativeRigidbody AgunimFlightCollider;
      public SpeculativeRigidbody ThroneRoomDoor;
      public SpeculativeRigidbody ThroneRoomDoorTrigger;
      public VFXPool ThroneRoomDoorVfx;
      public ScreenShakeSettings ThroneRoomDoorShake;
      private IntVector2 m_agunimFloorBasePosition;
      private BulletPastRoomController RoomB;
      private BulletPastRoomController RoomC;
      private BulletPastRoomController RoomD;
      public GameObject BulletmanEndingQuad;
      public Texture2D[] BulletmanEndingFrames;
      private bool m_readyForTests;
      private float m_timeHovering;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletPastRoomController.<Start>c__Iterator0()
        {
          $this = this
        };
      }

      private void HandleFlightCollider(
        SpeculativeRigidbody specRigidbody,
        SpeculativeRigidbody sourceSpecRigidbody,
        CollisionData collisionData)
      {
        if (GameManager.Instance.IsLoadingLevel)
          return;
        PlayerController component = specRigidbody.GetComponent<PlayerController>();
        if (!(bool) (Object) component || !component.IsFlying || GameManager.Instance.IsLoadingLevel)
          return;
        this.m_timeHovering += BraveTime.DeltaTime;
        if ((double) this.m_timeHovering <= 0.5)
          return;
        component.ForceFall();
      }

      private void PitfallIntoGunonRoom() => this.StartCoroutine(this.DoGunonPitfall());

      [DebuggerHidden]
      private IEnumerator DoGunonPitfall()
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        BulletPastRoomController.<DoGunonPitfall>c__Iterator1 pitfallCIterator1 = new BulletPastRoomController.<DoGunonPitfall>c__Iterator1();
        return (IEnumerator) pitfallCIterator1;
      }

      private void PlayerEntered(PlayerController p)
      {
        this.transform.position.GetAbsoluteRoom()?.SealRoom();
      }

      private void WalkedByOldBullet(
        SpeculativeRigidbody specrigidbody,
        SpeculativeRigidbody sourcespecrigidbody,
        CollisionData collisiondata)
      {
        if (!(bool) (Object) this.OldBulletTalkDoer)
          return;
        FsmBool fsmBool = this.OldBulletTalkDoer.playmakerFsm.FsmVariables.FindFsmBool("giftGiven");
        if (fsmBool == null || fsmBool.Value)
          return;
        this.OldBulletTalkDoer.playmakerFsm.SendEvent("playerInteract");
      }

      private void EnteredThroneDoorTrigger(
        SpeculativeRigidbody specrigidbody,
        SpeculativeRigidbody sourcespecrigidbody,
        CollisionData collisiondata)
      {
        if ((bool) (Object) this.ThroneRoomDoor)
          this.StartCoroutine(this.MoveThroneRoomDoor());
        this.ThroneRoomDoorTrigger.OnEnterTrigger -= new SpeculativeRigidbody.OnTriggerDelegate(this.EnteredThroneDoorTrigger);
      }

      private void HandleRoomBCleared()
      {
        if ((bool) (Object) this.ThroneRoomDoorTrigger)
          this.ThroneRoomDoorTrigger.enabled = true;
        this.ExitWarp.DISABLED_TEMPORARILY = false;
      }

      [DebuggerHidden]
      private IEnumerator MoveThroneRoomDoor()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletPastRoomController.<MoveThroneRoomDoor>c__Iterator2()
        {
          $this = this
        };
      }

      private void Update()
      {
        if (Dungeon.IsGenerating || this.RoomIdentifier != BulletPastRoomController.BulletRoomCategory.ROOM_A || !this.m_readyForTests || !this.ExitWarp.DISABLED_TEMPORARILY || !((Object) GameManager.Instance.PrimaryPlayer.CurrentGun != (Object) null))
          return;
        UnityEngine.Debug.LogError((object) GameManager.Instance.PrimaryPlayer.CurrentGun);
        this.ExitWarp.DISABLED_TEMPORARILY = false;
      }

      [DebuggerHidden]
      public IEnumerator HandleAgunimIntro(Transform bossTransform)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletPastRoomController.<HandleAgunimIntro>c__Iterator3()
        {
          bossTransform = bossTransform,
          $this = this
        };
      }

      public void OnGanonDeath(Transform bossTransform)
      {
        this.StartCoroutine(this.HandleGanonDeath(bossTransform));
      }

      [DebuggerHidden]
      public IEnumerator HandleGanonDeath(Transform bossTransform)
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        BulletPastRoomController.<HandleGanonDeath>c__Iterator4 ganonDeathCIterator4 = new BulletPastRoomController.<HandleGanonDeath>c__Iterator4();
        return (IEnumerator) ganonDeathCIterator4;
      }

      [DebuggerHidden]
      public IEnumerator HandleAgunimDeath(Transform bossTransform)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletPastRoomController.<HandleAgunimDeath>c__Iterator5()
        {
          bossTransform = bossTransform,
          $this = this
        };
      }

      public void TriggerAgumimFloorBreak()
      {
        if (!((Object) this.AgunimFloorChunk != (Object) null))
          return;
        this.AgunimFloorChunk.transform.localPosition = new Vector3(9.75f, 10.625f, 12.375f);
        this.AgunimFloorChunk.PlayAndDisableRenderer("agunim_lair_burst");
        for (int x = 0; x < 4; ++x)
        {
          for (int y = 0; y < 4; ++y)
            GameManager.Instance.Dungeon.data[this.m_agunimFloorBasePosition + new IntVector2(x, y)].fallingPrevented = false;
        }
      }

      public void TriggerBulletmanEnding()
      {
        if (this.RoomIdentifier != BulletPastRoomController.BulletRoomCategory.ROOM_D)
          return;
        GameStatsManager.Instance.SetCharacterSpecificFlag(CharacterSpecificGungeonFlags.KILLED_PAST, true);
        this.StartCoroutine(this.TriggerBulletmanEnding_CR());
      }

      [DebuggerHidden]
      public IEnumerator TriggerBulletmanEnding_CR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletPastRoomController.<TriggerBulletmanEnding_CR>c__Iterator6()
        {
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator AnimateBulletmanEnding(tk2dSpriteFromTexture sft)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletPastRoomController.<AnimateBulletmanEnding>c__Iterator7()
        {
          sft = sft,
          $this = this
        };
      }

      public enum BulletRoomCategory
      {
        ROOM_A,
        ROOM_B,
        ROOM_C,
        ROOM_D,
      }
    }

}
