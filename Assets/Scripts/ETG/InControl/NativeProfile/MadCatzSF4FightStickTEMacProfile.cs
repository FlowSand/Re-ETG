// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.MadCatzSF4FightStickTEMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzSF4FightStickTEMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzSF4FightStickTEMacProfile()
    {
      this.Name = "Mad Catz SF4 Fight Stick TE";
      this.Meta = "Mad Catz SF4 Fight Stick TE on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1848),
          ProductID = new ushort?((ushort) 18232)
        }
      };
    }
  }
}
