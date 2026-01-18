using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/Bashellisk/RandomShots1")]
public class BashelliskRandomShots1 : Script
  {
    public int NumBullets = 5;
    public float BulletSpeed = 10f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BashelliskRandomShots1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

