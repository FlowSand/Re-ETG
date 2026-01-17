// Decompiled with JetBrains decompiler
// Type: InControl.TestInputManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace InControl
{
  public class TestInputManager : MonoBehaviour
  {
    public Font font;
    private GUIStyle style = new GUIStyle();
    private List<LogMessage> logMessages = new List<LogMessage>();
    private bool isPaused;

    private void OnEnable()
    {
      this.isPaused = false;
      Time.timeScale = 1f;
      Logger.OnLogMessage += (Action<LogMessage>) (logMessage => this.logMessages.Add(logMessage));
      InputManager.OnDeviceAttached += (Action<InputDevice>) (inputDevice => Debug.Log((object) ("Attached: " + inputDevice.Name)));
      InputManager.OnDeviceDetached += (Action<InputDevice>) (inputDevice => Debug.Log((object) ("Detached: " + inputDevice.Name)));
      InputManager.OnActiveDeviceChanged += (Action<InputDevice>) (inputDevice => Debug.Log((object) ("Active device changed to: " + inputDevice.Name)));
      InputManager.OnUpdate += new Action<ulong, float>(this.HandleInputUpdate);
    }

    private void HandleInputUpdate(ulong updateTick, float deltaTime)
    {
      this.CheckForPauseButton();
      int count = InputManager.Devices.Count;
      for (int index = 0; index < count; ++index)
      {
        InputDevice device = InputManager.Devices[index];
        device.Vibrate((float) (OneAxisInputControl) device.LeftTrigger, (float) (OneAxisInputControl) device.RightTrigger);
      }
    }

    private void Start()
    {
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.R))
        Utility.LoadScene(nameof (TestInputManager));
      if (!Input.GetKeyDown(KeyCode.E))
        return;
      InputManager.Enabled = !InputManager.Enabled;
    }

    private void CheckForPauseButton()
    {
      if (!Input.GetKeyDown(KeyCode.P) && !InputManager.CommandWasPressed)
        return;
      Time.timeScale = !this.isPaused ? 0.0f : 1f;
      this.isPaused = !this.isPaused;
    }

    private void SetColor(Color color) => this.style.normal.textColor = color;

    private void OnGUI()
    {
      int num1 = Mathf.FloorToInt((float) (Screen.width / Mathf.Max(1, InputManager.Devices.Count)));
      int x1 = 10;
      int y1 = 10;
      int num2 = 15;
      GUI.skin.font = this.font;
      this.SetColor(Color.white);
      string text1 = $"{$"Devices: (Platform: {InputManager.Platform})"} {(object) InputManager.ActiveDevice.Direction.Vector}";
      if (this.isPaused)
      {
        this.SetColor(Color.red);
        text1 = "+++ PAUSED +++";
      }
      GUI.Label(new Rect((float) x1, (float) y1, (float) (x1 + num1), (float) (y1 + 10)), text1, this.style);
      this.SetColor(Color.white);
      foreach (InputDevice device in InputManager.Devices)
      {
        bool flag = InputManager.ActiveDevice == device;
        Color color1 = !flag ? Color.white : Color.yellow;
        int y2 = 35;
        if (device.IsUnknown)
        {
          this.SetColor(Color.red);
          GUI.Label(new Rect((float) x1, (float) y2, (float) (x1 + num1), (float) (y2 + 10)), "Unknown Device", this.style);
        }
        else
        {
          this.SetColor(color1);
          GUI.Label(new Rect((float) x1, (float) y2, (float) (x1 + num1), (float) (y2 + 10)), device.Name, this.style);
        }
        int y3 = y2 + num2;
        this.SetColor(color1);
        if (device.IsUnknown)
        {
          GUI.Label(new Rect((float) x1, (float) y3, (float) (x1 + num1), (float) (y3 + 10)), device.Meta, this.style);
          y3 += num2;
        }
        GUI.Label(new Rect((float) x1, (float) y3, (float) (x1 + num1), (float) (y3 + 10)), "Style: " + (object) device.DeviceStyle, this.style);
        int y4 = y3 + num2;
        GUI.Label(new Rect((float) x1, (float) y4, (float) (x1 + num1), (float) (y4 + 10)), "GUID: " + (object) device.GUID, this.style);
        int y5 = y4 + num2;
        GUI.Label(new Rect((float) x1, (float) y5, (float) (x1 + num1), (float) (y5 + 10)), "SortOrder: " + (object) device.SortOrder, this.style);
        int y6 = y5 + num2;
        GUI.Label(new Rect((float) x1, (float) y6, (float) (x1 + num1), (float) (y6 + 10)), "LastChangeTick: " + (object) device.LastChangeTick, this.style);
        int y7 = y6 + num2;
        if (device is NativeInputDevice nativeInputDevice)
        {
          string text2 = $"VID = 0x{nativeInputDevice.Info.vendorID:x}, PID = 0x{nativeInputDevice.Info.productID:x}, VER = 0x{nativeInputDevice.Info.versionNumber:x}";
          GUI.Label(new Rect((float) x1, (float) y7, (float) (x1 + num1), (float) (y7 + 10)), text2, this.style);
          y7 += num2;
        }
        int y8 = y7 + num2;
        foreach (InputControl control in device.Controls)
        {
          if (control != null && !Utility.TargetIsAlias(control.Target))
          {
            string str = !device.IsKnown ? control.Handle : $"{control.Target} ({control.Handle})";
            this.SetColor(!control.State ? color1 : Color.green);
            string text3 = $"{str} {(!control.State ? (object) string.Empty : (object) ("= " + (object) control.Value))}";
            GUI.Label(new Rect((float) x1, (float) y8, (float) (x1 + num1), (float) (y8 + 10)), text3, this.style);
            y8 += num2;
          }
        }
        int y9 = y8 + num2;
        Color color2 = !flag ? Color.white : new Color(1f, 0.7f, 0.2f);
        if (device.IsKnown)
        {
          InputControl command = device.Command;
          this.SetColor(!command.State ? color2 : Color.green);
          string text4 = $"{"Command"} {(!command.State ? (object) string.Empty : (object) ("= " + (object) command.Value))}";
          GUI.Label(new Rect((float) x1, (float) y9, (float) (x1 + num1), (float) (y9 + 10)), text4, this.style);
          int y10 = y9 + num2;
          InputControl leftStickX = device.LeftStickX;
          this.SetColor(!leftStickX.State ? color2 : Color.green);
          string text5 = $"{"Left Stick X"} {(!leftStickX.State ? (object) string.Empty : (object) ("= " + (object) leftStickX.Value))}";
          GUI.Label(new Rect((float) x1, (float) y10, (float) (x1 + num1), (float) (y10 + 10)), text5, this.style);
          int y11 = y10 + num2;
          InputControl leftStickY = device.LeftStickY;
          this.SetColor(!leftStickY.State ? color2 : Color.green);
          string text6 = $"{"Left Stick Y"} {(!leftStickY.State ? (object) string.Empty : (object) ("= " + (object) leftStickY.Value))}";
          GUI.Label(new Rect((float) x1, (float) y11, (float) (x1 + num1), (float) (y11 + 10)), text6, this.style);
          int y12 = y11 + num2;
          this.SetColor(!device.LeftStick.State ? color2 : Color.green);
          string text7 = $"{"Left Stick A"} {(!device.LeftStick.State ? (object) string.Empty : (object) ("= " + (object) device.LeftStick.Angle))}";
          GUI.Label(new Rect((float) x1, (float) y12, (float) (x1 + num1), (float) (y12 + 10)), text7, this.style);
          int y13 = y12 + num2;
          InputControl rightStickX = device.RightStickX;
          this.SetColor(!rightStickX.State ? color2 : Color.green);
          string text8 = $"{"Right Stick X"} {(!rightStickX.State ? (object) string.Empty : (object) ("= " + (object) rightStickX.Value))}";
          GUI.Label(new Rect((float) x1, (float) y13, (float) (x1 + num1), (float) (y13 + 10)), text8, this.style);
          int y14 = y13 + num2;
          InputControl rightStickY = device.RightStickY;
          this.SetColor(!rightStickY.State ? color2 : Color.green);
          string text9 = $"{"Right Stick Y"} {(!rightStickY.State ? (object) string.Empty : (object) ("= " + (object) rightStickY.Value))}";
          GUI.Label(new Rect((float) x1, (float) y14, (float) (x1 + num1), (float) (y14 + 10)), text9, this.style);
          int y15 = y14 + num2;
          this.SetColor(!device.RightStick.State ? color2 : Color.green);
          string text10 = $"{"Right Stick A"} {(!device.RightStick.State ? (object) string.Empty : (object) ("= " + (object) device.RightStick.Angle))}";
          GUI.Label(new Rect((float) x1, (float) y15, (float) (x1 + num1), (float) (y15 + 10)), text10, this.style);
          int y16 = y15 + num2;
          InputControl dpadX = device.DPadX;
          this.SetColor(!dpadX.State ? color2 : Color.green);
          string text11 = $"{"DPad X"} {(!dpadX.State ? (object) string.Empty : (object) ("= " + (object) dpadX.Value))}";
          GUI.Label(new Rect((float) x1, (float) y16, (float) (x1 + num1), (float) (y16 + 10)), text11, this.style);
          int y17 = y16 + num2;
          InputControl dpadY = device.DPadY;
          this.SetColor(!dpadY.State ? color2 : Color.green);
          string text12 = $"{"DPad Y"} {(!dpadY.State ? (object) string.Empty : (object) ("= " + (object) dpadY.Value))}";
          GUI.Label(new Rect((float) x1, (float) y17, (float) (x1 + num1), (float) (y17 + 10)), text12, this.style);
          y9 = y17 + num2;
        }
        this.SetColor(Color.cyan);
        InputControl anyButton = device.AnyButton;
        if ((bool) (OneAxisInputControl) anyButton)
          GUI.Label(new Rect((float) x1, (float) y9, (float) (x1 + num1), (float) (y9 + 10)), "AnyButton = " + anyButton.Handle, this.style);
        x1 += num1;
      }
      Color[] colorArray = new Color[3]
      {
        Color.gray,
        Color.yellow,
        Color.white
      };
      this.SetColor(Color.white);
      int x2 = 10;
      int y18 = Screen.height - (10 + num2);
      for (int index = this.logMessages.Count - 1; index >= 0; --index)
      {
        LogMessage logMessage = this.logMessages[index];
        if (logMessage.type != LogMessageType.Info)
        {
          this.SetColor(colorArray[(int) logMessage.type]);
          string text13 = logMessage.text;
          char[] chArray = new char[1]{ '\n' };
          foreach (string text14 in text13.Split(chArray))
          {
            GUI.Label(new Rect((float) x2, (float) y18, (float) Screen.width, (float) (y18 + 10)), text14, this.style);
            y18 -= num2;
          }
        }
      }
    }

    private void DrawUnityInputDebugger()
    {
      int num1 = 300;
      int x = Screen.width / 2;
      int y1 = 10;
      int num2 = 20;
      this.SetColor(Color.white);
      string[] joystickNames = Input.GetJoystickNames();
      int length = joystickNames.Length;
      for (int index1 = 0; index1 < length; ++index1)
      {
        string str = joystickNames[index1];
        int num3 = index1 + 1;
        GUI.Label(new Rect((float) x, (float) y1, (float) (x + num1), (float) (y1 + 10)), $"Joystick {(object) num3}: \"{str}\"", this.style);
        int y2 = y1 + num2;
        string text1 = "Buttons: ";
        for (int index2 = 0; index2 < 20; ++index2)
        {
          if (Input.GetKey($"joystick {(object) num3} button {(object) index2}"))
            text1 = $"{text1}B{(object) index2}  ";
        }
        GUI.Label(new Rect((float) x, (float) y2, (float) (x + num1), (float) (y2 + 10)), text1, this.style);
        int y3 = y2 + num2;
        string text2 = "Analogs: ";
        for (int index3 = 0; index3 < 20; ++index3)
        {
          float axisRaw = Input.GetAxisRaw($"joystick {(object) num3} analog {(object) index3}");
          if (Utility.AbsoluteIsOverThreshold(axisRaw, 0.2f))
            text2 = $"{text2}A{(object) index3}: {axisRaw.ToString("0.00")}  ";
        }
        GUI.Label(new Rect((float) x, (float) y3, (float) (x + num1), (float) (y3 + 10)), text2, this.style);
        y1 = y3 + num2 + 25;
      }
    }

    private void OnDrawGizmos()
    {
      InputDevice activeDevice = InputManager.ActiveDevice;
      Vector2 vector = activeDevice.Direction.Vector;
      Gizmos.color = Color.blue;
      Vector2 vector2_1 = new Vector2(-3f, -1f);
      Vector2 vector2_2 = vector2_1 + vector * 2f;
      Gizmos.DrawSphere((Vector3) vector2_1, 0.1f);
      Gizmos.DrawLine((Vector3) vector2_1, (Vector3) vector2_2);
      Gizmos.DrawSphere((Vector3) vector2_2, 1f);
      Gizmos.color = Color.red;
      Vector2 vector2_3 = new Vector2(3f, -1f);
      Vector2 vector2_4 = vector2_3 + activeDevice.RightStick.Vector * 2f;
      Gizmos.DrawSphere((Vector3) vector2_3, 0.1f);
      Gizmos.DrawLine((Vector3) vector2_3, (Vector3) vector2_4);
      Gizmos.DrawSphere((Vector3) vector2_4, 1f);
    }
  }
}
