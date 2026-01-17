// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.RockCandyXboxOneControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class RockCandyXboxOneControllerMacProfile : XboxOneDriverMacProfile
  {
    public RockCandyXboxOneControllerMacProfile()
    {
      this.Name = "Rock Candy Xbox One Controller";
      this.Meta = "Rock Candy Xbox One Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[3]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 326)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 582)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 838)
        }
      };
    }
  }
}
