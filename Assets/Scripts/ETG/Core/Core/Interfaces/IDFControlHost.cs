// Decompiled with JetBrains decompiler
// Type: IDFControlHost
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public interface IDFControlHost
  {
    T AddControl<T>() where T : dfControl;

    dfControl AddControl(System.Type controlType);

    void AddControl(dfControl child);

    dfControl AddPrefab(GameObject prefab);
  }

