// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.PDPXbox360ControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class PDPXbox360ControllerMacProfile : Xbox360DriverMacProfile
{
  public PDPXbox360ControllerMacProfile()
  {
    this.Name = "PDP Xbox 360 Controller";
    this.Meta = "PDP Xbox 360 Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 3695),
        ProductID = new ushort?((ushort) 1281)
      }
    };
  }
}
