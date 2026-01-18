using System;

#nullable disable
namespace FullSerializer
{
  public abstract class fsDirectConverter : fsBaseConverter
  {
    public abstract Type ModelType { get; }
  }
}
