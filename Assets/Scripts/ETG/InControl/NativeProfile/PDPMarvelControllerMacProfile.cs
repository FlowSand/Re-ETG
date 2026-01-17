// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.PDPMarvelControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class PDPMarvelControllerMacProfile : Xbox360DriverMacProfile
  {
    public PDPMarvelControllerMacProfile()
    {
      this.Name = "PDP Marvel Controller";
      this.Meta = "PDP Marvel Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 327)
        }
      };
    }
  }
}
