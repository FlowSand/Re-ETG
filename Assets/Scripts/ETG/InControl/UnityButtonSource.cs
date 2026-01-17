// Decompiled with JetBrains decompiler
// Type: InControl.UnityButtonSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  public class UnityButtonSource : InputControlSource
  {
    public int ButtonIndex;

    public UnityButtonSource(int buttonIndex) => this.ButtonIndex = buttonIndex;

    public float GetValue(InputDevice inputDevice) => this.GetState(inputDevice) ? 1f : 0.0f;

    public bool GetState(InputDevice inputDevice)
    {
      return (inputDevice as UnityInputDevice).ReadRawButtonState(this.ButtonIndex);
    }
  }
}
