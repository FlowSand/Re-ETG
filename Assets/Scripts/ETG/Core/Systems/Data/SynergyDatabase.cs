// Decompiled with JetBrains decompiler
// Type: SynergyDatabase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class SynergyDatabase : ScriptableObject
  {
    public static Color SynergyBlue = new Color(0.596078455f, 0.980392158f, 1f);
    [SerializeField]
    public SynergyEntry[] synergies;

    public void RebuildSynergies(PlayerController p, List<int> previouslyActiveSynergies)
    {
      if (!(bool) (Object) p)
        return;
      if (p.ActiveExtraSynergies == null)
        p.ActiveExtraSynergies = new List<int>();
      p.ActiveExtraSynergies.Clear();
      if (p.inventory == null)
        return;
      for (int index = 0; index < this.synergies.Length; ++index)
      {
        if (this.synergies[index].SynergyIsAvailable(GameManager.Instance.PrimaryPlayer, GameManager.Instance.SecondaryPlayer))
          p.ActiveExtraSynergies.Add(index);
      }
    }
  }

