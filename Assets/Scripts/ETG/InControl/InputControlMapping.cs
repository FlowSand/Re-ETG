// Decompiled with JetBrains decompiler
// Type: InControl.InputControlMapping
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace InControl;

public class InputControlMapping
{
  public InputControlSource Source;
  public InputControlType Target;
  public bool Invert;
  public float Scale = 1f;
  public bool Raw;
  public bool Passive;
  public bool IgnoreInitialZeroValue;
  public float Sensitivity = 1f;
  public float LowerDeadZone;
  public float UpperDeadZone = 1f;
  public InputRange SourceRange = InputRange.MinusOneToOne;
  public InputRange TargetRange = InputRange.MinusOneToOne;
  private string handle;

  public float MapValue(float value)
  {
    if (this.Raw)
    {
      value *= this.Scale;
      value = !this.SourceRange.Excludes(value) ? value : 0.0f;
    }
    else
    {
      value = Mathf.Clamp(value * this.Scale, -1f, 1f);
      value = InputRange.Remap(value, this.SourceRange, this.TargetRange);
    }
    if (this.Invert)
      value = -value;
    return value;
  }

  public string Handle
  {
    get => string.IsNullOrEmpty(this.handle) ? this.Target.ToString() : this.handle;
    set => this.handle = value;
  }
}
