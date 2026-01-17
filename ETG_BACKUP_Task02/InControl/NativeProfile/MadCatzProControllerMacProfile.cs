// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.MadCatzProControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class MadCatzProControllerMacProfile : Xbox360DriverMacProfile
{
  public MadCatzProControllerMacProfile()
  {
    this.Name = "Mad Catz Pro Controller";
    this.Meta = "Mad Catz Pro Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 1848),
        ProductID = new ushort?((ushort) 18214)
      }
    };
  }
}
