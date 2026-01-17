// Decompiled with JetBrains decompiler
// Type: InControl.UnityKeyCodeAxisSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace InControl
{
  public class UnityKeyCodeAxisSource : InputControlSource
  {
    public KeyCode NegativeKeyCode;
    public KeyCode PositiveKeyCode;

    public UnityKeyCodeAxisSource()
    {
    }

    public UnityKeyCodeAxisSource(KeyCode negativeKeyCode, KeyCode positiveKeyCode)
    {
      this.NegativeKeyCode = negativeKeyCode;
      this.PositiveKeyCode = positiveKeyCode;
    }

    public float GetValue(InputDevice inputDevice)
    {
      int num = 0;
      if (Input.GetKey(this.NegativeKeyCode))
        --num;
      if (Input.GetKey(this.PositiveKeyCode))
        ++num;
      return (float) num;
    }

    public bool GetState(InputDevice inputDevice) => Utility.IsNotZero(this.GetValue(inputDevice));
  }
}
