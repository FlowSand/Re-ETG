using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using XInputDotNetPure;

#nullable disable
namespace InControl
{
  public class XInputDeviceManager : InputDeviceManager
  {
    private bool[] deviceConnected = new bool[4];
    private const int maxDevices = 4;
    private RingBuffer<GamePadState>[] gamePadState = new RingBuffer<GamePadState>[4];
    private Thread thread;
    private int timeStep;
    private int bufferSize;

    public XInputDeviceManager()
    {
      this.timeStep = InputManager.XInputUpdateRate != 0U ? Mathf.FloorToInt((float) (1.0 / (double) InputManager.XInputUpdateRate * 1000.0)) : Mathf.FloorToInt(Time.fixedDeltaTime * 1000f);
      this.bufferSize = (int) Math.Max(InputManager.XInputBufferSize, 1U);
      for (int index = 0; index < 4; ++index)
        this.gamePadState[index] = new RingBuffer<GamePadState>(this.bufferSize);
      this.StartWorker();
      for (int deviceIndex = 0; deviceIndex < 4; ++deviceIndex)
        this.devices.Add((InputDevice) new XInputDevice(deviceIndex, this));
      this.Update(0UL, 0.0f);
    }

    private void StartWorker()
    {
      if (this.thread != null)
        return;
      this.thread = new Thread(new ThreadStart(this.Worker));
      this.thread.IsBackground = true;
      this.thread.Start();
    }

    private void StopWorker()
    {
      if (this.thread == null)
        return;
      this.thread.Abort();
      this.thread.Join();
      this.thread = (Thread) null;
    }

    private void Worker()
    {
      while (true)
      {
        for (int index = 0; index < 4; ++index)
          this.gamePadState[index].Enqueue(GamePad.GetState((PlayerIndex) index));
        Thread.Sleep(this.timeStep);
      }
    }

    internal GamePadState GetState(int deviceIndex) => this.gamePadState[deviceIndex].Dequeue();

    public override void Update(ulong updateTick, float deltaTime)
    {
      for (int index = 0; index < 4; ++index)
      {
        XInputDevice device = this.devices[index] as XInputDevice;
        if (!device.IsConnected)
          device.GetState();
        if (device.IsConnected != this.deviceConnected[index])
        {
          if (device.IsConnected)
            InputManager.AttachDevice((InputDevice) device);
          else
            InputManager.DetachDevice((InputDevice) device);
          this.deviceConnected[index] = device.IsConnected;
        }
      }
    }

    public override void Destroy() => this.StopWorker();

    public static bool CheckPlatformSupport(ICollection<string> errors)
    {
      if (Application.platform != RuntimePlatform.WindowsPlayer)
      {
        if (Application.platform != RuntimePlatform.WindowsEditor)
          return false;
      }
      try
      {
        GamePad.GetState(PlayerIndex.One);
      }
      catch (DllNotFoundException ex)
      {
        errors?.Add(ex.Message + ".dll could not be found or is missing a dependency.");
        return false;
      }
      return true;
    }

    internal static void Enable()
    {
      List<string> errors = new List<string>();
      if (XInputDeviceManager.CheckPlatformSupport((ICollection<string>) errors))
      {
        InputManager.HideDevicesWithProfile(typeof (Xbox360WinProfile));
        InputManager.HideDevicesWithProfile(typeof (XboxOneWinProfile));
        InputManager.HideDevicesWithProfile(typeof (XboxOneWin10Profile));
        InputManager.HideDevicesWithProfile(typeof (XboxOneWin10AEProfile));
        InputManager.HideDevicesWithProfile(typeof (LogitechF310ModeXWinProfile));
        InputManager.HideDevicesWithProfile(typeof (LogitechF510ModeXWinProfile));
        InputManager.HideDevicesWithProfile(typeof (LogitechF710ModeXWinProfile));
        InputManager.AddDeviceManager<XInputDeviceManager>();
      }
      else
      {
        foreach (string text in errors)
          Logger.LogError(text);
      }
    }
  }
}
