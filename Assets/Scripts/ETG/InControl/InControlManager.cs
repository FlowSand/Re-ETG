using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#nullable disable
namespace InControl
{
  public class InControlManager : SingletonMonoBehavior<InControlManager, MonoBehaviour>
  {
    public bool logDebugInfo;
    public bool invertYAxis;
    public bool useFixedUpdate;
    public bool dontDestroyOnLoad;
    public bool suspendInBackground;
    public bool enableICade;
    public bool enableXInput;
    public bool xInputOverrideUpdateRate;
    public int xInputUpdateRate;
    public bool xInputOverrideBufferSize;
    public int xInputBufferSize;
    public bool enableNativeInput;
    public bool nativeInputEnableXInput = true;
    public bool nativeInputPreventSleep;
    public bool nativeInputOverrideUpdateRate;
    public int nativeInputUpdateRate;
    public List<string> customProfiles = new List<string>();

    private void OnEnable()
    {
      if (!this.EnforceSingleton())
        return;
      if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        this.enableXInput = true;
      InputManager.InvertYAxis = this.invertYAxis;
      InputManager.SuspendInBackground = this.suspendInBackground;
      InputManager.EnableICade = this.enableICade;
      InputManager.EnableXInput = this.enableXInput;
      InputManager.XInputUpdateRate = (uint) Mathf.Max(this.xInputUpdateRate, 0);
      InputManager.XInputBufferSize = (uint) Mathf.Max(this.xInputBufferSize, 0);
      InputManager.EnableNativeInput = this.enableNativeInput;
      InputManager.NativeInputEnableXInput = this.nativeInputEnableXInput;
      InputManager.NativeInputUpdateRate = (uint) Mathf.Max(this.nativeInputUpdateRate, 0);
      InputManager.NativeInputPreventSleep = this.nativeInputPreventSleep;
      if (InputManager.SetupInternal())
      {
        Debug.Log((object) $"InControl (version {(object) InputManager.Version})");
        // ISSUE: reference to a compiler-generated field
        if (InControlManager._f__mg_cache0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          InControlManager._f__mg_cache0 = new Action<InControl.LogMessage>(InControlManager.LogMessage);
        }
        // ISSUE: reference to a compiler-generated field
        Logger.OnLogMessage -= InControlManager._f__mg_cache0;
        // ISSUE: reference to a compiler-generated field
        if (InControlManager._f__mg_cache1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          InControlManager._f__mg_cache1 = new Action<InControl.LogMessage>(InControlManager.LogMessage);
        }
        // ISSUE: reference to a compiler-generated field
        Logger.OnLogMessage += InControlManager._f__mg_cache1;
        foreach (string customProfile in this.customProfiles)
        {
          System.Type type = System.Type.GetType(customProfile);
          if (type == null)
            Debug.LogError((object) ("Cannot find class for custom profile: " + customProfile));
          else if (Activator.CreateInstance(type) is UnityInputDeviceProfileBase instance)
            InputManager.AttachDevice((InputDevice) new UnityInputDevice(instance));
        }
      }
      SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(this.OnSceneWasLoaded);
      SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.OnSceneWasLoaded);
      if (!this.dontDestroyOnLoad)
        return;
      UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this);
    }

    private void OnDisable()
    {
      SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(this.OnSceneWasLoaded);
      if (!((UnityEngine.Object) SingletonMonoBehavior<InControlManager, MonoBehaviour>.Instance == (UnityEngine.Object) this))
        return;
      InputManager.ResetInternal();
    }

    private void Update()
    {
      if (this.useFixedUpdate && !Utility.IsZero(Time.timeScale))
        return;
      InputManager.UpdateInternal();
    }

    private void FixedUpdate()
    {
      if (!this.useFixedUpdate)
        return;
      InputManager.UpdateInternal();
    }

    private void OnApplicationFocus(bool focusState) => InputManager.OnApplicationFocus(focusState);

    private void OnApplicationPause(bool pauseState) => InputManager.OnApplicationPause(pauseState);

    private void OnApplicationQuit() => InputManager.OnApplicationQuit();

    private void OnSceneWasLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
      InputManager.OnLevelWasLoaded();
    }

    private static void LogMessage(InControl.LogMessage logMessage)
    {
      switch (logMessage.type)
      {
        case LogMessageType.Info:
          Debug.Log((object) logMessage.text);
          break;
        case LogMessageType.Warning:
          Debug.LogWarning((object) logMessage.text);
          break;
        case LogMessageType.Error:
          Debug.LogError((object) logMessage.text);
          break;
      }
    }
  }
}
