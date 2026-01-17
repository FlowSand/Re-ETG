// Decompiled with JetBrains decompiler
// Type: ArkController
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
public class ArkController : BraveBehaviour, IPlayerInteractable
{
  public tk2dSpriteAnimator LidAnimator;
  public tk2dSpriteAnimator ChestAnimator;
  public tk2dSpriteAnimator PoofAnimator;
  public tk2dSprite LightSpriteBeam;
  public tk2dSprite HellCrackSprite;
  public Transform GunSpawnPoint;
  public GameObject GunPrefab;
  public GameObject HeldGunPrefab;
  public List<Transform> ParallaxTransforms;
  public List<float> ParallaxFractions;
  [NonSerialized]
  private List<Vector3> ParallaxStartingPositions = new List<Vector3>();
  [NonSerialized]
  private List<DFGentleBob> Bobbers = new List<DFGentleBob>();
  [NonSerialized]
  private RoomHandler m_parentRoom;
  [NonSerialized]
  private Transform m_heldPastGun;
  private bool m_hasBeenInteracted;
  protected bool m_isLocalPointing;
  public static bool IsResettingPlayers;

  [DebuggerHidden]
  private IEnumerator Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ArkController.\u003CStart\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void Update()
  {
    if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.END_TIMES)
      return;
    float num1 = (float) (this.m_parentRoom.area.basePosition.y + this.m_parentRoom.area.dimensions.y) - (float) this.m_parentRoom.area.basePosition.y;
    float x1 = GameManager.Instance.MainCameraController.transform.position.x;
    float y1 = GameManager.Instance.MainCameraController.transform.position.y;
    for (int index = 0; index < this.ParallaxTransforms.Count; ++index)
    {
      float num2 = num1 * this.ParallaxFractions[index];
      float num3 = y1 - this.ParallaxStartingPositions[index].y;
      float num4 = x1 - this.ParallaxStartingPositions[index].x;
      float num5 = Mathf.Clamp(num3 / num1, -1f, 1f);
      float num6 = Mathf.Clamp(num4 / num1, -1f, 1f);
      float y2 = this.ParallaxStartingPositions[index].y + num5 * num2;
      float x2 = this.ParallaxStartingPositions[index].x + num6 * num2;
      Vector3 vector3 = this.ParallaxStartingPositions[index].WithY(y2).WithX(x2);
      if ((UnityEngine.Object) this.Bobbers[index] != (UnityEngine.Object) null)
        this.Bobbers[index].AbsoluteStartPosition = vector3;
      else
        this.ParallaxTransforms[index].position = vector3;
    }
  }

  public float GetDistanceToPoint(Vector2 point)
  {
    return this.m_hasBeenInteracted ? 100000f : Vector2.Distance(point, this.specRigidbody.UnitCenter) / 2f;
  }

  public void OnEnteredRange(PlayerController interactor)
  {
    SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
    SpriteOutlineManager.AddOutlineToSprite(this.LidAnimator.sprite, Color.white);
  }

  public void OnExitRange(PlayerController interactor)
  {
    SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
    SpriteOutlineManager.RemoveOutlineFromSprite(this.LidAnimator.sprite, true);
  }

  public void Interact(PlayerController interactor)
  {
    SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
    SpriteOutlineManager.RemoveOutlineFromSprite(this.LidAnimator.sprite);
    if (!this.m_hasBeenInteracted)
      this.m_hasBeenInteracted = true;
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      GameManager.Instance.AllPlayers[index].RemoveBrokenInteractable((IPlayerInteractable) this);
    BraveInput.DoVibrationForAllPlayers(Vibration.Time.Normal, Vibration.Strength.Medium);
    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
    {
      PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(interactor);
      float num = Vector2.Distance(otherPlayer.CenterPosition, interactor.CenterPosition);
      if ((double) num > 8.0 || (double) num < 0.75)
      {
        Vector2 vector2 = Vector2.right;
        if ((double) interactor.CenterPosition.x < (double) this.ChestAnimator.sprite.WorldCenter.x)
          vector2 = Vector2.left;
        otherPlayer.WarpToPoint(otherPlayer.transform.position.XY() + vector2 * 2f, true);
      }
    }
    this.StartCoroutine(this.Open(interactor));
  }

  [DebuggerHidden]
  private IEnumerator HandleLightSprite()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ArkController.\u003CHandleLightSprite\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator Open(PlayerController interactor)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ArkController.\u003COpen\u003Ec__Iterator2()
    {
      interactor = interactor,
      \u0024this = this
    };
  }

  private Vector2 GetTargetClockhairPosition(BraveInput input, Vector2 currentClockhairPosition)
  {
    return Vector2.Min(GameManager.Instance.MainCameraController.MaxVisiblePoint, Vector2.Max(GameManager.Instance.MainCameraController.MinVisiblePoint, !input.IsKeyboardAndMouse() ? currentClockhairPosition + input.ActiveActions.Aim.Vector * 10f * BraveTime.DeltaTime : GameManager.Instance.MainCameraController.Camera.ScreenToWorldPoint(Input.mousePosition).XY() + new Vector2(0.375f, -0.25f)));
  }

  private void UpdateCameraPositionDuringClockhair(Vector2 targetPosition)
  {
    if ((double) Vector2.Distance(targetPosition, this.ChestAnimator.sprite.WorldCenter) > 8.0)
      targetPosition = this.ChestAnimator.sprite.WorldCenter;
    Vector2 vector2 = (Vector2) GameManager.Instance.MainCameraController.OverridePosition;
    if ((double) Vector2.Distance(vector2, targetPosition) > 10.0)
      vector2 = GameManager.Instance.MainCameraController.transform.position.XY();
    GameManager.Instance.MainCameraController.OverridePosition = Vector3.MoveTowards((Vector3) vector2, (Vector3) targetPosition, BraveTime.DeltaTime);
  }

  private bool CheckPlayerTarget(PlayerController target, Transform clockhairTransform)
  {
    return (double) Vector2.Distance(clockhairTransform.position.XY() + new Vector2(-0.375f, 0.25f), target.CenterPosition) < 0.625;
  }

  private bool CheckHellTarget(tk2dBaseSprite hellTarget, Transform clockhairTransform)
  {
    return !((UnityEngine.Object) hellTarget == (UnityEngine.Object) null) && (double) Vector2.Distance(clockhairTransform.position.XY() + new Vector2(-0.375f, 0.25f), hellTarget.WorldCenter) < 0.625;
  }

  public void HandleHeldGunSpriteFlip(bool flipped)
  {
    tk2dSprite component = this.m_heldPastGun.GetComponent<tk2dSprite>();
    if (flipped)
    {
      if (!component.FlipY)
        component.FlipY = true;
    }
    else if (component.FlipY)
      component.FlipY = false;
    this.m_heldPastGun.localPosition = -this.m_heldPastGun.Find("PrimaryHand").localPosition;
    if (flipped)
      this.m_heldPastGun.localPosition = Vector3.Scale(this.m_heldPastGun.localPosition, new Vector3(1f, -1f, 1f));
    this.m_heldPastGun.localPosition = BraveUtility.QuantizeVector(this.m_heldPastGun.localPosition, 16f);
    component.ForceRotationRebuild();
    component.UpdateZDepth();
  }

  private void PointGunAtClockhair(PlayerController interactor, Transform clockhairTransform)
  {
    Vector2 centerPosition = interactor.CenterPosition;
    Vector2 vector2 = clockhairTransform.position.XY() - centerPosition;
    if (this.m_isLocalPointing && (double) vector2.sqrMagnitude > 9.0)
      this.m_isLocalPointing = false;
    else if (this.m_isLocalPointing || (double) vector2.sqrMagnitude < 4.0)
    {
      this.m_isLocalPointing = true;
      float t = (float) ((double) vector2.sqrMagnitude / 4.0 - 0.05000000074505806);
      vector2 = Vector2.Lerp(Vector2.right, vector2, t);
    }
    float z = BraveMathCollege.Atan2Degrees(vector2).Quantize(3f);
    interactor.GunPivot.rotation = Quaternion.Euler(0.0f, 0.0f, z);
    interactor.ForceIdleFacePoint(vector2, false);
    this.HandleHeldGunSpriteFlip(interactor.SpriteFlipped);
  }

  [DebuggerHidden]
  private IEnumerator HandleClockhair(PlayerController interactor)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ArkController.\u003CHandleClockhair\u003Ec__Iterator3()
    {
      interactor = interactor,
      \u0024this = this
    };
  }

  private void ResetPlayers(bool isGunslingerPast = false)
  {
    ArkController.IsResettingPlayers = true;
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      if (GameManager.Instance.AllPlayers[index].healthHaver.IsAlive)
      {
        if (!isGunslingerPast)
          GameManager.Instance.AllPlayers[index].ResetToFactorySettings(true, true);
        if (!isGunslingerPast)
          GameManager.Instance.AllPlayers[index].CharacterUsesRandomGuns = false;
        GameManager.Instance.AllPlayers[index].IsVisible = true;
        GameManager.Instance.AllPlayers[index].ClearInputOverride("ark");
        GameManager.Instance.AllPlayers[index].ClearAllInputOverrides();
      }
    }
    ArkController.IsResettingPlayers = false;
  }

  private void DestroyPlayers()
  {
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      UnityEngine.Object.Destroy((UnityEngine.Object) GameManager.Instance.AllPlayers[index].gameObject);
  }

  private bool CharacterStoryComplete(PlayableCharacters shotCharacter)
  {
    return GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_BULLET_COMPLETE) && GameManager.Instance.PrimaryPlayer.PastAccessible;
  }

  private void SpawnVFX(string vfxResourcePath, Vector2 pos)
  {
    tk2dSprite component = UnityEngine.Object.Instantiate<GameObject>((GameObject) BraveResources.Load(vfxResourcePath, typeof (GameObject))).GetComponent<tk2dSprite>();
    component.PlaceAtPositionByAnchor((Vector3) pos, tk2dBaseSprite.Anchor.MiddleCenter);
    component.UpdateZDepth();
  }

  [DebuggerHidden]
  private IEnumerator HandleGun(PlayerController interactor)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ArkController.\u003CHandleGun\u003Ec__Iterator4()
    {
      interactor = interactor,
      \u0024this = this
    };
  }

  public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
  {
    shouldBeFlipped = false;
    return string.Empty;
  }

  public float GetOverrideMaxDistance() => -1f;

  protected override void OnDestroy() => base.OnDestroy();
}
