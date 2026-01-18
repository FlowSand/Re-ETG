using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class BossFinalConvictHegemonyReinforceDoer : CustomReinforceDoer
  {
    public GameObject ropeVfx;
    private bool m_isFinished;

    public override void StartIntro() => this.StartCoroutine(this.DoIntro());

    [DebuggerHidden]
    private IEnumerator DoIntro()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossFinalConvictHegemonyReinforceDoer__DoIntroc__Iterator0()
      {
        _this = this
      };
    }

    public override bool IsFinished => this.m_isFinished;
  }

