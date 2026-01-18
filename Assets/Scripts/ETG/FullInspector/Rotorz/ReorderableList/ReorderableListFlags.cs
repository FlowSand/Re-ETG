using System;

#nullable disable
namespace FullInspector.Rotorz.ReorderableList
{
  [Flags]
  public enum ReorderableListFlags
  {
    DisableReordering = 1,
    HideAddButton = 2,
    HideRemoveButtons = 4,
    DisableContextMenu = 8,
    DisableDuplicateCommand = 16, // 0x00000010
    DisableAutoFocus = 32, // 0x00000020
    ShowIndices = 64, // 0x00000040
    DisableClipping = 128, // 0x00000080
  }
}
