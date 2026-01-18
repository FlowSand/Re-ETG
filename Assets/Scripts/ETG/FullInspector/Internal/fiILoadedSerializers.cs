using System;

#nullable disable
namespace FullInspector.Internal
{
  public interface fiILoadedSerializers
  {
    Type DefaultSerializerProvider { get; }

    Type[] AllLoadedSerializerProviders { get; }
  }
}
