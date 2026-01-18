using System;

#nullable disable
namespace FullSerializer
{
    public sealed class fsMissingVersionConstructorException : Exception
    {
        public fsMissingVersionConstructorException(Type versionedType, Type constructorType)
            : base($"{(object) versionedType} is missing a constructor for previous model type {(object) constructorType}")
        {
        }
    }
}
