// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.APlayControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class APlayControllerMacProfile : Xbox360DriverMacProfile
  {
    public APlayControllerMacProfile()
    {
      this.Name = "A Play Controller";
      this.Meta = "A Play Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 9414),
          ProductID = new ushort?((ushort) 64251)
        }
      };
    }
  }
}
