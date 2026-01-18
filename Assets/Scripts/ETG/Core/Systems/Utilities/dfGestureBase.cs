// Decompiled with JetBrains decompiler
// Type: dfGestureBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

  public abstract class dfGestureBase : MonoBehaviour
  {
    private dfControl control;

    public dfGestureState State { get; protected set; }

    public Vector2 StartPosition { get; protected set; }

    public Vector2 CurrentPosition { get; protected set; }

    public float StartTime { get; protected set; }

    public dfControl Control
    {
      get
      {
        if ((Object) this.control == (Object) null)
          this.control = this.GetComponent<dfControl>();
        return this.control;
      }
    }
  }

