using System;
using UnityEngine;

#nullable disable

public class ExplodingEnemiesChallengeModifier : ChallengeModifier
  {
    public ExplosionData explosion;

    private void Start()
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        GameManager.Instance.AllPlayers[index].OnAnyEnemyReceivedDamage += new Action<float, bool, HealthHaver>(this.OnEnemyDamaged);
    }

    private void OnEnemyDamaged(float damage, bool fatal, HealthHaver enemyHealth)
    {
      if (!(bool) (UnityEngine.Object) enemyHealth || enemyHealth.IsBoss || !fatal || !(bool) (UnityEngine.Object) enemyHealth.aiActor || !enemyHealth.aiActor.IsNormalEnemy)
        return;
      string name = enemyHealth.name;
      if (name.StartsWith("Bashellisk") || name.StartsWith("Blobulin") || name.StartsWith("Poisbulin"))
        return;
      Exploder.Explode((Vector3) enemyHealth.aiActor.CenterPosition, this.explosion, Vector2.zero);
    }

    private void OnDestroy()
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        GameManager.Instance.AllPlayers[index].OnAnyEnemyReceivedDamage -= new Action<float, bool, HealthHaver>(this.OnEnemyDamaged);
    }
  }

