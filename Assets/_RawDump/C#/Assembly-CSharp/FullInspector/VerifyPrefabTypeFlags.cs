// Decompiled with JetBrains decompiler
// Type: FullInspector.VerifyPrefabTypeFlags
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullInspector;

[Flags]
public enum VerifyPrefabTypeFlags
{
  None = 1,
  Prefab = 2,
  ModelPrefab = 4,
  PrefabInstance = 8,
  ModelPrefabInstance = 16, // 0x00000010
  MissingPrefabInstance = 32, // 0x00000020
  DisconnectedPrefabInstance = 64, // 0x00000040
  DisconnectedModelPrefabInstance = 128, // 0x00000080
}
