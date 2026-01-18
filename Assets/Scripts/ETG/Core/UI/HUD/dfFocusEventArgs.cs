// Decompiled with JetBrains decompiler
// Type: dfFocusEventArgs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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

