// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.MVCTEStickMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class MVCTEStickMacProfile : Xbox360DriverMacProfile
  {
    public MVCTEStickMacProfile()
    {
      this.Name = "MVC TE Stick";
      this.Meta = "MVC TE Stick on Mac";
      this.Matchers = new NativeInputDeviceMatcher[2]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 61497)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1848),
          ProductID = new ushort?((ushort) 46904)
        }
      };
    }
  }
}
