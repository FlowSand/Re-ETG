// Decompiled with JetBrains decompiler
// Type: VirtualDeviceExample.VirtualDeviceExample
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using System;
using UnityEngine;

#nullable disable
namespace VirtualDeviceExample
{
  public class VirtualDeviceExample : MonoBehaviour
  {
    public GameObject leftObject;
    public GameObject rightObject;
    private VirtualDevice virtualDevice;

    private void OnEnable()
    {
      this.virtualDevice = new VirtualDevice();
      InputManager.OnSetup += (Action) (() => InputManager.AttachDevice((InputDevice) this.virtualDevice));
    }

    private void OnDisable() => InputManager.DetachDevice((InputDevice) this.virtualDevice);

    private void Update()
    {
      InputDevice activeDevice = InputManager.ActiveDevice;
      this.leftObject.transform.Rotate(Vector3.down, 500f * Time.deltaTime * (float) (OneAxisInputControl) activeDevice.LeftStickX, Space.World);
      this.leftObject.transform.Rotate(Vector3.right, 500f * Time.deltaTime * (float) (OneAxisInputControl) activeDevice.LeftStickY, Space.World);
      this.rightObject.transform.Rotate(Vector3.down, 500f * Time.deltaTime * (float) (OneAxisInputControl) activeDevice.RightStickX, Space.World);
      this.rightObject.transform.Rotate(Vector3.right, 500f * Time.deltaTime * (float) (OneAxisInputControl) activeDevice.RightStickY, Space.World);
      Color color = Color.white;
      if (activeDevice.Action1.IsPressed)
        color = Color.green;
      if (activeDevice.Action2.IsPressed)
        color = Color.red;
      if (activeDevice.Action3.IsPressed)
        color = Color.blue;
      if (activeDevice.Action4.IsPressed)
        color = Color.yellow;
      this.leftObject.GetComponent<Renderer>().material.color = color;
    }
  }
}
