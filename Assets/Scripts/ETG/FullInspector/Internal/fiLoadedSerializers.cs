using FullInspector.Serializers.FullSerializer;
using System;

#nullable disable
namespace FullInspector.Internal
{
  public class fiLoadedSerializers : fiILoadedSerializers
  {
    public Type DefaultSerializerProvider => typeof (FullSerializerMetadata);

    public Type[] AllLoadedSerializerProviders
    {
      get => new Type[1]{ typeof (FullSerializerMetadata) };
    }
  }
}
