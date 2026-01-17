// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.PDPVersusControllerMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class PDPVersusControllerMacProfile : Xbox360DriverMacProfile
  {
    public PDPVersusControllerMacProfile()
    {
      this.Name = "PDP Versus Controller";
      this.Meta = "PDP Versus Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 63748)
        }
      };
    }
  }
}
