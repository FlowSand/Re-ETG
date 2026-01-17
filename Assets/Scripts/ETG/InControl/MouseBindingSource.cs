// Decompiled with JetBrains decompiler
// Type: InControl.MouseBindingSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.IO;
using UnityEngine;

#nullable disable
namespace InControl;

public class MouseBindingSource : BindingSource
{
  public static float ScaleX = 0.05f;
  public static float ScaleY = 0.05f;
  public static float ScaleZ = 0.05f;
  public static float JitterThreshold = 0.05f;
  private static readonly int[] buttonTable = new int[16 /*0x10*/]
  {
    -1,
    0,
    1,
    2,
    -1,
    -1,
    -1,
    -1,
    -1,
    -1,
    3,
    4,
    5,
    6,
    7,
    8
  };

  internal MouseBindingSource()
  {
  }

  public MouseBindingSource(Mouse mouseControl) => this.Control = mouseControl;

  public Mouse Control { get; protected set; }

  internal static bool SafeGetMouseButton(int button)
  {
    try
    {
      return Input.GetMouseButton(button);
    }
    catch (ArgumentException ex)
    {
    }
    return false;
  }

  internal static bool ButtonIsPressed(Mouse control)
  {
    int button = MouseBindingSource.buttonTable[(int) control];
    return button >= 0 && MouseBindingSource.SafeGetMouseButton(button);
  }

  internal static bool NegativeScrollWheelIsActive(float threshold)
  {
    return (double) Mathf.Min(Input.GetAxisRaw("mouse z") * MouseBindingSource.ScaleZ, 0.0f) < -(double) threshold;
  }

  internal static bool PositiveScrollWheelIsActive(float threshold)
  {
    return (double) Mathf.Max(0.0f, Input.GetAxisRaw("mouse z") * MouseBindingSource.ScaleZ) > (double) threshold;
  }

  internal static float GetValue(Mouse mouseControl)
  {
    int button = MouseBindingSource.buttonTable[(int) mouseControl];
    if (button >= 0)
      return MouseBindingSource.SafeGetMouseButton(button) ? 1f : 0.0f;
    switch (mouseControl)
    {
      case Mouse.NegativeX:
        return -Mathf.Min(Input.GetAxisRaw("mouse x") * MouseBindingSource.ScaleX, 0.0f);
      case Mouse.PositiveX:
        return Mathf.Max(0.0f, Input.GetAxisRaw("mouse x") * MouseBindingSource.ScaleX);
      case Mouse.NegativeY:
        return -Mathf.Min(Input.GetAxisRaw("mouse y") * MouseBindingSource.ScaleY, 0.0f);
      case Mouse.PositiveY:
        return Mathf.Max(0.0f, Input.GetAxisRaw("mouse y") * MouseBindingSource.ScaleY);
      case Mouse.PositiveScrollWheel:
        return Mathf.Max(0.0f, Input.GetAxisRaw("mouse z") * MouseBindingSource.ScaleZ);
      case Mouse.NegativeScrollWheel:
        return -Mathf.Min(Input.GetAxisRaw("mouse z") * MouseBindingSource.ScaleZ, 0.0f);
      default:
        return 0.0f;
    }
  }

  public override float GetValue(InputDevice inputDevice)
  {
    return MouseBindingSource.GetValue(this.Control);
  }

  public override bool GetState(InputDevice inputDevice)
  {
    return Utility.IsNotZero(this.GetValue(inputDevice));
  }

  public static string GetLocalizedMouseButtonName(int buttonIndex)
  {
    if ((bool) (UnityEngine.Object) GameUIRoot.Instance)
    {
      dfControl pPlayerCoinLabel = (dfControl) GameUIRoot.Instance.p_playerCoinLabel;
      if ((bool) (UnityEngine.Object) pPlayerCoinLabel)
      {
        switch (buttonIndex)
        {
          case 0:
            return pPlayerCoinLabel.ForceGetLocalizedValue("#CONTROL_LMB");
          case 1:
            return pPlayerCoinLabel.ForceGetLocalizedValue("#CONTROL_MMB");
          case 2:
            return pPlayerCoinLabel.ForceGetLocalizedValue("#CONTROL_RMB");
        }
      }
    }
    return string.Empty;
  }

  public override string Name
  {
    get
    {
      if ((bool) (UnityEngine.Object) GameUIRoot.Instance)
      {
        dfControl pPlayerCoinLabel = (dfControl) GameUIRoot.Instance.p_playerCoinLabel;
        if ((bool) (UnityEngine.Object) pPlayerCoinLabel)
        {
          if (this.Control == Mouse.LeftButton)
            return pPlayerCoinLabel.ForceGetLocalizedValue("#CONTROL_LMB");
          if (this.Control == Mouse.MiddleButton)
            return pPlayerCoinLabel.ForceGetLocalizedValue("#CONTROL_MMB");
          if (this.Control == Mouse.RightButton)
            return pPlayerCoinLabel.ForceGetLocalizedValue("#CONTROL_RMB");
        }
      }
      return this.Control.ToString();
    }
  }

  public override string DeviceName => "Mouse";

  public override InputDeviceClass DeviceClass => InputDeviceClass.Mouse;

  public override InputDeviceStyle DeviceStyle => InputDeviceStyle.Unknown;

  public override bool Equals(BindingSource other)
  {
    if (other == (BindingSource) null)
      return false;
    MouseBindingSource mouseBindingSource = other as MouseBindingSource;
    return (BindingSource) mouseBindingSource != (BindingSource) null && this.Control == mouseBindingSource.Control;
  }

  public override bool Equals(object other)
  {
    if (other == null)
      return false;
    MouseBindingSource mouseBindingSource = other as MouseBindingSource;
    return (BindingSource) mouseBindingSource != (BindingSource) null && this.Control == mouseBindingSource.Control;
  }

  public override int GetHashCode() => this.Control.GetHashCode();

  public override BindingSourceType BindingSourceType => BindingSourceType.MouseBindingSource;

  internal override void Save(BinaryWriter writer) => writer.Write((int) this.Control);

  internal override void Load(BinaryReader reader, ushort dataFormatVersion, bool upgrade)
  {
    this.Control = (Mouse) reader.ReadInt32();
  }
}
