// Decompiled with JetBrains decompiler
// Type: InControl.UnityKeyCodeSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace InControl;

public class UnityKeyCodeSource : InputControlSource
{
  public KeyCode[] KeyCodeList;

  public UnityKeyCodeSource()
  {
  }

  public UnityKeyCodeSource(params KeyCode[] keyCodeList) => this.KeyCodeList = keyCodeList;

  public float GetValue(InputDevice inputDevice) => this.GetState(inputDevice) ? 1f : 0.0f;

  public bool GetState(InputDevice inputDevice)
  {
    for (int index = 0; index < this.KeyCodeList.Length; ++index)
    {
      if (Input.GetKey(this.KeyCodeList[index]))
        return true;
    }
    return false;
  }
}
