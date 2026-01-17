// Decompiled with JetBrains decompiler
// Type: IntVector2EqualityComparer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class IntVector2EqualityComparer : IEqualityComparer<IntVector2>
    {
      public bool Equals(IntVector2 a, IntVector2 b) => a.x == b.x && a.y == b.y;

      public int GetHashCode(IntVector2 obj)
      {
        return (17 * 23 + obj.x.GetHashCode()) * 23 + obj.y.GetHashCode();
      }
    }

}
