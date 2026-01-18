using UnityEngine;

#nullable disable

public class dfKeyEventArgs : dfControlEventArgs
  {
    internal dfKeyEventArgs(dfControl source, KeyCode Key, bool Control, bool Shift, bool Alt)
      : base(source)
    {
      this.KeyCode = Key;
      this.Control = Control;
      this.Shift = Shift;
      this.Alt = Alt;
    }

    public KeyCode KeyCode { get; set; }

    public char Character { get; set; }

    public bool Control { get; set; }

    public bool Shift { get; set; }

    public bool Alt { get; set; }

    public override string ToString()
    {
      return $"Key: {this.KeyCode}, Control: {this.Control}, Shift: {this.Shift}, Alt: {this.Alt}";
    }
  }

