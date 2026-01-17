// Decompiled with JetBrains decompiler
// Type: InControl.InputManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

#nullable disable
namespace InControl;

public class InputManager
{
  public static readonly VersionInfo Version = VersionInfo.InControlVersion();
  private static List<InputDeviceManager> deviceManagers = new List<InputDeviceManager>();
  private static Dictionary<System.Type, InputDeviceManager> deviceManagerTable = new Dictionary<System.Type, InputDeviceManager>();
  private static InputDevice activeDevice = InputDevice.Null;
  private static List<InputDevice> devices = new List<InputDevice>();
  private static List<PlayerActionSet> playerActionSets = new List<PlayerActionSet>();
  public static ReadOnlyCollection<InputDevice> Devices;
  private static bool applicationIsFocused;
  private static float initialTime;
  private static float currentTime;
  private static float lastUpdateTime;
  private static ulong currentTick;
  private static VersionInfo? unityVersion;
  private static bool enabled;
  private static bool enableXInput;

  public static event System.Action OnSetup;

  public static event Action<ulong, float> OnUpdate;

  public static event System.Action OnReset;

  public static event Action<InputDevice> OnDeviceAttached;

  public static event Action<InputDevice> OnDeviceDetached;

  public static event Action<InputDevice> OnActiveDeviceChanged;

  internal static event Action<ulong, float> OnUpdateDevices;

  internal static event Action<ulong, float> OnCommitDevices;

  public static bool CommandWasPressed { get; private set; }

  public static bool InvertYAxis { get; set; }

  public static bool IsSetup { get; private set; }

  internal static string Platform { get; private set; }

  [Obsolete("Use InputManager.CommandWasPressed instead.")]
  public static bool MenuWasPressed => InputManager.CommandWasPressed;

  [Obsolete("Calling InputManager.Setup() directly is no longer supported. Use the InControlManager component to manage the lifecycle of the input manager instead.", true)]
  public static void Setup() => InputManager.SetupInternal();

  internal static bool SetupInternal()
  {
    if (InputManager.IsSetup)
      return false;
    InputManager.Platform = Utility.GetWindowsVersion().ToUpper();
    InputManager.initialTime = 0.0f;
    InputManager.currentTime = 0.0f;
    InputManager.lastUpdateTime = 0.0f;
    InputManager.currentTick = 0UL;
    InputManager.applicationIsFocused = true;
    InputManager.deviceManagers.Clear();
    InputManager.deviceManagerTable.Clear();
    InputManager.devices.Clear();
    InputManager.Devices = new ReadOnlyCollection<InputDevice>((IList<InputDevice>) InputManager.devices);
    InputManager.activeDevice = InputDevice.Null;
    InputManager.playerActionSets.Clear();
    InputManager.IsSetup = true;
    bool flag = true;
    if (InputManager.EnableNativeInput && NativeInputDeviceManager.Enable())
      flag = false;
    if (InputManager.EnableXInput && flag)
      XInputDeviceManager.Enable();
    if (InputManager.OnSetup != null)
    {
      InputManager.OnSetup();
      InputManager.OnSetup = (System.Action) null;
    }
    if (flag)
      InputManager.AddDeviceManager<UnityInputDeviceManager>();
    return true;
  }

  [Obsolete("Calling InputManager.Reset() method directly is no longer supported. Use the InControlManager component to manage the lifecycle of the input manager instead.", true)]
  public static void Reset() => InputManager.ResetInternal();

  internal static void ResetInternal()
  {
    if (InputManager.OnReset != null)
      InputManager.OnReset();
    InputManager.OnSetup = (System.Action) null;
    InputManager.OnUpdate = (Action<ulong, float>) null;
    InputManager.OnReset = (System.Action) null;
    InputManager.OnActiveDeviceChanged = (Action<InputDevice>) null;
    InputManager.OnDeviceAttached = (Action<InputDevice>) null;
    InputManager.OnDeviceDetached = (Action<InputDevice>) null;
    InputManager.OnUpdateDevices = (Action<ulong, float>) null;
    InputManager.OnCommitDevices = (Action<ulong, float>) null;
    InputManager.DestroyDeviceManagers();
    InputManager.DestroyDevices();
    InputManager.playerActionSets.Clear();
    InputManager.IsSetup = false;
  }

