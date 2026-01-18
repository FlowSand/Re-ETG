// Decompiled with JetBrains decompiler
// Type: dfKeyEventArgs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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

