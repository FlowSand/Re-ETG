using System;
using System.Collections.Generic;

#nullable disable

[Serializable]
public class EnemyFactoryWaveDefinition
  {
    public bool exactDefinition;
    public List<AIActor> enemyList;
    public int inexactMinCount = 2;
    public int inexactMaxCount = 4;
  }

