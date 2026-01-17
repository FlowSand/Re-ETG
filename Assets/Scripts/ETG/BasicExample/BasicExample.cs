// Decompiled with JetBrains decompiler
// Type: BasicExample.BasicExample
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using UnityEngine;

#nullable disable
namespace BasicExample;

public class BasicExample : MonoBehaviour
{
  private void Update()
  {
    InputDevice activeDevice = InputManager.ActiveDevice;
    this.transform.Rotate(Vector3.down, 500f * Time.deltaTime * (float) (OneAxisInputControl) activeDevice.LeftStickX, Space.World);
    this.transform.Rotate(Vector3.right, 500f * Time.deltaTime * (float) (OneAxisInputControl) activeDevice.LeftStickY, Space.World);
    Color a = !activeDevice.Action1.IsPressed ? Color.white : Color.red;
    Color b = !activeDevice.Action2.IsPressed ? Color.white : Color.green;
    this.GetComponent<Renderer>().material.color = Color.Lerp(a, b, 0.5f);
  }
}
