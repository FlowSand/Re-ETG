// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.HoriRealArcadeProEXMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class HoriRealArcadeProEXMacProfile : Xbox360DriverMacProfile
  {
    public HoriRealArcadeProEXMacProfile()
    {
      this.Name = "Hori Real Arcade Pro EX";
      this.Meta = "Hori Real Arcade Pro EX on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 62724)
        }
      };
    }
  }
}
