// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.MadCatzFightStickTE2MacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class MadCatzFightStickTE2MacProfile : Xbox360DriverMacProfile
{
  public MadCatzFightStickTE2MacProfile()
  {
    this.Name = "Mad Catz Fight Stick TE2";
    this.Meta = "Mad Catz Fight Stick TE2 on Mac";
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 7085),
        ProductID = new ushort?((ushort) 61568)
      }
    };
  }
}
