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

