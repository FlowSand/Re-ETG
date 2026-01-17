// Decompiled with JetBrains decompiler
// Type: InControl.XTR_G2_MacUnityProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class XTR_G2_MacUnityProfile : UnityInputDeviceProfile
  {
    public XTR_G2_MacUnityProfile()
    {
      this.Name = "KMODEL Simulator XTR G2 FMS Controller";
      this.Meta = "KMODEL Simulator XTR G2 FMS Controller on OS X";
      this.DeviceClass = InputDeviceClass.Controller;
      this.IncludePlatforms = new string[1]{ "OS X" };
      this.JoystickNames = new string[1]
      {
        "FeiYing Model KMODEL Simulator - XTR+G2+FMS Controller"
      };
    }
  }
}
