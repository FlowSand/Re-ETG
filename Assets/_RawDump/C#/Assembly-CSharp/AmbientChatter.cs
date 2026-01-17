// Decompiled with JetBrains decompiler
// Type: AmbientChatter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class AmbientChatter : MonoBehaviour
{
  public float MinTimeBetweenChatter = 10f;
  public float MaxTimeBetweenChatter = 20f;
  public float ChatterDuration = 5f;
  public string ChatterStringKey;
  public Transform SpeakPoint;
  public bool WanderInRadius;
  public float WanderRadius = 3f;
  private Transform m_transform;
  private Vector3 m_startPosition;

  private void Start()
  {
    this.m_transform = this.transform;
    this.m_startPosition = this.m_transform.position;
    if (this.WanderInRadius)
      this.StartCoroutine(this.HandleWander());
    this.StartCoroutine(this.HandleAmbientChatter());
  }

  [DebuggerHidden]
  private IEnumerator HandleWander()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new AmbientChatter.\u003CHandleWander\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator HandleAmbientChatter()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new AmbientChatter.\u003CHandleAmbientChatter\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }
}