  [Obsolete("Calling InputManager.Update() directly is no longer supported. Use the InControlManager component to manage the lifecycle of the input manager instead.", true)]
  public static void Update() => InputManager.UpdateInternal();

  internal static void UpdateInternal()
  {
    InputManager.AssertIsSetup();
    if (InputManager.OnSetup != null)
    {
      InputManager.OnSetup();
      InputManager.OnSetup = (System.Action) null;
    }
    if (!InputManager.enabled || InputManager.SuspendInBackground && !InputManager.applicationIsFocused)
      return;
    ++InputManager.currentTick;
    InputManager.UpdateCurrentTime();
    float deltaTime = InputManager.currentTime - InputManager.lastUpdateTime;
    InputManager.UpdateDeviceManagers(deltaTime);
    InputManager.CommandWasPressed = false;
    InputManager.UpdateDevices(deltaTime);
    InputManager.CommitDevices(deltaTime);
    InputManager.UpdateActiveDevice();
    InputManager.UpdatePlayerActionSets(deltaTime);
    if (InputManager.OnUpdate != null)
      InputManager.OnUpdate(InputManager.currentTick, deltaTime);
    InputManager.lastUpdateTime = InputManager.currentTime;
  }

  public static void Reload()
  {
    InputManager.ResetInternal();
    InputManager.SetupInternal();
  }

  private static void AssertIsSetup()
  {
    if (!InputManager.IsSetup)
      throw new Exception("InputManager is not initialized. Call InputManager.Setup() first.");
  }

  private static void SetZeroTickOnAllControls()
  {
    int count1 = InputManager.devices.Count;
    for (int index1 = 0; index1 < count1; ++index1)
    {
      ReadOnlyCollection<InputControl> controls = InputManager.devices[index1].Controls;
      int count2 = controls.Count;
      for (int index2 = 0; index2 < count2; ++index2)
        controls[index2]?.SetZeroTick();
    }
  }

  public static void ClearInputState()
  {
    int count1 = InputManager.devices.Count;
    for (int index = 0; index < count1; ++index)
      InputManager.devices[index].ClearInputState();
    int count2 = InputManager.playerActionSets.Count;
    for (int index = 0; index < count2; ++index)
      InputManager.playerActionSets[index].ClearInputState();
    InputManager.activeDevice = InputDevice.Null;
  }

  internal static void OnApplicationFocus(bool focusState)
  {
    if (!focusState)
    {
      if (InputManager.SuspendInBackground)
        InputManager.ClearInputState();
      InputManager.SetZeroTickOnAllControls();
    }
    InputManager.applicationIsFocused = focusState;
  }

  internal static void OnApplicationPause(bool pauseState)
  {
  }

  internal static void OnApplicationQuit() => InputManager.ResetInternal();

  internal static void OnLevelWasLoaded()
  {
    InputManager.SetZeroTickOnAllControls();
    InputManager.UpdateInternal();
  }

  public static void AddDeviceManager(InputDeviceManager deviceManager)
  {
    InputManager.AssertIsSetup();
    System.Type type = deviceManager.GetType();
    if (InputManager.deviceManagerTable.ContainsKey(type))
    {
      Logger.LogError($"A device manager of type '{type.Name}' already exists; cannot add another.");
    }
    else
    {
      InputManager.deviceManagers.Add(deviceManager);
      InputManager.deviceManagerTable.Add(type, deviceManager);
      deviceManager.Update(InputManager.currentTick, InputManager.currentTime - InputManager.lastUpdateTime);
    }
  }

  public static void AddDeviceManager<T>() where T : InputDeviceManager, new()
  {
    InputManager.AddDeviceManager((InputDeviceManager) new T());
  }

  public static T GetDeviceManager<T>() where T : InputDeviceManager
  {
    InputDeviceManager inputDeviceManager;
    return InputManager.deviceManagerTable.TryGetValue(typeof (T), out inputDeviceManager) ? inputDeviceManager as T : (T) null;
  }

  public static bool HasDeviceManager<T>() where T : InputDeviceManager
  {
    return InputManager.deviceManagerTable.ContainsKey(typeof (T));
  }

