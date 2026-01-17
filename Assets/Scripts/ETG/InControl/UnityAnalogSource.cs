// Decompiled with JetBrains decompiler
// Type: InControl.UnityAnalogSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl;

public class UnityAnalogSource : InputControlSource
{
  public int AnalogIndex;

  public UnityAnalogSource(int analogIndex) => this.AnalogIndex = analogIndex;

  public float GetValue(InputDevice inputDevice)
  {
    return (inputDevice as UnityInputDevice).ReadRawAnalogValue(this.AnalogIndex);
  }

  public bool GetState(InputDevice inputDevice) => Utility.IsNotZero(this.GetValue(inputDevice));
}
