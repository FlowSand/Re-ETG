// Decompiled with JetBrains decompiler
// Type: LootDataGlobalSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class LootDataGlobalSettings : ScriptableObject
{
  private static LootDataGlobalSettings m_instance;
  public float GunClassModifier = 0.2f;
  [SerializeField]
  public List<GunClassModifierOverride> GunClassOverrides;

  public static LootDataGlobalSettings Instance
  {
    get
    {
      if ((Object) LootDataGlobalSettings.m_instance == (Object) null)
        LootDataGlobalSettings.m_instance = (LootDataGlobalSettings) BraveResources.Load("GlobalLootSettings", ".asset");
      return LootDataGlobalSettings.m_instance;
    }
  }

  public float GetModifierForClass(GunClass targetClass)
  {
    for (int index = 0; index < this.GunClassOverrides.Count; ++index)
    {
      if (this.GunClassOverrides[index].classToModify == targetClass)
        return this.GunClassOverrides[index].modifier;
    }
    return this.GunClassModifier;
  }
}