  private static void UpdateCurrentTime()
  {
    if ((double) InputManager.initialTime < 1.4012984643248171E-45)
      InputManager.initialTime = UnityEngine.Time.realtimeSinceStartup;
    InputManager.currentTime = Mathf.Max(0.0f, UnityEngine.Time.realtimeSinceStartup - InputManager.initialTime);
  }

  private static void UpdateDeviceManagers(float deltaTime)
  {
    int count = InputManager.deviceManagers.Count;
    for (int index = 0; index < count; ++index)
      InputManager.deviceManagers[index].Update(InputManager.currentTick, deltaTime);
  }

  private static void DestroyDeviceManagers()
  {
    int count = InputManager.deviceManagers.Count;
    for (int index = 0; index < count; ++index)
      InputManager.deviceManagers[index].Destroy();
    InputManager.deviceManagers.Clear();
    InputManager.deviceManagerTable.Clear();
  }

  private static void DestroyDevices()
  {
    int count = InputManager.devices.Count;
    for (int index = 0; index < count; ++index)
      InputManager.devices[index].OnDetached();
    InputManager.devices.Clear();
    InputManager.activeDevice = InputDevice.Null;
  }

  private static void UpdateDevices(float deltaTime)
  {
    int count = InputManager.devices.Count;
    for (int index = 0; index < count; ++index)
      InputManager.devices[index].Update(InputManager.currentTick, deltaTime);
    if (InputManager.OnUpdateDevices == null)
      return;
    InputManager.OnUpdateDevices(InputManager.currentTick, deltaTime);
  }

  private static void CommitDevices(float deltaTime)
  {
    int count = InputManager.devices.Count;
    for (int index = 0; index < count; ++index)
    {
      InputDevice device = InputManager.devices[index];
      device.Commit(InputManager.currentTick, deltaTime);
      if (device.CommandWasPressed)
        InputManager.CommandWasPressed = true;
    }
    if (InputManager.OnCommitDevices == null)
      return;
    InputManager.OnCommitDevices(InputManager.currentTick, deltaTime);
  }

  private static void UpdateActiveDevice()
  {
    InputDevice activeDevice = InputManager.ActiveDevice;
    if (InputManager.ActiveDevice is XInputDevice && !GameManager.Options.allowXinputControllers)
      InputManager.ActiveDevice = (InputDevice) null;
    if (!(InputManager.ActiveDevice is XInputDevice) && !GameManager.Options.allowNonXinputControllers)
      InputManager.ActiveDevice = (InputDevice) null;
    int count = InputManager.devices.Count;
    for (int index = 0; index < count; ++index)
    {
      InputDevice device = InputManager.devices[index];
      if ((GameManager.Options.allowXinputControllers || !(device is XInputDevice)) && (GameManager.Options.allowNonXinputControllers || device is XInputDevice) && device.LastChangedAfter(InputManager.ActiveDevice) && !device.Passive)
      {
        if (InputManager.ActiveDevice is XInputDevice && !(device is XInputDevice) || device is XInputDevice && !(InputManager.ActiveDevice is XInputDevice))
        {
          if (device.LastChangeAfterTime(InputManager.ActiveDevice))
            InputManager.ActiveDevice = device;
          else if (device is XInputDevice)
            InputManager.ActiveDevice = device;
        }
        else
          InputManager.ActiveDevice = device;
      }
    }
    if (activeDevice == InputManager.ActiveDevice)
      return;
    if (GameManager.HasInstance && GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER)
    {
      activeDevice?.Vibrate(0.0f);
      Debug.LogWarningFormat("swapping active device from {0} ({1}|{2}) to {3} ({4}|{5})", (object) activeDevice.Name, (object) activeDevice.LastChangeTick, (object) activeDevice.LastChangeTime, (object) InputManager.ActiveDevice.Name, (object) InputManager.ActiveDevice.LastChangeTick, (object) InputManager.ActiveDevice.LastChangeTime);
    }
    if (InputManager.OnActiveDeviceChanged == null)
      return;
    InputManager.OnActiveDeviceChanged(InputManager.ActiveDevice);
  }

