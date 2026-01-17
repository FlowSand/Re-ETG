// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.HoriPadUltimateMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class HoriPadUltimateMacProfile : Xbox360DriverMacProfile
  {
    public HoriPadUltimateMacProfile()
    {
      this.Name = "HoriPad Ultimate";
      this.Meta = "HoriPad Ultimate on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3853),
          ProductID = new ushort?((ushort) 144 /*0x90*/)
        }
      };
    }
  }
}
