// Decompiled with JetBrains decompiler
// Type: TrackedMaximumsComparer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class TrackedMaximumsComparer : IEqualityComparer<TrackedMaximums>
    {
      public bool Equals(TrackedMaximums x, TrackedMaximums y) => x == y;

      public int GetHashCode(TrackedMaximums obj) => (int) obj;
    }

}
