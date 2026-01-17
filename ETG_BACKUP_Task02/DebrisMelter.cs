// Decompiled with JetBrains decompiler
// Type: DebrisMelter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
    return (IEnumerator) new DebrisMelter.\u003CDoMeltCR\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
