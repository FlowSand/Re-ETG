// Decompiled with JetBrains decompiler
// Type: GenericIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
public class GenericIntroDoer : TimeInvariantMonoBehaviour, IPlaceConfigurable
{
  public GenericIntroDoer.TriggerType triggerType = GenericIntroDoer.TriggerType.PlayerEnteredRoom;
  public float initialDelay = 1f;
  public float cameraMoveSpeed = 5f;
  public AIAnimator specifyIntroAiAnimator;
  public string BossMusicEvent = "Play_MUS_Boss_Theme_Beholster";
  public bool PreventBossMusic;
  public bool InvisibleBeforeIntroAnim;
  [CheckAnimation(null)]
  public string preIntroAnim = string.Empty;
  [CheckDirectionalAnimation(null)]
  public string preIntroDirectionalAnim = string.Empty;
  [FormerlySerializedAs("preIntroAnimationName")]
  [CheckAnimation(null)]
  public string introAnim = string.Empty;
  [FormerlySerializedAs("preIntroDirectionalAnimation")]
  [CheckDirectionalAnimation(null)]
  public string introDirectionalAnim = string.Empty;
  public bool continueAnimDuringOutro;
  public GameObject cameraFocus;
  public Vector2 roomPositionCameraFocus;
  public bool restrictPlayerMotionToRoom;
  public bool fusebombLock;
  public float AdditionalHeightOffset;
  public bool SkipBossCard;
  [HideInInspectorIf("SkipBossCard", false)]
  public PortraitSlideSettings portraitSlideSettings;
  public bool HideGunAndHand;
  public System.Action OnIntroFinished;
  private Tribool m_singleFrameSkipDelay = Tribool.Unready;
  private bool m_isRunning;
  private bool m_waitingForBossCard;
  private bool m_hasTriggeredWalkIn;
  private GenericIntroDoer.Phase m_currentPhase;
  private bool m_phaseComplete = true;
  private float m_phaseCountdown;
  private CameraController m_camera;
  private Transform m_cameraTransform;
  private RoomHandler m_room;
  private GameUIBossHealthController bossUI;
  private List<tk2dSpriteAnimator> m_animators = new List<tk2dSpriteAnimator>();
  private List<CutsceneMotion> activeMotions = new List<CutsceneMotion>();
  private SpecificIntroDoer m_specificIntroDoer;
  private bool m_waitingForSpecificIntroCompletion;
  private GameObject m_roomCameraFocus;
  private bool m_isCameraModified;
  private bool m_isMotionRestricted;
  private int m_minPlayerX;
  private int m_minPlayerY;
  private int m_maxPlayerX;
  private int m_maxPlayerY;
  private Vector2[] m_idealStartingPositions;
  private bool m_hasCoopTeleported;

  public Vector2 BossCenter
  {
    get
    {
      if ((UnityEngine.Object) this.m_specificIntroDoer != (UnityEngine.Object) null)
      {
        Vector2? overrideIntroPosition = this.m_specificIntroDoer.OverrideIntroPosition;
        if (overrideIntroPosition.HasValue)
          return overrideIntroPosition.Value;
      }
      if ((bool) (UnityEngine.Object) this.specRigidbody)
        return this.specRigidbody.UnitCenter;
      return (bool) (UnityEngine.Object) this.dungeonPlaceable ? this.transform.position.XY() + new Vector2((float) this.dungeonPlaceable.placeableWidth / 2f, (float) this.dungeonPlaceable.placeableHeight / 2f) : (Vector2) this.transform.position;
    }
  }

  public bool SkipFinalizeAnimation { get; set; }

  public bool SuppressSkipping { get; set; }

