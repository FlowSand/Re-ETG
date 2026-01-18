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

