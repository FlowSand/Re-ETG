// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.POWERAFUS1ONTournamentControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class POWERAFUS1ONTournamentControllerMacProfile : Xbox360DriverMacProfile
{
  public POWERAFUS1ONTournamentControllerMacProfile()
  {
    this.Name = "POWER A FUS1ON Tournament Controller";
    this.Meta = "POWER A FUS1ON Tournament Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 9414),
        ProductID = new ushort?((ushort) 21399)
      }
    };
  }
}
