// Decompiled with JetBrains decompiler
// Type: VFXObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class VFXObject
{
  public GameObject effect;
  public bool orphaned;
  public bool attached = true;
  public bool persistsOnDeath;
  public bool usesZHeight;
  public float zHeight;
  public VFXAlignment alignment;
  [HideInInspector]
  public bool destructible;
}
