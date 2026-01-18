using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class MeduziDeathController : BraveBehaviour
  {
    public void Start()
    {
      this.healthHaver.ManualDeathHandling = true;
      this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
    }

    protected override void OnDestroy()
    {
      this.specRigidbody.OnRigidbodyCollision -= new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
      base.OnDestroy();
    }

    public void Shatter()
    {
      this.aiAnimator.enabled = false;
      this.spriteAnimator.PlayAndDestroyObject("burst");
      this.specRigidbody.enabled = false;
      this.specRigidbody.OnRigidbodyCollision -= new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
    }

    private void OnBossDeath(Vector2 dir)
    {
      this.aiAnimator.PlayUntilCancelled("death", true);
      this.StartCoroutine(this.HandlePostDeathExplosionCR());
      this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnBossDeath);
    }

    [DebuggerHidden]
    private IEnumerator HandlePostDeathExplosionCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MeduziDeathController__HandlePostDeathExplosionCRc__Iterator0()
      {
        _this = this
      };
    }

    private void OnRigidbodyCollision(CollisionData collision)
    {
      if (!(bool) (UnityEngine.Object) collision.OtherRigidbody.projectile)
        return;
      this.Shatter();
    }
  }

