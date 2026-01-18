#nullable disable

public class dfFocusEventArgs : dfControlEventArgs
  {
    public bool AllowScrolling;

    internal dfFocusEventArgs(dfControl GotFocus, dfControl LostFocus, bool AllowScrolling)
      : base(GotFocus)
    {
      this.LostFocus = LostFocus;
      this.AllowScrolling = AllowScrolling;
    }

    public dfControl GotFocus => this.Source;

    public dfControl LostFocus { get; private set; }
  }

