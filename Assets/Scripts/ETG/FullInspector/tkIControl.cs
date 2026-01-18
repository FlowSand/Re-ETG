using UnityEngine;

#nullable disable
namespace FullInspector
{
  public interface tkIControl
  {
    object Edit(Rect rect, object obj, object context, fiGraphMetadata metadata);

    float GetHeight(object obj, object context, fiGraphMetadata metadata);

    void InitializeId(ref int nextId);

    System.Type ContextType { get; }
  }
}