  public static void AttachDevice(InputDevice inputDevice)
  {
    InputManager.AssertIsSetup();
    if (!inputDevice.IsSupportedOnThisPlatform || inputDevice.IsAttached)
      return;
    if (!InputManager.devices.Contains(inputDevice))
    {
      InputManager.devices.Add(inputDevice);
      InputManager.devices.Sort((Comparison<InputDevice>) ((d1, d2) => d1.SortOrder.CompareTo(d2.SortOrder)));
    }
    inputDevice.OnAttached();
    if (InputManager.OnDeviceAttached == null)
      return;
    InputManager.OnDeviceAttached(inputDevice);
  }

  public static void DetachDevice(InputDevice inputDevice)
  {
    if (!InputManager.IsSetup || !inputDevice.IsAttached)
      return;
    InputManager.devices.Remove(inputDevice);
    if (InputManager.ActiveDevice == inputDevice)
      InputManager.ActiveDevice = InputDevice.Null;
    inputDevice.OnDetached();
    if (InputManager.OnDeviceDetached == null)
      return;
    InputManager.OnDeviceDetached(inputDevice);
  }

  public static void HideDevicesWithProfile(System.Type type)
  {
    if (!type.IsSubclassOf(typeof (UnityInputDeviceProfile)))
      return;
    InputDeviceProfile.Hide(type);
  }

  internal static void AttachPlayerActionSet(PlayerActionSet playerActionSet)
  {
    if (InputManager.playerActionSets.Contains(playerActionSet))
      return;
    InputManager.playerActionSets.Add(playerActionSet);
  }

  internal static void DetachPlayerActionSet(PlayerActionSet playerActionSet)
  {
    InputManager.playerActionSets.Remove(playerActionSet);
  }

  internal static void UpdatePlayerActionSets(float deltaTime)
  {
    int count = InputManager.playerActionSets.Count;
    for (int index = 0; index < count; ++index)
      InputManager.playerActionSets[index].Update(InputManager.currentTick, deltaTime);
  }

  public static bool AnyKeyIsPressed => KeyCombo.Detect(true).IncludeCount > 0;

  public static InputDevice GetActiveDeviceForPlayer(int playerID)
  {
    if (GameManager.Instance.AllPlayers.Length < 2)
      return InputManager.ActiveDevice;
    int index = playerID;
    if (GameManager.Options.PlayerIDtoDeviceIndexMap.ContainsKey(playerID))
      index = GameManager.Options.PlayerIDtoDeviceIndexMap[playerID];
    else
      GameManager.Options.PlayerIDtoDeviceIndexMap.Add(playerID, playerID);
    return index >= InputManager.devices.Count || index < 0 ? (InputDevice) null : InputManager.devices[index];
  }

  public static InputDevice ActiveDevice
  {
    get => InputManager.activeDevice == null ? InputDevice.Null : InputManager.activeDevice;
    set => InputManager.activeDevice = value != null ? value : InputDevice.Null;
  }

  public static bool Enabled
  {
    get => InputManager.enabled;
    set
    {
      if (InputManager.enabled == value)
        return;
      if (value)
      {
        InputManager.SetZeroTickOnAllControls();
        InputManager.UpdateInternal();
      }
      else
      {
        InputManager.ClearInputState();
        InputManager.SetZeroTickOnAllControls();
      }
      InputManager.enabled = value;
    }
  }

  public static bool SuspendInBackground { get; internal set; }

  public static bool EnableNativeInput { get; internal set; }

  public static bool EnableXInput
  {
    get
    {
      return Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || InputManager.enableXInput;
    }
    set => InputManager.enableXInput = value;
  }

  public static uint XInputUpdateRate { get; internal set; }

  public static uint XInputBufferSize { get; internal set; }

  public static bool NativeInputEnableXInput { get; internal set; }

  public static bool NativeInputPreventSleep { get; internal set; }

  public static uint NativeInputUpdateRate { get; internal set; }

  public static bool EnableICade { get; internal set; }

  internal static VersionInfo UnityVersion
  {
    get
    {
      if (!InputManager.unityVersion.HasValue)
        InputManager.unityVersion = new VersionInfo?(VersionInfo.UnityVersion());
      return InputManager.unityVersion.Value;
    }
  }

  public static ulong GetCurrentTick() => InputManager.CurrentTick;

  internal static ulong CurrentTick => InputManager.currentTick;
}
