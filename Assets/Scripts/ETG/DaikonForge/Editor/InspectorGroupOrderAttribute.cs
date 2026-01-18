using System;
using System.Collections.Generic;

#nullable disable
namespace DaikonForge.Editor
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class InspectorGroupOrderAttribute : Attribute
    {
        public List<string> Groups = new List<string>();

        public InspectorGroupOrderAttribute(params string[] groups)
        {
            this.Groups.AddRange((IEnumerable<string>) groups);
        }
    }
}
