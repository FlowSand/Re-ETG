// Decompiled with JetBrains decompiler
// Type: FullInspector.BackupService.fiStorageComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Internal;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullInspector.BackupService;

[AddComponentMenu("")]
public class fiStorageComponent : MonoBehaviour, fiIEditorOnlyTag
{
  public List<fiSerializedObject> Objects = new List<fiSerializedObject>();

  public void RemoveInvalidBackups()
  {
    int index = 0;
    while (index < this.Objects.Count)
    {
      if (this.Objects[index].Target.Target == (Object) null)
        this.Objects.RemoveAt(index);
      else
        ++index;
    }
  }
}
