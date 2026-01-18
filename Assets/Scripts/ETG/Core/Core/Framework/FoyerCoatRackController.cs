using System.Collections;
using System.Diagnostics;

#nullable disable

public class FoyerCoatRackController : BraveBehaviour
  {
    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FoyerCoatRackController__Startc__Iterator0()
      {
        _this = this
      };
    }
  }

