using System;

#nullable disable
namespace FullSerializer
{
    public sealed class fsDuplicateVersionNameException : Exception
    {
        public fsDuplicateVersionNameException(Type typeA, Type typeB, string version)
            : base($"{(object) typeA} and {(object) typeB} have the same version string ({version}); please change one of them.")
        {
        }
    }
}
