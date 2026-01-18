// Decompiled with JetBrains decompiler
// Type: DamageTypeEffectMatrix
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class DamageTypeEffectMatrix : ScriptableObject
  {
    public List<DamageTypeEffectDefinition> definitions;

    public DamageTypeEffectDefinition GetDefinitionForType(CoreDamageTypes typeFlags)
    {
      for (int index = 0; index < this.definitions.Count; ++index)
      {
        if ((typeFlags & this.definitions[index].damageType) == this.definitions[index].damageType)
          return this.definitions[index];
      }
      return (DamageTypeEffectDefinition) null;
    }
  }

