// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.HoriEX2ControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class HoriEX2ControllerMacProfile : Xbox360DriverMacProfile
{
  public HoriEX2ControllerMacProfile()
  {
    this.Name = "Hori EX2 Controller";
    this.Meta = "Hori EX2 Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[3]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 3853),
        ProductID = new ushort?((ushort) 13)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 7085),
        ProductID = new ushort?((ushort) 62721)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 9414),
        ProductID = new ushort?((ushort) 21760)
      }
    };
  }
}
