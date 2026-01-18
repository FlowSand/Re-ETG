using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class DungeonDoorController : PersistentVFXManagerBehaviour, IPlayerInteractable
  {
    [SerializeField]
    protected DungeonDoorController.DungeonDoorMode doorMode;
    [SerializeField]
    protected bool doorClosesAfterEveryOpen;
    [NonSerialized]
    private bool hasEverBeenOpen;
    public DungeonDoorController.DoorModule[] doorModules;
    public bool hideSealAnimators = true;
    public tk2dSpriteAnimator[] sealAnimators;
    public tk2dSpriteAnimator[] sealChainAnimators;
    public tk2dSpriteAnimator[] sealVFX;
    public float unsealDistanceMaximum = -1f;
    public GameObject unsealedVFXOverride;
    public string sealAnimationName;
    public string unsealAnimationName;
    public string playerNearSealedAnimationName;
    public bool SupportsSubsidiaryDoors = true;
    public bool northSouth = true;
    [NonSerialized]
    public RuntimeExitDefinition exitDefinition;
    [NonSerialized]
    public RoomHandler upstreamRoom;
    [NonSerialized]
    public RoomHandler downstreamRoom;
    [NonSerialized]
    public bool OneWayDoor;
    [HideInInspector]
    public DungeonDoorSubsidiaryBlocker subsidiaryBlocker;
    [HideInInspector]
    public DungeonDoorController subsidiaryDoor;
    [HideInInspector]
    public DungeonDoorController parentDoor;
    [NonSerialized]
    public Transform messageTransformPoint;
    [NonSerialized]
    public string messageToDisplay;
    public tk2dSpriteAnimator LockAnimator;
    public tk2dSpriteAnimator ChainsAnimator;
    private bool m_open;
    public bool isLocked;
    public bool lockIsBusted;
    [SerializeField]
    private bool isSealed;
    private bool m_openIsFlipped;
    public bool usesUnsealScreenShake;
    public ScreenShakeSettings unsealScreenShake;
    private bool m_isDestroyed;
    private bool m_wasOpenWhenSealed;
    private bool m_lockHasApproached;
    private bool m_lockHasLaughed;
    private bool m_lockHasSpit;
    private bool m_finalBossDoorHasOpened;
    private bool m_isCoopArrowing;
    private bool m_hasGC;

    public bool IsUniqueVisiblityDoor => this.hasEverBeenOpen && this.doorClosesAfterEveryOpen;

    public DungeonDoorController.DungeonDoorMode Mode => this.doorMode;

    public bool IsOpen
    {
      get
      {
        return this.doorMode == DungeonDoorController.DungeonDoorMode.BOSS_DOOR_ONLY_UNSEALS || this.doorMode == DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR || this.doorMode == DungeonDoorController.DungeonDoorMode.ONE_WAY_DOOR_ONLY_UNSEALS || this.m_open;
      }
    }

    public bool IsOpenForVisibilityTest
    {
      get
      {
        if (this.doorMode == DungeonDoorController.DungeonDoorMode.BOSS_DOOR_ONLY_UNSEALS || this.doorMode == DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR || this.doorMode == DungeonDoorController.DungeonDoorMode.ONE_WAY_DOOR_ONLY_UNSEALS)
          return true;
        if (this.IsSealed)
          return false;
        return this.IsUniqueVisiblityDoor || this.m_open;
      }
    }

    public bool IsSealed
    {
      get => this.isSealed;
      set
      {
        if (value && this.m_open)
          this.Close();
        if (this.isSealed == value)
          return;
        if (value)
          this.SealInternal();
        else
          this.UnsealInternal();
      }
    }

    public bool KeepBossDoorSealed { get; set; }

    public void SetSealedSilently(bool v) => this.isSealed = v;

    public void DoSeal(RoomHandler sourceRoom)
    {
      if (this.doorMode == DungeonDoorController.DungeonDoorMode.ONE_WAY_DOOR_ONLY_UNSEALS)
      {
        if ((UnityEngine.Object) this.subsidiaryDoor != (UnityEngine.Object) null)
          this.subsidiaryDoor.SealInternal();
        if (!((UnityEngine.Object) this.subsidiaryBlocker != (UnityEngine.Object) null))
          return;
        this.subsidiaryBlocker.Seal();
      }
      else if ((UnityEngine.Object) this.subsidiaryBlocker != (UnityEngine.Object) null || (UnityEngine.Object) this.subsidiaryDoor != (UnityEngine.Object) null)
      {
        if (this.exitDefinition.upstreamExit.jointedExit)
        {
          if ((this.exitDefinition.upstreamExit.referencedExit.exitDirection == DungeonData.Direction.NORTH || this.exitDefinition.upstreamExit.referencedExit.exitDirection == DungeonData.Direction.SOUTH) && this.exitDefinition.upstreamRoom == sourceRoom || (this.exitDefinition.downstreamExit.referencedExit.exitDirection == DungeonData.Direction.NORTH || this.exitDefinition.downstreamExit.referencedExit.exitDirection == DungeonData.Direction.SOUTH) && this.exitDefinition.downstreamRoom == sourceRoom)
          {
            this.SealInternal();
          }
          else
          {
            if ((UnityEngine.Object) this.subsidiaryDoor != (UnityEngine.Object) null)
              this.subsidiaryDoor.SealInternal();
            if (!((UnityEngine.Object) this.subsidiaryBlocker != (UnityEngine.Object) null))
              return;
            this.subsidiaryBlocker.Seal();
          }
        }
        else if (sourceRoom == this.exitDefinition.upstreamRoom)
        {
          this.SealInternal();
        }
        else
        {
          if ((UnityEngine.Object) this.subsidiaryDoor != (UnityEngine.Object) null)
            this.subsidiaryDoor.SealInternal();
          if (!((UnityEngine.Object) this.subsidiaryBlocker != (UnityEngine.Object) null))
            return;
          this.subsidiaryBlocker.Seal();
        }
      }
      else
        this.SealInternal();
    }

    public void AssignPressurePlate(PressurePlate source)
    {
      source.OnPressurePlateDepressed += new Action<PressurePlate>(this.OnPressurePlateTriggered);
    }

    private void OnPressurePlateTriggered(PressurePlate source)
    {
      source.OnPressurePlateDepressed -= new Action<PressurePlate>(this.OnPressurePlateTriggered);
      this.DoUnseal(this.downstreamRoom);
    }

    public void DoUnseal(RoomHandler sourceRoom)
    {
      if ((UnityEngine.Object) this.subsidiaryDoor != (UnityEngine.Object) null && this.subsidiaryDoor.isSealed)
        this.subsidiaryDoor.UnsealInternal();
      if ((UnityEngine.Object) this.subsidiaryBlocker != (UnityEngine.Object) null && this.subsidiaryBlocker.isSealed)
        this.subsidiaryBlocker.Unseal();
      if (!this.isSealed)
        return;
      this.UnsealInternal();
    }

    private void Start()
    {
      if (this.doorMode != DungeonDoorController.DungeonDoorMode.BOSS_DOOR_ONLY_UNSEALS && this.doorMode != DungeonDoorController.DungeonDoorMode.ONE_WAY_DOOR_ONLY_UNSEALS && this.doorMode != DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR)
      {
        for (int index = 0; index < this.doorModules.Length; ++index)
        {
          tk2dSprite component1 = this.doorModules[index].animator.GetComponent<tk2dSprite>();
          component1.depthUsesTrimmedBounds = true;
          SpeculativeRigidbody component2 = this.doorModules[index].animator.GetComponent<SpeculativeRigidbody>();
          component2.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
          component2.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.OnEnterTrigger);
          this.doorModules[index].sprite = component1;
          this.doorModules[index].rigidbody = component2;
          this.doorModules[index].animator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted);
        }
        this.UpdateDoorDepths();
      }
      if (this.doorMode == DungeonDoorController.DungeonDoorMode.COMPLEX && !this.northSouth)
      {
        for (int index = 0; index < this.sealAnimators.Length; ++index)
        {
          if ((UnityEngine.Object) this.sealAnimators[index].sprite.attachParent != (UnityEngine.Object) null)
            this.sealAnimators[index].sprite.attachParent.DetachRenderer(this.sealAnimators[index].sprite);
          this.sealAnimators[index].sprite.automaticallyManagesDepth = true;
          this.sealAnimators[index].sprite.attachParent = (tk2dBaseSprite) null;
          this.sealAnimators[index].sprite.depthUsesTrimmedBounds = false;
          this.sealAnimators[index].sprite.HeightOffGround = 0.0f;
        }
      }
      if (this.doorMode == DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR)
      {
        IntVector2 intVector2_1 = this.transform.position.IntXY() + new IntVector2(-2, -1);
        for (int y = 0; y < 8; ++y)
        {
          for (int x = 0; x < 6; ++x)
          {
            IntVector2 intVector2_2 = intVector2_1 + new IntVector2(x, y);
            if (this.upstreamRoom != null)
              this.upstreamRoom.FeatureCells.Add(intVector2_2);
          }
        }
      }
      if (this.sealAnimators != null)
      {
        for (int index = 0; index < this.sealAnimators.Length; ++index)
          this.sealAnimators[index].alwaysUpdateOffscreen = true;
      }
      if (this.exitDefinition != null && this.exitDefinition.upstreamExit != null && this.exitDefinition.upstreamExit.isLockedDoor)
        this.isLocked = true;
      if (this.isLocked && (UnityEngine.Object) this.parentDoor != (UnityEngine.Object) null && (UnityEngine.Object) this.parentDoor.subsidiaryDoor == (UnityEngine.Object) this)
        this.isLocked = false;
      if (this.isLocked)
      {
        if ((UnityEngine.Object) this.LockAnimator == (UnityEngine.Object) null)
          this.BecomeLockedDoor();
        RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) this);
      }
      foreach (SpeculativeRigidbody componentsInChild in this.GetComponentsInChildren<SpeculativeRigidbody>())
        componentsInChild.PreventPiercing = true;
    }

    public void ForceBecomeLockedDoor()
    {
      if (!this.isLocked)
        return;
      if ((UnityEngine.Object) this.LockAnimator == (UnityEngine.Object) null)
        this.BecomeLockedDoor();
      RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) this);
    }

    protected void BecomeLockedDoor()
    {
      if (this.doorMode != DungeonDoorController.DungeonDoorMode.COMPLEX)
        return;
      if (!this.northSouth)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject) BraveResources.Load("Global Prefabs/DoorLockPrefab_Horizontal"));
        float x = 0.0f;
        if ((bool) (UnityEngine.Object) this.doorModules[0].animator && (bool) (UnityEngine.Object) this.doorModules[0].animator.Sprite)
          x = this.doorModules[0].animator.transform.localPosition.x + this.doorModules[0].animator.Sprite.GetBounds().max.x;
        else if ((bool) (UnityEngine.Object) this.doorModules[0].sprite)
          x = this.doorModules[0].sprite.transform.localPosition.x + this.doorModules[0].sprite.GetBounds().max.x;
        gameObject.transform.parent = this.transform;
        gameObject.transform.localPosition = new Vector3(x, 0.0f, 0.0f);
        this.LockAnimator = gameObject.GetComponent<tk2dSpriteAnimator>();
        this.ChainsAnimator = gameObject.transform.GetChild(0).GetComponent<tk2dSpriteAnimator>();
      }
      else
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject) BraveResources.Load("Global Prefabs/DoorLockPrefab_Vertical"));
        gameObject.transform.parent = this.transform;
        gameObject.transform.localPosition = new Vector3(0.0f, -0.75f, 0.0f);
        this.LockAnimator = gameObject.GetComponent<tk2dSpriteAnimator>();
        this.ChainsAnimator = gameObject.transform.GetChild(0).GetComponent<tk2dSpriteAnimator>();
      }
      this.LockAnimator.sprite.UpdateZDepth();
      this.ChainsAnimator.sprite.UpdateZDepth();
      if (!this.northSouth)
      {
        this.LockAnimator.sprite.IsPerpendicular = true;
        this.ChainsAnimator.sprite.UpdateZDepth();
      }
      if (this.northSouth || this.exitDefinition.upstreamExit.referencedExit.exitDirection != DungeonData.Direction.EAST)
        return;
      this.FlipLockToOtherSide();
    }

    protected void UpdateDoorDepths()
    {
      for (int index = 0; index < this.doorModules.Length; ++index)
      {
        if (!this.doorModules[index].isLerping)
        {
          float targetDepth = !this.m_open ? this.doorModules[index].closedDepth : this.doorModules[index].openDepth;
          if (!this.northSouth && !this.doorModules[index].sprite.depthUsesTrimmedBounds)
            targetDepth = -5.25f;
          if ((double) this.doorModules[index].sprite.HeightOffGround != (double) targetDepth)
            this.AnimationDepthLerp(this.doorModules[index].sprite, targetDepth, (tk2dSpriteAnimationClip) null, this.doorModules[index], !this.northSouth && index == 0);
        }
      }
    }

    private void Update()
    {
      if (this.isSealed && (this.doorMode == DungeonDoorController.DungeonDoorMode.BOSS_DOOR_ONLY_UNSEALS || !this.m_finalBossDoorHasOpened && this.doorMode == DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR))
      {
        for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
        {
          if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index1] && (bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index1].healthHaver && !GameManager.Instance.AllPlayers[index1].healthHaver.IsDead && !GameManager.Instance.PreventPausing && GameManager.Instance.AllPlayers[index1].CurrentRoom != null && (GameManager.Instance.AllPlayers[index1].CurrentRoom == this.upstreamRoom || GameManager.Instance.AllPlayers[index1].CurrentRoom == this.downstreamRoom) && GameManager.Instance.AllPlayers[index1].CurrentRoom.UnsealConditionsMet() && ((double) this.unsealDistanceMaximum <= 0.0 || (double) Vector2.Distance(this.sealAnimators[0].Sprite.WorldCenter, GameManager.Instance.AllPlayers[index1].specRigidbody.UnitCenter) < (double) this.unsealDistanceMaximum))
          {
            if (this.doorMode == DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR)
            {
              bool flag = false;
              if (GameManager.Instance.AllPlayers[index1].CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
              {
                List<AIActor> activeEnemies = GameManager.Instance.AllPlayers[index1].CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                for (int index2 = 0; index2 < activeEnemies.Count; ++index2)
                {
                  if ((bool) (UnityEngine.Object) activeEnemies[index2] && !activeEnemies[index2].IgnoreForRoomClear && activeEnemies[index2].HasBeenEngaged && activeEnemies[index2].IsNormalEnemy)
                    flag = true;
                }
              }
              if (!flag)
              {
                this.m_finalBossDoorHasOpened = true;
                this.UnsealInternal();
              }
            }
            else if (this.doorMode != DungeonDoorController.DungeonDoorMode.BOSS_DOOR_ONLY_UNSEALS || !this.KeepBossDoorSealed)
              this.UnsealInternal();
          }
        }
      }
      if (this.isLocked && this.lockIsBusted)
      {
        string name = !this.northSouth ? "lock_guy_side_busted" : "lock_guy_busted";
        if (!this.LockAnimator.IsPlaying(name))
          this.LockAnimator.Play(name);
      }
      else if (this.northSouth && this.isLocked)
      {
        Vector2 zero = Vector2.zero;
        for (int index = 0; index < this.doorModules.Length; ++index)
          zero += this.doorModules[index].rigidbody.UnitCenter;
        float num = Vector2.Distance(zero / (float) this.doorModules.Length, GameManager.Instance.PrimaryPlayer.specRigidbody.UnitCenter);
        if (!this.m_lockHasApproached && (double) num < 2.5)
        {
          this.LockAnimator.Play("lock_guy_approach");
          this.m_lockHasApproached = true;
        }
        else if ((double) num > 2.5)
        {
          if (this.m_lockHasLaughed)
            this.LockAnimator.Play("lock_guy_spit");
          this.m_lockHasLaughed = false;
          this.m_lockHasApproached = false;
        }
        if (!this.m_lockHasSpit && (UnityEngine.Object) this.LockAnimator != (UnityEngine.Object) null && this.LockAnimator.IsPlaying("lock_guy_spit") && this.LockAnimator.CurrentFrame == 3)
        {
          this.m_lockHasSpit = true;
          tk2dSprite componentInChildren = SpawnManager.SpawnVFX(BraveResources.Load("Global VFX/VFX_Lock_Spit") as GameObject).GetComponentInChildren<tk2dSprite>();
          componentInChildren.UpdateZDepth();
          componentInChildren.PlaceAtPositionByAnchor((Vector3) this.LockAnimator.sprite.WorldCenter, tk2dBaseSprite.Anchor.UpperCenter);
        }
      }
      if (!this.northSouth || !this.isSealed)
        return;
      for (int index = 0; index < this.sealAnimators.Length; ++index)
        this.sealAnimators[index].sprite.UpdateZDepth();
      if (string.IsNullOrEmpty(this.playerNearSealedAnimationName))
        return;
      Vector2 zero1 = Vector2.zero;
      for (int index = 0; index < this.doorModules.Length; ++index)
        zero1 += this.doorModules[index].rigidbody.UnitCenter;
      if ((double) Vector2.Distance(zero1 / (float) this.doorModules.Length, GameManager.Instance.PrimaryPlayer.specRigidbody.UnitCenter) < 4.0)
      {
        for (int index = 0; index < this.sealAnimators.Length; ++index)
        {
          if (!this.sealAnimators[index].IsPlaying(this.playerNearSealedAnimationName) && !this.sealAnimators[index].IsPlaying(this.unsealAnimationName) && !this.sealAnimators[index].IsPlaying(this.sealAnimationName))
            this.sealAnimators[index].Play(this.playerNearSealedAnimationName);
        }
      }
      else
      {
        for (int index = 0; index < this.sealAnimators.Length; ++index)
        {
          if (this.sealAnimators[index].IsPlaying(this.playerNearSealedAnimationName))
          {
            this.sealAnimators[index].Stop();
            tk2dSpriteAnimationClip clipByName = this.sealAnimators[index].GetClipByName(this.sealAnimationName);
            this.sealAnimators[index].Sprite.SetSprite(clipByName.frames[clipByName.frames.Length - 1].spriteId);
          }
        }
      }
    }

    protected void AnimationDepthLerp(
      tk2dSprite targetSprite,
      float targetDepth,
      tk2dSpriteAnimationClip clip,
      DungeonDoorController.DoorModule m = null,
      bool isSpecialHorizontalTopCase = false)
    {
      float duration = 1f;
      if (clip != null)
        duration = (float) clip.frames.Length / clip.fps;
      this.StartCoroutine(this.DepthLerp(targetSprite, targetDepth, duration, m, isSpecialHorizontalTopCase));
    }

    [DebuggerHidden]
    private IEnumerator DepthLerp(
      tk2dSprite targetSprite,
      float targetDepth,
      float duration,
      DungeonDoorController.DoorModule m = null,
      bool isSpecialHorizontalTopCase = false)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DungeonDoorController__DepthLerpc__Iterator0()
      {
        m = m,
        targetSprite = targetSprite,
        duration = duration,
        targetDepth = targetDepth,
        isSpecialHorizontalTopCase = isSpecialHorizontalTopCase,
        _this = this
      };
    }

    public void OnAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
    {
      this.UpdateDoorDepths();
    }

    public void Open(bool flipped = false)
    {
      if (this.doorMode == DungeonDoorController.DungeonDoorMode.BOSS_DOOR_ONLY_UNSEALS || this.doorMode == DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR || this.doorMode == DungeonDoorController.DungeonDoorMode.ONE_WAY_DOOR_ONLY_UNSEALS || this.IsSealed || this.isLocked || this.m_isDestroyed || this.m_open)
        return;
      if (!this.hasEverBeenOpen)
      {
        RoomHandler roomHandler = (RoomHandler) null;
        if (this.exitDefinition != null)
        {
          if (this.exitDefinition.upstreamRoom != null && this.exitDefinition.upstreamRoom.WillSealOnEntry())
            roomHandler = this.exitDefinition.upstreamRoom;
          else if (this.exitDefinition.downstreamRoom != null && this.exitDefinition.downstreamRoom.WillSealOnEntry())
            roomHandler = this.exitDefinition.downstreamRoom;
        }
        if (roomHandler != null && ((bool) (UnityEngine.Object) this.subsidiaryDoor || (bool) (UnityEngine.Object) this.parentDoor))
        {
          DungeonDoorController dungeonDoorController = !(bool) (UnityEngine.Object) this.subsidiaryDoor ? this.parentDoor : this.subsidiaryDoor;
          Vector2 center = roomHandler.area.Center;
          float num = Vector2.Distance(center, (Vector2) this.transform.position);
          if ((double) Vector2.Distance(center, (Vector2) dungeonDoorController.transform.position) < (double) num)
            roomHandler = (RoomHandler) null;
        }
        if (roomHandler != null)
          BraveMemory.HandleRoomEntered(roomHandler.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All));
      }
      int num1 = (int) AkSoundEngine.PostEvent("play_OBJ_door_open_01", this.gameObject);
      this.SetState(true, flipped);
      if (!this.doorClosesAfterEveryOpen)
        return;
      this.StartCoroutine(this.DelayedReclose());
    }

    [DebuggerHidden]
    private IEnumerator DelayedReclose()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DungeonDoorController__DelayedReclosec__Iterator1()
      {
        _this = this
      };
    }

    protected virtual void OnRigidbodyCollision(CollisionData rigidbodyCollision)
    {
      this.CheckForPlayerCollision(rigidbodyCollision.OtherRigidbody, rigidbodyCollision.Normal);
      if (this.IsSealed || !this.isLocked || !(bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody)
        return;
      if ((bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody.GetComponent<KeyProjModifier>())
        this.Unlock();
      if (!((UnityEngine.Object) rigidbodyCollision.OtherRigidbody.GetComponent<KeyBullet>() != (UnityEngine.Object) null))
        return;
      GameStatsManager.Instance.RegisterStatChange(TrackedStats.DOORS_UNLOCKED_WITH_KEY_BULLETS, 1f);
      this.isLocked = false;
      bool flipped = false;
      if ((double) rigidbodyCollision.Normal.y < 0.0 && this.northSouth)
        flipped = true;
      if ((double) rigidbodyCollision.Normal.x < 0.0 && !this.northSouth)
        flipped = true;
      this.Open(flipped);
      this.m_isDestroyed = true;
    }

    private void OnEnterTrigger(
      SpeculativeRigidbody specRigidbody,
      SpeculativeRigidbody sourceSpecRigidbody,
      CollisionData collisionData)
    {
      this.CheckForPlayerCollision(specRigidbody, sourceSpecRigidbody.UnitCenter - specRigidbody.UnitCenter);
    }

    private void CheckForPlayerCollision(SpeculativeRigidbody otherRigidbody, Vector2 normal)
    {
      if (this.isSealed || this.isLocked)
        return;
      PlayerController component = otherRigidbody.GetComponent<PlayerController>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || this.m_open)
        return;
      bool flipped = false;
      if ((double) normal.y < 0.0 && this.northSouth)
        flipped = true;
      if ((double) normal.x < 0.0 && !this.northSouth)
        flipped = true;
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER)
      {
        this.Open(flipped);
        BraveInput.DoVibrationForAllPlayers(Vibration.Time.Quick, Vibration.Strength.Light);
      }
      else
      {
        bool flag = true;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          if (!GameManager.Instance.AllPlayers[index].IsGhost && (!GameManager.Instance.AllPlayers[index].healthHaver.IsDead || GameManager.Instance.AllPlayers[index].IsGhost) && (double) this.GetDistanceToPlayer(GameManager.Instance.AllPlayers[index].specRigidbody) > 0.30000001192092896)
          {
            flag = false;
            break;
          }
        }
        if (flag)
        {
          this.Open(flipped);
          BraveInput.DoVibrationForAllPlayers(Vibration.Time.Quick, Vibration.Strength.Light);
          if (this.exitDefinition == null || this.exitDefinition.downstreamRoom == null || (this.exitDefinition.upstreamRoom == null || !this.exitDefinition.upstreamRoom.WillSealOnEntry()) && (this.exitDefinition.downstreamRoom == null || !this.exitDefinition.downstreamRoom.WillSealOnEntry()))
            return;
          this.HandleCoopPlayers(flipped);
        }
        else
        {
          if (this.m_isCoopArrowing)
            return;
          this.StartCoroutine(this.DoCoopArrowWhilePlayerIsNear(component));
        }
      }
    }

    [DebuggerHidden]
    private IEnumerator DoCoopArrowWhilePlayerIsNear(PlayerController nearPlayer)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DungeonDoorController__DoCoopArrowWhilePlayerIsNearc__Iterator2()
      {
        nearPlayer = nearPlayer,
        _this = this
      };
    }

    private void HandleCoopPlayers(bool flipped)
    {
      if (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
        return;
      Vector2 zero = Vector2.zero;
      Vector2 vector2_1 = !this.northSouth ? (!flipped ? Vector2.right * 1.25f : -Vector2.right * 1.25f) : (!flipped ? Vector2.up * 2.75f : -Vector2.up * 1.25f);
      for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
      {
        if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index1] && !GameManager.Instance.AllPlayers[index1].IsGhost)
        {
          Vector2 vector2_2 = this.GetSRBAveragePosition() + vector2_1;
          float initialDelay = !this.northSouth ? 0.1f : 0.2f;
          List<SpeculativeRigidbody> passThroughRigidbodies = new List<SpeculativeRigidbody>();
          for (int index2 = 0; index2 < this.doorModules.Length; ++index2)
            passThroughRigidbodies.Add(this.doorModules[index2].sprite.specRigidbody);
          for (int index3 = 0; index3 < this.sealAnimators.Length; ++index3)
            passThroughRigidbodies.Add(this.sealAnimators[index3].sprite.specRigidbody);
          GameManager.Instance.AllPlayers[index1].ForceMoveInDirectionUntilThreshold(vector2_1.normalized, !this.northSouth ? vector2_2.x : vector2_2.y, initialDelay, passThroughRigidbodies: passThroughRigidbodies);
        }
      }
    }

    public void Close()
    {
      if (this.doorMode == DungeonDoorController.DungeonDoorMode.BOSS_DOOR_ONLY_UNSEALS || this.doorMode == DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR || this.doorMode == DungeonDoorController.DungeonDoorMode.ONE_WAY_DOOR_ONLY_UNSEALS || this.m_isDestroyed || !this.m_open)
        return;
      this.SetState(false);
    }

    [DebuggerHidden]
    private IEnumerator MoveTransformSmoothly(
      Transform target,
      Vector3 delta,
      float animationTime,
      Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> action)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DungeonDoorController__MoveTransformSmoothlyc__Iterator3()
      {
        target = target,
        delta = delta,
        animationTime = animationTime,
        action = action
      };
    }

    private void SealInternal()
    {
      this.m_wasOpenWhenSealed = this.m_open;
      if (this.m_open)
        this.Close();
      if (this.Mode == DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR)
      {
        this.sealAnimators[0].Play(this.sealAnimationName);
        this.sealAnimators[0].specRigidbody.PrimaryPixelCollider.Enabled = true;
      }
      else if (!string.IsNullOrEmpty(this.sealAnimationName))
      {
        for (int index1 = 0; index1 < this.sealAnimators.Length; ++index1)
        {
          this.sealAnimators[index1].Sprite.UpdateZDepth();
          this.sealAnimators[index1].AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>) null;
          this.sealAnimators[index1].AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnSealAnimationEvent);
          this.sealAnimators[index1].gameObject.SetActive(true);
          this.sealAnimators[index1].Play(this.sealAnimationName);
          if ((UnityEngine.Object) this.sealAnimators[index1].Sprite.specRigidbody != (UnityEngine.Object) null)
          {
            this.sealAnimators[index1].Sprite.specRigidbody.enabled = true;
            this.sealAnimators[index1].Sprite.specRigidbody.Initialize();
            for (int index2 = 0; index2 < GameManager.Instance.AllPlayers.Length; ++index2)
              this.sealAnimators[index1].Sprite.specRigidbody.RegisterGhostCollisionException(GameManager.Instance.AllPlayers[index2].specRigidbody);
          }
        }
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_gate_slam_01", this.gameObject);
        for (int index = 0; index < this.sealChainAnimators.Length; ++index)
        {
          if (this.sealChainAnimators[index].GetClipByName(this.sealAnimationName + "_chain") != null)
          {
            this.sealChainAnimators[index].AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>) null;
            this.sealChainAnimators[index].gameObject.SetActive(true);
            this.sealChainAnimators[index].Play(this.sealAnimationName + "_chain");
          }
        }
      }
      else if (this.Mode != DungeonDoorController.DungeonDoorMode.ONE_WAY_DOOR_ONLY_UNSEALS && this.Mode == DungeonDoorController.DungeonDoorMode.BOSS_DOOR_ONLY_UNSEALS)
      {
        for (int index = 0; index < this.sealAnimators.Length; ++index)
          this.StartCoroutine(this.MoveTransformSmoothly(this.sealAnimators[index].transform, new Vector3(0.0f, -25f / 16f, 0.0f), 1.5f, (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>) null));
      }
      if (this.doorMode == DungeonDoorController.DungeonDoorMode.SINGLE_DOOR)
      {
        for (int index = 0; index < this.doorModules.Length; ++index)
          this.doorModules[index].rigidbody.enabled = true;
      }
      for (int index = 0; index < this.sealAnimators.Length; ++index)
      {
        tk2dSpriteAnimator sealAnimator = this.sealAnimators[index];
        if ((UnityEngine.Object) sealAnimator.GetComponent<SpeculativeRigidbody>() != (UnityEngine.Object) null)
          sealAnimator.GetComponent<SpeculativeRigidbody>().enabled = true;
      }
      this.isSealed = true;
    }

    private GameObject SpawnVFXAtPoint(GameObject vfx, Vector3 position)
    {
      GameObject gameObject = SpawnManager.SpawnVFX(vfx, position, Quaternion.identity, true);
      tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
      component.HeightOffGround = 0.25f;
      component.PlaceAtPositionByAnchor(position, tk2dBaseSprite.Anchor.MiddleCenter);
      component.IsPerpendicular = false;
      component.UpdateZDepth();
      return gameObject;
    }

    [DebuggerHidden]
    private IEnumerator DelayedLayerChange(GameObject targetObject, string layer, float delay)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DungeonDoorController__DelayedLayerChangec__Iterator4()
      {
        delay = delay,
        layer = layer,
        targetObject = targetObject
      };
    }

    private void UnsealInternal()
    {
      if (this.Mode == DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR)
      {
        if (!this.IsSealed)
          return;
        this.sealAnimators[0].Play(this.unsealAnimationName);
        this.sealVFX[0].gameObject.SetActive(true);
        this.sealVFX[0].PlayAndDisableObject(string.Empty);
      }
      else if (!string.IsNullOrEmpty(this.unsealAnimationName))
      {
        for (int index = 0; index < this.sealAnimators.Length; ++index)
        {
          this.sealAnimators[index].Sprite.UpdateZDepth();
          this.sealAnimators[index].Play(this.unsealAnimationName);
          this.sealAnimators[index].AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnUnsealAnimationCompleted);
          this.sealAnimators[index].AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>) null;
          if ((UnityEngine.Object) this.sealAnimators[index].Sprite.specRigidbody != (UnityEngine.Object) null)
            this.sealAnimators[index].Sprite.specRigidbody.enabled = false;
        }
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_gate_open_01", this.gameObject);
        for (int index = 0; index < this.sealChainAnimators.Length; ++index)
        {
          if (this.sealChainAnimators[index].GetClipByName(this.unsealAnimationName + "_chain") != null)
          {
            this.sealChainAnimators[index].Play(this.unsealAnimationName + "_chain");
            this.sealChainAnimators[index].AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnUnsealAnimationCompleted);
            this.sealChainAnimators[index].AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>) null;
          }
        }
      }
      else if (this.Mode == DungeonDoorController.DungeonDoorMode.ONE_WAY_DOOR_ONLY_UNSEALS)
      {
        if (this.northSouth)
        {
          for (int index = 0; index < this.sealAnimators.Length; ++index)
          {
            this.sealAnimators[index].gameObject.layer = LayerMask.NameToLayer("BG_Critical");
            this.StartCoroutine(this.MoveTransformSmoothly(this.sealAnimators[index].transform, new Vector3(0.0f, -2f, -2.25f), 1.5f, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnUnsealAnimationCompleted)));
          }
          this.StartCoroutine(this.DelayedLayerChange(this.sealAnimators[0].transform.GetChild(0).gameObject, "BG_Critical", 1.5f));
          if ((UnityEngine.Object) GameManager.Instance.Dungeon.SecretRoomHorizontalPoofVFX != (UnityEngine.Object) null)
            this.SpawnVFXAtPoint(GameManager.Instance.Dungeon.SecretRoomHorizontalPoofVFX, this.transform.position + new Vector3(0.0f, -0.25f, 0.0f)).GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject(string.Empty);
        }
        else
        {
          for (int index = 0; index < this.sealAnimators.Length; ++index)
          {
            this.sealAnimators[index].gameObject.layer = LayerMask.NameToLayer("BG_Critical");
            this.StartCoroutine(this.MoveTransformSmoothly(this.sealAnimators[index].transform, new Vector3(0.0f, -2.5f, -3.25f), 1.5f, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnUnsealAnimationCompleted)));
          }
          this.StartCoroutine(this.DelayedLayerChange(this.sealAnimators[0].transform.GetChild(0).gameObject, "BG_Critical", 1.35f));
          if ((UnityEngine.Object) GameManager.Instance.Dungeon.SecretRoomVerticalPoofVFX != (UnityEngine.Object) null)
          {
            GameObject gameObject = this.SpawnVFXAtPoint(GameManager.Instance.Dungeon.SecretRoomVerticalPoofVFX, this.transform.position + Vector3.up);
            gameObject.transform.position = this.transform.position + new Vector3(-1.25f, 0.75f, 0.0f);
            gameObject.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject(string.Empty);
          }
        }
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_secret_door_01", this.gameObject);
      }
      else if (this.Mode == DungeonDoorController.DungeonDoorMode.BOSS_DOOR_ONLY_UNSEALS)
      {
        for (int index = 0; index < this.sealAnimators.Length; ++index)
          this.StartCoroutine(this.MoveTransformSmoothly(this.sealAnimators[index].transform, new Vector3(0.0f, 25f / 16f, 0.0f), 1.5f, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnUnsealAnimationCompleted)));
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_bossdoor_open_01", this.gameObject);
      }
      if (this.doorMode == DungeonDoorController.DungeonDoorMode.SINGLE_DOOR && this.m_open && !this.isLocked)
      {
        for (int index = 0; index < this.doorModules.Length; ++index)
          this.doorModules[index].rigidbody.enabled = false;
      }
      if (this.usesUnsealScreenShake)
        GameManager.Instance.MainCameraController.DoScreenShake(this.unsealScreenShake, new Vector2?((Vector2) this.transform.position));
      this.isSealed = false;
      if (!this.m_wasOpenWhenSealed)
        return;
      this.Open();
    }

    public void OnSealAnimationEvent(
      tk2dSpriteAnimator animator,
      tk2dSpriteAnimationClip clip,
      int frameNo)
    {
      if (!(clip.GetFrame(frameNo).eventInfo == "SealVFX") || this.sealVFX.Length <= 0)
        return;
      for (int index = 0; index < this.sealVFX.Length; ++index)
      {
        this.sealVFX[index].gameObject.SetActive(true);
        this.sealVFX[index].Play();
      }
      animator.Sprite.UpdateZDepth();
    }

    [DebuggerHidden]
    private IEnumerator HandleFrameDelayedUnsealedVFXOverride()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DungeonDoorController__HandleFrameDelayedUnsealedVFXOverridec__Iterator5()
      {
        _this = this
      };
    }

    public void OnUnsealAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
    {
      if (this.hideSealAnimators)
        a.gameObject.SetActive(false);
      if ((UnityEngine.Object) a.GetComponent<SpeculativeRigidbody>() != (UnityEngine.Object) null)
        a.GetComponent<SpeculativeRigidbody>().enabled = false;
      if (!((UnityEngine.Object) this.unsealedVFXOverride != (UnityEngine.Object) null))
        return;
      this.StartCoroutine(this.HandleFrameDelayedUnsealedVFXOverride());
    }

    public void OnCloseAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
    {
      a.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnCloseAnimationCompleted);
      if (!a.Sprite.FlipX)
        return;
      a.Sprite.FlipX = false;
    }

    private void SetState(bool openState, bool flipped = false)
    {
      if (openState)
        this.hasEverBeenOpen = true;
      this.TriggerPersistentVFXClear();
      this.m_open = openState;
      if (!this.northSouth)
      {
        for (int index = 0; index < this.doorModules.Length; ++index)
        {
          if (this.doorModules[index].horizontalFlips)
            this.doorModules[index].sprite.FlipX = !openState ? this.m_openIsFlipped : flipped;
        }
      }
      if (openState)
      {
        for (int index1 = 0; index1 < this.doorModules.Length; ++index1)
        {
          this.m_openIsFlipped = flipped;
          DungeonDoorController.DoorModule doorModule = this.doorModules[index1];
          string name = doorModule.openAnimationName;
          tk2dSpriteAnimationClip clip = (tk2dSpriteAnimationClip) null;
          if (!string.IsNullOrEmpty(name))
          {
            if (flipped && this.northSouth)
              name = name.Replace("_north", "_south");
            clip = doorModule.animator.GetClipByName(name);
          }
          if (clip != null)
            doorModule.animator.Play(clip);
          for (int index2 = 0; index2 < doorModule.AOAnimatorsToDisable.Count; ++index2)
            doorModule.AOAnimatorsToDisable[index2].PlayAndDisableObject(string.Empty);
          doorModule.rigidbody.enabled = false;
          this.AnimationDepthLerp(doorModule.sprite, doorModule.openDepth, clip, doorModule, !this.northSouth && index1 == 0);
        }
      }
      else
      {
        for (int index3 = 0; index3 < this.doorModules.Length; ++index3)
        {
          DungeonDoorController.DoorModule doorModule = this.doorModules[index3];
          string name = doorModule.closeAnimationName;
          tk2dSpriteAnimationClip clip = (tk2dSpriteAnimationClip) null;
          if (!string.IsNullOrEmpty(name))
          {
            if (this.m_openIsFlipped && this.northSouth)
              name = name.Replace("_north", "_south");
            clip = doorModule.animator.GetClipByName(name);
          }
          if (clip != null)
          {
            doorModule.animator.Play(clip);
            doorModule.animator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnCloseAnimationCompleted);
          }
          else
            doorModule.animator.StopAndResetFrame();
          for (int index4 = 0; index4 < doorModule.AOAnimatorsToDisable.Count; ++index4)
          {
            doorModule.AOAnimatorsToDisable[index4].gameObject.SetActive(true);
            doorModule.AOAnimatorsToDisable[index4].StopAndResetFrame();
          }
          doorModule.rigidbody.enabled = true;
          this.AnimationDepthLerp(doorModule.sprite, doorModule.closedDepth, clip, doorModule);
        }
      }
      IntVector2 startingPosition = this.transform.position.IntXY(VectorConversions.Floor);
      if (this.upstreamRoom != null && this.upstreamRoom.visibility != RoomHandler.VisibilityStatus.OBSCURED)
        Pixelator.Instance.ProcessRoomAdditionalExits(startingPosition, this.upstreamRoom, false);
      if (this.downstreamRoom == null || this.downstreamRoom.visibility == RoomHandler.VisibilityStatus.OBSCURED)
        return;
      Pixelator.Instance.ProcessRoomAdditionalExits(startingPosition, this.downstreamRoom, false);
    }

    public float GetDistanceToPlayer(SpeculativeRigidbody playerRigidbody)
    {
      Vector4 srbBoundingBox = this.GetSRBBoundingBox();
      return BraveMathCollege.DistBetweenRectangles(new Vector2(srbBoundingBox.x, srbBoundingBox.y), new Vector2(srbBoundingBox.z - srbBoundingBox.x, srbBoundingBox.w - srbBoundingBox.y), playerRigidbody.UnitBottomLeft, playerRigidbody.UnitDimensions);
    }

    protected override void OnDestroy() => base.OnDestroy();

    private Vector2 GetModuleAveragePosition()
    {
      Vector2 zero = Vector2.zero;
      for (int index = 0; index < this.doorModules.Length; ++index)
        zero += this.doorModules[index].animator.Sprite.WorldCenter;
      return zero / (float) this.doorModules.Length;
    }

    private Vector4 GetSRBBoundingBox()
    {
      Vector2 lhs1 = new Vector2(float.MaxValue, float.MaxValue);
      Vector2 lhs2 = new Vector2(float.MinValue, float.MinValue);
      for (int index = 0; index < this.doorModules.Length; ++index)
      {
        if ((UnityEngine.Object) this.doorModules[index].rigidbody != (UnityEngine.Object) null)
        {
          lhs1 = Vector2.Min(lhs1, this.doorModules[index].rigidbody.UnitBottomLeft);
          lhs2 = Vector2.Max(lhs2, this.doorModules[index].rigidbody.UnitTopRight);
        }
      }
      return new Vector4(lhs1.x, lhs1.y, lhs2.x, lhs2.y);
    }

    private Vector2 GetSRBAveragePosition()
    {
      Vector2 zero = Vector2.zero;
      for (int index = 0; index < this.doorModules.Length; ++index)
      {
        zero += this.doorModules[index].animator.GetComponent<SpeculativeRigidbody>().UnitCenter;
        if (!this.northSouth)
          zero += new Vector2(0.0f, -0.375f);
      }
      return zero / (float) this.doorModules.Length;
    }

    public float GetDistanceToPoint(Vector2 point)
    {
      return !this.isLocked || this.lockIsBusted ? 1000f : Vector2.Distance(point, this.GetModuleAveragePosition()) / 3f;
    }

    public void OnEnteredRange(PlayerController interactor)
    {
      if (!(bool) (UnityEngine.Object) this || this.isLocked && interactor.carriedConsumables.KeyBullets <= 0 && !interactor.carriedConsumables.InfiniteKeys || this.IsSealed)
        return;
      SpriteOutlineManager.AddOutlineToSprite(this.LockAnimator.Sprite, Color.white, 0.05f);
      this.LockAnimator.sprite.UpdateZDepth();
    }

    public void OnExitRange(PlayerController interactor)
    {
      if (!(bool) (UnityEngine.Object) this || this.IsSealed)
        return;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.LockAnimator.Sprite);
    }

    public void FlipLockToOtherSide()
    {
      this.LockAnimator.Sprite.FlipX = true;
      this.LockAnimator.Sprite.transform.position += new Vector3(-0.375f, 0.0f, 0.0f);
      this.ChainsAnimator.Sprite.FlipX = true;
      if (!((UnityEngine.Object) this.ChainsAnimator.transform.parent != (UnityEngine.Object) this.LockAnimator.transform))
        return;
      this.ChainsAnimator.Sprite.transform.position += new Vector3(-0.375f, 0.0f, 0.0f);
    }

    public void Unlock()
    {
      if (!this.isLocked)
        return;
      this.isLocked = false;
      if ((UnityEngine.Object) this.LockAnimator != (UnityEngine.Object) null)
      {
        if (this.northSouth)
        {
          this.LockAnimator.PlayAndDestroyObject("look_guy_unlock");
          this.ChainsAnimator.PlayAndDestroyObject("lock_guy_chain_north_unlock");
        }
        else
        {
          this.LockAnimator.PlayAndDestroyObject("lock_guy_side_unlock");
          this.ChainsAnimator.PlayAndDestroyObject("lock_guy_chain_side_unlock");
        }
      }
      RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) this);
    }

    public void BreakLock()
    {
      if (!this.isLocked || this.lockIsBusted)
        return;
      this.lockIsBusted = true;
    }

    public void Interact(PlayerController interactor)
    {
      if (this.IsSealed || this.lockIsBusted || !this.isLocked)
        return;
      if (interactor.carriedConsumables.KeyBullets <= 0 && !interactor.carriedConsumables.InfiniteKeys)
      {
        if (!this.northSouth || !((UnityEngine.Object) this.LockAnimator != (UnityEngine.Object) null))
          return;
        this.LockAnimator.Play("lock_guy_laugh");
        this.m_lockHasLaughed = true;
        this.m_lockHasSpit = false;
      }
      else
      {
        if (!interactor.carriedConsumables.InfiniteKeys)
          --interactor.carriedConsumables.KeyBullets;
        this.Unlock();
      }
    }

    public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
    {
      shouldBeFlipped = false;
      return string.Empty;
    }

    public float GetOverrideMaxDistance() => -1f;

    public enum DungeonDoorMode
    {
      COMPLEX,
      BOSS_DOOR_ONLY_UNSEALS,
      SINGLE_DOOR,
      ONE_WAY_DOOR_ONLY_UNSEALS,
      FINAL_BOSS_DOOR,
    }

    [Serializable]
    public class DoorModule
    {
      public tk2dSpriteAnimator animator;
      public float openDepth;
      public float closedDepth;
      public bool openPerpendicular = true;
      public bool horizontalFlips = true;
      public string openAnimationName;
      public string closeAnimationName;
      public List<tk2dSpriteAnimator> AOAnimatorsToDisable;
      [HideInInspector]
      public tk2dSprite sprite;
      [HideInInspector]
      public SpeculativeRigidbody rigidbody;
      [HideInInspector]
      [NonSerialized]
      public bool isLerping;
    }
  }

