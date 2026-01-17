// Decompiled with JetBrains decompiler
// Type: InControl.UnityInputDeviceManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace InControl;

public class UnityInputDeviceManager : InputDeviceManager
{
  public static Action OnAllDevicesReattached;
  private const float deviceRefreshInterval = 1f;
  private float deviceRefreshTimer;
  private List<UnityInputDeviceProfileBase> systemDeviceProfiles = new List<UnityInputDeviceProfileBase>(UnityInputDeviceProfileList.Profiles.Length);
  private List<UnityInputDeviceProfileBase> customDeviceProfiles = new List<UnityInputDeviceProfileBase>();
  private string[] joystickNames;
  private int lastJoystickCount;
  private int lastJoystickHash;
  private int joystickCount;
  private int joystickHash;

  public UnityInputDeviceManager()
  {
    this.AddSystemDeviceProfiles();
    this.QueryJoystickInfo();
    this.AttachDevices();
  }

  public override void Update(ulong updateTick, float deltaTime)
  {
    this.deviceRefreshTimer += deltaTime;
    if ((double) this.deviceRefreshTimer < 1.0)
      return;
    this.deviceRefreshTimer = 0.0f;
    this.QueryJoystickInfo();
    if (!this.JoystickInfoHasChanged)
      return;
    Logger.LogInfo("Change in attached Unity joysticks detected; refreshing device list.");
    this.DetachDevices();
    this.AttachDevices();
  }

  private void QueryJoystickInfo()
  {
    this.joystickNames = Input.GetJoystickNames();
    this.joystickCount = this.joystickNames.Length;
    this.joystickHash = 527 + this.joystickCount;
    for (int index = 0; index < this.joystickCount; ++index)
      this.joystickHash = this.joystickHash * 31 /*0x1F*/ + this.joystickNames[index].GetHashCode();
  }

  private bool JoystickInfoHasChanged
  {
    get
    {
      return this.joystickHash != this.lastJoystickHash || this.joystickCount != this.lastJoystickCount;
    }
  }

  private void AttachDevices()
  {
    this.AttachKeyboardDevices();
    this.AttachJoystickDevices();
    this.lastJoystickCount = this.joystickCount;
    this.lastJoystickHash = this.joystickHash;
    if (UnityInputDeviceManager.OnAllDevicesReattached == null)
      return;
    UnityInputDeviceManager.OnAllDevicesReattached();
  }

  private void DetachDevices()
  {
    int count = this.devices.Count;
    for (int index = 0; index < count; ++index)
      InputManager.DetachDevice(this.devices[index]);
    this.devices.Clear();
  }

  public void ReloadDevices()
  {
    this.QueryJoystickInfo();
    this.DetachDevices();
    this.AttachDevices();
  }

  private void AttachDevice(UnityInputDevice device)
  {
    this.devices.Add((InputDevice) device);
    InputManager.AttachDevice((InputDevice) device);
  }

  private void AttachKeyboardDevices()
  {
    int count = this.systemDeviceProfiles.Count;
    for (int index = 0; index < count; ++index)
    {
      UnityInputDeviceProfileBase systemDeviceProfile = this.systemDeviceProfiles[index];
      if (systemDeviceProfile.IsNotJoystick && systemDeviceProfile.IsSupportedOnThisPlatform)
        this.AttachDevice(new UnityInputDevice(systemDeviceProfile));
    }
  }

  private void AttachJoystickDevices()
  {
    try
    {
      for (int index = 0; index < this.joystickCount; ++index)
        this.DetectJoystickDevice(index + 1, this.joystickNames[index]);
    }
    catch (Exception ex)
    {
      Logger.LogError(ex.Message);
      Logger.LogError(ex.StackTrace);
    }
  }

  private bool HasAttachedDeviceWithJoystickId(int unityJoystickId)
  {
    int count = this.devices.Count;
    for (int index = 0; index < count; ++index)
    {
      if (this.devices[index] is UnityInputDevice device && device.JoystickId == unityJoystickId)
        return true;
    }
    return false;
  }

  private void DetectJoystickDevice(int unityJoystickId, string unityJoystickName)
  {
    if (this.HasAttachedDeviceWithJoystickId(unityJoystickId) || unityJoystickName.IndexOf("webcam", StringComparison.OrdinalIgnoreCase) != -1 || InputManager.UnityVersion < new VersionInfo(4, 5, 0, 0) && (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) && unityJoystickName == "Unknown Wireless Controller" || InputManager.UnityVersion >= new VersionInfo(4, 6, 3, 0) && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) && string.IsNullOrEmpty(unityJoystickName))
      return;
    UnityInputDeviceProfileBase deviceProfile = ((((UnityInputDeviceProfileBase) null ?? this.customDeviceProfiles.Find((Predicate<UnityInputDeviceProfileBase>) (config => config.HasJoystickName(unityJoystickName)))) ?? this.systemDeviceProfiles.Find((Predicate<UnityInputDeviceProfileBase>) (config => config.HasJoystickName(unityJoystickName)))) ?? this.customDeviceProfiles.Find((Predicate<UnityInputDeviceProfileBase>) (config => config.HasLastResortRegex(unityJoystickName)))) ?? this.systemDeviceProfiles.Find((Predicate<UnityInputDeviceProfileBase>) (config => config.HasLastResortRegex(unityJoystickName)));
    if (deviceProfile == null)
    {
      this.AttachDevice(new UnityInputDevice(unityJoystickId, unityJoystickName));
      Debug.Log((object) $"[InControl] Joystick {(object) unityJoystickId}: \"{unityJoystickName}\"");
      Logger.LogWarning($"Device {(object) unityJoystickId} with name \"{unityJoystickName}\" does not match any supported profiles and will be considered an unknown controller.");
    }
    else if (!deviceProfile.IsHidden)
    {
      this.AttachDevice(new UnityInputDevice(deviceProfile, unityJoystickId, unityJoystickName));
      Logger.LogInfo($"Device {(object) unityJoystickId} matched profile {deviceProfile.GetType().Name} ({deviceProfile.Name})");
    }
    else
      Logger.LogInfo($"Device {(object) unityJoystickId} matching profile {deviceProfile.GetType().Name} ({deviceProfile.Name}) is hidden and will not be attached.");
  }

  private void AddSystemDeviceProfile(UnityInputDeviceProfile deviceProfile)
  {
    if (!deviceProfile.IsSupportedOnThisPlatform)
      return;
    this.systemDeviceProfiles.Add((UnityInputDeviceProfileBase) deviceProfile);
  }

  private void AddSystemDeviceProfiles()
  {
    foreach (string profile in UnityInputDeviceProfileList.Profiles)
      this.AddSystemDeviceProfile((UnityInputDeviceProfile) Activator.CreateInstance(System.Type.GetType(profile)));
  }
}
