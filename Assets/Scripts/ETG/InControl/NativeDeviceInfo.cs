// Decompiled with JetBrains decompiler
// Type: InControl.NativeDeviceInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Runtime.InteropServices;

#nullable disable
namespace InControl;

public struct NativeDeviceInfo
{
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128 /*0x80*/)]
  public string name;
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128 /*0x80*/)]
  public string location;
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64 /*0x40*/)]
  public string serialNumber;
  public ushort vendorID;
  public ushort productID;
  public uint versionNumber;
  public NativeDeviceDriverType driverType;
  public NativeDeviceTransportType transportType;
  public uint numButtons;
  public uint numAnalogs;

  public bool HasSameVendorID(NativeDeviceInfo deviceInfo)
  {
    return (int) this.vendorID == (int) deviceInfo.vendorID;
  }

  public bool HasSameProductID(NativeDeviceInfo deviceInfo)
  {
    return (int) this.productID == (int) deviceInfo.productID;
  }

  public bool HasSameVersionNumber(NativeDeviceInfo deviceInfo)
  {
    return (int) this.versionNumber == (int) deviceInfo.versionNumber;
  }

  public bool HasSameLocation(NativeDeviceInfo deviceInfo)
  {
    return !string.IsNullOrEmpty(this.location) && this.location == deviceInfo.location;
  }

  public bool HasSameSerialNumber(NativeDeviceInfo deviceInfo)
  {
    return !string.IsNullOrEmpty(this.serialNumber) && this.serialNumber == deviceInfo.serialNumber;
  }
}
