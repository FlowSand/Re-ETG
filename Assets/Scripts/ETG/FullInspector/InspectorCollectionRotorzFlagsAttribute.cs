using System;

using FullInspector.Rotorz.ReorderableList;

#nullable disable
namespace FullInspector
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class InspectorCollectionRotorzFlagsAttribute : Attribute
    {
        public ReorderableListFlags Flags;

        public bool DisableReordering
        {
            get => this.HasFlag(ReorderableListFlags.DisableReordering);
            set => this.UpdateFlag(value, ReorderableListFlags.DisableReordering);
        }

        public bool HideAddButton
        {
            get => this.HasFlag(ReorderableListFlags.HideAddButton);
            set => this.UpdateFlag(value, ReorderableListFlags.HideAddButton);
        }

        public bool HideRemoveButtons
        {
            get => this.HasFlag(ReorderableListFlags.HideRemoveButtons);
            set => this.UpdateFlag(value, ReorderableListFlags.HideRemoveButtons);
        }

        public bool ShowIndices
        {
            get => this.HasFlag(ReorderableListFlags.ShowIndices);
            set => this.UpdateFlag(value, ReorderableListFlags.ShowIndices);
        }

        private void UpdateFlag(bool shouldSet, ReorderableListFlags flag)
        {
            if (shouldSet)
                this.Flags |= flag;
            else
                this.Flags &= ~flag;
        }

        private bool HasFlag(ReorderableListFlags flag)
        {
            return (this.Flags & flag) != (ReorderableListFlags) 0;
        }
    }
}
