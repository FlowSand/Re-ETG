using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Minibosses/PhantomAgunim/Arc1")]
public class PhantomAgunimArc1 : Script
  {
    private const float NumBullets = 19f;
    private const int ArcTime = 15;
    private const float EllipseA = 2.25f;
    private const float EllipseB = 1.5f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new PhantomAgunimArc1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

