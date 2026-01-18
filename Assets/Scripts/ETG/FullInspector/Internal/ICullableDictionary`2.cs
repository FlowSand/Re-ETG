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
