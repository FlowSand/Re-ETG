// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.NaconGC100XFControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class NaconGC100XFControllerMacProfile : Xbox360DriverMacProfile
  {
    public NaconGC100XFControllerMacProfile()
    {
      this.Name = "Nacon GC-100XF Controller";
      this.Meta = "Nacon GC-100XF Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 4553),
          ProductID = new ushort?((ushort) 22000)
        }
      };
    }
  }
}
