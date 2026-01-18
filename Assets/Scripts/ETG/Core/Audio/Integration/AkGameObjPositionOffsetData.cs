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

