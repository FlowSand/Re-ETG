// Decompiled with JetBrains decompiler
// Type: InControl.UnknownDeviceBindingSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.IO;
using UnityEngine;

#nullable disable
namespace InControl
{
  public class UnknownDeviceBindingSource : BindingSource
  {
    internal UnknownDeviceBindingSource() => this.Control = UnknownDeviceControl.None;

    public UnknownDeviceBindingSource(UnknownDeviceControl control) => this.Control = control;

    public UnknownDeviceControl Control { get; protected set; }

    public override float GetValue(InputDevice device) => this.Control.GetValue(device);

    public override bool GetState(InputDevice device)
    {
      return device != null && Utility.IsNotZero(this.GetValue(device));
    }

    public override string Name
    {
      get
      {
        if (this.BoundTo == null)
          return string.Empty;
        string str = string.Empty;
        if (this.Control.SourceRange == InputRangeType.ZeroToMinusOne)
          str = "Negative ";
        else if (this.Control.SourceRange == InputRangeType.ZeroToOne)
          str = "Positive ";
        InputDevice device = this.BoundTo.Device;
        if (device == InputDevice.Null)
          return str + this.Control.Control.ToString();
        InputControl control = device.GetControl(this.Control.Control);
        return control == InputControl.Null ? str + this.Control.Control.ToString() : str + control.Handle;
      }
    }

    public override string DeviceName
    {
      get
      {
        if (this.BoundTo == null)
          return string.Empty;
        InputDevice device = this.BoundTo.Device;
        return device == InputDevice.Null ? "Unknown Controller" : device.Name;
      }
    }

    public override InputDeviceClass DeviceClass => InputDeviceClass.Controller;

    public override InputDeviceStyle DeviceStyle => InputDeviceStyle.Unknown;

    public override bool Equals(BindingSource other)
    {
      if (other == (BindingSource) null)
        return false;
      UnknownDeviceBindingSource deviceBindingSource = other as UnknownDeviceBindingSource;
      return (BindingSource) deviceBindingSource != (BindingSource) null && this.Control == deviceBindingSource.Control;
    }

    public override bool Equals(object other)
    {
      if (other == null)
        return false;
      UnknownDeviceBindingSource deviceBindingSource = other as UnknownDeviceBindingSource;
      return (BindingSource) deviceBindingSource != (BindingSource) null && this.Control == deviceBindingSource.Control;
    }

    public override int GetHashCode() => this.Control.GetHashCode();

    public override BindingSourceType BindingSourceType
    {
      get => BindingSourceType.UnknownDeviceBindingSource;
    }

    internal override bool IsValid
    {
      get
      {
        if (this.BoundTo == null)
        {
          Debug.LogError((object) "Cannot query property 'IsValid' for unbound BindingSource.");
          return false;
        }
        InputDevice device = this.BoundTo.Device;
        return device == InputDevice.Null || device.HasControl(this.Control.Control);
      }
    }

    internal override void Load(BinaryReader reader, ushort dataFormatVersion, bool upgrade)
    {
      UnknownDeviceControl unknownDeviceControl = new UnknownDeviceControl();
      unknownDeviceControl.Load(reader, upgrade);
      this.Control = unknownDeviceControl;
    }

    internal override void Save(BinaryWriter writer) => this.Control.Save(writer);
  }
}
