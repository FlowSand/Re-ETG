// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.MadCatzSSF4ChunLiFightStickTEMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzSSF4ChunLiFightStickTEMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzSSF4ChunLiFightStickTEMacProfile()
    {
      this.Name = "Mad Catz SSF4 Chun-Li Fight Stick TE";
      this.Meta = "Mad Catz SSF4 Chun-Li Fight Stick TE on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 61501)
        }
      };
    }
  }
}
