// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.RedOctaneControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class RedOctaneControllerMacProfile : Xbox360DriverMacProfile
  {
    public RedOctaneControllerMacProfile()
    {
      this.Name = "Red Octane Controller";
      this.Meta = "Red Octane Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[2]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 5168),
          ProductID = new ushort?((ushort) 63489)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 5168),
          ProductID = new ushort?((ushort) 672)
        }
      };
    }
  }
}
