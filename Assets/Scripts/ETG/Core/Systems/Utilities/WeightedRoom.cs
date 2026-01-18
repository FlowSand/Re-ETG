using System;

#nullable disable

[Serializable]
public class WeightedRoom
  {
    public PrototypeDungeonRoom room;
    public float weight;
    public bool limitedCopies;
    public int maxCopies = 1;
    public DungeonPrerequisite[] additionalPrerequisites;

    public bool CheckPrerequisites()
    {
      if (this.additionalPrerequisites == null || this.additionalPrerequisites.Length == 0)
        return true;
      for (int index = 0; index < this.additionalPrerequisites.Length; ++index)
      {
        if (!this.additionalPrerequisites[index].CheckConditionsFulfilled())
          return false;
      }
      return true;
    }
  }

