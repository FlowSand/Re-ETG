// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.HoriPadEXTurboControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class HoriPadEXTurboControllerMacProfile : Xbox360DriverMacProfile
  {
    public HoriPadEXTurboControllerMacProfile()
    {
      this.Name = "Hori Pad EX Turbo Controller";
      this.Meta = "Hori Pad EX Turbo Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3853),
          ProductID = new ushort?((ushort) 12)
        }
      };
    }
  }
}
