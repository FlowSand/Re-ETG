// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.RockCandyXbox360ControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class RockCandyXbox360ControllerMacProfile : Xbox360DriverMacProfile
{
  public RockCandyXbox360ControllerMacProfile()
  {
    this.Name = "Rock Candy Xbox 360 Controller";
    this.Meta = "Rock Candy Xbox 360 Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[2]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 3695),
        ProductID = new ushort?((ushort) 543)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 9414),
        ProductID = new ushort?((ushort) 64254)
      }
    };
  }
}
