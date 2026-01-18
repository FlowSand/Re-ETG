// Decompiled with JetBrains decompiler
// Type: tk2dSpriteCollectionSize
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

[Serializable]
public class tk2dSpriteCollectionSize
  {
    public tk2dSpriteCollectionSize.Type type = tk2dSpriteCollectionSize.Type.PixelsPerMeter;
    public float orthoSize = 10f;
    public float pixelsPerMeter = 100f;
    public float width = 960f;
    public float height = 640f;

    public static tk2dSpriteCollectionSize Explicit(float orthoSize, float targetHeight)
    {
      return tk2dSpriteCollectionSize.ForResolution(orthoSize, targetHeight, targetHeight);
    }

    public static tk2dSpriteCollectionSize PixelsPerMeter(float pixelsPerMeter)
    {
      return new tk2dSpriteCollectionSize()
      {
        type = tk2dSpriteCollectionSize.Type.PixelsPerMeter,
        pixelsPerMeter = pixelsPerMeter
      };
    }

    public static tk2dSpriteCollectionSize ForResolution(float orthoSize, float width, float height)
    {
      return new tk2dSpriteCollectionSize()
      {
        type = tk2dSpriteCollectionSize.Type.Explicit,
        orthoSize = orthoSize,
        width = width,
        height = height
      };
    }

    public static tk2dSpriteCollectionSize ForTk2dCamera()
    {
      return new tk2dSpriteCollectionSize()
      {
        type = tk2dSpriteCollectionSize.Type.PixelsPerMeter,
        pixelsPerMeter = 1f
      };
    }

    public static tk2dSpriteCollectionSize ForTk2dCamera(tk2dCamera camera)
    {
      tk2dSpriteCollectionSize spriteCollectionSize = new tk2dSpriteCollectionSize();
      tk2dCameraSettings cameraSettings = camera.SettingsRoot.CameraSettings;
      if (cameraSettings.projection == tk2dCameraSettings.ProjectionType.Orthographic)
      {
        switch (cameraSettings.orthographicType)
        {
          case tk2dCameraSettings.OrthographicType.PixelsPerMeter:
            spriteCollectionSize.type = tk2dSpriteCollectionSize.Type.PixelsPerMeter;
            spriteCollectionSize.pixelsPerMeter = cameraSettings.orthographicPixelsPerMeter;
            break;
          case tk2dCameraSettings.OrthographicType.OrthographicSize:
            spriteCollectionSize.type = tk2dSpriteCollectionSize.Type.Explicit;
            spriteCollectionSize.height = (float) camera.nativeResolutionHeight;
            spriteCollectionSize.orthoSize = cameraSettings.orthographicSize;
            break;
        }
      }
      else if (cameraSettings.projection == tk2dCameraSettings.ProjectionType.Perspective)
      {
        spriteCollectionSize.type = tk2dSpriteCollectionSize.Type.PixelsPerMeter;
        spriteCollectionSize.pixelsPerMeter = 100f;
      }
      return spriteCollectionSize;
    }

    public static tk2dSpriteCollectionSize Default() => tk2dSpriteCollectionSize.PixelsPerMeter(100f);

    public void CopyFromLegacy(bool useTk2dCamera, float orthoSize, float targetHeight)
    {
      if (useTk2dCamera)
      {
        this.type = tk2dSpriteCollectionSize.Type.PixelsPerMeter;
        this.pixelsPerMeter = 1f;
      }
      else
      {
        this.type = tk2dSpriteCollectionSize.Type.Explicit;
        this.height = targetHeight;
        this.orthoSize = orthoSize;
      }
    }

    public void CopyFrom(tk2dSpriteCollectionSize source)
    {
      this.type = source.type;
      this.width = source.width;
      this.height = source.height;
      this.orthoSize = source.orthoSize;
      this.pixelsPerMeter = source.pixelsPerMeter;
    }

    public float OrthoSize
    {
      get
      {
        switch (this.type)
        {
          case tk2dSpriteCollectionSize.Type.Explicit:
            return this.orthoSize;
          case tk2dSpriteCollectionSize.Type.PixelsPerMeter:
            return 0.5f;
          default:
            return this.orthoSize;
        }
      }
    }

    public float TargetHeight
    {
      get
      {
        switch (this.type)
        {
          case tk2dSpriteCollectionSize.Type.Explicit:
            return this.height;
          case tk2dSpriteCollectionSize.Type.PixelsPerMeter:
            return this.pixelsPerMeter;
          default:
            return this.height;
        }
      }
    }

    public enum Type
    {
      Explicit,
      PixelsPerMeter,
    }
  }

