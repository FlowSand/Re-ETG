// Decompiled with JetBrains decompiler
// Type: dfMouseEventArgs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class dfMouseEventArgs : dfControlEventArgs
  {
    public dfMouseEventArgs(
      dfControl Source,
      dfMouseButtons button,
      int clicks,
      Ray ray,
      Vector2 location,
      float wheel)
      : base(Source)
    {
      this.Buttons = button;
      this.Clicks = clicks;
      this.Position = location;
      this.WheelDelta = wheel;
      this.Ray = ray;
    }

    public dfMouseEventArgs(dfControl Source)
      : base(Source)
    {
      this.Buttons = dfMouseButtons.None;
      this.Clicks = 0;
      this.Position = Vector2.zero;
      this.WheelDelta = 0.0f;
    }

    public dfMouseButtons Buttons { get; private set; }

    public int Clicks { get; private set; }

    public float WheelDelta { get; private set; }

    public Vector2 MoveDelta { get; set; }

    public Vector2 Position { get; set; }

    public Ray Ray { get; set; }
  }

