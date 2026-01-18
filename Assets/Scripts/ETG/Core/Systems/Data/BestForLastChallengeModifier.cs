using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class BestForLastChallengeModifier : ChallengeModifier
  {
    public GameObject KingVFX;
    private bool m_isActive;
    private HealthHaver m_king;
    private RoomHandler m_room;
    private List<AIActor> m_activeEnemies = new List<AIActor>();
    private GameObject m_instanceVFX;

    private bool IsValidEnemy(AIActor testEnemy)
    {
      return (bool) (Object) testEnemy && !testEnemy.IsHarmlessEnemy && (!(bool) (Object) testEnemy.healthHaver || !testEnemy.healthHaver.PreventAllDamage) && (!(bool) (Object) testEnemy.GetComponent<ExplodeOnDeath>() || testEnemy.IsSignatureEnemy);
    }

    private void Start()
    {
      RoomHandler currentRoom = GameManager.Instance.PrimaryPlayer.CurrentRoom;
      this.m_room = currentRoom;
      currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear, ref this.m_activeEnemies);
      int num = Random.Range(0, this.m_activeEnemies.Count);
      for (int index = 0; index < this.m_activeEnemies.Count; ++index)
      {
        if (index == num)
        {
          if (this.IsValidEnemy(this.m_activeEnemies[index]))
          {
            Vector2 offset = !(bool) (Object) this.m_activeEnemies[index].sprite ? Vector2.up : Vector2.up * (this.m_activeEnemies[index].sprite.WorldTopCenter.y - this.m_activeEnemies[index].sprite.WorldBottomCenter.y);
            GameObject gameObject = this.m_activeEnemies[index].PlayEffectOnActor(this.KingVFX, (Vector3) offset);
            Bounds bounds = this.m_activeEnemies[index].sprite.GetBounds();
            Vector3 vector3 = (this.m_activeEnemies[index].transform.position + new Vector3((float) (((double) bounds.max.x + (double) bounds.min.x) / 2.0), bounds.max.y, 0.0f).Quantize(1f / 16f)) with
            {
              y = this.m_activeEnemies[index].transform.position.y + this.m_activeEnemies[index].sprite.GetUntrimmedBounds().max.y
            };
            vector3.x -= gameObject.GetComponent<tk2dSprite>().GetBounds().extents.x;
            vector3.y -= gameObject.GetComponent<tk2dSprite>().GetBounds().extents.y;
            gameObject.transform.position = vector3;
            this.m_instanceVFX = gameObject;
            ChallengeManager.Instance.StartCoroutine(this.DelayedSpawnIcon());
            this.m_activeEnemies[index].healthHaver.PreventAllDamage = true;
            this.m_king = this.m_activeEnemies[index].healthHaver;
          }
          else
            ++num;
        }
      }
      this.m_isActive = true;
    }

    [DebuggerHidden]
    private IEnumerator DelayedSpawnIcon()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BestForLastChallengeModifier__DelayedSpawnIconc__Iterator0()
      {
        _this = this
      };
    }

    private void Update()
    {
      if (!this.m_isActive)
        return;
      this.m_room.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear, ref this.m_activeEnemies);
      if (!(bool) (Object) this.m_king || this.m_activeEnemies.Count != 1)
        return;
      this.m_king.PreventAllDamage = false;
      if (!(bool) (Object) this.m_instanceVFX)
        return;
      this.m_instanceVFX.GetComponent<tk2dSprite>().SetSprite("lastmanstanding_check_001");
    }
  }

