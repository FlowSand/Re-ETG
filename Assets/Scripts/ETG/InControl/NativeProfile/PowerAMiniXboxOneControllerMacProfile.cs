// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.PowerAMiniXboxOneControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class PowerAMiniXboxOneControllerMacProfile : XboxOneDriverMacProfile
{
  public PowerAMiniXboxOneControllerMacProfile()
  {
    this.Name = "Power A Mini Xbox One Controller";
    this.Meta = "Power A Mini Xbox One Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 9414),
        ProductID = new ushort?((ushort) 21562)
      }
    };
  }
}
