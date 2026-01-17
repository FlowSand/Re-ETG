// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.TSZPelicanControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class TSZPelicanControllerMacProfile : Xbox360DriverMacProfile
{
  public TSZPelicanControllerMacProfile()
  {
    this.Name = "TSZ Pelican Controller";
    this.Meta = "TSZ Pelican Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 3695),
        ProductID = new ushort?((ushort) 513)
      }
    };
  }
}
