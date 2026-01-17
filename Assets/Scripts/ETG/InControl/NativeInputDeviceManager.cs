// Decompiled with JetBrains decompiler
// Type: InControl.NativeInputDeviceManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;

#nullable disable
namespace InControl
{
  public class NativeInputDeviceManager : InputDeviceManager
  {
    public static Func<NativeDeviceInfo, ReadOnlyCollection<NativeInputDevice>, NativeInputDevice> CustomFindDetachedDevice;
    private List<NativeInputDevice> attachedDevices;
    private List<NativeInputDevice> detachedDevices;
    private List<NativeInputDeviceProfile> systemDeviceProfiles;
    private List<NativeInputDeviceProfile> customDeviceProfiles;
    private uint[] deviceEvents;

    public NativeInputDeviceManager()
    {
      this.attachedDevices = new List<NativeInputDevice>();
      this.detachedDevices = new List<NativeInputDevice>();
      this.systemDeviceProfiles = new List<NativeInputDeviceProfile>(NativeInputDeviceProfileList.Profiles.Length);
      this.customDeviceProfiles = new List<NativeInputDeviceProfile>();
      this.deviceEvents = new uint[32 /*0x20*/];
      this.AddSystemDeviceProfiles();
      Native.Init(new NativeInputOptions()
      {
        enableXInput = InputManager.NativeInputEnableXInput,
        preventSleep = InputManager.NativeInputPreventSleep,
        updateRate = InputManager.NativeInputUpdateRate <= 0U ? (ushort) Mathf.FloorToInt(1f / Time.fixedDeltaTime) : (ushort) InputManager.NativeInputUpdateRate
      });
    }

    public override void Destroy() => Native.Stop();

    private uint NextPowerOfTwo(uint x)
    {
      if (x < 0U)
        return 0;
      --x;
      x |= x >> 1;
      x |= x >> 2;
      x |= x >> 4;
      x |= x >> 8;
      x |= x >> 16 /*0x10*/;
      return x + 1U;
    }

    public override void Update(ulong updateTick, float deltaTime)
    {
      IntPtr deviceEvents1;
      int deviceEvents2 = Native.GetDeviceEvents(out deviceEvents1);
      if (deviceEvents2 <= 0)
        return;
      Utility.ArrayExpand<uint>(ref this.deviceEvents, deviceEvents2);
      MarshalUtility.Copy(deviceEvents1, this.deviceEvents, deviceEvents2);
      int num1 = 0;
      uint[] deviceEvents3 = this.deviceEvents;
      int index1 = num1;
      int num2 = index1 + 1;
      uint num3 = deviceEvents3[index1];
      for (int index2 = 0; (long) index2 < (long) num3; ++index2)
      {
        uint deviceEvent = this.deviceEvents[num2++];
        StringBuilder stringBuilder = new StringBuilder(256 /*0x0100*/);
        stringBuilder.Append($"Attached native device with handle {(object) deviceEvent}:\n");
        NativeDeviceInfo deviceInfo;
        if (Native.GetDeviceInfo(deviceEvent, out deviceInfo))
        {
          stringBuilder.AppendFormat("Name: {0}\n", (object) deviceInfo.name);
          stringBuilder.AppendFormat("Driver Type: {0}\n", (object) deviceInfo.driverType);
          stringBuilder.AppendFormat("Location ID: {0}\n", (object) deviceInfo.location);
          stringBuilder.AppendFormat("Serial Number: {0}\n", (object) deviceInfo.serialNumber);
          stringBuilder.AppendFormat("Vendor ID: 0x{0:x}\n", (object) deviceInfo.vendorID);
          stringBuilder.AppendFormat("Product ID: 0x{0:x}\n", (object) deviceInfo.productID);
          stringBuilder.AppendFormat("Version Number: 0x{0:x}\n", (object) deviceInfo.versionNumber);
          stringBuilder.AppendFormat("Buttons: {0}\n", (object) deviceInfo.numButtons);
          stringBuilder.AppendFormat("Analogs: {0}\n", (object) deviceInfo.numAnalogs);
          this.DetectDevice(deviceEvent, deviceInfo);
        }
        Logger.LogInfo(stringBuilder.ToString());
      }
      uint[] deviceEvents4 = this.deviceEvents;
      int index3 = num2;
      int num4 = index3 + 1;
      uint num5 = deviceEvents4[index3];
      for (int index4 = 0; (long) index4 < (long) num5; ++index4)
      {
        uint deviceEvent = this.deviceEvents[num4++];
        Logger.LogInfo($"Detached native device with handle {(object) deviceEvent}:");
        NativeInputDevice attachedDevice = this.FindAttachedDevice(deviceEvent);
        if (attachedDevice != null)
          this.DetachDevice(attachedDevice);
        else
          Logger.LogWarning("Couldn't find device to detach with handle: " + (object) deviceEvent);
      }
    }

