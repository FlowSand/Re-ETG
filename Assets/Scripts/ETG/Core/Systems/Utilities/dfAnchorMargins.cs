// Decompiled with JetBrains decompiler
// Type: dfAnchorMargins
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class dfAnchorMargins
    {
      [SerializeField]
      public float left;
      [SerializeField]
      public float top;
      [SerializeField]
      public float right;
      [SerializeField]
      public float bottom;

      public override string ToString()
      {
        return $"[L:{this.left},T:{this.top},R:{this.right},B:{this.bottom}]";
      }
    }

}
