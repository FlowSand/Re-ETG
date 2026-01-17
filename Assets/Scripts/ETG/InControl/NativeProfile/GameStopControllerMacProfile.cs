// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.GameStopControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class GameStopControllerMacProfile : Xbox360DriverMacProfile
{
  public GameStopControllerMacProfile()
  {
    this.Name = "GameStop Controller";
    this.Meta = "GameStop Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[4]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 3695),
        ProductID = new ushort?((ushort) 1025)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 3695),
        ProductID = new ushort?((ushort) 769)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 4779),
        ProductID = new ushort?((ushort) 770)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 7085),
        ProductID = new ushort?((ushort) 63745)
      }
    };
  }
}
