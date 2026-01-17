// Decompiled with JetBrains decompiler
// Type: GripMasterBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class GripMasterBehavior : BasicAttackBehavior
    {
      public float TrackTime;
      public float TellTime;
      public int RoomsToSendBackward = 1;
      [InspectorCategory("Visuals")]
      public string TellAnim;
      [InspectorCategory("Visuals")]
      public string GrabAnim;
      [InspectorCategory("Visuals")]
      public string MissAnim;
      [InspectorCategory("Visuals")]
      public string ShadowAnim;
      private GripMasterController m_gripMasterController;
      private Vector2 m_posOffset;
      private Vector2 m_targetPosition;
      private float m_timer;
      private Vector2 m_startPos;
      private bool m_hasHit;
      private bool m_sentPlayerBack;
      private GripMasterBehavior.State m_state;

      public override void Start()
      {
        base.Start();
        this.m_posOffset = -(this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.Ground) - this.m_aiActor.transform.position.XY());
        this.m_aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
        this.m_gripMasterController = this.m_aiActor.GetComponent<GripMasterController>();
      }

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_timer);
      }

      public override BehaviorResult Update()
      {
        int num1 = (int) base.Update();
        if (!this.IsReady() || (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody == (UnityEngine.Object) null)
          return BehaviorResult.Continue;
        this.m_state = GripMasterBehavior.State.Tell;
        this.m_timer = this.TellTime;
        this.m_startPos = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.Ground);
        this.m_aiAnimator.PlayUntilCancelled(this.TellAnim);
        this.m_aiActor.ClearPath();
        int num2 = (int) AkSoundEngine.PostEvent("Play_ENM_Grip_Master_Lockon_01", this.m_aiActor.gameObject);
        this.m_targetPosition = this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.Ground);
        this.m_gripMasterController.IsAttacking = true;
        this.m_updateEveryFrame = true;
        return BehaviorResult.RunContinuous;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        int num1 = (int) base.ContinuousUpdate();
        if (this.m_state == GripMasterBehavior.State.Tell)
        {
          if (this.m_state != GripMasterBehavior.State.Idle && (bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
          {
            float num2 = this.TellTime - this.m_timer;
            this.UpdateTargetPosition();
            this.m_aiActor.transform.position = (Vector3) (Vector2Extensions.SmoothStep(this.m_startPos, this.m_targetPosition, Mathf.Clamp01(num2 / this.TrackTime)) + this.m_posOffset);
            this.m_aiActor.specRigidbody.Reinitialize();
          }
          if ((double) this.m_timer <= 0.0)
          {
            this.m_state = GripMasterBehavior.State.Grab;
            this.m_aiAnimator.PlayUntilFinished(this.GrabAnim);
            if (!string.IsNullOrEmpty(this.ShadowAnim))
              this.m_aiActor.ShadowObject.GetComponent<tk2dSpriteAnimator>().Play(this.ShadowAnim);
            return ContinuousBehaviorResult.Continue;
          }
        }
        else if (this.m_state == GripMasterBehavior.State.Grab)
        {
          if (this.m_state != GripMasterBehavior.State.Idle && (bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody && !this.m_hasHit)
          {
            this.UpdateTargetPosition();
            this.m_aiActor.transform.position = (Vector3) (this.m_targetPosition + this.m_posOffset);
            this.m_aiActor.specRigidbody.Reinitialize();
          }
          if (!this.m_aiAnimator.IsPlaying(this.GrabAnim))
            return ContinuousBehaviorResult.Finished;
        }
        else if (this.m_state == GripMasterBehavior.State.Miss && !this.m_aiAnimator.IsPlaying(this.MissAnim))
          return ContinuousBehaviorResult.Finished;
        return ContinuousBehaviorResult.Continue;
      }

      public override void EndContinuousUpdate()
      {
        base.EndContinuousUpdate();
        this.m_state = GripMasterBehavior.State.Idle;
        this.m_hasHit = false;
        this.m_aiActor.sprite.HeightOffGround = 4f;
        if (!this.m_sentPlayerBack)
          this.m_gripMasterController.IsAttacking = false;
        this.m_updateEveryFrame = false;
        this.UpdateCooldowns();
      }

      private void AnimationEventTriggered(
        tk2dSpriteAnimator animator,
        tk2dSpriteAnimationClip clip,
        int frameNo)
      {
        tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNo);
        if (!this.m_hasHit && this.m_state == GripMasterBehavior.State.Grab && frame.eventInfo == "hit")
        {
          this.m_aiActor.sprite.HeightOffGround = 0.0f;
          this.m_hasHit = true;
          if ((bool) (UnityEngine.Object) this.m_gripMasterController)
            this.m_gripMasterController.OnAttack();
          this.ForceBlank();
          bool flag = false;
          if ((bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
          {
            PlayerController gameActor = this.m_aiActor.TargetRigidbody.gameActor as PlayerController;
            if ((bool) (UnityEngine.Object) gameActor)
            {
              Vector2 unitCenter = this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.Ground);
              if (gameActor.CanBeGrabbed && (double) Vector2.Distance(unitCenter, this.m_targetPosition) < 1.0)
              {
                this.BanishPlayer(gameActor);
                flag = true;
              }
            }
          }
          if (!flag)
          {
            this.m_state = GripMasterBehavior.State.Miss;
            this.m_aiAnimator.PlayUntilFinished(this.MissAnim);
          }
          this.m_aiActor.MoveToSafeSpot(0.5f);
        }
        if (!(frame.eventInfo == "lift"))
          return;
        this.m_aiActor.sprite.HeightOffGround = 4f;
      }

      private void UpdateTargetPosition()
      {
        if (!(bool) (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody)
          return;
        this.m_targetPosition = Vector2.MoveTowards(this.m_targetPosition, this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.Ground), 10f * this.m_deltaTime);
      }

      private void BanishPlayer(PlayerController player)
      {
        GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_HIT_WITH_THE_GRIPPY, 1f);
        int num = this.RoomsToSendBackward;
        if ((bool) (UnityEngine.Object) this.m_gripMasterController && this.m_gripMasterController.Grip_OverrideRoomsToSendBackward > 0)
          num = this.m_gripMasterController.Grip_OverrideRoomsToSendBackward;
        if (num < 1)
          num = 1;
        List<RoomHandler> roomHandlerList = new List<RoomHandler>();
        List<RoomHandler> list = new List<RoomHandler>();
        roomHandlerList.Add(player.CurrentRoom);
        while (roomHandlerList.Count - 1 < this.RoomsToSendBackward)
        {
          RoomHandler roomHandler = roomHandlerList[roomHandlerList.Count - 1];
          list.Clear();
          foreach (RoomHandler connectedRoom in roomHandler.connectedRooms)
          {
            if (connectedRoom.hasEverBeenVisited && connectedRoom.distanceFromEntrance < roomHandler.distanceFromEntrance && !roomHandlerList.Contains(connectedRoom) && (!connectedRoom.area.IsProceduralRoom || connectedRoom.area.proceduralCells == null))
              list.Add(connectedRoom);
          }
          if (list.Count != 0)
            roomHandlerList.Add(BraveUtility.RandomElement<RoomHandler>(list));
          else
            break;
        }
        if (roomHandlerList.Count > 1)
        {
          player.RespawnInPreviousRoom(false, PlayerController.EscapeSealedRoomStyle.GRIP_MASTER, true, roomHandlerList[roomHandlerList.Count - 1]);
          for (int index = 1; index < roomHandlerList.Count - 1; ++index)
            roomHandlerList[index].ResetPredefinedRoomLikeDarkSouls();
          UnityEngine.Debug.LogFormat("Sending the player back {0} rooms (attempted {1})", (object) (roomHandlerList.Count - 1), (object) num);
        }
        else
        {
          player.RespawnInPreviousRoom(false, PlayerController.EscapeSealedRoomStyle.GRIP_MASTER, true);
          UnityEngine.Debug.LogFormat("Sending the player back with RespawnInPreviousRoom (no valid \"backwards\" rooms found!)");
        }
        player.specRigidbody.Velocity = Vector2.zero;
        player.knockbackDoer.TriggerTemporaryKnockbackInvulnerability(1f);
        this.m_aiActor.StartCoroutine(this.ForceAnimateCR(player));
        this.m_sentPlayerBack = true;
      }

      [DebuggerHidden]
      private IEnumerator ForceAnimateCR(PlayerController player)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GripMasterBehavior.<ForceAnimateCR>c__Iterator0()
        {
          $this = this
        };
      }

      public void ForceBlank(float overrideRadius = 5f, float overrideTimeAtMaxRadius = 0.65f)
      {
        if (!(bool) (UnityEngine.Object) this.m_aiActor || !(bool) (UnityEngine.Object) this.m_aiActor.specRigidbody)
          return;
        SilencerInstance silencerInstance = new GameObject("silencer").AddComponent<SilencerInstance>();
        silencerInstance.ForceNoDamage = true;
        silencerInstance.TriggerSilencer(this.m_aiActor.specRigidbody.UnitCenter, 50f, overrideRadius, (GameObject) null, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, overrideTimeAtMaxRadius, (PlayerController) null, false, true);
      }

      public enum State
      {
        Idle,
        Tell,
        Grab,
        Miss,
      }
    }

}
