// Decompiled with JetBrains decompiler
// Type: tk2dCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [ExecuteInEditMode]
    [AddComponentMenu("2D Toolkit/Camera/tk2dCamera")]
    public class tk2dCamera : MonoBehaviour
    {
      private static int CURRENT_VERSION = 1;
      public int version;
      [SerializeField]
      private tk2dCameraSettings cameraSettings = new tk2dCameraSettings();
      public tk2dCameraResolutionOverride[] resolutionOverride = new tk2dCameraResolutionOverride[1]
      {
        tk2dCameraResolutionOverride.DefaultOverride
      };
      [SerializeField]
      private tk2dCamera inheritSettings;
      public int nativeResolutionWidth = 960;
      public int nativeResolutionHeight = 640;
      [SerializeField]
      private Camera _unityCamera;
      private static tk2dCamera inst;
      private static List<tk2dCamera> allCameras = new List<tk2dCamera>();
      public bool viewportClippingEnabled;
      public Vector4 viewportRegion = new Vector4(0.0f, 0.0f, 100f, 100f);
      private Vector2 _targetResolution = Vector2.zero;
      [SerializeField]
      private float zoomFactor = 1f;
      [HideInInspector]
      public bool forceResolutionInEditor;
      [HideInInspector]
      public Vector2 forceResolution = new Vector2(960f, 640f);
      private Rect _screenExtents;
      private Rect _nativeScreenExtents;
      private Rect unitRect = new Rect(0.0f, 0.0f, 1f, 1f);
      private tk2dCamera _settingsRoot;

      public tk2dCameraSettings CameraSettings => this.cameraSettings;

      public tk2dCameraResolutionOverride CurrentResolutionOverride
      {
        get
        {
          tk2dCamera settingsRoot = this.SettingsRoot;
          Camera screenCamera = this.ScreenCamera;
          float pixelWidth = (float) screenCamera.pixelWidth;
          float pixelHeight = (float) screenCamera.pixelHeight;
          tk2dCameraResolutionOverride resolutionOverride1 = (tk2dCameraResolutionOverride) null;
          if (resolutionOverride1 == null || resolutionOverride1 != null && ((double) resolutionOverride1.width != (double) pixelWidth || (double) resolutionOverride1.height != (double) pixelHeight))
          {
            resolutionOverride1 = (tk2dCameraResolutionOverride) null;
            if (settingsRoot.resolutionOverride != null)
            {
              foreach (tk2dCameraResolutionOverride resolutionOverride2 in settingsRoot.resolutionOverride)
              {
                if (resolutionOverride2.Match((int) pixelWidth, (int) pixelHeight))
                {
                  resolutionOverride1 = resolutionOverride2;
                  break;
                }
              }
            }
          }
          return resolutionOverride1;
        }
      }

      public tk2dCamera InheritConfig
      {
        get => this.inheritSettings;
        set
        {
          if (!((UnityEngine.Object) this.inheritSettings != (UnityEngine.Object) value))
            return;
          this.inheritSettings = value;
          this._settingsRoot = (tk2dCamera) null;
        }
      }

      private Camera UnityCamera
      {
        get
        {
          if ((UnityEngine.Object) this._unityCamera == (UnityEngine.Object) null)
          {
            this._unityCamera = this.GetComponent<Camera>();
            if ((UnityEngine.Object) this._unityCamera == (UnityEngine.Object) null)
              Debug.LogError((object) "A unity camera must be attached to the tk2dCamera script");
          }
          return this._unityCamera;
        }
      }

      public static tk2dCamera Instance => tk2dCamera.inst;

      public static tk2dCamera CameraForLayer(int layer)
      {
        int num = 1 << layer;
        int count = tk2dCamera.allCameras.Count;
        for (int index = 0; index < count; ++index)
        {
          tk2dCamera allCamera = tk2dCamera.allCameras[index];
          if ((allCamera.UnityCamera.cullingMask & num) == num)
            return allCamera;
        }
        return (tk2dCamera) null;
      }

      public Rect ScreenExtents => this._screenExtents;

      public Rect NativeScreenExtents => this._nativeScreenExtents;

      public Vector2 TargetResolution => this._targetResolution;

      public Vector2 NativeResolution
      {
        get => new Vector2((float) this.nativeResolutionWidth, (float) this.nativeResolutionHeight);
      }

      [Obsolete]
      public Vector2 ScreenOffset
      {
        get
        {
          return new Vector2(this.ScreenExtents.xMin - this.NativeScreenExtents.xMin, this.ScreenExtents.yMin - this.NativeScreenExtents.yMin);
        }
      }

      [Obsolete]
      public Vector2 resolution => new Vector2(this.ScreenExtents.xMax, this.ScreenExtents.yMax);

      [Obsolete]
      public Vector2 ScreenResolution => new Vector2(this.ScreenExtents.xMax, this.ScreenExtents.yMax);

      [Obsolete]
      public Vector2 ScaledResolution
      {
        get => new Vector2(this.ScreenExtents.width, this.ScreenExtents.height);
      }

      public float ZoomFactor
      {
        get => this.zoomFactor;
        set => this.zoomFactor = Mathf.Max(0.01f, value);
      }

      [Obsolete]
      public float zoomScale
      {
        get => 1f / Mathf.Max(1f / 1000f, this.zoomFactor);
        set => this.ZoomFactor = 1f / Mathf.Max(1f / 1000f, value);
      }

      public Camera ScreenCamera
      {
        get
        {
          return this.viewportClippingEnabled && (UnityEngine.Object) this.inheritSettings != (UnityEngine.Object) null && this.inheritSettings.UnityCamera.rect == this.unitRect ? this.inheritSettings.UnityCamera : this.UnityCamera;
        }
      }

      private void Awake()
      {
        this.Upgrade();
        if (tk2dCamera.allCameras.IndexOf(this) == -1)
          tk2dCamera.allCameras.Add(this);
        tk2dCameraSettings cameraSettings = this.SettingsRoot.CameraSettings;
        if (cameraSettings.projection != tk2dCameraSettings.ProjectionType.Perspective)
          return;
        this.UnityCamera.transparencySortMode = cameraSettings.transparencySortMode;
      }

      private void OnEnable()
      {
        if ((UnityEngine.Object) this.UnityCamera != (UnityEngine.Object) null)
          this.UpdateCameraMatrix();
        else
          this.GetComponent<Camera>().enabled = false;
        if (!this.viewportClippingEnabled)
          tk2dCamera.inst = this;
        if (tk2dCamera.allCameras.IndexOf(this) != -1)
          return;
        tk2dCamera.allCameras.Add(this);
      }

      private void OnDestroy()
      {
        int index = tk2dCamera.allCameras.IndexOf(this);
        if (index == -1)
          return;
        tk2dCamera.allCameras.RemoveAt(index);
      }

      private void OnPreCull()
      {
        tk2dUpdateManager.FlushQueues();
        this.UpdateCameraMatrix();
      }

      public float GetSizeAtDistance(float distance)
      {
        tk2dCameraSettings cameraSettings = this.SettingsRoot.CameraSettings;
        switch (cameraSettings.projection)
        {
          case tk2dCameraSettings.ProjectionType.Orthographic:
            return cameraSettings.orthographicType == tk2dCameraSettings.OrthographicType.PixelsPerMeter ? 1f / cameraSettings.orthographicPixelsPerMeter : 2f * cameraSettings.orthographicSize / (float) this.SettingsRoot.nativeResolutionHeight;
          case tk2dCameraSettings.ProjectionType.Perspective:
            return (float) ((double) Mathf.Tan((float) ((double) this.CameraSettings.fieldOfView * (Math.PI / 180.0) * 0.5)) * (double) distance * 2.0) / (float) this.SettingsRoot.nativeResolutionHeight;
          default:
            return 1f;
        }
      }

      public tk2dCamera SettingsRoot
      {
        get
        {
          if ((UnityEngine.Object) this._settingsRoot == (UnityEngine.Object) null)
            this._settingsRoot = (UnityEngine.Object) this.inheritSettings == (UnityEngine.Object) null || (UnityEngine.Object) this.inheritSettings == (UnityEngine.Object) this ? this : this.inheritSettings.SettingsRoot;
          return this._settingsRoot;
        }
      }

      public Matrix4x4 OrthoOffCenter(
        Vector2 scale,
        float left,
        float right,
        float bottom,
        float top,
        float near,
        float far)
      {
        float num1 = (float) (2.0 / ((double) right - (double) left)) * scale.x;
        float num2 = (float) (2.0 / ((double) top - (double) bottom)) * scale.y;
        float num3 = (float) (-2.0 / ((double) far - (double) near));
        float num4 = (float) (-((double) right + (double) left) / ((double) right - (double) left));
        float num5 = (float) (-((double) bottom + (double) top) / ((double) top - (double) bottom));
        float num6 = (float) (-((double) far + (double) near) / ((double) far - (double) near));
        return new Matrix4x4()
        {
          [0, 0] = num1,
          [0, 1] = 0.0f,
          [0, 2] = 0.0f,
          [0, 3] = num4,
          [1, 0] = 0.0f,
          [1, 1] = num2,
          [1, 2] = 0.0f,
          [1, 3] = num5,
          [2, 0] = 0.0f,
          [2, 1] = 0.0f,
          [2, 2] = num3,
          [2, 3] = num6,
          [3, 0] = 0.0f,
          [3, 1] = 0.0f,
          [3, 2] = 0.0f,
          [3, 3] = 1f
        };
      }

      private Vector2 GetScaleForOverride(
        tk2dCamera settings,
        tk2dCameraResolutionOverride currentOverride,
        float width,
        float height)
      {
        Vector2 one = Vector2.one;
        if (currentOverride == null)
          return one;
        switch (currentOverride.autoScaleMode)
        {
          case tk2dCameraResolutionOverride.AutoScaleMode.FitWidth:
            float num1 = width / (float) settings.nativeResolutionWidth;
            one.Set(num1, num1);
            break;
          case tk2dCameraResolutionOverride.AutoScaleMode.FitHeight:
            float num2 = height / (float) settings.nativeResolutionHeight;
            one.Set(num2, num2);
            break;
          case tk2dCameraResolutionOverride.AutoScaleMode.FitVisible:
          case tk2dCameraResolutionOverride.AutoScaleMode.ClosestMultipleOfTwo:
            float num3 = (float) settings.nativeResolutionWidth / (float) settings.nativeResolutionHeight;
            float num4 = (double) (width / height) >= (double) num3 ? height / (float) settings.nativeResolutionHeight : width / (float) settings.nativeResolutionWidth;
            if (currentOverride.autoScaleMode == tk2dCameraResolutionOverride.AutoScaleMode.ClosestMultipleOfTwo)
              num4 = (double) num4 <= 1.0 ? Mathf.Pow(2f, Mathf.Floor(Mathf.Log(num4, 2f))) : Mathf.Floor(num4);
            one.Set(num4, num4);
            break;
          case tk2dCameraResolutionOverride.AutoScaleMode.StretchToFit:
            one.Set(width / (float) settings.nativeResolutionWidth, height / (float) settings.nativeResolutionHeight);
            break;
          case tk2dCameraResolutionOverride.AutoScaleMode.PixelPerfect:
            float num5 = 1f;
            one.Set(num5, num5);
            break;
          case tk2dCameraResolutionOverride.AutoScaleMode.Fill:
            float num6 = Mathf.Max(width / (float) settings.nativeResolutionWidth, height / (float) settings.nativeResolutionHeight);
            one.Set(num6, num6);
            break;
          default:
            float scale = currentOverride.scale;
            one.Set(scale, scale);
            break;
        }
        return one;
      }

      private Vector2 GetOffsetForOverride(
        tk2dCamera settings,
        tk2dCameraResolutionOverride currentOverride,
        Vector2 scale,
        float width,
        float height)
      {
        Vector2 offsetForOverride = Vector2.zero;
        if (currentOverride == null)
          return offsetForOverride;
        switch (currentOverride.fitMode)
        {
          case tk2dCameraResolutionOverride.FitMode.Center:
            if (settings.cameraSettings.orthographicOrigin == tk2dCameraSettings.OrthographicOrigin.BottomLeft)
            {
              offsetForOverride = new Vector2(Mathf.Round((float) (((double) settings.nativeResolutionWidth * (double) scale.x - (double) width) / 2.0)), Mathf.Round((float) (((double) settings.nativeResolutionHeight * (double) scale.y - (double) height) / 2.0)));
              break;
            }
            break;
          default:
            offsetForOverride = -currentOverride.offsetPixels;
            break;
        }
        return offsetForOverride;
      }

      private Matrix4x4 GetProjectionMatrixForOverride(
        tk2dCamera settings,
        tk2dCameraResolutionOverride currentOverride,
        float pixelWidth,
        float pixelHeight,
        bool halfTexelOffset,
        out Rect screenExtents,
        out Rect unscaledScreenExtents)
      {
        Vector2 scaleForOverride = this.GetScaleForOverride(settings, currentOverride, pixelWidth, pixelHeight);
        Vector2 offsetForOverride = this.GetOffsetForOverride(settings, currentOverride, scaleForOverride, pixelWidth, pixelHeight);
        float x1 = offsetForOverride.x;
        float y1 = offsetForOverride.y;
        float num1 = pixelWidth + offsetForOverride.x;
        float num2 = pixelHeight + offsetForOverride.y;
        Vector2 zero = Vector2.zero;
        if (this.viewportClippingEnabled && (UnityEngine.Object) this.InheritConfig != (UnityEngine.Object) null)
        {
          float num3 = (num1 - x1) / scaleForOverride.x;
          float num4 = (num2 - y1) / scaleForOverride.y;
          Vector4 vector4 = new Vector4((float) (int) this.viewportRegion.x, (float) (int) this.viewportRegion.y, (float) (int) this.viewportRegion.z, (float) (int) this.viewportRegion.w);
          float x2 = (float) (-(double) offsetForOverride.x / (double) pixelWidth + (double) vector4.x / (double) num3);
          float y2 = (float) (-(double) offsetForOverride.y / (double) pixelHeight + (double) vector4.y / (double) num4);
          float width = vector4.z / num3;
          float height = vector4.w / num4;
          if (settings.cameraSettings.orthographicOrigin == tk2dCameraSettings.OrthographicOrigin.Center)
          {
            x2 += (float) (((double) pixelWidth - (double) settings.nativeResolutionWidth * (double) scaleForOverride.x) / (double) pixelWidth / 2.0);
            y2 += (float) (((double) pixelHeight - (double) settings.nativeResolutionHeight * (double) scaleForOverride.y) / (double) pixelHeight / 2.0);
          }
          Rect rect = new Rect(x2, y2, width, height);
          if ((double) this.UnityCamera.rect.x != (double) x2 || (double) this.UnityCamera.rect.y != (double) y2 || (double) this.UnityCamera.rect.width != (double) width || (double) this.UnityCamera.rect.height != (double) height)
            this.UnityCamera.rect = rect;
          float num5 = Mathf.Min(1f - rect.x, rect.width);
          float num6 = Mathf.Min(1f - rect.y, rect.height);
          float num7 = vector4.x * scaleForOverride.x - offsetForOverride.x;
          float num8 = vector4.y * scaleForOverride.y - offsetForOverride.y;
          if (settings.cameraSettings.orthographicOrigin == tk2dCameraSettings.OrthographicOrigin.Center)
          {
            num7 -= (float) settings.nativeResolutionWidth * 0.5f * scaleForOverride.x;
            num8 -= (float) settings.nativeResolutionHeight * 0.5f * scaleForOverride.y;
          }
          if ((double) rect.x < 0.0)
          {
            num7 += -rect.x * pixelWidth;
            num5 = rect.x + rect.width;
          }
          if ((double) rect.y < 0.0)
          {
            num8 += -rect.y * pixelHeight;
            num6 = rect.y + rect.height;
          }
          x1 += num7;
          y1 += num8;
          num1 = pixelWidth * num5 + offsetForOverride.x + num7;
          num2 = pixelHeight * num6 + offsetForOverride.y + num8;
        }
        else
        {
          if (this.UnityCamera.rect != this.CameraSettings.rect)
            this.UnityCamera.rect = this.CameraSettings.rect;
          if (settings.cameraSettings.orthographicOrigin == tk2dCameraSettings.OrthographicOrigin.Center)
          {
            float num9 = (float) (((double) num1 - (double) x1) * 0.5);
            x1 -= num9;
            num1 -= num9;
            float num10 = (float) (((double) num2 - (double) y1) * 0.5);
            num2 -= num10;
            y1 -= num10;
            zero.Set((float) -this.nativeResolutionWidth / 2f, (float) -this.nativeResolutionHeight / 2f);
          }
        }
        float num11 = 1f / this.ZoomFactor;
        bool flag = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
        float num12 = !halfTexelOffset || !flag ? 0.0f : 0.5f;
        float num13 = settings.cameraSettings.orthographicSize;
        switch (settings.cameraSettings.orthographicType)
        {
          case tk2dCameraSettings.OrthographicType.PixelsPerMeter:
            num13 = 1f / settings.cameraSettings.orthographicPixelsPerMeter;
            break;
          case tk2dCameraSettings.OrthographicType.OrthographicSize:
            num13 = 2f * settings.cameraSettings.orthographicSize / (float) settings.nativeResolutionHeight;
            break;
        }
        float num14 = num13 * num11;
        screenExtents = new Rect(x1 * num14 / scaleForOverride.x, y1 * num14 / scaleForOverride.y, (num1 - x1) * num14 / scaleForOverride.x, (num2 - y1) * num14 / scaleForOverride.y);
        unscaledScreenExtents = new Rect(zero.x * num14, zero.y * num14, (float) this.nativeResolutionWidth * num14, (float) this.nativeResolutionHeight * num14);
        return this.OrthoOffCenter(scaleForOverride, num13 * (x1 + num12) * num11, num13 * (num1 + num12) * num11, num13 * (y1 - num12) * num11, num13 * (num2 - num12) * num11, this.UnityCamera.nearClipPlane, this.UnityCamera.farClipPlane);
      }

      private Vector2 GetScreenPixelDimensions(tk2dCamera settings)
      {
        return new Vector2((float) this.ScreenCamera.pixelWidth, (float) this.ScreenCamera.pixelHeight);
      }

      private void Upgrade()
      {
        if (this.version == tk2dCamera.CURRENT_VERSION)
          return;
        if (this.version == 0)
        {
          this.cameraSettings.orthographicPixelsPerMeter = 1f;
          this.cameraSettings.orthographicType = tk2dCameraSettings.OrthographicType.PixelsPerMeter;
          this.cameraSettings.orthographicOrigin = tk2dCameraSettings.OrthographicOrigin.BottomLeft;
          this.cameraSettings.projection = tk2dCameraSettings.ProjectionType.Orthographic;
          foreach (tk2dCameraResolutionOverride resolutionOverride in this.resolutionOverride)
            resolutionOverride.Upgrade(this.version);
          Camera component = this.GetComponent<Camera>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          {
            this.cameraSettings.rect = component.rect;
            if (!component.orthographic)
            {
              this.cameraSettings.projection = tk2dCameraSettings.ProjectionType.Perspective;
              this.cameraSettings.fieldOfView = component.fieldOfView * this.ZoomFactor;
            }
            component.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
          }
        }
        Debug.Log((object) $"tk2dCamera '{this.name}' - Upgraded from version {this.version.ToString()}");
        this.version = tk2dCamera.CURRENT_VERSION;
      }

      public void UpdateCameraMatrix()
      {
        this.Upgrade();
        if (!this.viewportClippingEnabled)
          tk2dCamera.inst = this;
        Camera unityCamera = this.UnityCamera;
        tk2dCamera settingsRoot = this.SettingsRoot;
        tk2dCameraSettings cameraSettings = settingsRoot.CameraSettings;
        if (unityCamera.rect != this.cameraSettings.rect)
          unityCamera.rect = this.cameraSettings.rect;
        this._targetResolution = this.GetScreenPixelDimensions(settingsRoot);
        if (cameraSettings.projection == tk2dCameraSettings.ProjectionType.Perspective)
        {
          if (unityCamera.orthographic)
            unityCamera.orthographic = false;
          float num = Mathf.Min(179.9f, cameraSettings.fieldOfView / Mathf.Max(1f / 1000f, this.ZoomFactor));
          if ((double) unityCamera.fieldOfView != (double) num)
            unityCamera.fieldOfView = num;
          this._screenExtents.Set(-unityCamera.aspect, -1f, unityCamera.aspect * 2f, 2f);
          this._nativeScreenExtents = this._screenExtents;
          unityCamera.ResetProjectionMatrix();
        }
        else
        {
          if (!unityCamera.orthographic)
            unityCamera.orthographic = true;
          Matrix4x4 matrix4x4 = this.GetProjectionMatrixForOverride(settingsRoot, settingsRoot.CurrentResolutionOverride, this._targetResolution.x, this._targetResolution.y, true, out this._screenExtents, out this._nativeScreenExtents);
          if (Application.platform == RuntimePlatform.WP8Player && (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight))
            matrix4x4 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, Screen.orientation != ScreenOrientation.LandscapeRight ? -90f : 90f), Vector3.one) * matrix4x4;
          if (!(unityCamera.projectionMatrix != matrix4x4))
            return;
          unityCamera.projectionMatrix = matrix4x4;
        }
      }
    }

}
