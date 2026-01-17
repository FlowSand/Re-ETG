// Decompiled with JetBrains decompiler
// Type: PhantomAgunimArc1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
    return (IEnumerator) new PhantomAgunimArc1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
