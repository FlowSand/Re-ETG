// Decompiled with JetBrains decompiler
// Type: GunNutSlamVfx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class GunNutSlamVfx : MonoBehaviour
{
  public VFXPool SlamVfx;
  public float SlamCount;
  public float SlamDistance;
  public float SlamDelay;
  public VFXPool DustVfx;
  public float DustOffset;

  public void OnSpawned() => this.StartCoroutine(this.DoVfx());

  [DebuggerHidden]
  private IEnumerator DoVfx()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new GunNutSlamVfx.\u003CDoVfx\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
