// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.LogitechF710ControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class LogitechF710ControllerMacProfile : Xbox360DriverMacProfile
  {
    public LogitechF710ControllerMacProfile()
    {
      this.Name = "Logitech F710 Controller";
      this.Meta = "Logitech F710 Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1133),
          ProductID = new ushort?((ushort) 49695)
        }
      };
    }
  }
}
