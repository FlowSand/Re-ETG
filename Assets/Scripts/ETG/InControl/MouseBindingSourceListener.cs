// Decompiled with JetBrains decompiler
// Type: InControl.MouseBindingSourceListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  public class MouseBindingSourceListener : BindingSourceListener
  {
    public static float ScrollWheelThreshold = 1f / 1000f;
    private Mouse detectFound;
    private int detectPhase;

    public void Reset()
    {
      this.detectFound = Mouse.None;
      this.detectPhase = 0;
    }

    public BindingSource Listen(BindingListenOptions listenOptions, InputDevice device)
    {
      if (this.detectFound != Mouse.None && !this.IsPressed(this.detectFound) && this.detectPhase == 2)
      {
        MouseBindingSource mouseBindingSource = new MouseBindingSource(this.detectFound);
        this.Reset();
        return (BindingSource) mouseBindingSource;
      }
      Mouse mouse = this.ListenForControl(listenOptions);
      if (mouse != Mouse.None)
      {
        if (this.detectPhase == 1)
        {
          this.detectFound = mouse;
          this.detectPhase = 2;
        }
      }
      else if (this.detectPhase == 0)
        this.detectPhase = 1;
      return (BindingSource) null;
    }

    private bool IsPressed(Mouse control)
    {
      if (control == Mouse.NegativeScrollWheel)
        return MouseBindingSource.NegativeScrollWheelIsActive(MouseBindingSourceListener.ScrollWheelThreshold);
      return control == Mouse.PositiveScrollWheel ? MouseBindingSource.PositiveScrollWheelIsActive(MouseBindingSourceListener.ScrollWheelThreshold) : MouseBindingSource.ButtonIsPressed(control);
    }

    private Mouse ListenForControl(BindingListenOptions listenOptions)
    {
      if (listenOptions.IncludeMouseButtons)
      {
        for (Mouse control = Mouse.None; control <= Mouse.Button9; ++control)
        {
          if (MouseBindingSource.ButtonIsPressed(control))
            return control;
        }
      }
      if (listenOptions.IncludeMouseScrollWheel)
      {
        if (MouseBindingSource.NegativeScrollWheelIsActive(MouseBindingSourceListener.ScrollWheelThreshold))
          return Mouse.NegativeScrollWheel;
        if (MouseBindingSource.PositiveScrollWheelIsActive(MouseBindingSourceListener.ScrollWheelThreshold))
          return Mouse.PositiveScrollWheel;
      }
      return Mouse.None;
    }
  }
}
