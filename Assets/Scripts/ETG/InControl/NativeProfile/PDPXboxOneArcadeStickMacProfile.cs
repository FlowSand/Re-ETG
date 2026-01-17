// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.PDPXboxOneArcadeStickMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class PDPXboxOneArcadeStickMacProfile : XboxOneDriverMacProfile
  {
    public PDPXboxOneArcadeStickMacProfile()
    {
      this.Name = "PDP Xbox One Arcade Stick";
      this.Meta = "PDP Xbox One Arcade Stick on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 348)
        }
      };
    }
  }
}
