// Decompiled with JetBrains decompiler
// Type: HauntedChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable
public class HauntedChallengeModifier : ChallengeModifier
{
  [EnemyIdentifier]
  public string GhostGuid;
  public float Chance = 0.5f;
  public string GhostOverrideSpawnAnimation;

  private void Start()
  {
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      GameManager.Instance.AllPlayers[index].OnAnyEnemyReceivedDamage += new Action<float, bool, HealthHaver>(this.OnEnemyDamaged);
  }

  private void OnEnemyDamaged(float damage, bool fatal, HealthHaver enemyHealth)
  {
    if (!(bool) (UnityEngine.Object) enemyHealth || enemyHealth.IsBoss || !fatal || (double) UnityEngine.Random.value >= (double) this.Chance || !(bool) (UnityEngine.Object) enemyHealth.aiActor || !enemyHealth.aiActor.IsNormalEnemy || !(enemyHealth.aiActor.ActorName != "Hollow Point") || (bool) (UnityEngine.Object) enemyHealth.GetComponent<SpawnEnemyOnDeath>())
      return;
    string actorName = enemyHealth.aiActor.ActorName;
    switch (actorName)
    {
      case "Blobulin":
        break;
      case "Bombshee":
        break;
      default:
        if (actorName.Contains("Bullat") || actorName.StartsWith("Mine Flayer ") && (double) UnityEngine.Random.value > 0.25)
          break;
        PlayerController bestActivePlayer = GameManager.Instance.BestActivePlayer;
        if (enemyHealth.aiActor.ParentRoom != bestActivePlayer.CurrentRoom || enemyHealth.aiActor.ParentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.RoomClear) <= 0)
          break;
        Vector2 centerPosition = enemyHealth.aiActor.CenterPosition;
        IntVector2 intVector2 = centerPosition.ToIntVector2(VectorConversions.Floor);
        if (!GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2))
          break;
        CellData cellData = GameManager.Instance.Dungeon.data[intVector2];
        if (cellData.isExitCell || cellData.parentRoom != bestActivePlayer.CurrentRoom || centerPosition.GetAbsoluteRoom() != bestActivePlayer.CurrentRoom)
          break;
        AIActor aiActor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(this.GhostGuid), centerPosition, bestActivePlayer.CurrentRoom, true);
        if ((bool) (UnityEngine.Object) aiActor && !string.IsNullOrEmpty(this.GhostOverrideSpawnAnimation))
        {
          AIAnimator.NamedDirectionalAnimation directionalAnimation = aiActor.aiAnimator.OtherAnimations.Find((Predicate<AIAnimator.NamedDirectionalAnimation>) (vfx => vfx.name == "awaken"));
          if (directionalAnimation != null)
            directionalAnimation.anim.Prefix = this.GhostOverrideSpawnAnimation;
        }
        aiActor.HasBeenEngaged = true;
        break;
    }
  }

  private void OnDestroy()
  {
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      GameManager.Instance.AllPlayers[index].OnAnyEnemyReceivedDamage -= new Action<float, bool, HealthHaver>(this.OnEnemyDamaged);
  }
}
