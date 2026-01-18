using System.Collections.Generic;

#nullable disable
namespace FullInspector
{
  public interface fiICollectionAttributeProvider
  {
    IEnumerable<object> GetAttributes();
  }
}
