// Decompiled with JetBrains decompiler
// Type: GatlingGullIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class GatlingGullIntroDoer : TimeInvariantMonoBehaviour, IPlaceConfigurable
    {
      public float initialDelay = 1f;
      public float cameraMoveSpeed = 5f;
      public PortraitSlideSettings portraitSlideSettings;
      public ScreenShakeSettings landingShakeSettings;
      public ScreenShakeSettings featherShakeSettings;
      [HideInInspector]
      public tk2dSpriteAnimator gullAnimator;
      public GameObject feathersVFX;
      public GameObject feathersDebris;
      public int numFeathersToSpawn = 15;
      protected bool m_isRunning;
      protected List<tk2dSpriteAnimator> m_animators = new List<tk2dSpriteAnimator>();
      protected float m_elapsedFrameTime;
      protected CameraController m_camera;
      protected Transform m_cameraTransform;
      protected List<CutsceneMotion> activeMotions = new List<CutsceneMotion>();
      protected RoomHandler m_room;
      protected Transform m_shadowTransform;
      protected tk2dSpriteAnimator m_shadowAnimator;
      protected int m_currentPhase;
      protected bool m_phaseComplete = true;
      protected bool m_hasSkipped;
      protected float m_phaseCountdown;
      protected GameObject gunObject;
      protected ParticleSystem feathersSystem;
      protected GameUIBossHealthController bossUI;
      private bool m_hasTriggeredWalkIn;
      private bool m_waitingForBossCard;
      private Vector2[] m_idealStartingPositions;
      private bool m_hasCoopTeleported;

      public void ConfigureOnPlacement(RoomHandler room)
      {
        this.m_room = room;
        this.m_room.Entered += new RoomHandler.OnEnteredEventHandler(this.TriggerSequence);
      }

      private void HandlePlayerWalkIn(PlayerController leadPlayer)
      {
        if (this.m_hasTriggeredWalkIn)
          return;
        this.m_hasTriggeredWalkIn = true;
        this.m_hasCoopTeleported = false;
        RoomHandler otherRoom = (RoomHandler) null;
        for (int index = 0; index < this.m_room.connectedRooms.Count; ++index)
        {
          if (this.m_room.connectedRooms[index].distanceFromEntrance <= this.m_room.distanceFromEntrance)
          {
            otherRoom = this.m_room.connectedRooms[index];
            break;
          }
        }
        if (otherRoom == null)
          return;
        RuntimeExitDefinition forConnectedRoom = this.m_room.GetExitDefinitionForConnectedRoom(otherRoom);
        DungeonData.Direction exitDirection = forConnectedRoom.upstreamExit.referencedExit.exitDirection;
        IntVector2 intVector2_1 = forConnectedRoom.upstreamExit.referencedExit.GetExitAttachPoint() - IntVector2.One + DungeonData.GetIntVector2FromDirection(forConnectedRoom.upstreamExit.referencedExit.exitDirection) + forConnectedRoom.upstreamRoom.area.basePosition;
        float num1 = exitDirection == DungeonData.Direction.NORTH || exitDirection == DungeonData.Direction.SOUTH ? (float) intVector2_1.y : (float) intVector2_1.x;
        float thresholdValue = exitDirection == DungeonData.Direction.EAST || exitDirection == DungeonData.Direction.NORTH ? num1 + (float) (forConnectedRoom.upstreamExit.TotalExitLength + forConnectedRoom.downstreamExit.TotalExitLength) : num1 - (float) (forConnectedRoom.upstreamExit.TotalExitLength + forConnectedRoom.downstreamExit.TotalExitLength);
        leadPlayer.ForceWalkInDirectionWhilePaused(exitDirection, thresholdValue);
        if (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
          return;
        PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(leadPlayer);
        float num2 = exitDirection == DungeonData.Direction.NORTH || exitDirection == DungeonData.Direction.SOUTH ? Mathf.Abs(thresholdValue - leadPlayer.CenterPosition.y) : Mathf.Abs(thresholdValue - leadPlayer.CenterPosition.x);
        IntVector2 pixelsToMove = IntVector2.Zero;
        int num3 = Mathf.RoundToInt(num2 * 16f);
        switch (exitDirection)
        {
          case DungeonData.Direction.NORTH:
            pixelsToMove = new IntVector2(0, num3);
            break;
          case DungeonData.Direction.EAST:
            pixelsToMove = new IntVector2(num3, 0);
            break;
          case DungeonData.Direction.SOUTH:
            pixelsToMove = new IntVector2(0, -num3);
            break;
          case DungeonData.Direction.WEST:
            pixelsToMove = new IntVector2(-num3, 0);
            break;
        }
        CollisionData result;
        if (PhysicsEngine.Instance.RigidbodyCast(otherPlayer.specRigidbody, pixelsToMove, out result))
          num2 = PhysicsEngine.PixelToUnit(result.NewPixelsToMove).magnitude;
        CollisionData.Pool.Free(ref result);
        switch (exitDirection)
        {
          case DungeonData.Direction.NORTH:
            thresholdValue = otherPlayer.CenterPosition.y + num2;
            break;
          case DungeonData.Direction.EAST:
            thresholdValue = otherPlayer.CenterPosition.x + num2;
            break;
          case DungeonData.Direction.SOUTH:
            thresholdValue = otherPlayer.CenterPosition.y - num2;
            break;
          case DungeonData.Direction.WEST:
            thresholdValue = otherPlayer.CenterPosition.x - num2;
            break;
        }
        otherPlayer.ForceWalkInDirectionWhilePaused(exitDirection, thresholdValue);
        this.m_idealStartingPositions = new Vector2[2];
        IntVector2 intVector2_2 = exitDirection == DungeonData.Direction.NORTH || exitDirection == DungeonData.Direction.SOUTH ? intVector2_1 + IntVector2.Right : intVector2_1 + IntVector2.Up;
        float num4 = (float) (forConnectedRoom.upstreamExit.TotalExitLength + forConnectedRoom.downstreamExit.TotalExitLength);
        switch (exitDirection)
        {
          case DungeonData.Direction.NORTH:
            this.m_idealStartingPositions[0] = intVector2_2.ToVector2() + new Vector2(-0.5f, 0.0f) + new Vector2(0.0f, num4 + 0.5f);
            this.m_idealStartingPositions[1] = intVector2_2.ToVector2() + new Vector2(0.25f, -0.25f) + new Vector2(0.0f, num4 - 0.25f);
            break;
          case DungeonData.Direction.EAST:
            this.m_idealStartingPositions[0] = intVector2_2.ToVector2() + new Vector2(num4 + 0.5f, 0.0f);
            this.m_idealStartingPositions[1] = intVector2_2.ToVector2() + new Vector2(-0.25f, -1f) + new Vector2(num4 - 0.25f, 0.0f);
            break;
          case DungeonData.Direction.SOUTH:
            this.m_idealStartingPositions[0] = intVector2_2.ToVector2() + new Vector2(-0.5f, 0.0f) - new Vector2(0.0f, num4 + 0.5f);
            this.m_idealStartingPositions[1] = intVector2_2.ToVector2() + new Vector2(0.25f, 0.25f) - new Vector2(0.0f, num4 - 0.25f);
            break;
          case DungeonData.Direction.WEST:
            this.m_idealStartingPositions[0] = intVector2_2.ToVector2() - new Vector2(num4 + 0.5f, 0.0f);
            this.m_idealStartingPositions[1] = intVector2_2.ToVector2() + new Vector2(0.25f, -1f) - new Vector2(num4 - 0.25f, 0.0f);
            break;
        }
      }

      public void TriggerSequence(PlayerController enterer)
      {
        GameManager.Instance.StartCoroutine(this.FrameDelayedTriggerSequence(enterer));
      }

      [DebuggerHidden]
      public IEnumerator FrameDelayedTriggerSequence(PlayerController enterer)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GatlingGullIntroDoer.\u003CFrameDelayedTriggerSequence\u003Ec__Iterator0()
        {
          enterer = enterer,
          \u0024this = this
        };
      }

      protected void HandleAnimationEvent(
        tk2dSpriteAnimator animator,
        tk2dSpriteAnimationClip clip,
        int frameNo)
      {
        tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNo);
        for (int index = 0; index < this.aiActor.animationAudioEvents.Count; ++index)
        {
          if (this.aiActor.animationAudioEvents[index].eventTag == frame.eventInfo && GameManager.AUDIO_ENABLED)
          {
            int num = (int) AkSoundEngine.PostEvent(this.aiActor.animationAudioEvents[index].eventName, this.gameObject);
          }
        }
      }

      protected void EndSequence()
      {
        this.bossUI.EndBossPortraitEarly();
        this.m_camera.StartTrackingPlayer();
        this.m_camera.SetManualControl(false);
        this.aiAnimator.enabled = true;
        SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, true);
        this.renderer.enabled = true;
        if ((Object) this.m_shadowTransform != (Object) null)
          this.m_shadowTransform.position = (Vector3) this.specRigidbody.UnitCenter;
        if ((Object) this.m_shadowAnimator != (Object) null)
        {
          this.m_shadowAnimator.Play("shadow_static");
          this.m_shadowAnimator.Sprite.independentOrientation = true;
          this.m_shadowAnimator.Sprite.IsPerpendicular = false;
          this.m_shadowAnimator.Sprite.HeightOffGround = -1f;
        }
        this.specRigidbody.CollideWithOthers = true;
        if ((bool) (Object) this.aiActor)
        {
          this.aiActor.IsGone = false;
          this.aiActor.State = AIActor.ActorState.Normal;
        }
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          if (!GameManager.Instance.AllPlayers[index].healthHaver.IsDead)
            GameManager.Instance.AllPlayers[index].ToggleGunRenderers(true, string.Empty);
        }
        if ((Object) this.feathersSystem != (Object) null)
          Object.Destroy((Object) this.feathersSystem.gameObject);
        GameUIRoot.Instance.ShowCoreUI(string.Empty);
        GameUIRoot.Instance.ToggleLowerPanels(true, source: string.Empty);
        GameManager.Instance.PreventPausing = false;
        BraveTime.ClearMultiplier(this.gameObject);
        GameManager.IsBossIntro = false;
        tk2dSpriteAnimator[] componentsInChildren = this.GetComponentsInChildren<tk2dSpriteAnimator>();
        for (int index = 0; index < componentsInChildren.Length; ++index)
        {
          if ((bool) (Object) componentsInChildren[index])
            componentsInChildren[index].alwaysUpdateOffscreen = true;
        }
        Minimap.Instance.TemporarilyPreventMinimap = false;
        this.m_isRunning = false;
      }

      [DebuggerHidden]
      private IEnumerator DelayedTriggerAnimation(
        tk2dSpriteAnimator anim,
        string animName,
        float delay)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GatlingGullIntroDoer.\u003CDelayedTriggerAnimation\u003Ec__Iterator1()
        {
          delay = delay,
          anim = anim,
          animName = animName,
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator WaitForBossCard()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GatlingGullIntroDoer.\u003CWaitForBossCard\u003Ec__Iterator2()
        {
          \u0024this = this
        };
      }

      protected override void InvariantUpdate(float realDeltaTime)
      {
        if (!this.m_isRunning || !this.enabled)
          return;
        if (GenericIntroDoer.SkipFrame)
        {
          GenericIntroDoer.SkipFrame = false;
        }
        else
        {
          SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, false);
          if ((Object) this.m_shadowTransform == (Object) null && (bool) (Object) this.aiActor.ShadowObject)
            this.m_shadowTransform = this.aiActor.ShadowObject.transform;
          for (int index = 0; index < this.m_animators.Count; ++index)
            this.m_animators[index].UpdateAnimation(realDeltaTime);
          for (int index = 0; index < this.activeMotions.Count; ++index)
          {
            CutsceneMotion activeMotion = this.activeMotions[index];
            Vector2 vector2 = !activeMotion.lerpEnd.HasValue ? GameManager.Instance.MainCameraController.GetIdealCameraPosition() : activeMotion.lerpEnd.Value;
            float num1 = Vector2.Distance(vector2, activeMotion.lerpStart);
            float num2 = activeMotion.speed * realDeltaTime / num1;
            activeMotion.lerpProgress = Mathf.Clamp01(activeMotion.lerpProgress + num2);
            float t = activeMotion.lerpProgress;
            if (activeMotion.isSmoothStepped)
              t = Mathf.SmoothStep(0.0f, 1f, t);
            Vector2 vector = Vector2.Lerp(activeMotion.lerpStart, vector2, t);
            if ((Object) activeMotion.camera != (Object) null)
              activeMotion.camera.OverridePosition = vector.ToVector3ZUp(activeMotion.zOffset);
            else
              activeMotion.transform.position = BraveUtility.QuantizeVector(vector.ToVector3ZUp(activeMotion.zOffset), (float) PhysicsEngine.Instance.PixelsPerUnit);
            if ((double) activeMotion.lerpProgress == 1.0)
            {
              this.activeMotions.RemoveAt(index);
              --index;
              ++this.m_currentPhase;
              this.m_phaseComplete = true;
            }
          }
          bool flag = BraveInput.PrimaryPlayerInstance.MenuInteractPressed;
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            flag = flag || BraveInput.SecondaryPlayerInstance.MenuInteractPressed;
          if (flag && !this.m_hasSkipped)
          {
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && !this.m_hasCoopTeleported)
              this.TeleportCoopPlayers();
            this.m_currentPhase = 13;
            this.m_phaseComplete = true;
            this.activeMotions.Clear();
            this.m_hasSkipped = false;
            this.specRigidbody.CollideWithOthers = true;
            if ((bool) (Object) this.aiActor)
            {
              this.aiActor.IsGone = false;
              this.aiActor.State = AIActor.ActorState.Normal;
            }
            tk2dSpriteAnimator[] componentsInChildren = this.GetComponentsInChildren<tk2dSpriteAnimator>();
            for (int index = 0; index < componentsInChildren.Length; ++index)
            {
              if ((bool) (Object) componentsInChildren[index])
                componentsInChildren[index].alwaysUpdateOffscreen = true;
            }
          }
          Vector3 vector3 = new Vector3(0.0f, 1f, 0.0f);
          if (this.m_phaseComplete)
          {
            switch (this.m_currentPhase)
            {
              case 0:
                this.gullAnimator.transform.position = new Vector3(30f, 5f, 0.0f) + this.transform.position + vector3;
                SpriteOutlineManager.ToggleOutlineRenderers((tk2dBaseSprite) this.gullAnimator.GetComponent<tk2dSprite>(), false);
                this.activeMotions.Add(new CutsceneMotion(this.m_cameraTransform, new Vector2?(this.specRigidbody.UnitCenter), this.cameraMoveSpeed)
                {
                  camera = this.m_camera
                });
                this.m_phaseComplete = false;
                break;
              case 1:
                this.m_phaseCountdown = this.initialDelay;
                this.m_phaseComplete = false;
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && !this.m_hasCoopTeleported)
                {
                  this.TeleportCoopPlayers();
                  break;
                }
                break;
              case 2:
                this.m_shadowAnimator = this.aiActor.ShadowObject.GetComponent<tk2dSpriteAnimator>();
                this.m_animators.Add(this.m_shadowAnimator);
                this.m_animators.Add(this.gullAnimator);
                this.activeMotions.Add(new CutsceneMotion(this.gullAnimator.transform, new Vector2?(new Vector2(-60f, 0.0f) + this.gullAnimator.transform.position.XY()), 27f));
                int num3 = (int) AkSoundEngine.PostEvent("Play_ANM_Gull_Shadow_01", this.gameObject);
                this.m_phaseComplete = false;
                break;
              case 3:
                this.m_phaseCountdown = this.initialDelay;
                this.m_phaseComplete = false;
                break;
              case 4:
                this.gullAnimator.GetComponent<Renderer>().enabled = true;
                SpriteOutlineManager.ToggleOutlineRenderers((tk2dBaseSprite) this.gullAnimator.GetComponent<tk2dSprite>(), true);
                this.m_shadowAnimator.Play("shadow_out");
                int num4 = (int) AkSoundEngine.PostEvent("Play_ANM_Gull_Intro_01", this.gameObject);
                this.gullAnimator.enabled = false;
                this.gullAnimator.Play(this.gullAnimator.GetClipByName("fly"));
                this.activeMotions.Add(new CutsceneMotion(this.gullAnimator.transform, new Vector2?((Vector2) (this.transform.position + vector3)), 20f)
                {
                  isSmoothStepped = false
                });
                this.m_phaseComplete = false;
                break;
              case 5:
                Object.Destroy((Object) this.gunObject);
                this.gullAnimator.transform.position -= vector3;
                this.gullAnimator.Play(this.gullAnimator.GetClipByName("pick_up"));
                int num5 = (int) AkSoundEngine.PostEvent("Play_ANM_Gull_Lift_01", this.gameObject);
                this.m_phaseCountdown = (float) this.gullAnimator.CurrentClip.frames.Length / this.gullAnimator.CurrentClip.fps;
                this.m_phaseComplete = false;
                break;
              case 6:
                this.m_shadowAnimator.Play("shadow_in");
                this.gullAnimator.Play(this.gullAnimator.GetClipByName("fly_pick_up"));
                this.gullAnimator.Sprite.HeightOffGround += 3f;
                this.activeMotions.Add(new CutsceneMotion(this.gullAnimator.transform, new Vector2?((Vector2) (this.transform.position + new Vector3(20f, 20f, 0.0f))), 15f)
                {
                  isSmoothStepped = false
                });
                this.m_phaseComplete = false;
                break;
              case 7:
                this.m_phaseCountdown = 1f;
                this.m_phaseComplete = false;
                this.gullAnimator.Sprite.HeightOffGround -= 3f;
                this.gullAnimator.Play("land_feathered");
                this.gullAnimator.Stop();
                break;
              case 8:
                this.aiActor.ToggleShadowVisiblity(true);
                this.m_shadowAnimator.Play("shadow_out");
                int num6 = (int) AkSoundEngine.PostEvent("Play_ANM_Gull_Descend_01", this.gameObject);
                this.StartCoroutine(this.DelayedTriggerAnimation(this.gullAnimator, "land_feathered", 0.8f));
                this.gullAnimator.transform.position = this.transform.position + new Vector3(0.0f, 50f, 0.0f);
                this.activeMotions.Add(new CutsceneMotion(this.gullAnimator.transform, new Vector2?((Vector2) this.transform.position), 50f));
                this.m_phaseComplete = false;
                break;
              case 9:
                this.m_camera.DoScreenShake(this.landingShakeSettings, new Vector2?());
                this.m_phaseCountdown = 1.5f;
                this.m_phaseComplete = false;
                break;
              case 10:
                this.m_animators.Remove(this.m_shadowAnimator);
                this.gullAnimator.Play(this.gullAnimator.GetClipByName("awaken_feathered"));
                int num7 = (int) AkSoundEngine.PostEvent("Play_ANM_Gull_Flex_01", this.gameObject);
                this.m_phaseCountdown = (float) this.gullAnimator.CurrentClip.frames.Length / this.gullAnimator.CurrentClip.fps;
                this.m_phaseComplete = false;
                break;
              case 11:
                Vector3 position = this.transform.position + this.sprite.GetBounds().center + new Vector3(0.0f, 0.0f, -5f);
                this.feathersSystem = SpawnManager.SpawnVFX(this.feathersVFX, position, this.feathersVFX.transform.rotation).GetComponent<ParticleSystem>();
                this.feathersSystem.Play();
                for (int index = 0; index < this.numFeathersToSpawn; ++index)
                {
                  float z1 = (float) index * (360f / (float) this.numFeathersToSpawn);
                  DebrisObject component = SpawnManager.SpawnDebris(this.feathersDebris, position, Quaternion.identity).GetComponent<DebrisObject>();
                  float num8 = Random.Range(4f, 10f);
                  float z2 = Random.Range(2f, 5f);
                  float startingHeight = Random.Range(0.5f, 2f);
                  component.Trigger((Quaternion.Euler(0.0f, 0.0f, z1) * (Vector3) Vector2.right * num8).WithZ(z2), startingHeight);
                }
                this.m_camera.DoScreenShake(this.featherShakeSettings, new Vector2?());
                this.gullAnimator.Play(this.gullAnimator.GetClipByName("awaken_plucked"));
                this.m_phaseCountdown = (float) this.gullAnimator.CurrentClip.frames.Length / this.gullAnimator.CurrentClip.fps;
                this.m_phaseComplete = false;
                break;
              case 12:
                int num9 = (int) AkSoundEngine.PostEvent("Play_UI_boss_intro_01", this.gameObject);
                this.StartCoroutine(this.WaitForBossCard());
                this.m_phaseCountdown = 1E+10f;
                this.m_phaseComplete = false;
                break;
              case 13:
                this.gullAnimator.enabled = true;
                this.m_animators.Remove(this.gullAnimator);
                GameManager.Instance.MainCameraController.ForceUpdateControllerCameraState(CameraController.ControllerCameraState.RoomLock);
                this.activeMotions.Add(new CutsceneMotion(this.m_cameraTransform, new Vector2?(), this.cameraMoveSpeed)
                {
                  camera = this.m_camera
                });
                this.m_phaseComplete = false;
                break;
              case 14:
                if ((bool) (Object) this.gunObject)
                  Object.Destroy((Object) this.gunObject);
                Object.Destroy((Object) this.gullAnimator.gameObject);
                this.EndSequence();
                return;
            }
          }
          if (this.m_currentPhase > 14)
            this.m_currentPhase = 14;
          Bounds untrimmedBounds = this.gullAnimator.Sprite.GetUntrimmedBounds();
          if ((Object) this.m_shadowTransform != (Object) null)
            this.m_shadowTransform.position = this.m_shadowTransform.position.WithX(this.gullAnimator.transform.position.x + untrimmedBounds.extents.x);
          if (this.m_currentPhase == 12 && !this.m_waitingForBossCard)
          {
            this.m_phaseCountdown = 0.0f;
            ++this.m_currentPhase;
            this.m_phaseComplete = true;
          }
          if ((Object) this.feathersSystem != (Object) null)
            this.feathersSystem.Simulate(realDeltaTime, true, false);
          if ((double) this.m_phaseCountdown > 0.0)
          {
            this.m_phaseCountdown -= realDeltaTime;
            if ((double) this.m_phaseCountdown <= 0.0)
            {
              this.m_phaseCountdown = 0.0f;
              ++this.m_currentPhase;
              this.m_phaseComplete = true;
            }
          }
          this.gullAnimator.GetComponent<tk2dSprite>().UpdateZDepth();
        }
      }

      protected override void OnDestroy()
      {
        if (this.m_room != null)
          this.m_room.Entered -= new RoomHandler.OnEnteredEventHandler(this.TriggerSequence);
        base.OnDestroy();
      }

      private void TeleportCoopPlayers()
      {
        if (this.m_hasCoopTeleported)
          return;
        Vector2 centerPosition1 = GameManager.Instance.PrimaryPlayer.CenterPosition;
        Vector2 centerPosition2 = GameManager.Instance.SecondaryPlayer.CenterPosition;
        if ((double) Vector2.Distance(centerPosition2, this.m_idealStartingPositions[0]) < (double) Vector2.Distance(centerPosition1, this.m_idealStartingPositions[0]))
        {
          Vector2 startingPosition = this.m_idealStartingPositions[0];
          this.m_idealStartingPositions[0] = this.m_idealStartingPositions[1];
          this.m_idealStartingPositions[1] = startingPosition;
        }
        if ((double) Vector3.Distance((Vector3) centerPosition1, (Vector3) this.m_idealStartingPositions[0]) > 2.0)
          GameManager.Instance.PrimaryPlayer.WarpToPoint(this.m_idealStartingPositions[0], true);
        if ((double) Vector3.Distance((Vector3) centerPosition2, (Vector3) this.m_idealStartingPositions[1]) <= 2.0)
          return;
        GameManager.Instance.SecondaryPlayer.WarpToPoint(this.m_idealStartingPositions[1], true);
      }
    }

}
