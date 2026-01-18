// Decompiled with JetBrains decompiler
// Type: AkInitializer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof (AkTerminator))]
[AddComponentMenu("Wwise/AkInitializer")]
public class AkInitializer : MonoBehaviour
  {
    public string basePath = AkSoundEngineController.s_DefaultBasePath;
    public string language = AkSoundEngineController.s_Language;
    public int defaultPoolSize = AkSoundEngineController.s_DefaultPoolSize;
    public int lowerPoolSize = AkSoundEngineController.s_LowerPoolSize;
    public int streamingPoolSize = AkSoundEngineController.s_StreamingPoolSize;
    public int preparePoolSize = AkSoundEngineController.s_PreparePoolSize;
    public float memoryCutoffThreshold = AkSoundEngineController.s_MemoryCutoffThreshold;
    public int monitorPoolSize = AkSoundEngineController.s_MonitorPoolSize;
    public int monitorQueuePoolSize = AkSoundEngineController.s_MonitorQueuePoolSize;
    public int callbackManagerBufferSize = AkSoundEngineController.s_CallbackManagerBufferSize;
    public int spatialAudioPoolSize = AkSoundEngineController.s_SpatialAudioPoolSize;
    [Range(0.0f, 8f)]
    public uint maxSoundPropagationDepth = 8;
    [Tooltip("Default Diffraction Flags combine all the diffraction flags")]
    public AkDiffractionFlags diffractionFlags = AkDiffractionFlags.DefaultDiffractionFlags;
    public bool engineLogging = AkSoundEngineController.s_EngineLogging;
    private static AkInitializer ms_Instance;

    public static string GetBasePath() => AkSoundEngineController.Instance.basePath;

    public static string GetCurrentLanguage() => AkSoundEngineController.Instance.language;

    private void Awake()
    {
      if ((bool) (Object) AkInitializer.ms_Instance)
      {
        Object.DestroyImmediate((Object) this);
      }
      else
      {
        AkInitializer.ms_Instance = this;
        Object.DontDestroyOnLoad((Object) this);
      }
    }

    private void OnEnable()
    {
      if (!((Object) AkInitializer.ms_Instance == (Object) this))
        return;
      AkSoundEngineController.Instance.Init(this);
    }

    private void OnDisable()
    {
      if (!((Object) AkInitializer.ms_Instance == (Object) this))
        return;
      AkSoundEngineController.Instance.OnDisable();
    }

    private void OnDestroy()
    {
      if (!((Object) AkInitializer.ms_Instance == (Object) this))
        return;
      AkInitializer.ms_Instance = (AkInitializer) null;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
      if (!((Object) AkInitializer.ms_Instance == (Object) this))
        return;
      AkSoundEngineController.Instance.OnApplicationPause(pauseStatus);
    }

    private void OnApplicationFocus(bool focus)
    {
      if (!((Object) AkInitializer.ms_Instance == (Object) this))
        return;
      AkSoundEngineController.Instance.OnApplicationFocus(focus);
    }

    private void OnApplicationQuit()
    {
      if (!((Object) AkInitializer.ms_Instance == (Object) this))
        return;
      AkSoundEngineController.Instance.Terminate();
    }

    private void LateUpdate()
    {
      if (!((Object) AkInitializer.ms_Instance == (Object) this))
        return;
      AkSoundEngineController.Instance.LateUpdate();
    }
  }

