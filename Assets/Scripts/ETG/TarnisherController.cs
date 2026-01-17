// Decompiled with JetBrains decompiler
// Type: TarnisherController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class TarnisherController : BraveBehaviour
{
  public void Awake() => this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);

  protected override void OnDestroy()
  {
    base.OnDestroy();
    this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
  }

  private void OnPreDeath(Vector2 vector2)
  {
    this.aiAnimator.OtherAnimations.Find((Predicate<AIAnimator.NamedDirectionalAnimation>) (a => a.name == "pitfall")).anim.Prefix = "pitfall_dead";
  }
}
