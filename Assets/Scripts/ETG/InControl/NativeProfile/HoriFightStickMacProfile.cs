// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.HoriFightStickMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class HoriFightStickMacProfile : Xbox360DriverMacProfile
  {
    public HoriFightStickMacProfile()
    {
      this.Name = "Hori Fight Stick";
      this.Meta = "Hori Fight Stick on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3853),
          ProductID = new ushort?((ushort) 13)
        }
      };
    }
  }
}
