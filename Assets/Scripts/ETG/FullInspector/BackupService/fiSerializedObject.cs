// Decompiled with JetBrains decompiler
// Type: FullInspector.BackupService.fiSerializedObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Internal;
using System;
using System.Collections.Generic;

#nullable disable
namespace FullInspector.BackupService;

[Serializable]
public class fiSerializedObject
{
  public fiUnityObjectReference Target;
  public string SavedAt;
  public bool ShowDeserialized;
  public fiDeserializedObject DeserializedState;
  public List<fiSerializedMember> Members = new List<fiSerializedMember>();
  public List<fiUnityObjectReference> ObjectReferences = new List<fiUnityObjectReference>();
}
