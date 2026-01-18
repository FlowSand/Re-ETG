using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[ExecuteInEditMode]
public class ExperimentalGodrayHelper : MonoBehaviour
  {
    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ExperimentalGodrayHelper__Startc__Iterator0()
      {
        _this = this
      };
    }
  }

