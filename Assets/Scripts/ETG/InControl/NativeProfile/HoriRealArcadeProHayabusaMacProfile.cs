// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.HoriRealArcadeProHayabusaMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class HoriRealArcadeProHayabusaMacProfile : Xbox360DriverMacProfile
  {
    public HoriRealArcadeProHayabusaMacProfile()
    {
      this.Name = "Hori Real Arcade Pro Hayabusa";
      this.Meta = "Hori Real Arcade Pro Hayabusa on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3853),
          ProductID = new ushort?((ushort) 99)
        }
      };
    }
  }
}
