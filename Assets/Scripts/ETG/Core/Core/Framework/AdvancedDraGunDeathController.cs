// Decompiled with JetBrains decompiler
// Type: AdvancedDraGunDeathController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class AdvancedDraGunDeathController : BraveBehaviour
    {
      public GameObject fingerDebris;
      public GameObject neckDebris;
      private DraGunController m_dragunController;
      private tk2dSpriteAnimator m_roarDummy;

      public void Awake()
      {
        this.m_dragunController = this.GetComponent<DraGunController>();
        this.m_roarDummy = this.aiActor.transform.Find("RoarDummy").GetComponent<tk2dSpriteAnimator>();
      }

      public void Start()
      {
        this.healthHaver.ManualDeathHandling = true;
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
        this.healthHaver.OverrideKillCamTime = new float?(16.5f);
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnBossDeath(Vector2 dir)
      {
        this.behaviorSpeculator.enabled = false;
        GameManager.Instance.StartCoroutine(this.OnDeathExplosionsCR());
      }

      [DebuggerHidden]
      private IEnumerator OnDeathExplosionsCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AdvancedDraGunDeathController__OnDeathExplosionsCRc__Iterator0()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator ExplodeHand(AIAnimator hand, float headDirection)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AdvancedDraGunDeathController__ExplodeHandc__Iterator1()
        {
          headDirection = headDirection,
          hand = hand,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator ExplodeBall(
        Animation arm,
        string ballName,
        float headDirection,
        float postDelay)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AdvancedDraGunDeathController__ExplodeBallc__Iterator2()
        {
          arm = arm,
          ballName = ballName,
          headDirection = headDirection,
          postDelay = postDelay,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator ExplodeShoulder(
        Animation arm,
        string ballName,
        float headDirection,
        float postDelay)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AdvancedDraGunDeathController__ExplodeShoulderc__Iterator3()
        {
          arm = arm,
          ballName = ballName,
          headDirection = headDirection,
          postDelay = postDelay,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator FadeBodyCR(float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AdvancedDraGunDeathController__FadeBodyCRc__Iterator4()
        {
          duration = duration,
          _this = this
        };
      }

      private void SpawnBones(GameObject bonePrefab, int count, Vector2 min, Vector2 max)
      {
        Vector2 min1 = this.aiActor.ParentRoom.area.basePosition.ToVector2() + min + new Vector2(0.0f, (float) DraGunRoomPlaceable.HallHeight);
        Vector2 max1 = this.aiActor.ParentRoom.area.basePosition.ToVector2() + this.aiActor.ParentRoom.area.dimensions.ToVector2() + max;
        for (int index = 0; index < count; ++index)
        {
          Vector2 position = BraveUtility.RandomVector2(min1, max1);
          GameObject gameObject = SpawnManager.SpawnVFX(bonePrefab, (Vector3) position, Quaternion.identity);
          if ((bool) (UnityEngine.Object) gameObject)
          {
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            DebrisObject orAddComponent = gameObject.GetOrAddComponent<DebrisObject>();
            orAddComponent.decayOnBounce = 0.5f;
            orAddComponent.bounceCount = 1;
            orAddComponent.canRotate = true;
            Vector2 vector2 = BraveMathCollege.DegreesToVector(UnityEngine.Random.Range(-80f, -100f)) * UnityEngine.Random.Range(0.1f, 3f);
            Vector3 startingForce = new Vector3(vector2.x, (double) vector2.y >= 0.0 ? 0.0f : vector2.y, (double) vector2.y <= 0.0 ? 0.0f : vector2.y);
            if ((bool) (UnityEngine.Object) orAddComponent.minorBreakable)
              orAddComponent.minorBreakable.enabled = true;
            orAddComponent.Trigger(startingForce, UnityEngine.Random.Range(1f, 2f));
            if ((bool) (UnityEngine.Object) orAddComponent.specRigidbody)
              orAddComponent.OnGrounded += new Action<DebrisObject>(this.HandleComplexDebris);
          }
        }
      }

      private void HandleComplexDebris(DebrisObject debrisObject)
      {
        GameManager.Instance.StartCoroutine(this.DelayedSpriteFixer(debrisObject.sprite));
        SpeculativeRigidbody specRigidbody = debrisObject.specRigidbody;
        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(specRigidbody);
        UnityEngine.Object.Destroy((UnityEngine.Object) debrisObject);
        specRigidbody.RegenerateCache();
      }

      [DebuggerHidden]
      private IEnumerator DelayedSpriteFixer(tk2dBaseSprite sprite)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AdvancedDraGunDeathController__DelayedSpriteFixerc__Iterator5()
        {
          sprite = sprite
        };
      }
    }

}
