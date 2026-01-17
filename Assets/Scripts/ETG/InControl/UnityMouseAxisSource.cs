// Decompiled with JetBrains decompiler
// Type: InControl.UnityMouseAxisSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace InControl
{
  public class UnityMouseAxisSource : InputControlSource
  {
    public string MouseAxisQuery;

    public UnityMouseAxisSource()
    {
    }

    public UnityMouseAxisSource(string axis) => this.MouseAxisQuery = "mouse " + axis;

    public float GetValue(InputDevice inputDevice) => Input.GetAxisRaw(this.MouseAxisQuery);

    public bool GetState(InputDevice inputDevice) => Utility.IsNotZero(this.GetValue(inputDevice));
  }
}
