// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.MadCatzFightPadControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class MadCatzFightPadControllerMacProfile : Xbox360DriverMacProfile
{
  public MadCatzFightPadControllerMacProfile()
  {
    this.Name = "Mad Catz FightPad Controller";
    this.Meta = "Mad Catz FightPad Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[2]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 7085),
        ProductID = new ushort?((ushort) 61480)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 1848),
        ProductID = new ushort?((ushort) 18216)
      }
    };
  }
}
