using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/Bashellisk/SnakeBullets1")]
public class BashelliskSnakeBullets1 : Script
  {
    private const int NumBullets = 8;
    private const int BulletSpeed = 11;
    private const float SnakeMagnitude = 0.6f;
    private const float SnakePeriod = 3f;

    protected override IEnumerator Top()
    {
      float aimDirection = this.GetAimDirection((double) Random.value >= 0.5 ? 1f : 0.0f, 11f);
      for (int index = 0; index < 8; ++index)
        this.Fire(new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(11f), (Bullet) new BashelliskSnakeBullets1.SnakeBullet(index * 3));
      return (IEnumerator) null;
    }

    public class SnakeBullet : Bullet
    {
      private int delay;

      public SnakeBullet(int delay)
        : base("snakeBullet")
      {
        this.delay = delay;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BashelliskSnakeBullets1.SnakeBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

