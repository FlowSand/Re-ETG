using System;

#nullable disable
namespace FullInspector.Internal
{
    [Serializable]
    public class tkFoldoutMetadata : IGraphMetadataItemPersistent
    {
        public bool IsExpanded;

        bool IGraphMetadataItemPersistent.ShouldSerialize() => !this.IsExpanded;
    }
}
