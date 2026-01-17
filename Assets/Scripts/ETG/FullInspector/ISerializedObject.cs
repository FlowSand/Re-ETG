// Decompiled with JetBrains decompiler
// Type: FullInspector.ISerializedObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullInspector
{
  public interface ISerializedObject
  {
    void RestoreState();

    void SaveState();

    bool IsRestored { get; set; }

    List<Object> SerializedObjectReferences { get; set; }

    List<string> SerializedStateKeys { get; set; }

    List<string> SerializedStateValues { get; set; }
  }
}
