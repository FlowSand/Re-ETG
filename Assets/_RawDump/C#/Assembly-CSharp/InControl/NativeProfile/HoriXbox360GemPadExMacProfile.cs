// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.HoriXbox360GemPadExMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class HoriXbox360GemPadExMacProfile : Xbox360DriverMacProfile
{
  public HoriXbox360GemPadExMacProfile()
  {
    this.Name = "Hori Xbox 360 Gem Pad Ex";
    this.Meta = "Hori Xbox 360 Gem Pad Ex on Mac";
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 9414),
        ProductID = new ushort?((ushort) 21773)
      }
    };
  }
}
