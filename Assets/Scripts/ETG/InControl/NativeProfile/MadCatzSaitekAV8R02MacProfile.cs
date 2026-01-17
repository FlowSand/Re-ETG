// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.MadCatzSaitekAV8R02MacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzSaitekAV8R02MacProfile : Xbox360DriverMacProfile
  {
    public MadCatzSaitekAV8R02MacProfile()
    {
      this.Name = "Mad Catz Saitek AV8R02";
      this.Meta = "Mad Catz Saitek AV8R02 on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1848),
          ProductID = new ushort?((ushort) 52009)
        }
      };
    }
  }
}
