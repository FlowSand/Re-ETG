// Decompiled with JetBrains decompiler
// Type: TrackedStatsComparer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
public class TrackedStatsComparer : IEqualityComparer<TrackedStats>
{
  public bool Equals(TrackedStats x, TrackedStats y) => x == y;

  public int GetHashCode(TrackedStats obj) => (int) obj;
}
