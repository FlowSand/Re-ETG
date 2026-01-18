#nullable disable
namespace InControl
{
  public class InputControl : OneAxisInputControl
  {
    public static readonly InputControl Null = new InputControl();
    public bool Passive;
    private ulong zeroTick;

    private InputControl()
    {
      this.Handle = "None";
      this.Target = InputControlType.None;
      this.Passive = false;
      this.IsButton = false;
      this.IsAnalog = false;
    }

    public InputControl(string handle, InputControlType target)
    {
      this.Handle = handle;
      this.Target = target;
      this.Passive = false;
      this.IsButton = Utility.TargetIsButton(target);
      this.IsAnalog = !this.IsButton;
    }

    public InputControl(string handle, InputControlType target, bool passive)
      : this(handle, target)
    {
      this.Passive = passive;
    }

    public string Handle { get; protected set; }

    public InputControlType Target { get; protected set; }

    public bool IsButton { get; protected set; }

    public bool IsAnalog { get; protected set; }

    internal void SetZeroTick() => this.zeroTick = this.UpdateTick;

    internal bool IsOnZeroTick => (long) this.UpdateTick == (long) this.zeroTick;

    public bool IsStandard => Utility.TargetIsStandard(this.Target);
  }
}
