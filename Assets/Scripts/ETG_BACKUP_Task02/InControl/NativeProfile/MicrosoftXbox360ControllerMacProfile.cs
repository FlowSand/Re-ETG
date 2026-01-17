// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.MicrosoftXbox360ControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class MicrosoftXbox360ControllerMacProfile : Xbox360DriverMacProfile
{
  public MicrosoftXbox360ControllerMacProfile()
  {
    this.Name = "Microsoft Xbox 360 Controller";
    this.Meta = "Microsoft Xbox 360 Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[6]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 1118),
        ProductID = new ushort?((ushort) 654)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 1118),
        ProductID = new ushort?((ushort) 655)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 3695),
        ProductID = new ushort?((ushort) 307)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 3695),
        ProductID = new ushort?((ushort) 63233)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 3695),
        ProductID = new ushort?((ushort) 672)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 1118),
        ProductID = new ushort?((ushort) 672)
      }
    };
  }
}
