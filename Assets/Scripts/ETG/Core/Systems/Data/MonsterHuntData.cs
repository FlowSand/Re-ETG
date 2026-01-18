using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class MonsterHuntData : ScriptableObject
  {
    [SerializeField]
    public List<MonsterHuntQuest> OrderedQuests;
    [SerializeField]
    public List<MonsterHuntQuest> ProceduralQuests;
  }

