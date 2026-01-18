using System;
using System.Reflection;

#nullable disable
namespace FullInspector
{
  [Obsolete("Please use [fiInspectorOnly]")]
  public class NullSerializer : BaseSerializer
  {
    public override string Serialize(
      MemberInfo storageType,
      object value,
      ISerializationOperator serializationOperator)
    {
      return (string) null;
    }

    public override object Deserialize(
      MemberInfo storageType,
      string serializedState,
      ISerializationOperator serializationOperator)
    {
      return (object) null;
    }
  }
}
