// Decompiled with JetBrains decompiler
// Type: AkGameObjPositionOffsetData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

[Serializable]
public class AkGameObjPositionOffsetData
  {
    public bool KeepMe;
    public Vector3 positionOffset;

    public AkGameObjPositionOffsetData(bool IReallyWantToBeConstructed = false)
    {
      this.KeepMe = IReallyWantToBeConstructed;
    }
  }

