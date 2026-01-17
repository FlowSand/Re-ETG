// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.ICullableDictionary`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace FullInspector.Internal
{
  public interface ICullableDictionary<TKey, TValue>
  {
    TValue this[TKey key] { get; set; }

    bool TryGetValue(TKey key, out TValue value);

    void BeginCullZone();

    void EndCullZone();

    IEnumerable<KeyValuePair<TKey, TValue>> Items { get; }

    bool IsEmpty { get; }
  }
}
