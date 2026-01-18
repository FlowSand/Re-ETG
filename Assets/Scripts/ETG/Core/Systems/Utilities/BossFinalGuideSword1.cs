using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalGuide/Sword1")]
public class BossFinalGuideSword1 : Script
  {
    private const int SetupTime = 20;
    private const int HoldTime = 30;
    private const int SwingTime = 25;
    private float m_sign;
    private bool m_doubleSwing;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossFinalGuideSword1__Topc__Iterator0()
      {
        _this = this
      };
    }

    private void FireLine(Vector2 spawnPoint, Vector2 start, Vector2 end, int numBullets)
    {
      Vector2 vector2 = (end - start) / (float) Mathf.Max(1, numBullets - 1);
      float num = 0.333333343f;
      for (int index = 0; index < numBullets; ++index)
      {
        Vector2 a = numBullets != 1 ? start + vector2 * (float) index : end;
        float speed = Vector2.Distance(a, spawnPoint) / num;
        this.Fire(new Offset(spawnPoint, transform: string.Empty), new Brave.BulletScript.Direction((a - spawnPoint).ToAngle()), new Brave.BulletScript.Speed(speed), (Bullet) new BossFinalGuideSword1.SwordBullet(this.Position, this.m_sign, this.m_doubleSwing));
      }
    }

    public class SwordBullet : Bullet
    {
      private Vector2 m_origin;
      private float m_sign;
      private bool m_doubleSwing;

      public SwordBullet(Vector2 origin, float sign, bool doubleSwing)
        : base()
      {
        this.m_origin = origin;
        this.m_sign = sign;
        this.m_doubleSwing = doubleSwing;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossFinalGuideSword1.SwordBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

