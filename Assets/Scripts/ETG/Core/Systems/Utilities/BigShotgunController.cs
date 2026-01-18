using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class BigShotgunController : MonoBehaviour
  {
    [EnemyIdentifier]
    public string[] TargetEnemies;
    public float SuckRadius = 8f;
    private Gun m_gun;

    private void Awake() => this.m_gun = this.GetComponent<Gun>();

    private void LateUpdate()
    {
      if (!(bool) (UnityEngine.Object) this.m_gun || !this.m_gun.IsReloading || !(this.m_gun.CurrentOwner is PlayerController))
        return;
      PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
      if (currentOwner.CurrentRoom == null)
        return;
      currentOwner.CurrentRoom.ApplyActionToNearbyEnemies(currentOwner.CenterPosition, this.SuckRadius, new Action<AIActor, float>(this.ProcessEnemy));
    }

    private void ProcessEnemy(AIActor target, float distance)
    {
      for (int index = 0; index < this.TargetEnemies.Length; ++index)
      {
        if (target.EnemyGuid == this.TargetEnemies[index])
        {
          GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(target));
          target.EraseFromExistence(true);
          break;
        }
      }
    }

    [DebuggerHidden]
    private IEnumerator HandleEnemySuck(AIActor target)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BigShotgunController__HandleEnemySuckc__Iterator0()
      {
        target = target,
        _this = this
      };
    }

    private Transform CreateEmptySprite(AIActor target)
    {
      GameObject gameObject1 = new GameObject("suck image");
      gameObject1.layer = target.gameObject.layer;
      tk2dSprite tk2dSprite = gameObject1.AddComponent<tk2dSprite>();
      gameObject1.transform.parent = SpawnManager.Instance.VFX;
      tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
      tk2dSprite.transform.position = target.sprite.transform.position;
      GameObject gameObject2 = new GameObject("image parent");
      gameObject2.transform.position = (Vector3) tk2dSprite.WorldCenter;
      tk2dSprite.transform.parent = gameObject2.transform;
      if ((UnityEngine.Object) target.optionalPalette != (UnityEngine.Object) null)
        tk2dSprite.renderer.material.SetTexture("_PaletteTex", (Texture) target.optionalPalette);
      return gameObject2.transform;
    }
  }

