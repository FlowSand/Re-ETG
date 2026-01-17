// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.MadCatzControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzControllerMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzControllerMacProfile()
    {
      this.Name = "Mad Catz Controller";
      this.Meta = "Mad Catz Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[3]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1848),
          ProductID = new ushort?((ushort) 18198)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 63746)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 61642)
        }
      };
    }
  }
}
