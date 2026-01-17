// Decompiled with JetBrains decompiler
// Type: dfMobileFPSInputController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [RequireComponent(typeof (dfCharacterMotorCS))]
    public class dfMobileFPSInputController : MonoBehaviour
    {
      public string joystickID = "LeftJoystick";
      private dfCharacterMotorCS motor;

      private void Awake() => this.motor = this.GetComponent<dfCharacterMotorCS>();

      private void Update()
      {
        Vector2 joystickPosition = dfTouchJoystick.GetJoystickPosition(this.joystickID);
        Vector3 vector3_1 = new Vector3(joystickPosition.x, 0.0f, joystickPosition.y);
        if (vector3_1 != Vector3.zero)
        {
          float magnitude = vector3_1.magnitude;
          Vector3 vector3_2 = vector3_1 / magnitude;
          float num1 = Mathf.Min(1f, magnitude);
          float num2 = num1 * num1;
          vector3_1 = vector3_2 * num2;
        }
        this.motor.inputMoveDirection = this.transform.rotation * vector3_1;
      }
    }

}
