// Decompiled with JetBrains decompiler
// Type: InControl.InputControlState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  public struct InputControlState
  {
    public bool State;
    public float Value;
    public float RawValue;

    public void Reset()
    {
      this.State = false;
      this.Value = 0.0f;
      this.RawValue = 0.0f;
    }

    public void Set(float value)
    {
      this.Value = value;
      this.State = Utility.IsNotZero(value);
    }

    public void Set(float value, float threshold)
    {
      this.Value = value;
      this.State = Utility.AbsoluteIsOverThreshold(value, threshold);
    }

    public void Set(bool state)
    {
      this.State = state;
      this.Value = !state ? 0.0f : 1f;
      this.RawValue = this.Value;
    }

    public static implicit operator bool(InputControlState state) => state.State;

    public static implicit operator float(InputControlState state) => state.Value;

    public static bool operator ==(InputControlState a, InputControlState b)
    {
      return a.State == b.State && Utility.Approximately(a.Value, b.Value);
    }

    public static bool operator !=(InputControlState a, InputControlState b)
    {
      return a.State != b.State || !Utility.Approximately(a.Value, b.Value);
    }
  }
}
