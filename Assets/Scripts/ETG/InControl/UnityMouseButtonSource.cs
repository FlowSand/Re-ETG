// Decompiled with JetBrains decompiler
// Type: InControl.UnityMouseButtonSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace InControl;

public class UnityMouseButtonSource : InputControlSource
{
  public int ButtonId;

  public UnityMouseButtonSource()
  {
  }

  public UnityMouseButtonSource(int buttonId) => this.ButtonId = buttonId;

  public float GetValue(InputDevice inputDevice) => this.GetState(inputDevice) ? 1f : 0.0f;

  public bool GetState(InputDevice inputDevice) => Input.GetMouseButton(this.ButtonId);
}
