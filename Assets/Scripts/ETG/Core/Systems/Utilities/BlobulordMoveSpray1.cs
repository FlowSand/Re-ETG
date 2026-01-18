using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/Blobulord/MoveSpray1")]
public class BlobulordMoveSpray1 : Script
  {
    private const float NumBullets = 30f;
    private const float ArcDegrees = 150f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BlobulordMoveSpray1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

