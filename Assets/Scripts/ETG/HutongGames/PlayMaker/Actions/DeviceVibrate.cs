// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DeviceVibrate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Causes the device to vibrate for half a second.")]
  [ActionCategory(ActionCategory.Device)]
  public class DeviceVibrate : FsmStateAction
  {
    public override void Reset()
    {
    }

    public override void OnEnter() => this.Finish();
  }
}
