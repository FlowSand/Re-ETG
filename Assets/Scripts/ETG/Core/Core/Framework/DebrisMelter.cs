using System;
using System.Collections;
using System.Diagnostics;

#nullable disable

public class DebrisMelter : BraveBehaviour
  {
    public float delay;
    public float meltTime;
    public bool doesGoop;
    [ShowInInspectorIf("doesGoop", false)]
    public GoopDefinition goop;
    [ShowInInspectorIf("doesGoop", false)]
    public float goopRadius = 1f;

    public void Start() => this.debris.OnGrounded += new Action<DebrisObject>(this.OnGrounded);

    protected override void OnDestroy() => base.OnDestroy();

    private void OnGrounded(DebrisObject debrisObject) => this.StartCoroutine(this.DoMeltCR());

    [DebuggerHidden]
    private IEnumerator DoMeltCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DebrisMelter__DoMeltCRc__Iterator0()
      {
        _this = this
      };
    }
  }

