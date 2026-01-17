// Decompiled with JetBrains decompiler
// Type: InControl.UnknownDeviceBindingSourceListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  public class UnknownDeviceBindingSourceListener : BindingSourceListener
  {
    private UnknownDeviceControl detectFound;
    private UnknownDeviceBindingSourceListener.DetectPhase detectPhase;

    public void Reset()
    {
      this.detectFound = UnknownDeviceControl.None;
      this.detectPhase = UnknownDeviceBindingSourceListener.DetectPhase.WaitForInitialRelease;
      this.TakeSnapshotOnUnknownDevices();
    }

    private void TakeSnapshotOnUnknownDevices()
    {
      int count = InputManager.Devices.Count;
      for (int index = 0; index < count; ++index)
      {
        InputDevice device = InputManager.Devices[index];
        if (device.IsUnknown)
          device.TakeSnapshot();
      }
    }

    public BindingSource Listen(BindingListenOptions listenOptions, InputDevice device)
    {
      if (!listenOptions.IncludeUnknownControllers || device.IsKnown)
        return (BindingSource) null;
      if (this.detectPhase == UnknownDeviceBindingSourceListener.DetectPhase.WaitForControlRelease && (bool) this.detectFound && !this.IsPressed(this.detectFound, device))
      {
        UnknownDeviceBindingSource deviceBindingSource = new UnknownDeviceBindingSource(this.detectFound);
        this.Reset();
        return (BindingSource) deviceBindingSource;
      }
      UnknownDeviceControl unknownDeviceControl = this.ListenForControl(listenOptions, device);
      if ((bool) unknownDeviceControl)
      {
        if (this.detectPhase == UnknownDeviceBindingSourceListener.DetectPhase.WaitForControlPress)
        {
          this.detectFound = unknownDeviceControl;
          this.detectPhase = UnknownDeviceBindingSourceListener.DetectPhase.WaitForControlRelease;
        }
      }
      else if (this.detectPhase == UnknownDeviceBindingSourceListener.DetectPhase.WaitForInitialRelease)
        this.detectPhase = UnknownDeviceBindingSourceListener.DetectPhase.WaitForControlPress;
      return (BindingSource) null;
    }

    private bool IsPressed(UnknownDeviceControl control, InputDevice device)
    {
      return Utility.AbsoluteIsOverThreshold(control.GetValue(device), 0.5f);
    }

    private UnknownDeviceControl ListenForControl(
      BindingListenOptions listenOptions,
      InputDevice device)
    {
      if (device.IsUnknown)
      {
        UnknownDeviceControl firstPressedButton = device.GetFirstPressedButton();
        if ((bool) firstPressedButton)
          return firstPressedButton;
        UnknownDeviceControl firstPressedAnalog = device.GetFirstPressedAnalog();
        if ((bool) firstPressedAnalog)
          return firstPressedAnalog;
      }
      return UnknownDeviceControl.None;
    }

    private enum DetectPhase
    {
      WaitForInitialRelease,
      WaitForControlPress,
      WaitForControlRelease,
    }
  }
}
