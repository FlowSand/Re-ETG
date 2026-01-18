using FullSerializer;
using System;
using UnityEngine;

#nullable disable
namespace FullInspector.Serializers.FullSerializer
{
  public class FullSerializerMetadata : fiISerializerMetadata
  {
    public Guid SerializerGuid => new Guid("bc898177-6ff4-423f-91bb-589bc83d8fde");

    public System.Type SerializerType => typeof (FullSerializerSerializer);

    public System.Type[] SerializationOptInAnnotationTypes
    {
      get
      {
        return new System.Type[2]
        {
          typeof (SerializeField),
          typeof (fsPropertyAttribute)
        };
      }
    }

    public System.Type[] SerializationOptOutAnnotationTypes
    {
      get => new System.Type[1]{ typeof (fsIgnoreAttribute) };
    }
  }
}
