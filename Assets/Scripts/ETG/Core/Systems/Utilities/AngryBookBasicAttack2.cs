using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("AngryBook/BasicAttack2")]
public class AngryBookBasicAttack2 : Script
  {
    public int LineBullets = 10;
    public const float Height = 2.5f;
    public const float Width = 1.9f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AngryBookBasicAttack2__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class DefaultBullet : Bullet
    {
      public int spawnTime;

      public DefaultBullet(int spawnTime)
        : base()
      {
        this.spawnTime = spawnTime;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AngryBookBasicAttack2.DefaultBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

