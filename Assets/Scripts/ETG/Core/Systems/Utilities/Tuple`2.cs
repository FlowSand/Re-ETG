// Decompiled with JetBrains decompiler
// Type: Tuple`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public sealed class Tuple<T1, T2>
    {
      public T1 First;
      public T2 Second;

      public Tuple(T1 first, T2 second)
      {
        this.First = first;
        this.Second = second;
      }

      public override string ToString() => $"[{this.First}, {this.Second}]";
    }

}
