// Decompiled with JetBrains decompiler
// Type: Tuple
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public static class Tuple
    {
      public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 second)
      {
        return new Tuple<T1, T2>(item1, second);
      }
    }

}
