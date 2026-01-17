// Decompiled with JetBrains decompiler
// Type: dfDragEventArgs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    public class dfDragEventArgs : dfControlEventArgs
    {
      internal dfDragEventArgs(dfControl source)
        : base(source)
      {
        this.State = dfDragDropState.None;
      }

      internal dfDragEventArgs(
        dfControl source,
        dfDragDropState state,
        object data,
        Ray ray,
        Vector2 position)
        : base(source)
      {
        this.Data = data;
        this.State = state;
        this.Position = position;
        this.Ray = ray;
      }

      public dfDragDropState State { get; set; }

      public object Data { get; set; }

      public Vector2 Position { get; set; }

      public dfControl Target { get; set; }

      public Ray Ray { get; set; }
    }

}
