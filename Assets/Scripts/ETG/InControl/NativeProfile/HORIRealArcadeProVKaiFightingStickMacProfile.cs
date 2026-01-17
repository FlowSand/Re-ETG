// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.HORIRealArcadeProVKaiFightingStickMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class HORIRealArcadeProVKaiFightingStickMacProfile : Xbox360DriverMacProfile
  {
    public HORIRealArcadeProVKaiFightingStickMacProfile()
    {
      this.Name = "HORI Real Arcade Pro V Kai Fighting Stick";
      this.Meta = "HORI Real Arcade Pro V Kai Fighting Stick on Mac";
      this.Matchers = new NativeInputDeviceMatcher[2]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 9414),
          ProductID = new ushort?((ushort) 21774)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3853),
          ProductID = new ushort?((ushort) 120)
        }
      };
    }
  }
}
