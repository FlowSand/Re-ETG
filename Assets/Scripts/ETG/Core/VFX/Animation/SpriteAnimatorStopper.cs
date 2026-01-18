using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class SpriteAnimatorStopper : MonoBehaviour
  {
    public float duration = 10f;
    private tk2dSpriteAnimator animator;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new SpriteAnimatorStopper__Startc__Iterator0()
      {
        _this = this
      };
    }
  }

