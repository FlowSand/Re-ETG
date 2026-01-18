using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class AdvancedSynergyDatabase : ScriptableObject
  {
    public static Color SynergyBlue = new Color(0.596078455f, 0.980392158f, 1f);
    [SerializeField]
    public AdvancedSynergyEntry[] synergies;

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

