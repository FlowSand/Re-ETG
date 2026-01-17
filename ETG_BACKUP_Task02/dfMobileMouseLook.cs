// Decompiled with JetBrains decompiler
// Type: dfMobileMouseLook
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Camera-Control/Mobile Mouse Look")]
public class dfMobileMouseLook : MonoBehaviour
{
  public string joystickName = "RightJoystick";
  public dfMobileMouseLook.RotationAxes axes;
  public float sensitivityX = 15f;
  public float sensitivityY = 15f;
  public float minimumX = -360f;
  public float maximumX = 360f;
  public float minimumY = -60f;
  public float maximumY = 60f;
  private float rotationY;

  private void Update()
  {
    Vector2 joystickPosition = dfTouchJoystick.GetJoystickPosition(this.joystickName);
    if (this.axes == dfMobileMouseLook.RotationAxes.MouseXAndY)
    {
      float y = this.transform.localEulerAngles.y + joystickPosition.x * this.sensitivityX;
      this.rotationY += joystickPosition.y * this.sensitivityY;
      this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
      this.transform.localEulerAngles = new Vector3(-this.rotationY, y, 0.0f);
    }
    else if (this.axes == dfMobileMouseLook.RotationAxes.MouseX)
    {
      this.transform.Rotate(0.0f, joystickPosition.x * this.sensitivityX, 0.0f);
    }
    else
    {
      this.rotationY += joystickPosition.y * this.sensitivityY;
      this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
      this.transform.localEulerAngles = new Vector3(-this.rotationY, this.transform.localEulerAngles.y, 0.0f);
    }
  }

  private void Start()
  {
    if (!(bool) (Object) this.GetComponent<Rigidbody>())
      return;
    this.GetComponent<Rigidbody>().freezeRotation = true;
  }

  public enum RotationAxes
  {
    MouseXAndY,
    MouseX,
    MouseY,
  }
}
