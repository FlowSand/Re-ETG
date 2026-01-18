using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("BulletManMagic/Astral2")]
public class BulletManMagicAstral2 : Script
  {
    protected override IEnumerator Top()
    {
      this.Fire((Bullet) new BulletManMagicAstral2.AstralBullet());
      return (IEnumerator) null;
    }

    public class AstralBullet : Bullet
    {
      private const int NumBullets = 18;

      public AstralBullet()
        : base("astral")
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletManMagicAstral2.AstralBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

