using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class YellowChamberItem : PassiveItem
  {
    public float ChanceToHappen = 0.25f;
    public GameActorCharmEffect CharmEffect;
    public GameObject EraseVFX;
    private PlayerController m_player;
    private AIActor m_currentlyCharmedEnemy;
    private List<AIActor> m_enemyList = new List<AIActor>();

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      this.m_player = player;
      base.Pickup(player);
      player.OnEnteredCombat += new System.Action(this.OnEnteredCombat);
    }

    private void OnEnteredCombat()
    {
      if ((bool) (UnityEngine.Object) this.m_currentlyCharmedEnemy || (double) UnityEngine.Random.value >= (double) this.ChanceToHappen)
        return;
      this.m_player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref this.m_enemyList);
      for (int index = 0; index < this.m_enemyList.Count; ++index)
      {
        AIActor enemy = this.m_enemyList[index];
        if (!(bool) (UnityEngine.Object) enemy || !enemy.IsNormalEnemy || !(bool) (UnityEngine.Object) enemy.healthHaver || enemy.healthHaver.IsBoss || enemy.IsHarmlessEnemy)
        {
          this.m_enemyList.RemoveAt(index);
          --index;
        }
      }
      if (this.m_enemyList.Count <= 1)
        return;
      AIActor enemy1 = this.m_enemyList[UnityEngine.Random.Range(0, this.m_enemyList.Count)];
      enemy1.IgnoreForRoomClear = true;
      enemy1.ParentRoom.ResetEnemyHPPercentage();
      enemy1.ApplyEffect((GameActorEffect) this.CharmEffect);
      this.m_currentlyCharmedEnemy = enemy1;
    }

    protected override void Update()
    {
      if (this.m_pickedUp && (bool) (UnityEngine.Object) this.m_player && (bool) (UnityEngine.Object) this.m_currentlyCharmedEnemy && (this.m_player.CurrentRoom == null || this.m_player.CurrentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.RoomClear) <= 0))
        this.EatCharmedEnemy();
      base.Update();
    }

    private void EatCharmedEnemy()
    {
      if (!(bool) (UnityEngine.Object) this.m_currentlyCharmedEnemy)
        return;
      if ((bool) (UnityEngine.Object) this.m_currentlyCharmedEnemy.behaviorSpeculator)
        this.m_currentlyCharmedEnemy.behaviorSpeculator.Stun(1f);
      if ((bool) (UnityEngine.Object) this.m_currentlyCharmedEnemy.knockbackDoer)
        this.m_currentlyCharmedEnemy.knockbackDoer.SetImmobile(true, nameof (YellowChamberItem));
      this.m_currentlyCharmedEnemy.StartCoroutine(this.DelayedDestroyEnemy(this.m_currentlyCharmedEnemy, this.m_currentlyCharmedEnemy.PlayEffectOnActor(this.EraseVFX, new Vector3(0.0f, -1f, 0.0f), false).GetComponent<tk2dSpriteAnimator>()));
      this.m_currentlyCharmedEnemy = (AIActor) null;
    }

    [DebuggerHidden]
    private IEnumerator DelayedDestroyEnemy(AIActor enemy, tk2dSpriteAnimator vfxAnimator)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new YellowChamberItem__DelayedDestroyEnemyc__Iterator0()
      {
        vfxAnimator = vfxAnimator,
        enemy = enemy
      };
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      this.m_player = (PlayerController) null;
      debrisObject.GetComponent<YellowChamberItem>().m_pickedUpThisRun = true;
      player.OnEnteredCombat -= new System.Action(this.OnEnteredCombat);
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      if (!(bool) (UnityEngine.Object) this.m_player)
        return;
      this.m_player.OnEnteredCombat -= new System.Action(this.OnEnteredCombat);
    }
  }

