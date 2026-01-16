// Decompiled with JetBrains decompiler
// Type: FullInspector.Rotorz.ReorderableList.ReorderableListFlags
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector.Rotorz.ReorderableList;

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
