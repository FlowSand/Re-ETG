// Decompiled with JetBrains decompiler
// Type: ElevatorArrivalController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class ElevatorArrivalController : DungeonPlaceableBehaviour, IPlaceConfigurable
{
  public tk2dSpriteAnimator elevatorAnimator;
  public tk2dSpriteAnimator floorAnimator;
  public SpeculativeRigidbody elevatorCollider;
  public tk2dSprite[] priorSprites;
  public tk2dSprite[] postSprites;
  public BreakableChunk chunker;
  public List<GameObject> poofObjects;
  public Transform spawnTransform;
  public GameObject elevatorFloor;
  public tk2dSpriteAnimator crumblyBumblyAnimator;
  public tk2dSpriteAnimator smokeAnimator;
  [CheckAnimation("elevatorAnimator")]
  public string elevatorDescendAnimName;
  [CheckAnimation("elevatorAnimator")]
  public string elevatorOpenAnimName;
  [CheckAnimation("elevatorAnimator")]
  public string elevatorCloseAnimName;
  [CheckAnimation("elevatorAnimator")]
  public string elevatorDepartAnimName;
  public ScreenShakeSettings arrivalShake;
  public ScreenShakeSettings doorOpenShake;
  public ScreenShakeSettings doorCloseShake;
  public ScreenShakeSettings departureShake;
  private bool m_isArrived;

  public void ConfigureOnPlacement(RoomHandler room)
  {
    IntVector2 intVector2 = this.transform.position.IntXY(VectorConversions.Floor);
    for (int index1 = 0; index1 < 6; ++index1)
    {
      for (int index2 = -2; index2 < 6; ++index2)
      {
        CellData cellData = GameManager.Instance.Dungeon.data.cellData[intVector2.x + index1][intVector2.y + index2];
        cellData.cellVisualData.precludeAllTileDrawing = true;
        if (index2 < 4)
        {
          cellData.type = CellType.PIT;
          cellData.fallingPrevented = true;
        }
      }
    }
    for (int index3 = 0; index3 < 6; ++index3)
    {
      for (int index4 = -2; index4 < 8; ++index4)
      {
        CellData cellData = GameManager.Instance.Dungeon.data.cellData[intVector2.x + index3][intVector2.y + index4];
        cellData.cellVisualData.containsObjectSpaceStamp = true;
        cellData.cellVisualData.containsWallSpaceStamp = true;
      }
    }
    if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_READY_FOR_UNLOCKS))
      return;
    bool flag = false;
    switch (GameManager.Instance.Dungeon.tileIndices.tilesetId)
    {
      case GlobalDungeonData.ValidTilesets.GUNGEON:
        if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK1_COMPLETE))
        {
          flag = true;
          break;
        }
        break;
      case GlobalDungeonData.ValidTilesets.MINEGEON:
        if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK2_COMPLETE) && GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK1_COMPLETE))
        {
          flag = true;
          break;
        }
        break;
      case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
        if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK3_COMPLETE) && GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK2_COMPLETE))
        {
          flag = true;
          break;
        }
        break;
      case GlobalDungeonData.ValidTilesets.FORGEGEON:
        if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK4_COMPLETE) && GameStatsManager.Instance.GetFlag(GungeonFlags.SHERPA_UNLOCK3_COMPLETE))
        {
          flag = true;
          break;
        }
        break;
    }
    if (!flag)
      return;
    GameObject original = ResourceCache.Acquire("Global Prefabs/ElevatorMaintenanceSign") as GameObject;
    UnityEngine.Object.Instantiate<GameObject>(original, this.transform.position + original.transform.position, Quaternion.identity);
  }

  private void TransitionToDoorOpen(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
  {
    animator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.TransitionToDoorOpen);
    this.elevatorFloor.SetActive(true);
    this.smokeAnimator.gameObject.SetActive(true);
    this.smokeAnimator.PlayAndDisableObject(string.Empty);
    GameManager.Instance.MainCameraController.DoScreenShake(this.doorOpenShake, new Vector2?());
    animator.Play(this.elevatorOpenAnimName);
    animator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnDoorOpened);
  }

  private void OnDoorOpened(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
  {
    animator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnDoorOpened);
    if ((bool) (UnityEngine.Object) animator.specRigidbody)
      PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(animator.specRigidbody);
    if (!(bool) (UnityEngine.Object) this.elevatorCollider)
      return;
    PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.elevatorCollider);
  }

  private void TransitionToDoorClose(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
  {
    GameManager.Instance.MainCameraController.DoScreenShake(this.doorCloseShake, new Vector2?());
    animator.Play(this.elevatorCloseAnimName);
    animator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.TransitionToDepart);
  }

  private void TransitionToDepart(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
  {
    this.elevatorFloor.SetActive(false);
    GameManager.Instance.MainCameraController.DoDelayedScreenShake(this.departureShake, 0.25f, new Vector2?(animator.sprite.WorldCenter));
    animator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.TransitionToDepart);
    animator.PlayAndDisableObject(this.elevatorDepartAnimName);
    this.StartCoroutine(this.HandleDepartBumbly());
    if (!(bool) (UnityEngine.Object) this.elevatorCollider)
      return;
    this.elevatorCollider.enabled = false;
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
  private IEnumerator HandleDepartBumbly()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ElevatorArrivalController.\u003CHandleDepartBumbly\u003Ec__Iterator0()
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
    this.postSprites[3].renderer.material = material2;
    this.postSprites[1].HeightOffGround -= 1f / 16f;
    this.postSprites[3].HeightOffGround -= 1f / 16f;
    this.postSprites[1].UpdateZDepth();
    this.ToggleSprites(true);
  }

  private void Update()
  {
    if (!this.m_isArrived)
      return;
    bool flag = true;
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      if ((double) Vector2.Distance(this.spawnTransform.position.XY(), GameManager.Instance.AllPlayers[index].CenterPosition) < 6.0)
      {
        flag = false;
        break;
      }
    }
    if (!flag)
      return;
    this.DoDeparture();
  }

  public void DoDeparture()
  {
    this.m_isArrived = false;
    this.TransitionToDoorClose(this.elevatorAnimator, this.elevatorAnimator.CurrentClip);
    this.DeflagCells();
  }

  public void DoArrival(PlayerController player, float initialDelay)
  {
    if (this.m_isArrived)
      return;
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
      allPlayer.ToggleGunRenderers(false, string.Empty);
      allPlayer.ToggleShadowVisiblity(false);
      allPlayer.ToggleHandRenderers(false, string.Empty);
      allPlayer.ToggleFollowerRenderers(false);
      allPlayer.SetInputOverride("elevator arrival");
    }
    this.m_isArrived = true;
    this.StartCoroutine(this.HandleArrival(initialDelay));
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

  [DebuggerHidden]
  private IEnumerator HandleArrival(float initialDelay)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ElevatorArrivalController.\u003CHandleArrival\u003Ec__Iterator1()
    {
      initialDelay = initialDelay,
      \u0024this = this
    };
  }

  protected override void OnDestroy() => base.OnDestroy();
}