  private void Awake() => this.m_specificIntroDoer = this.GetComponent<SpecificIntroDoer>();

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
        if ((UnityEngine.Object) activeMotion.camera != (UnityEngine.Object) null)
          activeMotion.camera.OverridePosition = vector.ToVector3ZUp(activeMotion.zOffset);
        else
          activeMotion.transform.position = BraveUtility.QuantizeVector(vector.ToVector3ZUp(activeMotion.zOffset), (float) PhysicsEngine.Instance.PixelsPerUnit);
        if ((double) activeMotion.lerpProgress >= 1.0)
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
      if (this.SuppressSkipping)
        flag = false;
      if (this.m_singleFrameSkipDelay != Tribool.Unready)
        flag = false;
      if (flag)
      {
        BraveMemory.HandleBossCardSkip();
        this.m_singleFrameSkipDelay = Tribool.Ready;
      }
      else if (this.m_singleFrameSkipDelay == Tribool.Ready)
      {
        this.m_singleFrameSkipDelay = Tribool.Complete;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && !this.m_hasCoopTeleported)
          this.TeleportCoopPlayers();
        int num = (int) AkSoundEngine.PostEvent("STOP_SND_Diagetic", this.gameObject);
        this.m_currentPhase = GenericIntroDoer.Phase.CameraOutro;
        this.bossUI.EndBossPortraitEarly();
        this.m_phaseComplete = true;
        this.activeMotions.Clear();
        foreach (SpeculativeRigidbody componentsInChild in this.GetComponentsInChildren<SpeculativeRigidbody>())
          componentsInChild.CollideWithOthers = true;
        foreach (GameActor componentsInChild in this.GetComponentsInChildren<AIActor>())
          componentsInChild.IsGone = false;
        if ((bool) (UnityEngine.Object) this.aiActor)
          this.aiActor.State = AIActor.ActorState.Normal;
        if (this.InvisibleBeforeIntroAnim)
          this.aiActor.ToggleRenderers(true);
        if (!string.IsNullOrEmpty(this.preIntroDirectionalAnim))
          (!(bool) (UnityEngine.Object) this.specifyIntroAiAnimator ? this.aiAnimator : this.specifyIntroAiAnimator).EndAnimationIf(this.preIntroDirectionalAnim);
        if ((UnityEngine.Object) this.m_specificIntroDoer != (UnityEngine.Object) null)
          this.m_specificIntroDoer.EndIntro();
        tk2dSpriteAnimator[] componentsInChildren = this.GetComponentsInChildren<tk2dSpriteAnimator>();
        for (int index = 0; index < componentsInChildren.Length; ++index)
        {
          if ((bool) (UnityEngine.Object) componentsInChildren[index])
            componentsInChildren[index].alwaysUpdateOffscreen = true;
        }
      }
      if (this.m_phaseComplete)
      {
        DirectionalAnimation directionalAnimation = (DirectionalAnimation) null;
        if ((bool) (UnityEngine.Object) this.aiAnimator)
        {
          if (this.aiAnimator.IdleAnimation.HasAnimation)
            directionalAnimation = this.aiAnimator.IdleAnimation;
          else if (this.aiAnimator.MoveAnimation.HasAnimation)
            directionalAnimation = this.aiAnimator.MoveAnimation;
        }
        switch (this.m_currentPhase)
        {
          case GenericIntroDoer.Phase.CameraIntro:
            this.activeMotions.Add(new CutsceneMotion(this.m_cameraTransform, new Vector2?(this.BossCenter), this.cameraMoveSpeed)
            {
              camera = this.m_camera
            });
            this.m_phaseComplete = false;
            if ((bool) (UnityEngine.Object) this.spriteAnimator)
            {
              this.m_animators.Add(this.spriteAnimator);
              this.spriteAnimator.enabled = false;
            }
            if ((bool) (UnityEngine.Object) this.aiAnimator && (bool) (UnityEngine.Object) this.aiAnimator.ChildAnimator)
            {
              this.m_animators.Add(this.aiAnimator.ChildAnimator.spriteAnimator);
              this.aiAnimator.ChildAnimator.spriteAnimator.enabled = false;
            }
            if ((UnityEngine.Object) this.m_specificIntroDoer != (UnityEngine.Object) null)
            {
              this.m_specificIntroDoer.OnCameraIntro();
              break;
            }
            break;
          case GenericIntroDoer.Phase.InitialDelay:
            this.m_phaseCountdown = this.initialDelay;
            this.m_phaseComplete = false;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && !this.m_hasCoopTeleported)
            {
              this.TeleportCoopPlayers();
              break;
            }
            break;
          case GenericIntroDoer.Phase.PreIntroAnim:
            if (this.InvisibleBeforeIntroAnim)
              this.aiActor.ToggleRenderers(true);
            if ((UnityEngine.Object) this.m_specificIntroDoer != (UnityEngine.Object) null)
            {
              this.m_specificIntroDoer.StartIntro(this.m_animators);
              this.m_phaseCountdown = float.MaxValue;
              this.m_phaseComplete = false;
              this.m_waitingForSpecificIntroCompletion = !this.m_specificIntroDoer.IsIntroFinished;
              if (this.m_waitingForSpecificIntroCompletion)
                break;
            }
            if (!string.IsNullOrEmpty(this.introAnim))
            {
              this.spriteAnimator.Play(this.spriteAnimator.GetClipByName(this.introAnim));
              this.m_phaseCountdown = (float) this.spriteAnimator.CurrentClip.frames.Length / this.spriteAnimator.CurrentClip.fps;
              this.m_phaseCountdown += 0.25f;
              this.m_phaseComplete = false;
              break;
            }
            if (!string.IsNullOrEmpty(this.introDirectionalAnim))
            {
              AIAnimator aiAnimator = !(bool) (UnityEngine.Object) this.specifyIntroAiAnimator ? this.aiAnimator : this.specifyIntroAiAnimator;
              aiAnimator.PlayUntilFinished(this.introDirectionalAnim);
              tk2dSpriteAnimator spriteAnimator = aiAnimator.spriteAnimator;
              if ((bool) (UnityEngine.Object) aiAnimator.ChildAnimator && aiAnimator.ChildAnimator.HasDirectionalAnimation(this.introDirectionalAnim))
                spriteAnimator = aiAnimator.ChildAnimator.spriteAnimator;
              this.m_phaseCountdown = (float) spriteAnimator.CurrentClip.frames.Length / spriteAnimator.CurrentClip.fps;
              this.m_phaseCountdown += 0.25f;
              this.m_phaseComplete = false;
              break;
            }
            this.m_phaseCountdown = 0.0f;
            this.m_phaseComplete = false;
            break;
          case GenericIntroDoer.Phase.BossCard:
            if (!this.SkipBossCard)
            {
              int num = (int) AkSoundEngine.PostEvent("Play_UI_boss_intro_01", this.gameObject);
              this.StartCoroutine(this.WaitForBossCard());
              this.m_phaseCountdown = float.MaxValue;
              this.m_phaseComplete = false;
              if ((UnityEngine.Object) this.m_specificIntroDoer != (UnityEngine.Object) null)
              {
                this.m_specificIntroDoer.OnBossCard();
                break;
              }
              break;
            }
            break;
          case GenericIntroDoer.Phase.CameraOutro:
            if ((bool) (UnityEngine.Object) this.cameraFocus || this.roomPositionCameraFocus != Vector2.zero || this.fusebombLock)
              this.ModifyCamera(true);
            if (this.restrictPlayerMotionToRoom)
              this.RestrictMotion(true);
            Vector2? targetPosition = new Vector2?();
            if ((bool) (UnityEngine.Object) this.m_specificIntroDoer)
            {
              Vector2? overrideOutroPosition = this.m_specificIntroDoer.OverrideOutroPosition;
              if (overrideOutroPosition.HasValue)
                targetPosition = new Vector2?(overrideOutroPosition.Value);
            }
            GameManager.Instance.MainCameraController.ForceUpdateControllerCameraState(CameraController.ControllerCameraState.RoomLock);
            this.activeMotions.Add(new CutsceneMotion(this.m_cameraTransform, targetPosition, this.cameraMoveSpeed)
            {
              camera = this.m_camera
            });
            this.m_phaseComplete = false;
            if (!this.continueAnimDuringOutro)
            {
              if ((double) this.AdditionalHeightOffset != 0.0)
              {
                foreach (tk2dBaseSprite componentsInChild in this.GetComponentsInChildren<tk2dBaseSprite>())
                  componentsInChild.HeightOffGround -= this.AdditionalHeightOffset;
                this.sprite.UpdateZDepth();
              }
              if (!string.IsNullOrEmpty(this.introDirectionalAnim))
                (!(bool) (UnityEngine.Object) this.specifyIntroAiAnimator ? this.aiAnimator : this.specifyIntroAiAnimator).EndAnimationIf(this.introDirectionalAnim);
              if (directionalAnimation != null && !this.SkipFinalizeAnimation)
                this.spriteAnimator.Play(directionalAnimation.GetInfo(-90f).name);
            }
            if ((UnityEngine.Object) this.m_specificIntroDoer != (UnityEngine.Object) null)
            {
              this.m_specificIntroDoer.OnCameraOutro();
              break;
            }
            break;
          case GenericIntroDoer.Phase.Cleanup:
            if (this.continueAnimDuringOutro)
            {
              if ((double) this.AdditionalHeightOffset != 0.0)
              {
                foreach (tk2dBaseSprite componentsInChild in this.GetComponentsInChildren<tk2dBaseSprite>())
                  componentsInChild.HeightOffGround -= this.AdditionalHeightOffset;
                this.sprite.UpdateZDepth();
              }
              if (!string.IsNullOrEmpty(this.introDirectionalAnim))
                (!(bool) (UnityEngine.Object) this.specifyIntroAiAnimator ? this.aiAnimator : this.specifyIntroAiAnimator).EndAnimationIf(this.introDirectionalAnim);
              if (directionalAnimation != null && !this.SkipFinalizeAnimation)
                this.spriteAnimator.Play(directionalAnimation.GetInfo(-90f).name);
            }
            if ((bool) (UnityEngine.Object) this.spriteAnimator)
            {
              this.m_animators.Remove(this.spriteAnimator);
              this.spriteAnimator.enabled = true;
            }
            if ((bool) (UnityEngine.Object) this.aiAnimator && (bool) (UnityEngine.Object) this.aiAnimator.ChildAnimator)
              this.aiAnimator.ChildAnimator.spriteAnimator.enabled = true;
            if ((UnityEngine.Object) this.m_specificIntroDoer != (UnityEngine.Object) null)
              this.m_specificIntroDoer.OnCleanup();
            this.EndSequence();
            return;
        }
      }
      if (this.m_currentPhase > GenericIntroDoer.Phase.Cleanup)
        this.m_currentPhase = GenericIntroDoer.Phase.Cleanup;
      if (this.m_currentPhase == GenericIntroDoer.Phase.PreIntroAnim)
      {
        if (this.m_waitingForSpecificIntroCompletion && this.m_specificIntroDoer.IsIntroFinished)
        {
          this.m_phaseCountdown = 0.0f;
          ++this.m_currentPhase;
          this.m_phaseComplete = true;
        }
        if (!string.IsNullOrEmpty(this.preIntroDirectionalAnim))
          (!(bool) (UnityEngine.Object) this.specifyIntroAiAnimator ? this.aiAnimator : this.specifyIntroAiAnimator).EndAnimationIf(this.preIntroDirectionalAnim);
      }
      else if (this.m_currentPhase == GenericIntroDoer.Phase.BossCard && !this.m_waitingForBossCard)
      {
        this.m_phaseCountdown = 0.0f;
        ++this.m_currentPhase;
        this.m_phaseComplete = true;
      }
      if ((double) this.m_phaseCountdown <= 0.0)
        return;
      this.m_phaseCountdown -= realDeltaTime;
      if ((double) this.m_phaseCountdown > 0.0)
        return;
      this.m_phaseCountdown = 0.0f;
      ++this.m_currentPhase;
      this.m_phaseComplete = true;
    }
  }

  protected override void OnDestroy()
  {
    if (this.m_room != null)
      this.m_room.Entered -= new RoomHandler.OnEnteredEventHandler(this.PlayerEntered);
    if (this.m_isCameraModified)
      this.ModifyCamera(false);
    if (this.m_isMotionRestricted)
      this.RestrictMotion(false);
    base.OnDestroy();
  }

  public void ConfigureOnPlacement(RoomHandler room)
  {
    this.m_room = room;
    this.m_room.Entered += new RoomHandler.OnEnteredEventHandler(this.PlayerEntered);
  }

  public void PlayerEntered(PlayerController player)
  {
    if (GameManager.HasInstance && GameManager.Instance.RunData.SpawnAngryToadie && (bool) (UnityEngine.Object) this.healthHaver && !this.healthHaver.IsSubboss && GameManager.Instance.CurrentFloor < 5 && !this.name.StartsWith("BossStatues", StringComparison.Ordinal) && !this.name.StartsWith("DemonWall", StringComparison.Ordinal))
    {
      Vector2 position = this.specRigidbody.UnitBottomRight + new Vector2(2.5f, 0.25f);
      AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(GlobalEnemyGuids.BulletKingToadieAngry), position, this.aiActor.ParentRoom, true);
      GameManager.Instance.RunData.SpawnAngryToadie = false;
    }
    if (this.triggerType != GenericIntroDoer.TriggerType.PlayerEnteredRoom)
      return;
    this.TriggerSequence(player);
  }

  public void TriggerSequence(PlayerController enterer)
  {
    this.StartCoroutine(this.FrameDelayedTriggerSequence(enterer));
  }

  [DebuggerHidden]
  public IEnumerator FrameDelayedTriggerSequence(PlayerController enterer)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new GenericIntroDoer.\u003CFrameDelayedTriggerSequence\u003Ec__Iterator0()
    {
      enterer = enterer,
      \u0024this = this
    };
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
    float num1 = float.MaxValue;
    RuntimeExitDefinition runtimeExitDefinition1 = (RuntimeExitDefinition) null;
    for (int index = 0; index < this.m_room.area.instanceUsedExits.Count; ++index)
    {
      PrototypeRoomExit instanceUsedExit = this.m_room.area.instanceUsedExits[index];
      if (this.m_room.area.exitToLocalDataMap.ContainsKey(instanceUsedExit))
      {
        RuntimeRoomExitData exitToLocalData = this.m_room.area.exitToLocalDataMap[instanceUsedExit];
        if (this.m_room.exitDefinitionsByExit.ContainsKey(exitToLocalData))
        {
          RuntimeExitDefinition runtimeExitDefinition2 = this.m_room.exitDefinitionsByExit[exitToLocalData];
          IntVector2 intVector2 = runtimeExitDefinition2.upstreamRoom != this.m_room ? runtimeExitDefinition2.GetDownstreamNearDoorPosition() : runtimeExitDefinition2.GetUpstreamNearDoorPosition();
          float num2 = Vector2.Distance(leadPlayer.CenterPosition, intVector2.ToCenterVector2());
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            runtimeExitDefinition1 = runtimeExitDefinition2;
          }
        }
      }
    }
    if (runtimeExitDefinition1 == null || (double) num1 > 10.0)
      runtimeExitDefinition1 = this.m_room.GetExitDefinitionForConnectedRoom(otherRoom);
    DungeonData.Direction direction = DungeonData.InvertDirection(runtimeExitDefinition1.GetDirectionFromRoom(this.m_room));
    IntVector2 exitBaseCenter = runtimeExitDefinition1.upstreamRoom != this.m_room ? runtimeExitDefinition1.GetDownstreamNearDoorPosition() : runtimeExitDefinition1.GetUpstreamNearDoorPosition();
    if ((bool) (UnityEngine.Object) this.m_specificIntroDoer)
      exitBaseCenter = this.m_specificIntroDoer.OverrideExitBasePosition(direction, exitBaseCenter);
    float num3 = direction == DungeonData.Direction.NORTH || direction == DungeonData.Direction.SOUTH ? (float) exitBaseCenter.y : (float) exitBaseCenter.x;
    float thresholdValue = direction == DungeonData.Direction.EAST || direction == DungeonData.Direction.NORTH ? num3 + 3f : num3 - 3f;
    UnityEngine.Debug.LogError((object) $"{(object) direction}|{(object) thresholdValue}");
    leadPlayer.ForceWalkInDirectionWhilePaused(direction, thresholdValue);
    if (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
      return;
    PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(leadPlayer);
    float num4 = direction == DungeonData.Direction.NORTH || direction == DungeonData.Direction.SOUTH ? Mathf.Abs(thresholdValue - leadPlayer.CenterPosition.y) : Mathf.Abs(thresholdValue - leadPlayer.CenterPosition.x);
    IntVector2 pixelsToMove = IntVector2.Zero;
    int num5 = Mathf.RoundToInt(num4 * 16f);
    switch (direction)
    {
      case DungeonData.Direction.NORTH:
        pixelsToMove = new IntVector2(0, num5);
        break;
      case DungeonData.Direction.EAST:
        pixelsToMove = new IntVector2(num5, 0);
        break;
      case DungeonData.Direction.SOUTH:
        pixelsToMove = new IntVector2(0, -num5);
        break;
      case DungeonData.Direction.WEST:
        pixelsToMove = new IntVector2(-num5, 0);
        break;
    }
    CollisionData result;
    if (PhysicsEngine.Instance.RigidbodyCast(otherPlayer.specRigidbody, pixelsToMove, out result))
      num4 = PhysicsEngine.PixelToUnit(result.NewPixelsToMove).magnitude;
    CollisionData.Pool.Free(ref result);
    switch (direction)
    {
      case DungeonData.Direction.NORTH:
        thresholdValue = otherPlayer.CenterPosition.y + num4;
        break;
      case DungeonData.Direction.EAST:
        thresholdValue = otherPlayer.CenterPosition.x + num4;
        break;
      case DungeonData.Direction.SOUTH:
        thresholdValue = otherPlayer.CenterPosition.y - num4;
        break;
      case DungeonData.Direction.WEST:
        thresholdValue = otherPlayer.CenterPosition.x - num4;
        break;
    }
    otherPlayer.ForceWalkInDirectionWhilePaused(direction, thresholdValue);
    this.m_idealStartingPositions = new Vector2[2];
    IntVector2 intVector2_1 = direction == DungeonData.Direction.NORTH || direction == DungeonData.Direction.SOUTH ? exitBaseCenter + IntVector2.Right : exitBaseCenter + IntVector2.Up;
    float num6 = 3f;
    switch (direction)
    {
      case DungeonData.Direction.NORTH:
        this.m_idealStartingPositions[0] = intVector2_1.ToVector2() + new Vector2(-0.5f, 0.0f) + new Vector2(0.0f, num6 + 0.5f);
        this.m_idealStartingPositions[1] = intVector2_1.ToVector2() + new Vector2(0.25f, -0.25f) + new Vector2(0.0f, num6 - 0.25f);
        break;
      case DungeonData.Direction.EAST:
        this.m_idealStartingPositions[0] = intVector2_1.ToVector2() + new Vector2(num6 + 0.5f, 0.0f);
        this.m_idealStartingPositions[1] = intVector2_1.ToVector2() + new Vector2(-0.25f, -1f) + new Vector2(num6 - 0.25f, 0.0f);
        break;
      case DungeonData.Direction.SOUTH:
        this.m_idealStartingPositions[0] = intVector2_1.ToVector2() + new Vector2(-0.5f, 0.0f) - new Vector2(0.0f, num6 + 0.5f);
        this.m_idealStartingPositions[1] = intVector2_1.ToVector2() + new Vector2(0.25f, 0.25f) - new Vector2(0.0f, num6 - 0.25f);
        break;
      case DungeonData.Direction.WEST:
        this.m_idealStartingPositions[0] = intVector2_1.ToVector2() - new Vector2(num6 + 0.5f, 0.0f);
        this.m_idealStartingPositions[1] = intVector2_1.ToVector2() + new Vector2(0.25f, -1f) - new Vector2(num6 - 0.25f, 0.0f);
        break;
    }
  }

  private void EndSequence(bool isChildSequence = false)
  {
    if (!isChildSequence)
    {
      List<AIActor> activeEnemies = this.m_room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
      for (int index = 0; index < activeEnemies.Count; ++index)
      {
        if ((UnityEngine.Object) activeEnemies[index].gameObject != (UnityEngine.Object) this.gameObject)
        {
          GenericIntroDoer component = activeEnemies[index].gameObject.GetComponent<GenericIntroDoer>();
          if ((bool) (UnityEngine.Object) component)
            component.EndSequence(true);
        }
      }
      this.bossUI.EndBossPortraitEarly();
      this.m_camera.StartTrackingPlayer();
      this.m_camera.SetManualControl(false);
    }
    if (this.HideGunAndHand && (bool) (UnityEngine.Object) this.aiShooter)
      this.aiShooter.ToggleGunAndHandRenderers(true, "genericIntro");
    if ((bool) (UnityEngine.Object) this.aiAnimator)
      this.aiAnimator.enabled = true;
    if ((bool) (UnityEngine.Object) this.renderer)
      this.renderer.enabled = true;
    if ((bool) (UnityEngine.Object) this.spriteAnimator)
      this.spriteAnimator.enabled = true;
    foreach (SpeculativeRigidbody componentsInChild in this.GetComponentsInChildren<SpeculativeRigidbody>())
      componentsInChild.CollideWithOthers = true;
    foreach (GameActor componentsInChild in this.GetComponentsInChildren<AIActor>())
      componentsInChild.IsGone = false;
    if ((UnityEngine.Object) this.m_specificIntroDoer != (UnityEngine.Object) null)
      this.m_specificIntroDoer.EndIntro();
    if ((bool) (UnityEngine.Object) this.aiActor)
      this.aiActor.State = AIActor.ActorState.Normal;
    if (this.InvisibleBeforeIntroAnim)
      this.aiActor.ToggleRenderers(true);
    if (this.m_room != null)
      Minimap.Instance.RevealMinimapRoom(this.m_room, true);
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      if (!GameManager.Instance.AllPlayers[index].healthHaver.IsDead)
        GameManager.Instance.AllPlayers[index].ToggleGunRenderers(true, string.Empty);
    }
    GameManager.Instance.PreventPausing = false;
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index])
        GameManager.Instance.AllPlayers[index].ClearInputOverride("BossIntro");
    }
    GameUIRoot.Instance.ToggleLowerPanels(true, source: string.Empty);
    GameUIRoot.Instance.ShowCoreUI(string.Empty);
    tk2dSpriteAnimator[] componentsInChildren = this.GetComponentsInChildren<tk2dSpriteAnimator>();
    for (int index = 0; index < componentsInChildren.Length; ++index)
    {
      if ((bool) (UnityEngine.Object) componentsInChildren[index])
        componentsInChildren[index].alwaysUpdateOffscreen = true;
    }
    BraveTime.ClearMultiplier(this.gameObject);
    GameManager.IsBossIntro = false;
    SuperReaperController.PreventShooting = false;
    Minimap.Instance.TemporarilyPreventMinimap = false;
    this.m_isRunning = false;
    if (this.OnIntroFinished == null)
      return;
    this.OnIntroFinished();
  }

  [DebuggerHidden]
  private IEnumerator WaitForBossCard()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new GenericIntroDoer.\u003CWaitForBossCard\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  private void TeleportCoopPlayers()
  {
    if (this.m_hasCoopTeleported || this.m_idealStartingPositions == null || this.m_idealStartingPositions.Length < 1)
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

  public void SkipWalkIn() => this.m_hasTriggeredWalkIn = true;

  private void ModifyCamera(bool value)
  {
    if (this.m_isCameraModified == value || !GameManager.HasInstance)
      return;
    CameraController cameraController = GameManager.Instance.MainCameraController;
    if (!(bool) (UnityEngine.Object) cameraController)
      return;
    if (value)
    {
      if ((bool) (UnityEngine.Object) this.cameraFocus)
      {
        cameraController.LockToRoom = true;
        cameraController.AddFocusPoint(this.cameraFocus);
      }
      if (this.roomPositionCameraFocus != Vector2.zero)
      {
        this.m_roomCameraFocus = new GameObject("room camera focus");
        this.m_roomCameraFocus.transform.position = (Vector3) (this.aiActor.ParentRoom.area.basePosition.ToVector2() + this.roomPositionCameraFocus);
        this.m_roomCameraFocus.transform.parent = this.aiActor.ParentRoom.hierarchyParent;
        cameraController.LockToRoom = true;
        cameraController.AddFocusPoint(this.m_roomCameraFocus);
      }
      if (this.fusebombLock)
        cameraController.PreventFuseBombAimOffset = true;
      cameraController.LockToRoom = true;
      this.m_isCameraModified = true;
      if (!(bool) (UnityEngine.Object) this.aiActor || !(bool) (UnityEngine.Object) this.aiActor.healthHaver)
        return;
      this.aiActor.healthHaver.OnDeath += new Action<Vector2>(this.OnDeath);
    }
    else
    {
      if ((bool) (UnityEngine.Object) this.cameraFocus)
      {
        cameraController.LockToRoom = false;
        cameraController.RemoveFocusPoint(this.cameraFocus);
      }
      if (this.roomPositionCameraFocus != Vector2.zero && (bool) (UnityEngine.Object) this.m_roomCameraFocus)
      {
        cameraController.LockToRoom = false;
        cameraController.RemoveFocusPoint(this.m_roomCameraFocus);
      }
      if (this.fusebombLock)
        cameraController.PreventFuseBombAimOffset = false;
      cameraController.LockToRoom = false;
      this.m_isCameraModified = false;
      if (!(bool) (UnityEngine.Object) this.aiActor || !(bool) (UnityEngine.Object) this.aiActor.healthHaver)
        return;
      this.aiActor.healthHaver.OnDeath -= new Action<Vector2>(this.OnDeath);
    }
  }

  public void RestrictMotion(bool value)
  {
    if (this.m_isMotionRestricted == value)
      return;
    if (value)
    {
      if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach)
        return;
      CellArea area = this.aiActor.ParentRoom.area;
      this.m_minPlayerX = area.basePosition.x * 16 /*0x10*/;
      this.m_minPlayerY = area.basePosition.y * 16 /*0x10*/ + 8;
      this.m_maxPlayerX = (area.basePosition.x + area.dimensions.x) * 16 /*0x10*/;
      this.m_maxPlayerY = (area.basePosition.y + area.dimensions.y - 1) * 16 /*0x10*/;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        GameManager.Instance.AllPlayers[index].specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.PlayerMovementRestrictor);
    }
    else
    {
      if (!GameManager.HasInstance || GameManager.IsReturningToBreach)
        return;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
        if ((bool) (UnityEngine.Object) allPlayer)
          allPlayer.specRigidbody.MovementRestrictor -= new SpeculativeRigidbody.MovementRestrictorDelegate(this.PlayerMovementRestrictor);
      }
    }
    this.m_isMotionRestricted = value;
  }

  private void PlayerMovementRestrictor(
    SpeculativeRigidbody playerSpecRigidbody,
    IntVector2 prevPixelOffset,
    IntVector2 pixelOffset,
    ref bool validLocation)
  {
    if (!validLocation)
      return;
    if (pixelOffset.y < prevPixelOffset.y && playerSpecRigidbody.PixelColliders[0].MinY + pixelOffset.y < this.m_minPlayerY)
      validLocation = false;
    if (pixelOffset.y > prevPixelOffset.y && playerSpecRigidbody.PixelColliders[0].MaxY + pixelOffset.y >= this.m_maxPlayerY)
      validLocation = false;
    if (pixelOffset.x < prevPixelOffset.x && playerSpecRigidbody.PixelColliders[0].MinX + pixelOffset.x < this.m_minPlayerX)
      validLocation = false;
    if (pixelOffset.x <= prevPixelOffset.x || playerSpecRigidbody.PixelColliders[0].MaxX + pixelOffset.x < this.m_maxPlayerX)
      return;
    validLocation = false;
  }

  private void OnDeath(Vector2 deathDir)
  {
    if (this.m_isCameraModified)
      this.ModifyCamera(false);
    if (!this.m_isMotionRestricted)
      return;
    this.RestrictMotion(false);
  }

  public static bool SkipFrame { get; set; }

  public enum TriggerType
  {
    PlayerEnteredRoom = 10, // 0x0000000A
    BossTriggerZone = 20, // 0x00000014
  }

  private enum Phase
  {
    CameraIntro,
    InitialDelay,
    PreIntroAnim,
    BossCard,
    CameraOutro,
    Cleanup,
  }
}
