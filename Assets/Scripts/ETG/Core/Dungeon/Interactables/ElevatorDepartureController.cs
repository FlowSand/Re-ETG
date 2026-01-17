// Decompiled with JetBrains decompiler
// Type: ElevatorDepartureController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class ElevatorDepartureController : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
      public tk2dSpriteAnimator elevatorAnimator;
      public tk2dSpriteAnimator ceilingAnimator;
      public tk2dSpriteAnimator facewallAnimator;
      public tk2dSpriteAnimator floorAnimator;
      public tk2dSprite[] priorSprites;
      public tk2dSprite[] postSprites;
      public BreakableChunk chunker;
      public Transform spawnTransform;
      public GameObject elevatorFloor;
      public tk2dSpriteAnimator crumblyBumblyAnimator;
      public tk2dSpriteAnimator smokeAnimator;
      public string elevatorDescendAnimName;
      public string elevatorOpenAnimName;
      public string elevatorCloseAnimName;
      public string elevatorDepartAnimName;
      public ScreenShakeSettings arrivalShake;
      public ScreenShakeSettings doorOpenShake;
      public ScreenShakeSettings doorCloseShake;
      public ScreenShakeSettings departureShake;
      public bool ReturnToFoyerWithNewInstance;
      public bool UsesOverrideTargetFloor;
      public GlobalDungeonData.ValidTilesets OverrideTargetFloor;
      private Tribool m_isArrived = Tribool.Unready;
      private Tribool m_isCryoArrived = Tribool.Unready;
      private TalkDoerLite m_cryoButton;
      private FsmBool m_cryoBool;
      private FsmBool m_normalBool;
      public const bool c_savingEnabled = true;
      private tk2dSpriteAnimator m_activeCryoElevatorAnimator;
      private bool m_depatureIsPlayerless;
      private bool m_hasEverArrived;

      public void ConfigureOnPlacement(RoomHandler room)
      {
        IntVector2 intVector2_1 = this.transform.position.IntXY(VectorConversions.Floor);
        for (int index1 = 0; index1 < 6; ++index1)
        {
          for (int index2 = -2; index2 < 6; ++index2)
          {
            CellData cellData = GameManager.Instance.Dungeon.data.cellData[intVector2_1.x + index1][intVector2_1.y + index2];
            cellData.cellVisualData.precludeAllTileDrawing = true;
            if (index2 < 4)
            {
              cellData.type = CellType.PIT;
              cellData.fallingPrevented = true;
            }
            cellData.isOccupied = true;
          }
        }
        if (GameManager.Instance.CurrentGameMode != GameManager.GameMode.NORMAL && GameManager.Instance.CurrentGameMode != GameManager.GameMode.SHORTCUT || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.TUTORIAL)
          return;
        GameObject gameObject = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Global Prefabs/CryoElevatorButton"), this.transform.position + new Vector3(-1f, 0.0f, 0.0f), Quaternion.identity);
        IntVector2 intVector2_2 = this.transform.position.IntXY(VectorConversions.Floor) + new IntVector2(-2, 0);
        for (int x = 0; x < 2; ++x)
        {
          for (int y = -1; y < 2; ++y)
          {
            if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2_2 + new IntVector2(x, y)))
            {
              CellData cellData = GameManager.Instance.Dungeon.data[intVector2_2 + new IntVector2(x, y)];
              cellData.cellVisualData.containsWallSpaceStamp = true;
              cellData.cellVisualData.containsObjectSpaceStamp = true;
            }
          }
        }
        this.m_cryoButton = gameObject.GetComponentInChildren<TalkDoerLite>();
        room.RegisterInteractable((IPlayerInteractable) this.m_cryoButton);
        this.m_cryoButton.OnGenericFSMActionA += new System.Action(this.SwitchToCryoElevator);
        this.m_cryoButton.OnGenericFSMActionB += new System.Action(this.RescindCryoElevator);
        this.m_cryoBool = this.m_cryoButton.playmakerFsm.FsmVariables.GetFsmBool("IS_CRYO");
        this.m_normalBool = this.m_cryoButton.playmakerFsm.FsmVariables.GetFsmBool("IS_NORMAL");
      }

      private void ToggleSprites(bool prior)
      {
        for (int index = 0; index < this.priorSprites.Length; ++index)
        {
          if ((bool) (UnityEngine.Object) this.priorSprites[index] && (bool) (UnityEngine.Object) this.priorSprites[index].renderer)
            this.priorSprites[index].renderer.enabled = prior;
        }
        for (int index = 0; index < this.postSprites.Length; ++index)
        {
          if ((bool) (UnityEngine.Object) this.postSprites[index] && (bool) (UnityEngine.Object) this.postSprites[index].renderer)
            this.postSprites[index].renderer.enabled = !prior;
        }
      }

      public void SwitchToCryoElevator()
      {
        if (this.m_isArrived != Tribool.Ready)
          return;
        this.DoPlayerlessDeparture();
        GameManager.Instance.Dungeon.StartCoroutine(this.CryoWaitForPreviousElevatorDeparture());
      }

      [DebuggerHidden]
      private IEnumerator CryoWaitForPreviousElevatorDeparture()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ElevatorDepartureController.\u003CCryoWaitForPreviousElevatorDeparture\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private void SetFSMStates()
      {
        if (!(bool) (UnityEngine.Object) this.m_cryoButton)
          return;
        this.m_cryoBool.Value = this.m_isCryoArrived == Tribool.Ready;
        this.m_normalBool.Value = this.m_isArrived == Tribool.Ready;
      }

      public void RescindCryoElevator()
      {
        if (this.m_isCryoArrived != Tribool.Ready)
          return;
        this.DoCryoDeparture();
      }

      public void DoCryoDeparture(bool playerless = true)
      {
        if ((UnityEngine.Object) this.m_activeCryoElevatorAnimator == (UnityEngine.Object) null || this.m_isCryoArrived != Tribool.Ready)
          return;
        this.m_isCryoArrived = Tribool.Complete;
        if (!playerless)
        {
          if ((bool) (UnityEngine.Object) Minimap.Instance)
            Minimap.Instance.PreventAllTeleports = true;
          if (GameManager.HasInstance && GameManager.Instance.AllPlayers != null)
          {
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
              if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index])
                GameManager.Instance.AllPlayers[index].CurrentInputState = PlayerInputState.NoInput;
            }
          }
        }
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleCryoDeparture(playerless));
      }

      [DebuggerHidden]
      public IEnumerator HandleCryoDeparture(bool playerless)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ElevatorDepartureController.\u003CHandleCryoDeparture\u003Ec__Iterator1()
        {
          playerless = playerless,
          \u0024this = this
        };
      }

      private void TransitionToDoorOpen(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
      {
        animator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.TransitionToDoorOpen);
        this.elevatorFloor.SetActive(true);
        this.elevatorFloor.GetComponent<MeshRenderer>().enabled = true;
        this.smokeAnimator.gameObject.SetActive(true);
        this.smokeAnimator.PlayAndDisableObject(string.Empty);
        GameManager.Instance.MainCameraController.DoScreenShake(this.doorOpenShake, new Vector2?());
        animator.Play(this.elevatorOpenAnimName);
      }

      private void TransitionToDoorClose(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
      {
        GameManager.Instance.MainCameraController.DoScreenShake(this.doorCloseShake, new Vector2?());
        animator.Play(this.elevatorCloseAnimName);
        animator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.TransitionToDepart);
      }

      private void TransitionToDepart(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
      {
        GameManager.Instance.MainCameraController.DoDelayedScreenShake(this.departureShake, 0.25f, new Vector2?());
        if (!this.m_depatureIsPlayerless)
        {
          for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            GameManager.Instance.AllPlayers[index].PrepareForSceneTransition();
          Pixelator.Instance.FadeToBlack(0.5f);
          GameUIRoot.Instance.HideCoreUI(string.Empty);
          GameUIRoot.Instance.ToggleLowerPanels(false, source: string.Empty);
          float delay = 0.5f;
          if (this.ReturnToFoyerWithNewInstance)
            GameManager.Instance.DelayedReturnToFoyer(delay);
          else if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH)
            GameManager.Instance.DelayedLoadBossrushFloor(delay);
          else if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH)
          {
            GameManager.Instance.DelayedLoadBossrushFloor(delay);
          }
          else
          {
            if (!GameManager.Instance.IsFoyer && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.NONE)
              GameManager.DoMidgameSave(GameManager.Instance.GetNextTileset(GameManager.Instance.Dungeon.tileIndices.tilesetId));
            if (this.UsesOverrideTargetFloor)
            {
              switch (this.OverrideTargetFloor)
              {
                case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
                  GameManager.Instance.DelayedLoadCustomLevel(delay, "tt_catacombs");
                  break;
                case GlobalDungeonData.ValidTilesets.FORGEGEON:
                  GameManager.Instance.DelayedLoadCustomLevel(delay, "tt_forge");
                  break;
              }
            }
            else
              GameManager.Instance.DelayedLoadNextLevel(delay);
            int num = (int) AkSoundEngine.PostEvent("Stop_MUS_All", this.gameObject);
          }
        }
        this.elevatorFloor.SetActive(false);
        animator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.TransitionToDepart);
        animator.PlayAndDisableObject(this.elevatorDepartAnimName);
      }

      private void DeflagCells()
      {
        IntVector2 intVector2 = this.transform.position.IntXY(VectorConversions.Floor);
        for (int index1 = 0; index1 < 6; ++index1)
        {
          for (int index2 = -2; index2 < 6; ++index2)
          {
            if ((index2 != -2 || index1 >= 2 && index1 <= 3) && (index2 != -1 || index1 >= 1 && index1 <= 4))
            {
              CellData cellData = GameManager.Instance.Dungeon.data.cellData[intVector2.x + index1][intVector2.y + index2];
              if (index2 < 4)
                cellData.fallingPrevented = false;
            }
          }
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleDepartMotion()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ElevatorDepartureController.\u003CHandleDepartMotion\u003Ec__Iterator2()
        {
          \u0024this = this
        };
      }

      private void Start()
      {
        Material material1 = UnityEngine.Object.Instantiate<Material>(this.priorSprites[1].renderer.material);
        material1.shader = ShaderCache.Acquire("Brave/Unity Transparent Cutout");
        this.priorSprites[1].renderer.material = material1;
        Material material2 = UnityEngine.Object.Instantiate<Material>(this.postSprites[2].renderer.material);
        material2.shader = ShaderCache.Acquire("Brave/Unity Transparent Cutout");
        this.postSprites[2].renderer.material = material2;
        this.postSprites[1].HeightOffGround -= 1f / 16f;
        this.postSprites[3].HeightOffGround -= 1f / 16f;
        this.postSprites[1].UpdateZDepth();
        SpeculativeRigidbody component = this.elevatorFloor.GetComponent<SpeculativeRigidbody>();
        if ((bool) (UnityEngine.Object) component)
        {
          component.PrimaryPixelCollider.ManualOffsetY -= 8;
          component.PrimaryPixelCollider.ManualHeight += 8;
          component.Reinitialize();
          component.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnElevatorTriggerEnter);
        }
        this.ToggleSprites(true);
      }

      private void OnElevatorTriggerEnter(
        SpeculativeRigidbody otherSpecRigidbody,
        SpeculativeRigidbody sourceSpecRigidbody,
        CollisionData collisionData)
      {
        if (this.m_isArrived == Tribool.Ready)
        {
          if (!((UnityEngine.Object) otherSpecRigidbody.GetComponent<PlayerController>() != (UnityEngine.Object) null))
            return;
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          {
            bool flag = true;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
              if (!GameManager.Instance.AllPlayers[index].healthHaver.IsDead && !sourceSpecRigidbody.ContainsPoint(GameManager.Instance.AllPlayers[index].SpriteBottomCenter.XY(), collideWithTriggers: true))
              {
                flag = false;
                break;
              }
            }
            if (!flag)
              return;
            this.DoDeparture();
          }
          else
            this.DoDeparture();
        }
        else
        {
          if (!(this.m_isCryoArrived == Tribool.Ready) || !((UnityEngine.Object) this.m_activeCryoElevatorAnimator != (UnityEngine.Object) null) || !((UnityEngine.Object) otherSpecRigidbody.GetComponent<PlayerController>() != (UnityEngine.Object) null))
            return;
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          {
            bool flag = true;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
              if (!GameManager.Instance.AllPlayers[index].healthHaver.IsDead && !sourceSpecRigidbody.ContainsPoint(GameManager.Instance.AllPlayers[index].SpriteBottomCenter.XY(), collideWithTriggers: true))
              {
                flag = false;
                break;
              }
            }
            if (!flag)
              return;
            this.DoCryoDeparture(false);
          }
          else
            this.DoCryoDeparture(false);
        }
      }

      private void Update()
      {
        PlayerController playerClosestToPoint = GameManager.Instance.GetActivePlayerClosestToPoint(this.spawnTransform.position.XY(), true);
        if ((UnityEngine.Object) playerClosestToPoint != (UnityEngine.Object) null && this.m_isArrived == Tribool.Unready && (double) Vector2.Distance(this.spawnTransform.position.XY(), playerClosestToPoint.CenterPosition) < 8.0)
          this.DoArrival();
        if (this.m_cryoBool == null || this.m_normalBool == null)
          return;
        this.SetFSMStates();
      }

      public void DoPlayerlessDeparture()
      {
        this.m_depatureIsPlayerless = true;
        this.m_isArrived = Tribool.Complete;
        this.TransitionToDoorClose(this.elevatorAnimator, this.elevatorAnimator.CurrentClip);
      }

      public void DoDeparture()
      {
        this.m_depatureIsPlayerless = false;
        this.m_isArrived = Tribool.Complete;
        if ((bool) (UnityEngine.Object) Minimap.Instance)
          Minimap.Instance.PreventAllTeleports = true;
        if (GameManager.HasInstance && GameManager.Instance.AllPlayers != null)
        {
          for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          {
            if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index])
              GameManager.Instance.AllPlayers[index].CurrentInputState = PlayerInputState.NoInput;
          }
        }
        this.TransitionToDoorClose(this.elevatorAnimator, this.elevatorAnimator.CurrentClip);
      }

      public void DoArrival()
      {
        this.m_isArrived = Tribool.Ready;
        this.m_hasEverArrived = true;
        this.StartCoroutine(this.HandleArrival(0.0f));
      }

      [DebuggerHidden]
      private IEnumerator HandleArrival(float initialDelay)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ElevatorDepartureController.\u003CHandleArrival\u003Ec__Iterator3()
        {
          initialDelay = initialDelay,
          \u0024this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
