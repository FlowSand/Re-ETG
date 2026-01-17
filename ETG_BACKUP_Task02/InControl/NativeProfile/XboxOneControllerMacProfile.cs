// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.XboxOneControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class XboxOneControllerMacProfile : XboxOneDriverMacProfile
{
  public XboxOneControllerMacProfile()
  {
    this.Name = "Xbox One Controller";
    this.Meta = "Xbox One Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[2]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 9414),
        ProductID = new ushort?((ushort) 22042)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 9414),
        ProductID = new ushort?((ushort) 21786)
      }
    };
  }
}
