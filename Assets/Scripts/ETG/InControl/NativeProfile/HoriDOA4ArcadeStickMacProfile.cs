// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.HoriDOA4ArcadeStickMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class HoriDOA4ArcadeStickMacProfile : Xbox360DriverMacProfile
{
  public HoriDOA4ArcadeStickMacProfile()
  {
    this.Name = "Hori DOA4 Arcade Stick";
    this.Meta = "Hori DOA4 Arcade Stick on Mac";
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 3853),
        ProductID = new ushort?((ushort) 10)
      }
    };
  }
}
