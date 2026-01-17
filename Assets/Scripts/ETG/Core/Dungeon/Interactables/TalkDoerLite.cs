// Decompiled with JetBrains decompiler
// Type: TalkDoerLite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class TalkDoerLite : DungeonPlaceableBehaviour, IPlayerInteractable
    {
      private const float c_reinteractDelay = 0.5f;
      [Header("Interactable Region")]
      public bool usesOverrideInteractionRegion;
      [ShowInInspectorIf("usesOverrideInteractionRegion", false)]
      public Vector2 overrideRegionOffset = Vector2.zero;
      [ShowInInspectorIf("usesOverrideInteractionRegion", false)]
      public Vector2 overrideRegionDimensions = Vector2.zero;
      public float overrideInteractionRadius = -1f;
      public bool PreventInteraction;
      public bool AllowPlayerToPassEventually = true;
      [Header("Speech Options")]
      public Transform speakPoint;
      public bool SpeaksGleepGlorpenese;
      public string audioCharacterSpeechTag = string.Empty;
      public float playerApproachRadius = 5f;
      public float conversationBreakRadius = 5f;
      public TalkDoerLite echo1;
      public TalkDoerLite echo2;
      [Header("Other Options")]
      public bool PreventCoopInteraction;
      public bool IsPaletteSwapped;
      public Texture2D PaletteTexture;
      public TalkDoerLite.TeleportSettings teleportInSettings;
      public TalkDoerLite.TeleportSettings teleportOutSettings;
      public List<GameObject> itemsToLeaveBehind;
      public GameObject shadow;
      public bool DisableOnShortcutRun;
      public GameObject OptionalMinimapIcon;
      public float OverheadUIElementDelay = 1f;
      private float m_overheadUIElementDelay;
      public GameObject OverheadUIElementOnPreInteract;
      [NonSerialized]
      private dfControl m_extantOverheadUIElement;
      public tk2dSprite OptionalCustomNotificationSprite;
      public float OutlineDepth = 0.4f;
      public float OutlineLuminanceCutoff = 0.05f;
      public List<GameObject> ReassignPrefabReferences;
      [NonSerialized]
      public System.Action OnGenericFSMActionA;
      [NonSerialized]
      public System.Action OnGenericFSMActionB;
      [NonSerialized]
      public System.Action OnGenericFSMActionC;
      [NonSerialized]
      public System.Action OnGenericFSMActionD;
      [NonSerialized]
      public bool ForcePlayerLookAt;
      [NonSerialized]
      public bool ForceNonInteractable;
      private PlayerController m_talkingPlayer;
      [NonSerialized]
      public Tribool ShopStockStatus = Tribool.Unready;
      private TalkDoerLite.TalkingState m_talkingState;
      private bool m_isPlayerInRange;
      private bool m_isInteractable = true;
      private bool m_showOutlines = true;
      private bool m_allowWalkAways = true;
      private bool m_hasPlayerLocked;
      private float m_setInteractableTimer;
      private bool m_isHighlighted;
      private bool m_currentlyHasOutlines = true;
      private bool m_playerFacingNPC;
      private bool m_playerInsideApproachDistance;
      private bool m_coopPlayerInsideApproachDistance;
      private int m_numTimesSpokenTo;
      private RoomHandler m_parentRoom;
      private bool m_hasZombieTextBox;
      private float m_zombieBoxTimer;
      private string m_zombieBoxTalkAnim;
      [NonSerialized]
      public bool SuppressClear;
      private int m_clearTextFrameCountdown = -1;
      private bool m_collidedWithPlayerLastFrame;
      private float m_collidedWithPlayerTimer;
      public float MovementSpeed = 3f;
      [EnumFlags]
      public CellTypes PathableTiles = CellTypes.FLOOR;
      [NonSerialized]
      public bool m_isReadyForRepath = true;
      [NonSerialized]
      private Path m_currentPath;
      [NonSerialized]
      private Vector2? m_overridePathEnd;
      private IntVector2? m_clearance;
      public bool IsDoingForcedSpeech;

      public TalkDoerLite.TalkingState State
      {
        get => this.m_talkingState;
        set
        {
          if (!this.SuppressReinteractDelay && this.m_talkingState != TalkDoerLite.TalkingState.None && value == TalkDoerLite.TalkingState.None && this.IsInteractable)
          {
            this.IsInteractable = false;
            this.m_setInteractableTimer = 0.5f;
          }
          this.m_talkingState = value;
          this.UpdateOutlines();
        }
      }

      public bool IsTalking
      {
        get => this.State != TalkDoerLite.TalkingState.None;
        set
        {
          this.State = !value ? TalkDoerLite.TalkingState.None : TalkDoerLite.TalkingState.Conversation;
        }
      }

      public bool IsPlayerInRange
      {
        get => this.m_isPlayerInRange;
        set
        {
          this.m_isPlayerInRange = value;
          this.UpdateOutlines();
        }
      }

      public bool ShowOutlines
      {
        get => this.m_showOutlines;
        set
        {
          this.m_showOutlines = value;
          this.UpdateOutlines();
        }
      }

      public bool AllowWalkAways
      {
        get => this.m_allowWalkAways;
        set => this.m_allowWalkAways = value;
      }

      public bool IsInteractable
      {
        get => this.m_isInteractable;
        set
        {
          this.m_isInteractable = value;
          this.UpdateOutlines();
        }
      }

      public bool HasPlayerLocked
      {
        get => this.m_hasPlayerLocked;
        set
        {
          this.m_hasPlayerLocked = value;
          this.UpdateOutlines();
        }
      }

      public bool SuppressReinteractDelay { get; set; }

      public bool SuppressRoomEnterExitEvents { get; set; }

      public PlayerInputState CachedPlayerInput { get; set; }

      public PlayerController TalkingPlayer
      {
        get => this.m_talkingPlayer;
        set
        {
          if ((UnityEngine.Object) value == (UnityEngine.Object) null && (UnityEngine.Object) this.m_talkingPlayer != (UnityEngine.Object) null)
          {
            this.m_talkingPlayer.TalkPartner = (TalkDoerLite) null;
            this.m_talkingPlayer.IsTalking = false;
          }
          if ((UnityEngine.Object) value != (UnityEngine.Object) null && (UnityEngine.Object) this.m_talkingPlayer == (UnityEngine.Object) null)
          {
            value.TalkPartner = this;
            value.IsTalking = true;
          }
          if ((UnityEngine.Object) value != (UnityEngine.Object) null && (UnityEngine.Object) this.m_talkingPlayer != (UnityEngine.Object) null)
          {
            this.m_talkingPlayer.IsTalking = false;
            this.m_talkingPlayer.TalkPartner = (TalkDoerLite) null;
            value.IsTalking = true;
            value.TalkPartner = this;
          }
          this.m_talkingPlayer = value;
        }
      }

      public PlayerController CompletedTalkingPlayer { get; set; }

      public int NumTimesSpokenTo => this.m_numTimesSpokenTo;

      public RoomHandler ParentRoom
      {
        get
        {
          if (this.m_parentRoom == null)
            this.m_parentRoom = GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor));
          return this.m_parentRoom;
        }
      }

      public AIActor HostileObject { get; set; }

      public bool IsPlayingZombieAnimation
      {
        get
        {
          return this.m_hasZombieTextBox && (double) this.m_zombieBoxTimer > 0.0 && (bool) (UnityEngine.Object) this.aiAnimator && this.aiAnimator.IsPlaying(this.m_zombieBoxTalkAnim);
        }
      }

      public static void ClearPerLevelData() => StaticReferenceManager.AllNpcs.Clear();

      private void Start()
      {
        if ((UnityEngine.Object) this.shadow != (UnityEngine.Object) null)
        {
          tk2dBaseSprite component = this.shadow.GetComponent<tk2dBaseSprite>();
          if ((bool) (UnityEngine.Object) component && (double) component.HeightOffGround >= -1.0 && component.GetCurrentSpriteDef().name == "rogue_shadow" && this.shadow.layer == LayerMask.NameToLayer("FG_Critical"))
          {
            component.HeightOffGround = -5f;
            component.UpdateZDepth();
          }
        }
        this.m_overheadUIElementDelay = this.OverheadUIElementDelay;
        StaticReferenceManager.AllNpcs.Add(this);
        if ((UnityEngine.Object) this.aiActor != (UnityEngine.Object) null && !this.aiActor.IsNormalEnemy && !RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
          RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) this);
        if ((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null)
          this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, this.OutlineDepth, this.OutlineLuminanceCutoff);
        this.m_parentRoom = GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor));
        if (this.AllowPlayerToPassEventually && (UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER)
          this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandlePlayerTemporaryIncorporeality);
        if (this.m_parentRoom != null)
        {
          this.m_parentRoom.Entered += new RoomHandler.OnEnteredEventHandler(this.PlayerEnteredRoom);
          this.m_parentRoom.Exited += new RoomHandler.OnExitedEventHandler(this.PlayerExitedRoom);
          if ((bool) (UnityEngine.Object) this.OptionalMinimapIcon)
            Minimap.Instance.RegisterRoomIcon(this.m_parentRoom, this.OptionalMinimapIcon);
        }
        if (this.IsPaletteSwapped)
        {
          this.sprite.usesOverrideMaterial = true;
          this.sprite.renderer.material.SetTexture("_PaletteTex", (Texture) this.PaletteTexture);
        }
        if (this.DisableOnShortcutRun && GameManager.Instance.CurrentGameMode == GameManager.GameMode.SHORTCUT)
        {
          this.IsInteractable = false;
          SetNpcVisibility.SetVisible(this, false);
          this.ShowOutlines = false;
        }
        if ((bool) (UnityEngine.Object) this.spriteAnimator)
          this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
        for (int index1 = 0; index1 < this.playmakerFsms.Length; ++index1)
        {
          PlayMakerFSM playmakerFsm = this.playmakerFsms[index1];
          if ((bool) (UnityEngine.Object) playmakerFsm && playmakerFsm.Fsm != null)
          {
            for (int index2 = 0; index2 < playmakerFsm.Fsm.States.Length; ++index2)
            {
              FsmState state = playmakerFsm.Fsm.States[index2];
              for (int index3 = 0; index3 < state.Actions.Length; ++index3)
              {
                FsmStateAction action = state.Actions[index3];
                if (action is DialogueBox)
                {
                  DialogueBox dialogueBox = action as DialogueBox;
                  if ((UnityEngine.Object) dialogueBox.AlternativeTalker != (UnityEngine.Object) null)
                  {
                    TalkDoerLite instanceReference = this.GetInstanceReference(dialogueBox.AlternativeTalker);
                    if ((bool) (UnityEngine.Object) instanceReference)
                      dialogueBox.AlternativeTalker = instanceReference;
                  }
                }
              }
            }
          }
        }
      }

      private TalkDoerLite GetInstanceReference(TalkDoerLite prefab)
      {
        if (!(bool) (UnityEngine.Object) prefab)
          return (TalkDoerLite) null;
        for (int index = 0; index < this.ReassignPrefabReferences.Count; ++index)
        {
          GameObject reassignPrefabReference = this.ReassignPrefabReferences[index];
          if (reassignPrefabReference.name.StartsWith(prefab.name))
          {
            TalkDoerLite component = reassignPrefabReference.GetComponent<TalkDoerLite>();
            if ((bool) (UnityEngine.Object) component)
              return component;
          }
        }
        return (TalkDoerLite) null;
      }

      private void OnEnable()
      {
        if (!(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) this.speakPoint)
          return;
        TextBoxManager.ClearTextBoxImmediate(this.speakPoint);
      }

      private void HandlePlayerTemporaryIncorporeality(CollisionData rigidbodyCollision)
      {
        if (!((UnityEngine.Object) rigidbodyCollision.OtherRigidbody.GetComponent<PlayerController>() != (UnityEngine.Object) null))
          return;
        this.m_collidedWithPlayerLastFrame = true;
        this.m_collidedWithPlayerTimer += BraveTime.DeltaTime;
        if ((double) this.m_collidedWithPlayerTimer <= 1.0)
          return;
        this.specRigidbody.RegisterTemporaryCollisionException(rigidbodyCollision.OtherRigidbody, 0.25f);
      }

      public void ConvertToGhost()
      {
        if (!(bool) (UnityEngine.Object) this.sprite || !(bool) (UnityEngine.Object) this.sprite.renderer)
          return;
        this.sprite.usesOverrideMaterial = true;
        this.sprite.renderer.material.shader = ShaderCache.Acquire(PlayerController.DefaultShaderName);
        this.sprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
        this.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
        this.sprite.renderer.material.SetColor("_FlatColor", new Color(0.2f, 0.25f, 0.67f, 1f));
        this.sprite.renderer.material.SetVector("_SpecialFlags", new Vector4(1f, 0.0f, 0.0f, 0.0f));
      }

      private void Update()
      {
        if (this.IsTalking && GameManager.Instance.IsLoadingLevel)
          EndConversation.ForceEndConversation(this);
        this.m_collidedWithPlayerLastFrame = false;
        if ((double) this.m_setInteractableTimer > 0.0)
        {
          if (this.IsInteractable)
          {
            this.m_setInteractableTimer = -1f;
          }
          else
          {
            this.m_setInteractableTimer -= BraveTime.DeltaTime;
            if ((double) this.m_setInteractableTimer <= 0.0)
              this.IsInteractable = true;
          }
        }
        if (this.AllowWalkAways && this.m_talkingState == TalkDoerLite.TalkingState.Conversation && (double) Vector2.Distance(this.TalkingPlayer.sprite.WorldCenter, this.sprite.WorldCenter) > (double) this.conversationBreakRadius)
          this.SendPlaymakerEvent("playerWalkedAway");
        if ((UnityEngine.Object) this.CompletedTalkingPlayer != (UnityEngine.Object) null && !this.IsTalking && (double) Vector2.Distance(this.CompletedTalkingPlayer.sprite.WorldCenter, this.sprite.WorldCenter) > (double) this.conversationBreakRadius)
        {
          this.SendPlaymakerEvent("playerWalkedAwayPolitely");
          this.CompletedTalkingPlayer = (PlayerController) null;
        }
        if (this.m_hasZombieTextBox && (double) this.m_zombieBoxTimer > 0.0)
        {
          this.m_zombieBoxTimer -= BraveTime.DeltaTime;
          if ((double) this.m_zombieBoxTimer <= 0.0)
            this.CloseTextBox(true);
        }
        if (this.IsPlayerInRange)
        {
          if ((double) this.m_overheadUIElementDelay > 0.0)
          {
            this.m_overheadUIElementDelay -= BraveTime.DeltaTime;
            if ((double) this.m_overheadUIElementDelay <= 0.0)
              this.CreateOverheadUI();
          }
        }
        else if ((double) this.m_overheadUIElementDelay < (double) this.OverheadUIElementDelay)
          this.m_overheadUIElementDelay += BraveTime.DeltaTime;
        if (GameManager.Instance.IsPaused && (UnityEngine.Object) this.m_extantOverheadUIElement != (UnityEngine.Object) null)
        {
          if (this.m_extantOverheadUIElement.IsVisible)
          {
            this.m_extantOverheadUIElement.IsVisible = false;
            foreach (BraveBehaviour componentsInChild in this.m_extantOverheadUIElement.GetComponentsInChildren<tk2dBaseSprite>())
              componentsInChild.renderer.enabled = false;
          }
        }
        else if ((UnityEngine.Object) this.m_extantOverheadUIElement != (UnityEngine.Object) null && !this.m_extantOverheadUIElement.IsVisible)
        {
          this.m_extantOverheadUIElement.IsVisible = true;
          foreach (BraveBehaviour componentsInChild in this.m_extantOverheadUIElement.GetComponentsInChildren<tk2dBaseSprite>())
            componentsInChild.renderer.enabled = true;
        }
        if ((bool) (UnityEngine.Object) GameManager.Instance && (bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer && !GameManager.Instance.PrimaryPlayer.IsStealthed)
        {
          bool flag = (double) Vector2.Distance(this.specRigidbody.UnitCenter, GameManager.Instance.PrimaryPlayer.specRigidbody.UnitCenter) < (double) this.playerApproachRadius;
          if (!this.m_playerInsideApproachDistance && flag)
            this.SendPlaymakerEvent("playerApproached");
          else if (this.m_playerInsideApproachDistance && !flag)
            this.SendPlaymakerEvent("playerUnapproached");
          this.m_playerInsideApproachDistance = flag;
        }
        if ((bool) (UnityEngine.Object) GameManager.Instance && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && (bool) (UnityEngine.Object) GameManager.Instance.SecondaryPlayer && !GameManager.Instance.SecondaryPlayer.IsStealthed)
        {
          bool flag = (double) Vector2.Distance(this.specRigidbody.UnitCenter, GameManager.Instance.SecondaryPlayer.specRigidbody.UnitCenter) < (double) this.playerApproachRadius;
          if (!this.m_coopPlayerInsideApproachDistance && flag)
            this.SendPlaymakerEvent("coopPlayerApproached");
          else if (this.m_coopPlayerInsideApproachDistance && !flag)
            this.SendPlaymakerEvent("coopPlayerUnapproached");
          this.m_coopPlayerInsideApproachDistance = flag;
        }
        if (!(bool) (UnityEngine.Object) GameManager.Instance || !(bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer || GameManager.Instance.PrimaryPlayer.CurrentRoom != this.m_parentRoom)
          return;
        Vector2 zero = Vector2.zero;
        bool flag1 = (double) Vector2.Dot(((UnityEngine.Object) GameManager.Instance.PrimaryPlayer.CurrentGun == (UnityEngine.Object) null || GameManager.Instance.PrimaryPlayer.inventory.ForceNoGun ? GameManager.Instance.PrimaryPlayer.NonZeroLastCommandedDirection : (Vector2) (GameManager.Instance.PrimaryPlayer.unadjustedAimPoint - GameManager.Instance.PrimaryPlayer.LockedApproximateSpriteCenter)).normalized, (this.specRigidbody.UnitCenter - GameManager.Instance.PrimaryPlayer.LockedApproximateSpriteCenter.XY()).normalized) > -0.25 || this.ForcePlayerLookAt;
        if (!this.m_playerFacingNPC && flag1)
          this.SendPlaymakerEvent("playerStartedFacing");
        else if (this.m_playerFacingNPC && !flag1)
          this.SendPlaymakerEvent("playerStoppedFacing");
        this.m_playerFacingNPC = flag1;
      }

      protected void LateUpdate()
      {
        if (!this.m_collidedWithPlayerLastFrame)
          this.m_collidedWithPlayerTimer = 0.0f;
        if (this.SuppressClear || this.m_clearTextFrameCountdown <= 0)
          return;
        --this.m_clearTextFrameCountdown;
        if (this.m_clearTextFrameCountdown > 0)
          return;
        TextBoxManager.ClearTextBox(this.speakPoint);
      }

      private void OnDisable()
      {
        if ((bool) (UnityEngine.Object) this && (bool) (UnityEngine.Object) this.speakPoint)
          TextBoxManager.ClearTextBoxImmediate(this.speakPoint);
        if (!((UnityEngine.Object) this.m_extantOverheadUIElement != (UnityEngine.Object) null))
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantOverheadUIElement.gameObject);
        this.m_extantOverheadUIElement = (dfControl) null;
      }

      protected override void OnDestroy()
      {
        if (this.m_parentRoom != null)
        {
          this.m_parentRoom.Entered -= new RoomHandler.OnEnteredEventHandler(this.PlayerEnteredRoom);
          this.m_parentRoom.Exited -= new RoomHandler.OnExitedEventHandler(this.PlayerExitedRoom);
        }
        StaticReferenceManager.AllNpcs.Remove(this);
        base.OnDestroy();
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        if (!(bool) (UnityEngine.Object) this || !this.IsInteractable || this.ForceNonInteractable || this.PreventInteraction || !this.gameObject.activeSelf || (bool) (UnityEngine.Object) this.aiActor && GameManager.Instance.BestActivePlayer.IsInCombat)
          return 1000f;
        if (this.usesOverrideInteractionRegion)
          return BraveMathCollege.DistToRectangle(point, this.transform.position.XY() + this.overrideRegionOffset * (1f / 16f), this.overrideRegionDimensions * (1f / 16f));
        float rectangle;
        if ((bool) (UnityEngine.Object) this.specRigidbody)
        {
          PixelCollider primaryPixelCollider = this.specRigidbody.PrimaryPixelCollider;
          rectangle = BraveMathCollege.DistToRectangle(point, primaryPixelCollider.UnitBottomLeft, primaryPixelCollider.UnitDimensions);
        }
        else
        {
          Bounds bounds = this.sprite.GetBounds();
          bounds.center += this.sprite.transform.position;
          rectangle = BraveMathCollege.DistToRectangle(point, (Vector2) bounds.min, (Vector2) bounds.size);
        }
        return rectangle;
      }

      public float GetOverrideMaxDistance() => this.overrideInteractionRadius;

      private void CreateOverheadUI()
      {
        if (this.IsTalking || !((UnityEngine.Object) this.OverheadUIElementOnPreInteract != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_extantOverheadUIElement == (UnityEngine.Object) null))
          return;
        this.m_extantOverheadUIElement = GameUIRoot.Instance.Manager.AddPrefab(this.OverheadUIElementOnPreInteract);
        FoyerInfoPanelController component = this.m_extantOverheadUIElement.GetComponent<FoyerInfoPanelController>();
        if (!(bool) (UnityEngine.Object) component)
          return;
        component.followTransform = this.transform;
        if (component.characterIdentity != PlayableCharacters.CoopCultist)
          component.offset = new Vector3(0.75f, 1.625f, 0.0f);
        else
          component.offset = new Vector3(0.75f, 2.25f, 0.0f);
      }

      private void DestroyOverheadUI()
      {
        if (!((UnityEngine.Object) this.OverheadUIElementOnPreInteract != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_extantOverheadUIElement != (UnityEngine.Object) null))
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantOverheadUIElement.gameObject);
        this.m_extantOverheadUIElement = (dfControl) null;
      }

      public void OnEnteredRange(PlayerController interactor)
      {
        this.IsPlayerInRange = true;
        this.UpdateOutlines();
      }

      public void OnExitRange(PlayerController interactor)
      {
        this.DestroyOverheadUI();
        this.IsPlayerInRange = false;
        this.UpdateOutlines();
      }

      public void Interact(PlayerController interactor)
      {
        if (!interactor.IsPrimaryPlayer && this.PreventCoopInteraction || !this.IsInteractable || this.m_talkingState == TalkDoerLite.TalkingState.Conversation || GameManager.Instance.IsFoyer && interactor.WasTalkingThisFrame)
          return;
        if (GameManager.Instance.IsFoyer)
        {
          FoyerCharacterSelectFlag component = this.GetComponent<FoyerCharacterSelectFlag>();
          if ((bool) (UnityEngine.Object) component && !component.CanBeSelected())
          {
            int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_cancel_01", this.gameObject);
            return;
          }
        }
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
          GameManager.Instance.LastUsedInputDeviceForConversation = BraveInput.GetInstanceForPlayer(interactor.PlayerIDX).ActiveActions.Device;
        if ((UnityEngine.Object) this.m_extantOverheadUIElement != (UnityEngine.Object) null)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantOverheadUIElement.gameObject);
          this.m_extantOverheadUIElement = (dfControl) null;
        }
        this.TalkingPlayer = interactor;
        EncounterTrackable component1 = this.GetComponent<EncounterTrackable>();
        if (this.m_numTimesSpokenTo == 0 && (UnityEngine.Object) component1 != (UnityEngine.Object) null)
          GameStatsManager.Instance.HandleEncounteredObject(component1);
        ++this.m_numTimesSpokenTo;
        this.SendPlaymakerEvent("playerInteract");
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      private void OnRigidbodyCollision(CollisionData rigidbodyCollision)
      {
        if (this.m_talkingState == TalkDoerLite.TalkingState.Conversation || this.IsTalking)
          return;
        SpeculativeRigidbody otherRigidbody = rigidbodyCollision.OtherRigidbody;
        if (!(bool) (UnityEngine.Object) otherRigidbody.projectile || !(otherRigidbody.projectile.Owner is PlayerController))
          return;
        this.SendPlaymakerEvent("takePlayerDamage");
      }

      private void PlayerEnteredRoom(PlayerController p)
      {
        if (p.IsStealthed || this.SuppressRoomEnterExitEvents)
          return;
        this.SendPlaymakerEvent("playerEnteredRoom");
      }

      private void PlayerExitedRoom()
      {
        if ((bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer && GameManager.Instance.PrimaryPlayer.IsStealthed || this.SuppressRoomEnterExitEvents)
          return;
        this.SendPlaymakerEvent("playerExitedRoom");
      }

      protected void HandleAnimationEvent(
        tk2dSpriteAnimator animator,
        tk2dSpriteAnimationClip clip,
        int frameNo)
      {
        tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNo);
        if (frame.eventOutline == tk2dSpriteAnimationFrame.OutlineModifier.Unspecified)
          return;
        if (frame.eventOutline == tk2dSpriteAnimationFrame.OutlineModifier.TurnOn)
        {
          this.ShowOutlines = true;
        }
        else
        {
          if (frame.eventOutline != tk2dSpriteAnimationFrame.OutlineModifier.TurnOff)
            return;
          this.ShowOutlines = false;
        }
      }

      public void SetZombieBoxTimer(float timer, string talkAnim)
      {
        this.m_hasZombieTextBox = true;
        this.m_zombieBoxTimer = timer;
        this.m_zombieBoxTalkAnim = talkAnim;
      }

      public void ShowText(
        Vector3 worldPosition,
        Transform parent,
        float duration,
        string text,
        bool instant = true,
        TextBoxManager.BoxSlideOrientation slideOrientation = TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT,
        bool showContinueText = false,
        bool isThoughtBox = false,
        string overrideSpeechAudioTag = null)
      {
        this.m_hasZombieTextBox = false;
        this.m_zombieBoxTimer = 0.0f;
        this.m_clearTextFrameCountdown = -1;
        if (isThoughtBox)
        {
          string overrideAudioTag = overrideSpeechAudioTag == null ? this.audioCharacterSpeechTag ?? string.Empty : overrideSpeechAudioTag;
          TextBoxManager.ShowThoughtBubble(worldPosition, parent, duration, text, instant, showContinueText, overrideAudioTag);
        }
        else
        {
          string audioTag = overrideSpeechAudioTag == null ? this.audioCharacterSpeechTag ?? string.Empty : overrideSpeechAudioTag;
          TextBoxManager.ShowTextBox(worldPosition, parent, duration, text, audioTag, instant, slideOrientation, showContinueText, this.SpeaksGleepGlorpenese);
        }
      }

      public void CloseTextBox(bool overrideZombieBoxes)
      {
        if (overrideZombieBoxes)
        {
          this.m_hasZombieTextBox = false;
          this.m_zombieBoxTimer = 0.0f;
          if ((bool) (UnityEngine.Object) this.aiAnimator)
            this.aiAnimator.EndAnimationIf(this.m_zombieBoxTalkAnim);
        }
        if (this.m_hasZombieTextBox)
          return;
        this.m_clearTextFrameCountdown = 2;
      }

      private void UpdateOutlines()
      {
        bool flag = this.IsInteractable && this.State != TalkDoerLite.TalkingState.Conversation && this.IsPlayerInRange && !this.HasPlayerLocked;
        if (flag == this.m_isHighlighted && this.m_currentlyHasOutlines == this.ShowOutlines)
          return;
        this.m_isHighlighted = flag;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        if (this.ShowOutlines)
          SpriteOutlineManager.AddOutlineToSprite(this.sprite, !this.m_isHighlighted ? Color.black : Color.white, this.OutlineDepth, !this.m_isHighlighted ? this.OutlineLuminanceCutoff : 0.0f);
        this.sprite.UpdateZDepth();
        this.m_currentlyHasOutlines = this.ShowOutlines;
      }

      public Path CurrentPath
      {
        get => this.m_currentPath;
        set => this.m_currentPath = value;
      }

      public IntVector2 PathTile
      {
        get => this.specRigidbody.UnitBottomLeft.ToIntVector2(VectorConversions.Floor);
      }

      public IntVector2 Clearance
      {
        get
        {
          if (!this.m_clearance.HasValue)
            this.m_clearance = new IntVector2?(this.specRigidbody.UnitDimensions.ToIntVector2(VectorConversions.Ceil));
          return this.m_clearance.Value;
        }
      }

      public Vector2 GetPathVelocityContribution(Vector2 lastPosition, int distanceThresholdPixels = 1)
      {
        if ((this.m_currentPath == null || this.m_currentPath.Count == 0) && !this.m_overridePathEnd.HasValue)
          return Vector2.zero;
        Vector2 unitCenter = this.specRigidbody.UnitCenter;
        Vector2 vector2_1 = this.m_currentPath == null ? this.m_overridePathEnd.Value : this.m_currentPath.GetFirstCenterVector2();
        bool flag1 = (this.m_currentPath != null ? this.m_currentPath.Count : 0) + (this.m_overridePathEnd.HasValue ? 1 : 0) == 1;
        bool flag2 = false;
        int pixel = !flag1 ? 1 : distanceThresholdPixels;
        if ((double) Vector2.Distance(unitCenter, vector2_1) < (double) PhysicsEngine.PixelToUnit(pixel))
          flag2 = true;
        else if (!flag1)
        {
          Vector2 b = BraveMathCollege.ClosestPointOnLineSegment(vector2_1, lastPosition, unitCenter);
          if ((double) Vector2.Distance(vector2_1, b) < (double) PhysicsEngine.PixelToUnit(pixel))
            flag2 = true;
        }
        if (flag2)
        {
          if (this.m_currentPath != null && this.m_currentPath.Count > 0)
          {
            this.m_currentPath.RemoveFirst();
            if (this.m_currentPath.Count == 0)
            {
              this.m_currentPath = (Path) null;
              return Vector2.zero;
            }
          }
          else if (this.m_overridePathEnd.HasValue)
            this.m_overridePathEnd = new Vector2?();
        }
        Vector2 vector2_2 = vector2_1 - unitCenter;
        return flag1 && (double) this.MovementSpeed > (double) vector2_2.magnitude ? vector2_2 : this.MovementSpeed * vector2_2.normalized;
      }

      public void PathfindToPosition(
        Vector2 targetPosition,
        Vector2? overridePathEnd = null,
        CellValidator cellValidator = null)
      {
        Path path = (Path) null;
        if (!Pathfinder.Instance.GetPath(this.PathTile, targetPosition.ToIntVector2(VectorConversions.Floor), out path, new IntVector2?(this.Clearance), this.PathableTiles, cellValidator))
          return;
        this.m_currentPath = path;
        this.m_overridePathEnd = overridePathEnd;
        if (this.m_currentPath.Count == 0)
          this.m_currentPath = (Path) null;
        else
          path.Smooth(this.specRigidbody.UnitCenter, this.specRigidbody.UnitDimensions / 2f, this.PathableTiles, false, this.Clearance);
      }

      public void ForceTimedSpeech(
        string words,
        float initialDelay,
        float duration,
        TextBoxManager.BoxSlideOrientation slideOrientation)
      {
        UnityEngine.Debug.Log((object) ("starting forced timed speech: " + words));
        this.StartCoroutine(this.HandleForcedTimedSpeech(words, initialDelay, duration, slideOrientation));
      }

      [DebuggerHidden]
      private IEnumerator HandleForcedTimedSpeech(
        string words,
        float initialDelay,
        float duration,
        TextBoxManager.BoxSlideOrientation slideOrientation)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TalkDoerLite.\u003CHandleForcedTimedSpeech\u003Ec__Iterator0()
        {
          initialDelay = initialDelay,
          words = words,
          slideOrientation = slideOrientation,
          duration = duration,
          \u0024this = this
        };
      }

      [Serializable]
      public class TeleportSettings
      {
        public string anim;
        public float animDelay;
        public GameObject vfx;
        public float vfxDelay;
        public Teleport.Timing timing;
        public GameObject vfxAnchor;
      }

      public enum TalkingState
      {
        None,
        Passive,
        Conversation,
      }
    }

}
