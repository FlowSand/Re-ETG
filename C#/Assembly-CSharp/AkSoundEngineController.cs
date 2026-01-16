// Decompiled with JetBrains decompiler
// Type: AkSoundEngineController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.IO;
using System.Threading;
using UnityEngine;

#nullable disable
public class AkSoundEngineController
{
  public static readonly string s_DefaultBasePath = Path.Combine("Audio", "GeneratedSoundBanks");
  public static string s_Language = "English(US)";
  public static int s_DefaultPoolSize = 16384 /*0x4000*/;
  public static int s_LowerPoolSize = 16384 /*0x4000*/;
  public static int s_StreamingPoolSize = 2048 /*0x0800*/;
  public static int s_PreparePoolSize = 0;
  public static float s_MemoryCutoffThreshold = 0.95f;
  public static int s_MonitorPoolSize = 128 /*0x80*/;
  public static int s_MonitorQueuePoolSize = 64 /*0x40*/;
  public static int s_CallbackManagerBufferSize = 4;
  public static bool s_EngineLogging = true;
  public static int s_SpatialAudioPoolSize = 8194;
  public string basePath = AkSoundEngineController.s_DefaultBasePath;
  public string language = AkSoundEngineController.s_Language;
  public bool engineLogging = AkSoundEngineController.s_EngineLogging;
  private static AkSoundEngineController ms_Instance;

  public static AkSoundEngineController Instance
  {
    get
    {
      if (AkSoundEngineController.ms_Instance == null)
        AkSoundEngineController.ms_Instance = new AkSoundEngineController();
      return AkSoundEngineController.ms_Instance;
    }
  }

  ~AkSoundEngineController()
  {
    if (AkSoundEngineController.ms_Instance != this)
      return;
    AkSoundEngineController.ms_Instance = (AkSoundEngineController) null;
  }

  public static string GetDecodedBankFolder() => "DecodedBanks";

  public static string GetDecodedBankFullPath()
  {
    return Path.Combine(AkBasePathGetter.GetPlatformBasePath(), AkSoundEngineController.GetDecodedBankFolder());
  }

  public void LateUpdate()
  {
    AkCallbackManager.PostCallbacks();
    AkBankManager.DoUnloadBanks();
    int num = (int) AkSoundEngine.RenderAudio();
  }

