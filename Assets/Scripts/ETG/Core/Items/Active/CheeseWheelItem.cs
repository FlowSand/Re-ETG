// Decompiled with JetBrains decompiler
// Type: CheeseWheelItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class CheeseWheelItem : PlayerItem
    {
      public float duration = 10f;
      public float BossContactDamage = 30f;
      public GameObject TransformationVFX;

      protected override void DoEffect(PlayerController user)
      {
        base.DoEffect(user);
        user.StartCoroutine(this.HandleDuration(user));
      }

      [DebuggerHidden]
      private IEnumerator HandleDuration(PlayerController user)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CheeseWheelItem.<HandleDuration>c__Iterator0()
        {
          user = user,
          $this = this
        };
      }

      private void HandlePrerigidbodyCollision(
        SpeculativeRigidbody myRigidbody,
        PixelCollider myPixelCollider,
        SpeculativeRigidbody otherRigidbody,
        PixelCollider otherPixelCollider)
      {
        if (!(bool) (Object) otherRigidbody || !(bool) (Object) otherRigidbody.healthHaver || !otherRigidbody.healthHaver.IsDead)
          return;
        PhysicsEngine.SkipCollision = true;
      }

      private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
      {
        AIActor component1 = rigidbodyCollision.OtherRigidbody.GetComponent<AIActor>();
        bool flag = false;
        if ((bool) (Object) component1 && component1.IsNormalEnemy && (bool) (Object) component1.healthHaver && component1.healthHaver.IsVulnerable)
        {
          if (component1.FlagToSetOnDeath == GungeonFlags.BOSSKILLED_DEMONWALL)
          {
            flag = true;
            component1.healthHaver.ApplyDamage(this.BossContactDamage, rigidbodyCollision.Normal * -1f, "pakku pakku");
            if ((bool) (Object) rigidbodyCollision.MyRigidbody && (bool) (Object) rigidbodyCollision.MyRigidbody.knockbackDoer)
              rigidbodyCollision.MyRigidbody.knockbackDoer.ApplyKnockback(Vector2.down, 80f);
          }
          else if (component1.healthHaver.IsBoss)
          {
            flag = true;
            component1.healthHaver.ApplyDamage(this.BossContactDamage, rigidbodyCollision.Normal * -1f, "pakku pakku");
            if ((bool) (Object) rigidbodyCollision.MyRigidbody && (bool) (Object) rigidbodyCollision.MyRigidbody.knockbackDoer)
              rigidbodyCollision.MyRigidbody.knockbackDoer.ApplyKnockback(rigidbodyCollision.Normal, 40f);
          }
          else
          {
            KeyBulletManController component2 = component1.GetComponent<KeyBulletManController>();
            if ((bool) (Object) component2)
              component2.ForceHandleRewards();
            GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(component1, rigidbodyCollision.MyRigidbody));
            component1.EraseFromExistenceWithRewards();
          }
        }
        else
        {
          MajorBreakable component3 = rigidbodyCollision.OtherRigidbody.GetComponent<MajorBreakable>();
          if ((bool) (Object) rigidbodyCollision.OtherRigidbody.GetComponent<BodyPartController>() && (bool) (Object) component3)
          {
            flag = true;
            Vector2 normalized = (rigidbodyCollision.MyRigidbody.UnitCenter - rigidbodyCollision.OtherRigidbody.UnitCenter).normalized;
            component3.ApplyDamage(this.BossContactDamage / 2f, normalized * -1f, false);
            if ((bool) (Object) component3.healthHaver)
              component3.healthHaver.ApplyDamage(this.BossContactDamage / 2f, normalized * -1f, "pakku pakku");
            if ((bool) (Object) rigidbodyCollision.MyRigidbody && (bool) (Object) rigidbodyCollision.MyRigidbody.knockbackDoer)
              rigidbodyCollision.MyRigidbody.knockbackDoer.ApplyKnockback(normalized.normalized, 40f);
          }
        }
        if (!flag)
          return;
        rigidbodyCollision.MyRigidbody.RegisterTemporaryCollisionException(rigidbodyCollision.OtherRigidbody, 0.5f);
      }

      [DebuggerHidden]
      private IEnumerator HandleEnemySuck(AIActor target, SpeculativeRigidbody ownerRigidbody)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CheeseWheelItem.<HandleEnemySuck>c__Iterator1()
        {
          target = target,
          ownerRigidbody = ownerRigidbody,
          $this = this
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
        if ((Object) target.optionalPalette != (Object) null)
          tk2dSprite.renderer.material.SetTexture("_PaletteTex", (Texture) target.optionalPalette);
        return gameObject2.transform;
      }

      protected override void OnPreDrop(PlayerController user)
      {
        this.IsCurrentlyActive = false;
        base.OnPreDrop(user);
      }
    }

}
