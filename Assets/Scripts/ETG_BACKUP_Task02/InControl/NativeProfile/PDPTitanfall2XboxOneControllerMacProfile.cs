// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.PDPTitanfall2XboxOneControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class PDPTitanfall2XboxOneControllerMacProfile : XboxOneDriverMacProfile
{
  public PDPTitanfall2XboxOneControllerMacProfile()
  {
    this.Name = "PDP Titanfall 2 Xbox One Controller";
    this.Meta = "PDP Titanfall 2 Xbox One Controller on Mac";
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 3695),
        ProductID = new ushort?((ushort) 357)
      }
    };
  }
}
