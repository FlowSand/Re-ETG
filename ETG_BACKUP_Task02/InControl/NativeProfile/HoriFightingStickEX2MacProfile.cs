// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.HoriFightingStickEX2MacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class HoriFightingStickEX2MacProfile : Xbox360DriverMacProfile
{
  public HoriFightingStickEX2MacProfile()
  {
    this.Name = "Hori Fighting Stick EX2";
    this.Meta = "Hori Fighting Stick EX2 on Mac";
    this.Matchers = new NativeInputDeviceMatcher[3]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 3853),
        ProductID = new ushort?((ushort) 10)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 7085),
        ProductID = new ushort?((ushort) 62725)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 3853),
        ProductID = new ushort?((ushort) 13)
      }
    };
  }
}
