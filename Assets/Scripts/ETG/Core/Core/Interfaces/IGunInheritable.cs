// Decompiled with JetBrains decompiler
// Type: IGunInheritable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable

public interface IGunInheritable
  {
    void InheritData(Gun sourceGun);

    void MidGameSerialize(List<object> data, int dataIndex);

    void MidGameDeserialize(List<object> data, ref int dataIndex);
  }

