using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/Helicopter/Missiles1")]
public class HelicoperMissiles1 : Script
  {
    public string[] s_targets = new string[4]
    {
      "shoot point 1",
      "shoot point 2",
      "shoot point 3",
      "shoot point 4"
    };

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new HelicoperMissiles1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class ArcBullet : Bullet
    {
      private Vector2 m_target;
      private float m_t;

      public ArcBullet(Vector2 target, float t)
        : base("missile")
      {
        this.m_target = target;
        this.m_t = t;
      }

      public override void Initialize()
      {
        tk2dSpriteAnimator spriteAnimator = this.Projectile.spriteAnimator;
        spriteAnimator.Play();
        spriteAnimator.SetFrame(spriteAnimator.CurrentClip.frames.Length - 1);
        base.Initialize();
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HelicoperMissiles1.ArcBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

