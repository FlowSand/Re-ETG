using System;

#nullable disable
namespace InControl
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SingletonPrefabAttribute : Attribute
    {
        public readonly string Name;

        public SingletonPrefabAttribute(string name) => this.Name = name;
    }
}
