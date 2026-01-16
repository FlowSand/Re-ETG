// Decompiled with JetBrains decompiler
// Type: DamageTypeEffectDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class DamageTypeEffectDefinition
{
  [HideInInspector]
  [SerializeField]
  public string name = "dongs";
  public CoreDamageTypes damageType;
  public VFXPool wallDecals;
}
