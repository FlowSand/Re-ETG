// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.RazerOnzaControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class RazerOnzaControllerMacProfile : Xbox360DriverMacProfile
  {
    public RazerOnzaControllerMacProfile()
    {
      this.Name = "Razer Onza Controller";
      this.Meta = "Razer Onza Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[2]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 64769)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 5769),
          ProductID = new ushort?((ushort) 64769)
        }
      };
    }
  }
}
