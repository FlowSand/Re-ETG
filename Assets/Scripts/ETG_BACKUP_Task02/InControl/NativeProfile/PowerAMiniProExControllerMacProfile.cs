// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.PowerAMiniProExControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class PowerAMiniProExControllerMacProfile : Xbox360DriverMacProfile
{
  public PowerAMiniProExControllerMacProfile()
  {
    this.Name = "PowerA Mini Pro Ex Controller";
    this.Meta = "PowerA Mini Pro Ex Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[3]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 5604),
        ProductID = new ushort?((ushort) 16128)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 9414),
        ProductID = new ushort?((ushort) 21274)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 9414),
        ProductID = new ushort?((ushort) 21248)
      }
    };
  }
}
