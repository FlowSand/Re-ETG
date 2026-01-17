// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.RazerSabertoothEliteControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class RazerSabertoothEliteControllerMacProfile : Xbox360DriverMacProfile
{
  public RazerSabertoothEliteControllerMacProfile()
  {
    this.Name = "Razer Sabertooth Elite Controller";
    this.Meta = "Razer Sabertooth Elite Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[2]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 5769),
        ProductID = new ushort?((ushort) 65024)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 9414),
        ProductID = new ushort?((ushort) 23812)
      }
    };
  }
}
