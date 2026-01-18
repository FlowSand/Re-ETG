using UnityEngine;

#nullable disable

public class AIActorDummy : AIActor
    {
        public bool isInBossTab;
        public GameObject realPrefab;

        public override bool InBossAmmonomiconTab => this.isInBossTab;
    }

