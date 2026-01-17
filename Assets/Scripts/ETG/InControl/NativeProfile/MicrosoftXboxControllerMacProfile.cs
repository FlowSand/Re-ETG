// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.MicrosoftXboxControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class MicrosoftXboxControllerMacProfile : Xbox360DriverMacProfile
  {
    public MicrosoftXboxControllerMacProfile()
    {
      this.Name = "Microsoft Xbox Controller";
      this.Meta = "Microsoft Xbox Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[7]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?(ushort.MaxValue),
          ProductID = new ushort?(ushort.MaxValue)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1118),
          ProductID = new ushort?((ushort) 649)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1118),
          ProductID = new ushort?((ushort) 648)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1118),
          ProductID = new ushort?((ushort) 645)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1118),
          ProductID = new ushort?((ushort) 514)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1118),
          ProductID = new ushort?((ushort) 647)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1118),
          ProductID = new ushort?((ushort) 648)
        }
      };
    }
  }
}
