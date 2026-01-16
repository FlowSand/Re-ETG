// Decompiled with JetBrains decompiler
// Type: KingEnemyChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class KingEnemyChallengeModifier : ChallengeModifier
{
  public GameObject KingVFX;
  private bool m_isActive;
  private HealthHaver m_king;

  private bool IsValidEnemy(AIActor testEnemy)
  {
    return (bool) (UnityEngine.Object) testEnemy && !testEnemy.IsHarmlessEnemy && (!(bool) (UnityEngine.Object) testEnemy.healthHaver || !testEnemy.healthHaver.PreventAllDamage) && (!(bool) (UnityEngine.Object) testEnemy.GetComponent<ExplodeOnDeath>() || testEnemy.IsSignatureEnemy);
  }

  private void Start()
  {
    List<AIActor> activeEnemies = GameManager.Instance.PrimaryPlayer.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
    int num = UnityEngine.Random.Range(0, activeEnemies.Count);
    for (int index = 0; index < activeEnemies.Count; ++index)
    {
      if (index == num)
      {
        if (this.IsValidEnemy(activeEnemies[index]))
        {
          Vector2 offset = !(bool) (UnityEngine.Object) activeEnemies[index].sprite ? Vector2.up : Vector2.up * (activeEnemies[index].sprite.WorldTopCenter.y - activeEnemies[index].sprite.WorldBottomCenter.y);
          GameObject gameObject = activeEnemies[index].PlayEffectOnActor(this.KingVFX, (Vector3) offset);
          if ((bool) (UnityEngine.Object) activeEnemies[index].OverrideBuffEffectPosition)
          {
            Vector3 position = activeEnemies[index].OverrideBuffEffectPosition.position;
            position.x -= gameObject.GetComponent<tk2dSprite>().GetBounds().extents.x;
            gameObject.transform.position = position;
          }
          else if ((bool) (UnityEngine.Object) activeEnemies[index].healthHaver && activeEnemies[index].healthHaver.IsBoss)
          {
            Vector3 unitTopCenter = (Vector3) activeEnemies[index].specRigidbody.HitboxPixelCollider.UnitTopCenter;
            unitTopCenter.x -= gameObject.GetComponent<tk2dSprite>().GetBounds().extents.x;
            unitTopCenter.y += gameObject.GetComponent<tk2dSprite>().GetBounds().extents.y;
            gameObject.transform.position = unitTopCenter;
          }
          else
          {
            Bounds bounds = activeEnemies[index].sprite.GetBounds();
            Vector3 vector3 = (activeEnemies[index].transform.position + new Vector3((float) (((double) bounds.max.x + (double) bounds.min.x) / 2.0), bounds.max.y, 0.0f).Quantize(1f / 16f)) with
            {
              y = activeEnemies[index].transform.position.y + activeEnemies[index].sprite.GetUntrimmedBounds().max.y
            };
            vector3.x -= gameObject.GetComponent<tk2dSprite>().GetBounds().extents.x;
            gameObject.transform.position = vector3;
          }
          activeEnemies[index].healthHaver.OnDeath += new Action<Vector2>(this.OnKingDeath);
          this.m_king = activeEnemies[index].healthHaver;
        }
        else
          ++num;
      }
      else if ((bool) (UnityEngine.Object) activeEnemies[index] && (bool) (UnityEngine.Object) activeEnemies[index].healthHaver && !activeEnemies[index].IsMimicEnemy)
        activeEnemies[index].healthHaver.PreventAllDamage = true;
    }
    this.m_isActive = true;
  }

  private void Update()
  {
    if (!this.m_isActive || (bool) (UnityEngine.Object) this.m_king && !this.m_king.IsDead)
      return;
    this.m_isActive = false;
    this.OnKingDeath(Vector2.zero);
  }

  private void OnKingDeath(Vector2 obj)
  {
    List<AIActor> activeEnemies = GameManager.Instance.PrimaryPlayer.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
    for (int index = 0; index < activeEnemies.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) activeEnemies[index] && (bool) (UnityEngine.Object) activeEnemies[index].healthHaver)
        activeEnemies[index].healthHaver.PreventAllDamage = false;
    }
  }
}
