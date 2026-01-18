using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("BulletManMagic/Astral1")]
public class BulletManMagicAstral1 : Script
  {
    private const int AirTime = 180;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletManMagicAstral1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class AstralBullet : Bullet
    {
      private Script m_parentScript;

      public AstralBullet(Script parentScript)
        : base("astral")
      {
        this.m_parentScript = parentScript;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletManMagicAstral1.AstralBullet__Topc__Iterator0()
        {
          _this = this
        };
      }

      public override void OnBulletDestruction(
        Bullet.DestroyType destroyType,
        SpeculativeRigidbody hitRigidbody,
        bool preventSpawningProjectiles)
      {
        if (this.m_parentScript == null)
          return;
        this.m_parentScript.ForceEnd();
      }
    }
  }

