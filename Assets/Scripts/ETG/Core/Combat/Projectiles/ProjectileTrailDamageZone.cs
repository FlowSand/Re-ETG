using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class ProjectileTrailDamageZone : MonoBehaviour
  {
    public float delayTime = 0.5f;
    public float additionalDestroyTime = 0.5f;
    public float damageToDeal = 5f;
    public bool AppliesFire;
    public GameActorFireEffect FireEffect;

    public void OnSpawned() => this.StartCoroutine(this.HandleSpawnBehavior());

    [DebuggerHidden]
    public IEnumerator HandleSpawnBehavior()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ProjectileTrailDamageZone__HandleSpawnBehaviorc__Iterator0()
      {
        _this = this
      };
    }
  }