    private void DetectDevice(uint deviceHandle, NativeDeviceInfo deviceInfo)
    {
      NativeInputDeviceProfile deviceProfile = ((((NativeInputDeviceProfile) null ?? this.customDeviceProfiles.Find((Predicate<NativeInputDeviceProfile>) (profile => profile.Matches(deviceInfo)))) ?? this.systemDeviceProfiles.Find((Predicate<NativeInputDeviceProfile>) (profile => profile.Matches(deviceInfo)))) ?? this.customDeviceProfiles.Find((Predicate<NativeInputDeviceProfile>) (profile => profile.LastResortMatches(deviceInfo)))) ?? this.systemDeviceProfiles.Find((Predicate<NativeInputDeviceProfile>) (profile => profile.LastResortMatches(deviceInfo)));
      NativeInputDevice device = this.FindDetachedDevice(deviceInfo) ?? new NativeInputDevice();
      device.Initialize(deviceHandle, deviceInfo, deviceProfile);
      this.AttachDevice(device);
    }

    private void AttachDevice(NativeInputDevice device)
    {
      this.detachedDevices.Remove(device);
      this.attachedDevices.Add(device);
      InputManager.AttachDevice((InputDevice) device);
    }

    private void DetachDevice(NativeInputDevice device)
    {
      this.attachedDevices.Remove(device);
      this.detachedDevices.Add(device);
      InputManager.DetachDevice((InputDevice) device);
    }

    private NativeInputDevice FindAttachedDevice(uint deviceHandle)
    {
      int count = this.attachedDevices.Count;
      for (int index = 0; index < count; ++index)
      {
        NativeInputDevice attachedDevice = this.attachedDevices[index];
        if ((int) attachedDevice.Handle == (int) deviceHandle)
          return attachedDevice;
      }
      return (NativeInputDevice) null;
    }

    private NativeInputDevice FindDetachedDevice(NativeDeviceInfo deviceInfo)
    {
      ReadOnlyCollection<NativeInputDevice> detachedDevices = new ReadOnlyCollection<NativeInputDevice>((IList<NativeInputDevice>) this.detachedDevices);
      return NativeInputDeviceManager.CustomFindDetachedDevice != null ? NativeInputDeviceManager.CustomFindDetachedDevice(deviceInfo, detachedDevices) : NativeInputDeviceManager.SystemFindDetachedDevice(deviceInfo, detachedDevices);
    }

    private static NativeInputDevice SystemFindDetachedDevice(
      NativeDeviceInfo deviceInfo,
      ReadOnlyCollection<NativeInputDevice> detachedDevices)
    {
      int count = detachedDevices.Count;
      for (int index = 0; index < count; ++index)
      {
        NativeInputDevice detachedDevice = detachedDevices[index];
        if (detachedDevice.Info.HasSameVendorID(deviceInfo) && detachedDevice.Info.HasSameProductID(deviceInfo) && detachedDevice.Info.HasSameSerialNumber(deviceInfo))
          return detachedDevice;
      }
      for (int index = 0; index < count; ++index)
      {
        NativeInputDevice detachedDevice = detachedDevices[index];
        if (detachedDevice.Info.HasSameVendorID(deviceInfo) && detachedDevice.Info.HasSameProductID(deviceInfo) && detachedDevice.Info.HasSameLocation(deviceInfo))
          return detachedDevice;
      }
      for (int index = 0; index < count; ++index)
      {
        NativeInputDevice detachedDevice = detachedDevices[index];
        if (detachedDevice.Info.HasSameVendorID(deviceInfo) && detachedDevice.Info.HasSameProductID(deviceInfo) && detachedDevice.Info.HasSameVersionNumber(deviceInfo))
          return detachedDevice;
      }
      for (int index = 0; index < count; ++index)
      {
        NativeInputDevice detachedDevice = detachedDevices[index];
        if (detachedDevice.Info.HasSameLocation(deviceInfo))
          return detachedDevice;
      }
      return (NativeInputDevice) null;
    }

    private void AddSystemDeviceProfile(NativeInputDeviceProfile deviceProfile)
    {
      if (!deviceProfile.IsSupportedOnThisPlatform)
        return;
      this.systemDeviceProfiles.Add(deviceProfile);
    }

    private void AddSystemDeviceProfiles()
    {
      foreach (string profile in NativeInputDeviceProfileList.Profiles)
        this.AddSystemDeviceProfile((NativeInputDeviceProfile) Activator.CreateInstance(System.Type.GetType(profile)));
    }

    public static bool CheckPlatformSupport(ICollection<string> errors)
    {
      if (Application.platform != RuntimePlatform.OSXPlayer)
      {
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
          if (Application.platform != RuntimePlatform.WindowsPlayer)
          {
            if (Application.platform != RuntimePlatform.WindowsEditor)
              return false;
          }
        }
      }
      try
      {
        NativeVersionInfo versionInfo;
        Native.GetVersionInfo(out versionInfo);
        Logger.LogInfo($"InControl Native (version {(object) versionInfo.major}.{(object) versionInfo.minor}.{(object) versionInfo.patch})");
      }
      catch (DllNotFoundException ex)
      {
        errors?.Add($"{ex.Message}{Utility.PluginFileExtension()} could not be found or is missing a dependency.");
        return false;
      }
      return true;
    }

    internal static bool Enable()
    {
      List<string> errors = new List<string>();
      if (NativeInputDeviceManager.CheckPlatformSupport((ICollection<string>) errors))
      {
        InputManager.AddDeviceManager<NativeInputDeviceManager>();
        return true;
      }
      foreach (string str in errors)
        Debug.LogError((object) ("Error enabling NativeInputDeviceManager: " + str));
      return false;
    }
  }
}
