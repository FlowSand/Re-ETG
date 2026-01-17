// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.MadCatzFightStickTESPlusMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class MadCatzFightStickTESPlusMacProfile : Xbox360DriverMacProfile
{
  public MadCatzFightStickTESPlusMacProfile()
  {
    this.Name = "Mad Catz Fight Stick TES Plus";
    this.Meta = "Mad Catz Fight Stick TES Plus on Mac";
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 7085),
        ProductID = new ushort?((ushort) 61506)
      }
    };
  }
}
