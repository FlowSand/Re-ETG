// Decompiled with JetBrains decompiler
// Type: RemoteMineController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class RemoteMineController : BraveBehaviour
{
  public ExplosionData explosionData;
  [CheckAnimation(null)]
  public string explodeAnimName;

  public void Detonate()
  {
    if (!string.IsNullOrEmpty(this.explodeAnimName))
    {
      this.StartCoroutine(this.DelayDetonateFrame());
    }
    else
    {
      Exploder.Explode(this.transform.position, this.explosionData, Vector2.zero);
      Object.Destroy((Object) this.gameObject);
    }
  }

  [DebuggerHidden]
  private IEnumerator DelayDetonateFrame()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new RemoteMineController.\u003CDelayDetonateFrame\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  protected override void OnDestroy() => base.OnDestroy();
}
