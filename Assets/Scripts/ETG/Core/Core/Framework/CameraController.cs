// Decompiled with JetBrains decompiler
// Type: CameraController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class CameraController : BraveBehaviour
    {
      public CameraController.ControllerCamSettings controllerCamera;
      private const float c_screenShakeClamp = 5f;
      public float screenShakeDist;
      [CurveRange(0.0f, 0.0f, 1f, 1f)]
      public AnimationCurve screenShakeCurve;
      private PlayerController m_player;
      [SerializeField]
      private float z_Offset = -10f;
      public bool IsPerspectiveMode;
      [HideInInspector]
      public float CurrentStickyFriction = 1f;
      [HideInInspector]
      public Vector3 OverridePosition;
      [HideInInspector]
      public bool UseOverridePlayerOnePosition;
      [HideInInspector]
      public Vector2 OverridePlayerOnePosition;
      [HideInInspector]
      public bool UseOverridePlayerTwoPosition;
      [HideInInspector]
      public Vector2 OverridePlayerTwoPosition;
      [NonSerialized]
      public float OverrideZoomScale = 1f;
      [NonSerialized]
      public float CurrentZoomScale = 1f;
      private float m_screenShakeVibration;
      private bool m_screenShakeVibrationDirty;
      private Vector3 screenShakeAmount = Vector3.zero;
      private Vector2 previousBasePosition;
      private Dictionary<Component, IEnumerator> continuousShakeMap = new Dictionary<Component, IEnumerator>();
      private List<IEnumerator> activeContinuousShakes = new List<IEnumerator>();
      private bool m_isTrackingPlayer = true;
      private bool m_manualControl;
      private bool m_isLerpingToManualControl;
      private bool m_isRecoveringFromManualControl;
      private Vector2 m_lastAimOffset = Vector2.zero;
      private Vector2 m_aimOffsetVelocity = Vector2.zero;
      private Vector3 m_currentVelocity;
      private Camera m_camera;
      [NonSerialized]
      public float OverrideRecoverySpeed = -1f;
      private const float RECOVERY_SPEED = 20f;
      [NonSerialized]
      public Vector3 FINAL_CAMERA_POSITION_OFFSET;
      public System.Action OnFinishedFrame;
      private List<UnityEngine.Object> m_focusObjects = new List<UnityEngine.Object>();
      private Vector2 m_cachedMinPos;
      private Vector2 m_cachedMaxPos;
      private Vector2 m_cachedSize;
      private static Vector2 m_cachedCameraMin;
      private static Vector2 m_cachedCameraMax;
      private const float COOP_REDUCTION = 0.3f;
      private const float c_newScreenShakeModeScalar = 0.5f;
      private bool m_terminateNextContinuousScreenShake;

      public void ClearPlayerCache() => this.m_player = (PlayerController) null;

      public float CurrentZOffset
      {
        get => this.IsPerspectiveMode ? this.transform.position.y - 40f : this.z_Offset;
      }

      public bool IsCurrentlyZoomIntermediate
      {
        get => (double) this.CurrentZoomScale != (double) this.OverrideZoomScale;
      }

      public void SetZoomScaleImmediate(float zoomScale)
      {
        this.OverrideZoomScale = zoomScale;
        this.CurrentZoomScale = zoomScale;
        if (!Pixelator.HasInstance)
          return;
        Pixelator.Instance.NUM_MACRO_PIXELS_HORIZONTAL = (int) ((float) BraveCameraUtility.H_PIXELS / this.CurrentZoomScale).Quantize(2f);
        Pixelator.Instance.NUM_MACRO_PIXELS_VERTICAL = (int) ((float) BraveCameraUtility.V_PIXELS / this.CurrentZoomScale).Quantize(2f);
      }

      public Vector3 ScreenShakeVector => this.screenShakeAmount;

      public float ScreenShakeVibration => this.m_screenShakeVibration;

      public void UpdateScreenShakeVibration(float newVibration)
      {
        if (this.m_screenShakeVibrationDirty)
        {
          this.m_screenShakeVibration = 0.0f;
          this.m_screenShakeVibrationDirty = false;
        }
        this.m_screenShakeVibration = Mathf.Max(this.m_screenShakeVibration, newVibration);
      }

      public void MarkScreenShakeVibrationDirty() => this.m_screenShakeVibrationDirty = true;

      public bool ManualControl => this.m_manualControl;

      public bool PreventAimLook { get; set; }

      public Camera Camera
      {
        get
        {
          if (!(bool) (UnityEngine.Object) this.m_camera && (bool) (UnityEngine.Object) this)
            this.m_camera = this.GetComponent<Camera>();
          return this.m_camera;
        }
      }

      private float m_deltaTime => GameManager.INVARIANT_DELTA_TIME;

      public bool LockX { get; set; }

      public bool LockY { get; set; }

      public bool LockToRoom { get; set; }

      public bool PreventFuseBombAimOffset { get; set; }

      public static Vector3 PLATFORM_CAMERA_OFFSET
      {
        get
        {
          return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 ? Vector3.zero : new Vector3(1f / 32f, 1f / 32f, 0.0f);
        }
      }

      private void Awake() => BraveTime.CacheDeltaTimeForFrame();

      private void Start()
      {
        this.m_camera = this.GetComponent<Camera>();
        this.FINAL_CAMERA_POSITION_OFFSET = CameraController.PLATFORM_CAMERA_OFFSET;
        if ((UnityEngine.Object) this.m_player == (UnityEngine.Object) null)
          this.m_player = GameManager.Instance.PrimaryPlayer;
        this.screenShakeAmount = new Vector3(0.0f, 0.0f, 0.0f);
      }

      public void AddFocusPoint(GameObject go)
      {
        if (this.m_focusObjects.Contains((UnityEngine.Object) go))
          return;
        this.m_focusObjects.Add((UnityEngine.Object) go);
      }

      public void AddFocusPoint(SpeculativeRigidbody specRigidbody)
      {
        if (this.m_focusObjects.Contains((UnityEngine.Object) specRigidbody))
          return;
        this.m_focusObjects.Add((UnityEngine.Object) specRigidbody);
      }

      public void RemoveFocusPoint(GameObject go) => this.m_focusObjects.Remove((UnityEngine.Object) go);

      public void RemoveFocusPoint(SpeculativeRigidbody specRigidbody)
      {
        this.m_focusObjects.Remove((UnityEngine.Object) specRigidbody);
      }

      public static bool SuperSmoothCamera => GameManager.Options.SuperSmoothCamera;

      private Vector2 GetPlayerPosition(PlayerController targetPlayer)
      {
        if (targetPlayer.IsPrimaryPlayer)
        {
          if (this.UseOverridePlayerOnePosition)
            return this.OverridePlayerOnePosition;
          return CameraController.SuperSmoothCamera ? targetPlayer.SmoothedCameraCenter : targetPlayer.CenterPosition;
        }
        if (this.UseOverridePlayerTwoPosition)
          return this.OverridePlayerTwoPosition;
        return CameraController.SuperSmoothCamera ? targetPlayer.SmoothedCameraCenter : targetPlayer.CenterPosition;
      }

      private bool UseMouseAim
      {
        get
        {
          return Application.platform != RuntimePlatform.PS4 && Application.platform != RuntimePlatform.XboxOne && GameManager.Options.mouseAimLook && BraveInput.GetInstanceForPlayer(0).IsKeyboardAndMouse() && !this.LockToRoom;
        }
      }

      public Vector2 GetCoreCurrentBasePosition()
      {
        if ((UnityEngine.Object) this.m_player == (UnityEngine.Object) null)
          this.m_player = GameManager.Instance.PrimaryPlayer;
        Vector2 zero = Vector2.zero;
        int prevCount = 0;
        if (GameManager.Instance.AllPlayers.Length < 2)
        {
          if ((UnityEngine.Object) this.m_player == (UnityEngine.Object) null)
            return Vector2.zero;
          BraveMathCollege.WeightedAverage(this.GetPlayerPosition(this.m_player), ref zero, ref prevCount);
        }
        else
        {
          for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          {
            if (GameManager.Instance.AllPlayers[index].gameObject.activeSelf && !GameManager.Instance.AllPlayers[index].IgnoredByCamera && !GameManager.Instance.AllPlayers[index].IsGhost)
              BraveMathCollege.WeightedAverage(this.GetPlayerPosition(GameManager.Instance.AllPlayers[index]), ref zero, ref prevCount);
          }
          if (prevCount > 1)
            prevCount = 1;
        }
        for (int index = 0; index < this.m_focusObjects.Count; ++index)
        {
          if (this.m_focusObjects[index] is GameObject)
            BraveMathCollege.WeightedAverage((Vector2) (this.m_focusObjects[index] as GameObject).transform.position, ref zero, ref prevCount);
          else if (this.m_focusObjects[index] is SpeculativeRigidbody)
            BraveMathCollege.WeightedAverage((this.m_focusObjects[index] as SpeculativeRigidbody).GetUnitCenter(ColliderType.HitBox), ref zero, ref prevCount);
        }
        return zero;
      }

      public Vector2 GetIdealCameraPosition()
      {
        Vector2 currentBasePosition = this.GetCoreCurrentBasePosition();
        return currentBasePosition + this.GetCoreOffset(currentBasePosition, false, true);
      }

      private Vector2 GetCoreOffset(Vector2 currentBasePosition, bool isUpdate, bool allowAimOffset)
      {
        if (this.UseMouseAim)
        {
          Vector2 coreOffset = Vector2.zero;
          if (allowAimOffset && GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER)
          {
            Vector2 vector2 = this.m_camera.ScreenToWorldPoint(Input.mousePosition).XY() - currentBasePosition;
            vector2 = new Vector2(vector2.x / this.m_camera.aspect, vector2.y);
            vector2.x = Mathf.Clamp(vector2.x, this.m_camera.orthographicSize * -1.5f, this.m_camera.orthographicSize * 1.5f);
            vector2.y = Mathf.Clamp(vector2.y, this.m_camera.orthographicSize * -1.5f, this.m_camera.orthographicSize * 1.5f);
            float num = Mathf.Lerp(0.0f, 0.33333f, Mathf.Pow(Mathf.Clamp01((float) (((double) vector2.magnitude - 1.0) / ((double) this.m_camera.orthographicSize - 1.0))), 0.5f));
            coreOffset = vector2 * num;
          }
          if ((double) coreOffset.magnitude < 1.0 / 64.0)
            coreOffset = Vector2.zero;
          return coreOffset;
        }
        Vector2 vector2_1 = Vector2.zero;
        GungeonActions activeActions = BraveInput.GetInstanceForPlayer(0).ActiveActions;
        if ((bool) (UnityEngine.Object) Minimap.Instance && !Minimap.Instance.IsFullscreen && activeActions != null)
        {
          Vector2 vector = activeActions.Aim.Vector;
          vector2_1 = (double) vector.magnitude <= 0.10000000149011612 ? Vector2.zero : vector.normalized * this.controllerCamera.ModifiedAimContribution;
          if ((double) vector2_1.y > 0.0 && this.PreventFuseBombAimOffset)
            vector2_1.y = 0.0f;
        }
        float smoothTime = !(vector2_1 == Vector2.zero) ? (!(this.m_lastAimOffset != Vector2.zero) || (double) Mathf.Abs(BraveMathCollege.ClampAngle180(vector2_1.ToAngle() - this.m_lastAimOffset.ToAngle())) <= 135.0 ? this.controllerCamera.AimContributionTime : this.controllerCamera.AimContributionFastTime) : this.controllerCamera.AimContributionSlowTime;
        Vector2 aimOffset = Vector2.SmoothDamp(this.m_lastAimOffset, vector2_1, ref this.m_aimOffsetVelocity, smoothTime, 20f, this.m_deltaTime);
        if (isUpdate)
          this.m_lastAimOffset = aimOffset;
        Vector2 vector2_2 = currentBasePosition;
        if (this.controllerCamera.state == CameraController.ControllerCameraState.RoomLock)
        {
          Rect cameraBoundingRect = GameManager.Instance.PrimaryPlayer.CurrentRoom.cameraBoundingRect;
          ++cameraBoundingRect.yMin;
          cameraBoundingRect.height += 2f;
          vector2_2 = this.GetBoundedCameraPositionInRect(cameraBoundingRect, currentBasePosition, ref aimOffset);
        }
        if (this.controllerCamera.UseAimContribution && (double) this.OverrideZoomScale == 1.0 && GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER)
          vector2_2 += aimOffset;
        Vector2 coreOffset1 = vector2_2 - currentBasePosition;
        if ((double) coreOffset1.magnitude < 1.0 / 64.0)
          coreOffset1 = Vector2.zero;
        return coreOffset1;
      }

      private void Update()
      {
        tk2dSpriteAnimator.CameraPositionThisFrame = this.transform.position.XY();
        if (!this.m_screenShakeVibrationDirty)
          return;
        this.m_screenShakeVibration = 0.0f;
        this.m_screenShakeVibrationDirty = false;
      }

      private void AdjustRecoverySpeedFoyer()
      {
        if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER || (double) this.OverrideRecoverySpeed <= 0.0)
          return;
        if ((double) this.OverrideRecoverySpeed >= 20.0)
          this.OverrideRecoverySpeed = -1f;
        else
          this.OverrideRecoverySpeed += BraveTime.DeltaTime * 3f;
      }

      private void LateUpdate()
      {
        this.controllerCamera.forceTimer = Mathf.Max(0.0f, this.controllerCamera.forceTimer - BraveTime.DeltaTime);
        this.m_terminateNextContinuousScreenShake = false;
        for (int index = 0; index < this.activeContinuousShakes.Count; ++index)
          this.activeContinuousShakes[index].MoveNext();
        if (GameManager.Instance.IsLoadingLevel)
          return;
        if ((bool) (UnityEngine.Object) Pixelator.Instance && ((double) this.CurrentZoomScale != (double) this.OverrideZoomScale || Pixelator.Instance.NUM_MACRO_PIXELS_HORIZONTAL != (int) ((float) BraveCameraUtility.H_PIXELS / this.CurrentZoomScale).Quantize(2f)))
        {
          this.CurrentZoomScale = Mathf.MoveTowards(this.CurrentZoomScale, this.OverrideZoomScale, 0.5f * GameManager.INVARIANT_DELTA_TIME);
          float aspect = this.m_camera.aspect;
          int hPixels = BraveCameraUtility.H_PIXELS;
          int vPixels = BraveCameraUtility.V_PIXELS;
          Pixelator.Instance.NUM_MACRO_PIXELS_HORIZONTAL = (int) ((float) hPixels / this.CurrentZoomScale).Quantize(2f);
          Pixelator.Instance.NUM_MACRO_PIXELS_VERTICAL = (int) ((float) vPixels / this.CurrentZoomScale).Quantize(2f);
        }
        if (!this.m_manualControl)
        {
          Vector2 currentBasePosition = !this.m_isTrackingPlayer ? this.previousBasePosition : this.GetCoreCurrentBasePosition();
          if (!this.UseMouseAim && (double) this.controllerCamera.forceTimer <= 0.0)
          {
            bool flag = (UnityEngine.Object) GameManager.Instance.PrimaryPlayer != (UnityEngine.Object) null && GameManager.Instance.PrimaryPlayer.CurrentRoom != null && GameManager.Instance.PrimaryPlayer.CurrentRoom.IsSealed;
            if ((this.controllerCamera.state == CameraController.ControllerCameraState.FollowPlayer || this.controllerCamera.state == CameraController.ControllerCameraState.Off) && flag)
            {
              this.controllerCamera.state = CameraController.ControllerCameraState.RoomLock;
              this.controllerCamera.isTransitioning = true;
              this.controllerCamera.transitionDuration = this.controllerCamera.ToRoomLockTime;
              this.controllerCamera.transitionStart = (Vector2) this.transform.position;
              this.controllerCamera.transitionTimer = 0.0f;
            }
            else if ((this.controllerCamera.state == CameraController.ControllerCameraState.RoomLock || this.controllerCamera.state == CameraController.ControllerCameraState.Off) && !flag)
            {
              this.controllerCamera.state = CameraController.ControllerCameraState.FollowPlayer;
              this.controllerCamera.isTransitioning = true;
              this.controllerCamera.transitionDuration = this.controllerCamera.EndRoomLockTime;
              this.controllerCamera.transitionStart = (Vector2) this.transform.position;
              this.controllerCamera.transitionTimer = 0.0f;
            }
          }
          Vector2 coreOffset = this.GetCoreOffset(currentBasePosition, true, this.m_isTrackingPlayer);
          Vector2 b1 = currentBasePosition + coreOffset;
          this.previousBasePosition = b1;
          Vector2 vector2_1 = b1;
          if (!this.UseMouseAim && this.controllerCamera.isTransitioning)
          {
            this.controllerCamera.transitionTimer += this.m_deltaTime;
            float t = Mathf.SmoothStep(0.0f, 1f, Mathf.Min(this.controllerCamera.transitionTimer / this.controllerCamera.transitionDuration, 1f));
            vector2_1 = Vector2.Lerp(this.controllerCamera.transitionStart, b1, t);
            if ((double) this.controllerCamera.transitionTimer > (double) this.controllerCamera.transitionDuration)
              this.controllerCamera.isTransitioning = false;
          }
          else if (this.m_isRecoveringFromManualControl)
          {
            Vector2 b2 = this.transform.PositionVector2() - this.FINAL_CAMERA_POSITION_OFFSET.XY();
            float num1 = Vector2.Distance(vector2_1, b2);
            this.AdjustRecoverySpeedFoyer();
            float num2 = (double) this.OverrideRecoverySpeed <= 0.0 ? 20f : this.OverrideRecoverySpeed;
            if ((double) num1 > (double) num2 * (double) this.m_deltaTime)
            {
              Vector2 vector2_2 = vector2_1 - b2;
              vector2_1 = b2 + vector2_2.normalized * num2 * this.m_deltaTime;
            }
            else
            {
              this.m_isRecoveringFromManualControl = false;
              this.OverrideRecoverySpeed = -1f;
            }
          }
          if (this.UseMouseAim)
          {
            this.controllerCamera.state = CameraController.ControllerCameraState.Off;
            this.controllerCamera.isTransitioning = false;
          }
          Vector3 vector3_1 = this.screenShakeAmount * ScreenShakeSettings.GLOBAL_SHAKE_MULTIPLIER * GameManager.Options.ScreenShakeMultiplier;
          Vector3 vector3_2 = (double) vector3_1.magnitude <= 5.0 ? vector3_1 : vector3_1.normalized * 5f;
          if (float.IsNaN(vector3_2.x) || float.IsInfinity(vector3_2.x))
            vector3_2.x = 0.0f;
          if (float.IsNaN(vector3_2.y) || float.IsInfinity(vector3_2.y))
            vector3_2.y = 0.0f;
          if (float.IsNaN(vector3_2.z) || float.IsInfinity(vector3_2.z))
            vector3_2.z = 0.0f;
          if (GameManager.Instance.IsPaused)
            vector3_2 = Vector3.zero;
          Vector3 vector3_3 = vector2_1.ToVector3ZUp(this.CurrentZOffset) + vector3_2 + this.FINAL_CAMERA_POSITION_OFFSET;
          if (this.LockX)
            vector3_3.x = this.transform.position.x;
          if (this.LockY)
            vector3_3.y = this.transform.position.y;
          this.transform.position = vector3_3;
        }
        else
        {
          if (this.controllerCamera != null)
          {
            this.controllerCamera.isTransitioning = false;
            this.controllerCamera.transitionStart = (Vector2) this.transform.position;
          }
          this.GetCoreOffset(this.GetCoreCurrentBasePosition(), true, true);
          Vector2 vector2_3 = this.OverridePosition.XY();
          if (this.m_isLerpingToManualControl)
          {
            Vector2 b = this.transform.PositionVector2() - this.FINAL_CAMERA_POSITION_OFFSET.XY();
            float num3 = Vector2.Distance(vector2_3, b);
            this.AdjustRecoverySpeedFoyer();
            float num4 = (double) this.OverrideRecoverySpeed <= 0.0 ? 20f : this.OverrideRecoverySpeed;
            if ((double) num3 > (double) num4 * (double) this.m_deltaTime)
            {
              Vector2 vector2_4 = vector2_3 - b;
              vector2_3 = b + vector2_4.normalized * num4 * this.m_deltaTime;
            }
            else
              this.m_isLerpingToManualControl = false;
          }
          Vector3 vector3_4 = (double) this.screenShakeAmount.magnitude <= 5.0 ? this.screenShakeAmount : this.screenShakeAmount.normalized * 5f;
          float screenShakeMultiplier = GameManager.Options.ScreenShakeMultiplier;
          Vector3 vector3_5 = vector2_3.ToVector3ZUp(this.CurrentZOffset) + vector3_4 * ScreenShakeSettings.GLOBAL_SHAKE_MULTIPLIER * screenShakeMultiplier + this.FINAL_CAMERA_POSITION_OFFSET;
          if (this.LockX)
            vector3_5.x = this.transform.position.x;
          if (this.LockY)
            vector3_5.y = this.transform.position.y;
          if (float.IsNaN(vector3_5.x) || float.IsNaN(vector3_5.y))
          {
            UnityEngine.Debug.LogWarning((object) "THERE'S NaNS IN THEM THAR HILLS");
            vector3_5 = (Vector3) this.GetCoreCurrentBasePosition();
          }
          this.transform.position = vector3_5;
        }
        if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW || GameManager.Options.CurrentPreferredScalingMode == GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST)
          this.transform.position = this.transform.position.Quantize(1f / 16f) + CameraController.PLATFORM_CAMERA_OFFSET;
        Ray ray1 = Camera.main.ViewportPointToRay((Vector3) new Vector2(0.0f, 0.0f));
        Plane plane = new Plane(Vector3.back, Vector3.zero);
        float enter;
        plane.Raycast(ray1, out enter);
        this.m_cachedMinPos = (Vector2) ray1.GetPoint(enter);
        Ray ray2 = Camera.main.ViewportPointToRay((Vector3) new Vector2(1f, 1f));
        plane.Raycast(ray2, out enter);
        this.m_cachedMaxPos = (Vector2) ray2.GetPoint(enter);
        this.m_cachedSize = this.m_cachedMaxPos - this.m_cachedMinPos;
        CameraController.m_cachedCameraMin = (Vector2) Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f));
        CameraController.m_cachedCameraMax = (Vector2) Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f));
        if (this.OnFinishedFrame == null)
          return;
        this.OnFinishedFrame();
      }

      public bool IsLerping
      {
        get
        {
          return this.m_manualControl ? this.m_isLerpingToManualControl : this.m_isRecoveringFromManualControl;
        }
        set
        {
          if (this.m_manualControl)
            this.m_isLerpingToManualControl = true;
          else
            this.m_isRecoveringFromManualControl = true;
        }
      }

      public void SetManualControl(bool manualControl, bool shouldLerp = true)
      {
        this.m_manualControl = manualControl;
        if (this.m_manualControl)
          this.m_isLerpingToManualControl = shouldLerp;
        else
          this.m_isRecoveringFromManualControl = shouldLerp;
      }

      public void ForceUpdateControllerCameraState(CameraController.ControllerCameraState newState)
      {
        this.controllerCamera.state = newState;
        this.controllerCamera.isTransitioning = false;
        this.controllerCamera.forceTimer = 6f;
      }

      public void UpdateOverridePosition(Vector3 newOverridePosition, float duration)
      {
        this.StartCoroutine(this.UpdateOverridePosition_CR(newOverridePosition, duration));
      }

      [DebuggerHidden]
      public IEnumerator UpdateOverridePosition_CR(Vector3 newOverridePosition, float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CameraController.\u003CUpdateOverridePosition_CR\u003Ec__Iterator0()
        {
          duration = duration,
          newOverridePosition = newOverridePosition,
          \u0024this = this
        };
      }

      public Vector2 GetAimContribution()
      {
        return this.m_manualControl || BraveInput.GetInstanceForPlayer(0).IsKeyboardAndMouse() || !this.controllerCamera.UseAimContribution || (double) this.OverrideZoomScale != 1.0 || GameManager.Instance.CurrentGameType != GameManager.GameType.SINGLE_PLAYER ? Vector2.zero : this.m_lastAimOffset;
      }

      public void ResetAimContribution()
      {
        this.m_lastAimOffset = Vector2.zero;
        this.m_aimOffsetVelocity = Vector2.zero;
      }

      public void ForceToPlayerPosition(PlayerController p)
      {
        this.transform.position = BraveUtility.QuantizeVector(p.transform.position.WithZ(this.CurrentZOffset), (float) PhysicsEngine.Instance.PixelsPerUnit) + new Vector3(1f / 32f, 1f / 32f, 0.0f);
        if (this.controllerCamera == null)
          return;
        this.controllerCamera.isTransitioning = false;
        this.controllerCamera.transitionStart = (Vector2) this.transform.position;
      }

      public void ForceToPlayerPosition(PlayerController p, Vector3 prevPlayerPosition)
      {
        Vector3 vector3 = this.transform.position - prevPlayerPosition;
        this.transform.position = BraveUtility.QuantizeVector((p.transform.position + vector3).WithZ(this.CurrentZOffset), (float) PhysicsEngine.Instance.PixelsPerUnit) + new Vector3(1f / 32f, 1f / 32f, 0.0f);
        if (this.controllerCamera == null)
          return;
        this.controllerCamera.isTransitioning = false;
        this.controllerCamera.transitionStart = (Vector2) this.transform.position;
      }

      public void AssignBoundingPolygon(RoomHandlerBoundingPolygon p)
      {
      }

      public void StartTrackingPlayer() => this.m_isTrackingPlayer = true;

      public void StopTrackingPlayer()
      {
        this.m_isRecoveringFromManualControl = true;
        this.m_isTrackingPlayer = false;
      }

      public void DoScreenShake(
        ScreenShakeSettings shakesettings,
        Vector2? shakeOrigin,
        bool isPlayerGun = false)
      {
        float magnitude = shakesettings.magnitude;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Options.CoopScreenShakeReduction)
          magnitude *= 0.3f;
        if (isPlayerGun)
          magnitude *= 0.75f;
        bool useCameraVibration = shakesettings.vibrationType != ScreenShakeSettings.VibrationType.None;
        if (shakesettings.vibrationType == ScreenShakeSettings.VibrationType.Simple)
        {
          BraveInput.DoVibrationForAllPlayers(shakesettings.simpleVibrationTime, shakesettings.simpleVibrationStrength);
          useCameraVibration = false;
        }
        this.StartCoroutine(this.HandleScreenShake(magnitude, shakesettings.speed, shakesettings.time, shakesettings.falloff, shakesettings.direction, shakeOrigin, useCameraVibration));
      }

      public void DoScreenShake(
        float magnitude,
        float shakeSpeed,
        float time,
        float falloffTime,
        Vector2? shakeOrigin)
      {
        float magnitude1 = magnitude;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Options.CoopScreenShakeReduction)
          magnitude1 *= 0.3f;
        this.StartCoroutine(this.HandleScreenShake(magnitude1, shakeSpeed, time, falloffTime, Vector2.zero, shakeOrigin, true));
      }

      public void DoGunScreenShake(
        ScreenShakeSettings shakesettings,
        Vector2 dir,
        Vector2? shakeOrigin,
        PlayerController playerOwner = null)
      {
        float magnitude = shakesettings.magnitude;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Options.CoopScreenShakeReduction)
          magnitude *= 0.3f;
        if ((bool) (UnityEngine.Object) playerOwner)
          magnitude *= 0.75f;
        bool useCameraVibration = shakesettings.vibrationType != ScreenShakeSettings.VibrationType.None;
        if ((bool) (UnityEngine.Object) playerOwner)
        {
          if (shakesettings.vibrationType == ScreenShakeSettings.VibrationType.Auto)
          {
            playerOwner.DoScreenShakeVibration(shakesettings.time, shakesettings.magnitude);
            useCameraVibration = false;
          }
          else if (shakesettings.vibrationType == ScreenShakeSettings.VibrationType.Simple)
          {
            playerOwner.DoVibration(shakesettings.simpleVibrationTime, shakesettings.simpleVibrationStrength);
            useCameraVibration = false;
          }
        }
        this.StartCoroutine(this.HandleScreenShake(magnitude, shakesettings.speed, shakesettings.time, shakesettings.falloff, dir, shakeOrigin, useCameraVibration));
      }

      public Vector2 MinVisiblePoint => this.m_cachedMinPos;

      public Vector2 MaxVisiblePoint => this.m_cachedMaxPos;

      public bool PointIsVisible(Vector2 flatPoint)
      {
        return (double) flatPoint.x > (double) this.m_cachedMinPos.x && (double) flatPoint.x < (double) this.m_cachedMaxPos.x && (double) flatPoint.y > (double) this.m_cachedMinPos.y && (double) flatPoint.y < (double) this.m_cachedMaxPos.y;
      }

      public bool PointIsVisible(Vector2 flatPoint, float percentBuffer)
      {
        Vector2 vector2 = this.m_cachedSize * percentBuffer;
        return (double) flatPoint.x > (double) this.m_cachedMinPos.x - (double) vector2.x && (double) flatPoint.x < (double) this.m_cachedMaxPos.x + (double) vector2.x && (double) flatPoint.y > (double) this.m_cachedMinPos.y - (double) vector2.y && (double) flatPoint.y < (double) this.m_cachedMaxPos.y + (double) vector2.y;
      }

      public void DoContinuousScreenShake(
        ScreenShakeSettings shakesettings,
        Component source,
        bool isPlayerGun = false)
      {
        float magnitude = shakesettings.magnitude;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Options.CoopScreenShakeReduction)
          magnitude *= 0.3f;
        if (isPlayerGun)
          magnitude *= 0.75f;
        bool useCameraVibration = shakesettings.vibrationType != ScreenShakeSettings.VibrationType.None;
        if (shakesettings.vibrationType == ScreenShakeSettings.VibrationType.Simple)
        {
          BraveInput.DoVibrationForAllPlayers(shakesettings.simpleVibrationTime, shakesettings.simpleVibrationStrength);
          useCameraVibration = false;
        }
        IEnumerator enumerator = this.HandleContinuousScreenShake(magnitude, shakesettings.speed, shakesettings.direction, source, useCameraVibration);
        if (this.continuousShakeMap.ContainsKey(source))
        {
          UnityEngine.Debug.LogWarning((object) ("Overwriting previous screen shake for " + (object) source), (UnityEngine.Object) source);
          this.StopContinuousScreenShake(source);
        }
        this.continuousShakeMap.Add(source, enumerator);
        this.activeContinuousShakes.Add(enumerator);
      }

      public void DoDelayedScreenShake(ScreenShakeSettings s, float delay, Vector2? shakeOrigin)
      {
        this.StartCoroutine(this.HandleDelayedScreenShake(s, delay, shakeOrigin));
      }

      public void StopContinuousScreenShake(Component source)
      {
        if (!this.continuousShakeMap.ContainsKey(source))
          return;
        IEnumerator continuousShake = this.continuousShakeMap[source];
        this.m_terminateNextContinuousScreenShake = true;
        continuousShake.MoveNext();
        this.continuousShakeMap.Remove(source);
        this.activeContinuousShakes.Remove(continuousShake);
      }

      public Vector3 DoFrameScreenShake(
        float magnitude,
        float shakeSpeed,
        Vector2 direction,
        Vector3 lastShakeAmount,
        float elapsedTime)
      {
        this.screenShakeAmount -= lastShakeAmount;
        if (direction == Vector2.zero)
        {
          float num = magnitude;
          Vector2 vector2 = new Vector2((float) ((double) Mathf.PerlinNoise((float) (0.314156711101532 + (double) elapsedTime * (double) shakeSpeed / 1.0729999542236328), (float) (0.11568319797515869 + (double) elapsedTime * (double) shakeSpeed / 4.8126997947692871)) * 2.0 - 1.0), (float) ((double) Mathf.PerlinNoise((float) (0.71593540906906128 + (double) elapsedTime * (double) shakeSpeed / 2.3726999759674072), (float) (0.931582510471344 + (double) elapsedTime * (double) shakeSpeed / 0.98119997978210449)) * 2.0 - 1.0)).normalized * (num - Mathf.PingPong(elapsedTime * shakeSpeed, magnitude) / magnitude * num);
          this.screenShakeAmount += new Vector3(vector2.x, vector2.y, 0.0f);
          BraveInput.DoSustainedScreenShakeVibration(magnitude);
          return new Vector3(vector2.x, vector2.y, 0.0f);
        }
        float num1 = Mathf.PingPong(elapsedTime * shakeSpeed, magnitude);
        Vector2 vector2_1 = new Vector2(num1 * direction.x, num1 * direction.y);
        this.screenShakeAmount += new Vector3(vector2_1.x, vector2_1.y, 0.0f);
        BraveInput.DoSustainedScreenShakeVibration(magnitude);
        return new Vector3(vector2_1.x, vector2_1.y, 0.0f);
      }

      public void ClearFrameScreenShake(Vector3 lastShakeAmount)
      {
        this.screenShakeAmount -= lastShakeAmount;
      }

      [DebuggerHidden]
      private IEnumerator HandleContinuousScreenShake(
        float magnitude,
        float shakeSpeed,
        Vector2 direction,
        Component source,
        bool useCameraVibration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CameraController.\u003CHandleContinuousScreenShake\u003Ec__Iterator1()
        {
          direction = direction,
          magnitude = magnitude,
          shakeSpeed = shakeSpeed,
          useCameraVibration = useCameraVibration,
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleDelayedScreenShake(
        ScreenShakeSettings sss,
        float delay,
        Vector2? origin)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CameraController.\u003CHandleDelayedScreenShake\u003Ec__Iterator2()
        {
          delay = delay,
          sss = sss,
          origin = origin,
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleScreenShake(
        float magnitude,
        float shakeSpeed,
        float time,
        float falloffTime,
        Vector2 direction,
        Vector2? origin,
        bool useCameraVibration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CameraController.\u003CHandleScreenShake\u003Ec__Iterator3()
        {
          origin = origin,
          magnitude = magnitude,
          shakeSpeed = shakeSpeed,
          direction = direction,
          time = time,
          falloffTime = falloffTime,
          useCameraVibration = useCameraVibration,
          \u0024this = this
        };
      }

      private Vector2 GetBoundedCameraPositionInRect(
        Rect rect,
        Vector2 focalPos,
        ref Vector2 aimOffset)
      {
        Vector2 cameraPositionInRect = focalPos;
        Vector2 worldPoint1 = (Vector2) this.m_camera.ViewportToWorldPoint((Vector3) Vector2.zero);
        Vector2 worldPoint2 = (Vector2) this.m_camera.ViewportToWorldPoint((Vector3) Vector2.one);
        Rect rect1 = new Rect(worldPoint1.x, worldPoint1.y, worldPoint2.x - worldPoint1.x, worldPoint2.y - worldPoint1.y);
        rect1.center = focalPos;
        float num1 = this.controllerCamera.VisibleBorder / this.controllerCamera.ModifiedAimContribution;
        if ((double) rect1.width > (double) rect.width)
        {
          float num2 = Mathf.Max(1f, this.controllerCamera.VisibleBorder - (float) (((double) rect1.width - (double) rect.width) / 2.0));
          if ((double) rect1.center.x < (double) rect.center.x)
          {
            float time = (float) (((double) rect.center.x - (double) rect1.center.x) / ((double) rect.width / 2.0));
            cameraPositionInRect.x = rect.center.x - this.controllerCamera.BorderBumperCurve.Evaluate(time) * num2;
            aimOffset.x = (float) ((1.0 - (double) time) * (double) aimOffset.x + (double) time * (double) aimOffset.x * (double) num1);
          }
          else if ((double) rect1.center.x > (double) rect.center.x)
          {
            float time = (float) (((double) rect1.center.x - (double) rect.center.x) / ((double) rect.width / 2.0));
            cameraPositionInRect.x = rect.center.x + this.controllerCamera.BorderBumperCurve.Evaluate(time) * num2;
            aimOffset.x = (float) ((1.0 - (double) time) * (double) aimOffset.x + (double) time * (double) aimOffset.x * (double) num1);
          }
        }
        else if ((double) rect1.xMin < (double) rect.xMin)
        {
          float time = (float) (((double) rect.xMin - (double) rect1.xMin) / ((double) rect1.width / 2.0));
          cameraPositionInRect.x = (float) ((double) rect.xMin - (double) this.controllerCamera.BorderBumperCurve.Evaluate(time) * (double) this.controllerCamera.VisibleBorder + (double) rect1.width / 2.0);
          aimOffset.x = (float) ((1.0 - (double) time) * (double) aimOffset.x + (double) time * (double) aimOffset.x * (double) num1);
        }
        else if ((double) rect1.xMax > (double) rect.xMax)
        {
          float time = (float) (((double) rect1.xMax - (double) rect.xMax) / ((double) rect1.width / 2.0));
          cameraPositionInRect.x = (float) ((double) rect.xMax + (double) this.controllerCamera.BorderBumperCurve.Evaluate(time) * (double) this.controllerCamera.VisibleBorder - (double) rect1.width / 2.0);
          aimOffset.x = (float) ((1.0 - (double) time) * (double) aimOffset.x + (double) time * (double) aimOffset.x * (double) num1);
        }
        if ((double) rect1.height > (double) rect.height)
        {
          float num3 = Mathf.Max(1f, this.controllerCamera.VisibleBorder - (float) (((double) rect1.height - (double) rect.height) / 2.0));
          if ((double) rect1.center.y < (double) rect.center.y)
          {
            float time = (float) (((double) rect.center.y - (double) rect1.center.y) / ((double) rect.height / 2.0));
            cameraPositionInRect.y = rect.center.y - this.controllerCamera.BorderBumperCurve.Evaluate(time) * num3;
            aimOffset.y = (float) ((1.0 - (double) time) * (double) aimOffset.y + (double) time * (double) aimOffset.y * (double) num1);
          }
          else if ((double) rect1.center.y > (double) rect.center.y)
          {
            float time = (float) (((double) rect1.center.y - (double) rect.center.y) / ((double) rect.height / 2.0));
            cameraPositionInRect.y = rect.center.y + this.controllerCamera.BorderBumperCurve.Evaluate(time) * num3;
            aimOffset.y = (float) ((1.0 - (double) time) * (double) aimOffset.y + (double) time * (double) aimOffset.y * (double) num1);
          }
        }
        else if ((double) rect1.yMin < (double) rect.yMin)
        {
          float time = (float) (((double) rect.yMin - (double) rect1.yMin) / ((double) rect1.height / 2.0));
          cameraPositionInRect.y = (float) ((double) rect.yMin - (double) this.controllerCamera.BorderBumperCurve.Evaluate(time) * (double) this.controllerCamera.VisibleBorder + (double) rect1.height / 2.0);
          aimOffset.y = (float) ((1.0 - (double) time) * (double) aimOffset.y + (double) time * (double) aimOffset.y * (double) num1);
        }
        else if ((double) rect1.yMax > (double) rect.yMax)
        {
          float time = (float) (((double) rect1.yMax - (double) rect.yMax) / ((double) rect1.height / 2.0));
          cameraPositionInRect.y = (float) ((double) rect.yMax + (double) this.controllerCamera.BorderBumperCurve.Evaluate(time) * (double) this.controllerCamera.VisibleBorder - (double) rect1.height / 2.0);
          aimOffset.y = (float) ((1.0 - (double) time) * (double) aimOffset.y + (double) time * (double) aimOffset.y * (double) num1);
        }
        return cameraPositionInRect;
      }

      public static Vector2 CameraToWorld(float x, float y)
      {
        return new Vector2(Mathf.Lerp(CameraController.m_cachedCameraMin.x, CameraController.m_cachedCameraMax.x, x), Mathf.Lerp(CameraController.m_cachedCameraMin.y, CameraController.m_cachedCameraMax.y, y));
      }

      public static Vector2 CameraToWorld(Vector2 point)
      {
        return new Vector2(Mathf.Lerp(CameraController.m_cachedCameraMin.x, CameraController.m_cachedCameraMax.x, point.x), Mathf.Lerp(CameraController.m_cachedCameraMin.y, CameraController.m_cachedCameraMax.y, point.y));
      }

      protected override void OnDestroy() => base.OnDestroy();

      [Serializable]
      public class ControllerCamSettings
      {
        public float VisibleBorder = 4f;
        public AnimationCurve BorderBumperCurve;
        public float ToHallwayTime = 1.5f;
        public float ToRoomTime = 1.5f;
        public float ToRoomLockTime = 1f;
        public float EndRoomLockTime = 2f;
        public float AimContribution = 5f;
        public float AimContributionTime = 0.5f;
        public float AimContributionFastTime = 0.25f;
        public float AimContributionSlowTime = 1f;
        [NonSerialized]
        public CameraController.ControllerCameraState state;
        [NonSerialized]
        public bool isTransitioning;
        [NonSerialized]
        public float transitionTimer;
        [NonSerialized]
        public float transitionDuration;
        [NonSerialized]
        public Vector2 transitionStart;
        [NonSerialized]
        public float forceTimer;
        [NonSerialized]
        public RoomHandler exitRoomOne;
        [NonSerialized]
        public RoomHandler exitRoomTwo;

        public bool UseAimContribution
        {
          get
          {
            return (double) GameManager.Options.controllerAimLookMultiplier > 0.0 && !GameManager.Instance.MainCameraController.PreventAimLook;
          }
        }

        public float ModifiedAimContribution
        {
          get => this.AimContribution * GameManager.Options.controllerAimLookMultiplier;
        }
      }

      public enum ControllerCameraState
      {
        FollowPlayer,
        RoomLock,
        Off,
      }
    }

}
