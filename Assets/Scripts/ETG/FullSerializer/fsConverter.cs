using System;

#nullable disable
namespace FullSerializer
{
  public abstract class fsConverter : fsBaseConverter
  {
    public abstract bool CanProcess(Type type);
  }
}