  public void Init(AkInitializer akInitializer)
  {
    bool flag = AkSoundEngine.IsInitialized();
    this.engineLogging = akInitializer.engineLogging;
    AkLogger.Instance.Init();
    if (flag)
      return;
    Debug.Log((object) "WwiseUnity: Initialize sound engine ...");
    this.basePath = akInitializer.basePath;
    this.language = akInitializer.language;
    AkMemSettings in_pMemSettings = new AkMemSettings();
    in_pMemSettings.uMaxNumPools = 20U;
    AkDeviceSettings akDeviceSettings = new AkDeviceSettings();
    AkSoundEngine.GetDefaultDeviceSettings(akDeviceSettings);
    AkStreamMgrSettings in_pStmSettings = new AkStreamMgrSettings();
    in_pStmSettings.uMemorySize = (uint) (akInitializer.streamingPoolSize * 1024 /*0x0400*/);
    AkInitSettings akInitSettings = new AkInitSettings();
    AkSoundEngine.GetDefaultInitSettings(akInitSettings);
    akInitSettings.uDefaultPoolSize = (uint) (akInitializer.defaultPoolSize * 1024 /*0x0400*/);
    akInitSettings.uMonitorPoolSize = (uint) (akInitializer.monitorPoolSize * 1024 /*0x0400*/);
    akInitSettings.uMonitorQueuePoolSize = (uint) (akInitializer.monitorQueuePoolSize * 1024 /*0x0400*/);
    akInitSettings.szPluginDLLPath = Path.Combine(Application.dataPath, "Plugins" + (object) Path.DirectorySeparatorChar);
    AkPlatformInitSettings platformInitSettings = new AkPlatformInitSettings();
    AkSoundEngine.GetDefaultPlatformInitSettings(platformInitSettings);
    platformInitSettings.uLEngineDefaultPoolSize = (uint) (akInitializer.lowerPoolSize * 1024 /*0x0400*/);
    platformInitSettings.fLEngineDefaultPoolRatioThreshold = akInitializer.memoryCutoffThreshold;
    AkMusicSettings akMusicSettings = new AkMusicSettings();
    AkSoundEngine.GetDefaultMusicSettings(akMusicSettings);
    AkSpatialAudioInitSettings in_pSpatialAudioSettings = new AkSpatialAudioInitSettings();
    in_pSpatialAudioSettings.uPoolSize = (uint) (akInitializer.spatialAudioPoolSize * 1024 /*0x0400*/);
    in_pSpatialAudioSettings.uMaxSoundPropagationDepth = akInitializer.maxSoundPropagationDepth;
    in_pSpatialAudioSettings.uDiffractionFlags = (uint) akInitializer.diffractionFlags;
    int num1 = (int) AkSoundEngine.SetGameName(Application.productName);
    AKRESULT akresult1 = AkSoundEngine.Init(in_pMemSettings, in_pStmSettings, akDeviceSettings, akInitSettings, platformInitSettings, akMusicSettings, in_pSpatialAudioSettings, (uint) (akInitializer.preparePoolSize * 1024 /*0x0400*/));
    if (akresult1 != AKRESULT.AK_Success)
    {
      Debug.LogError((object) ("WwiseUnity: Failed to initialize the sound engine. Abort. :" + akresult1.ToString()));
      AkSoundEngine.Term();
    }
    else
    {
      string soundbankBasePath = AkBasePathGetter.GetSoundbankBasePath();
      if (string.IsNullOrEmpty(soundbankBasePath))
      {
        Debug.LogError((object) "WwiseUnity: Couldn't find soundbanks base path. Terminate sound engine.");
        AkSoundEngine.Term();
      }
      else if (AkSoundEngine.SetBasePath(soundbankBasePath) != AKRESULT.AK_Success)
      {
        Debug.LogError((object) "WwiseUnity: Failed to set soundbanks base path. Terminate sound engine.");
        AkSoundEngine.Term();
      }
      else
      {
        string decodedBankFullPath = AkSoundEngineController.GetDecodedBankFullPath();
        int num2 = (int) AkSoundEngine.SetDecodedBankPath(decodedBankFullPath);
        int num3 = (int) AkSoundEngine.SetCurrentLanguage(this.language);
        int num4 = (int) AkSoundEngine.AddBasePath(Application.persistentDataPath + (object) Path.DirectorySeparatorChar);
        int num5 = (int) AkSoundEngine.AddBasePath(decodedBankFullPath);
        if (AkCallbackManager.Init(akInitializer.callbackManagerBufferSize * 1024 /*0x0400*/) != AKRESULT.AK_Success)
        {
          Debug.LogError((object) "WwiseUnity: Failed to initialize Callback Manager. Terminate sound engine.");
          AkSoundEngine.Term();
        }
        else
        {
          AkBankManager.Reset();
          Debug.Log((object) "WwiseUnity: Sound engine initialized.");
          AKRESULT akresult2 = AkSoundEngine.LoadBank("Init.bnk", -1, out uint _);
          if (akresult2 == AKRESULT.AK_Success)
            return;
          Debug.LogError((object) ("WwiseUnity: Failed load Init.bnk with result: " + (object) akresult2));
        }
      }
    }
  }

  public void OnDisable()
  {
  }

  public void Terminate()
  {
    if (!AkSoundEngine.IsInitialized())
      return;
    AkSoundEngine.StopAll();
    int num1 = (int) AkSoundEngine.ClearBanks();
    int num2 = (int) AkSoundEngine.RenderAudio();
    int num3 = 5;
    do
    {
      int num4;
      do
      {
        num4 = AkCallbackManager.PostCallbacks();
        using (EventWaitHandle eventWaitHandle = (EventWaitHandle) new ManualResetEvent(false))
          eventWaitHandle.WaitOne(TimeSpan.FromMilliseconds(1.0));
      }
      while (num4 > 0);
      using (EventWaitHandle eventWaitHandle = (EventWaitHandle) new ManualResetEvent(false))
        eventWaitHandle.WaitOne(TimeSpan.FromMilliseconds(10.0));
      --num3;
    }
    while (num3 > 0);
    AkSoundEngine.Term();
    AkCallbackManager.PostCallbacks();
    AkCallbackManager.Term();
    AkBankManager.Reset();
  }

  public void OnApplicationPause(bool pauseStatus)
  {
    AkSoundEngineController.ActivateAudio(!pauseStatus);
  }

  public void OnApplicationFocus(bool focus) => AkSoundEngineController.ActivateAudio(focus);

  private static void ActivateAudio(bool activate)
  {
    if (!AkSoundEngine.IsInitialized())
      return;
    if (activate)
    {
      int num1 = (int) AkSoundEngine.WakeupFromSuspend();
    }
    else
    {
      int num2 = (int) AkSoundEngine.Suspend();
    }
    int num3 = (int) AkSoundEngine.RenderAudio();
  }
}
