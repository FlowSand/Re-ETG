// Decompiled with JetBrains decompiler
// Type: CombineEvaporateEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class CombineEvaporateEffect : MonoBehaviour
    {
      public GameObject ParticleSystemToSpawn;
      public Shader FallbackShader;

      private void Start()
      {
        this.GetComponent<Projectile>().OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
      }

      private void HandleHitEnemy(Projectile proj, SpeculativeRigidbody hitRigidbody, bool fatal)
      {
        if (!fatal)
          return;
        AIActor aiActor = hitRigidbody.aiActor;
        if (!(bool) (UnityEngine.Object) aiActor || !aiActor.IsNormalEnemy || (bool) (UnityEngine.Object) aiActor.healthHaver && aiActor.healthHaver.IsBoss)
          return;
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemyDeath(aiActor, proj.LastVelocity));
      }

      [DebuggerHidden]
      private IEnumerator HandleEnemyDeath(AIActor target, Vector2 motionDirection)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CombineEvaporateEffect__HandleEnemyDeathc__Iterator0()
        {
          target = target,
          motionDirection = motionDirection,
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
        tk2dSprite.usesOverrideMaterial = true;
        bool flag = false;
        if ((UnityEngine.Object) target.optionalPalette != (UnityEngine.Object) null)
        {
          flag = true;
          tk2dSprite.renderer.material.SetTexture("_PaletteTex", (Texture) target.optionalPalette);
        }
        if (!tk2dSprite.renderer.material.shader.name.Contains("ColorEmissive"))
          ;
        return gameObject2.transform;
      }
    }

}
