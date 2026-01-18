using UnityEngine;

#nullable disable
namespace FullInspector.LayoutToolkit
{
    public abstract class fiLayout
    {
        public abstract bool RespondsTo(string id);

        public abstract Rect GetSectionRect(string id, Rect initial);

        public abstract float Height { get; }
    }
}
