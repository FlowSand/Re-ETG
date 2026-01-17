// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.RazerOnzaTEControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class RazerOnzaTEControllerMacProfile : Xbox360DriverMacProfile
  {
    public RazerOnzaTEControllerMacProfile()
    {
      this.Name = "Razer Onza TE Controller";
      this.Meta = "Razer Onza TE Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[2]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 64768)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 5769),
          ProductID = new ushort?((ushort) 64768)
        }
      };
    }
  }
}
