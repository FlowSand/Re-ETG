// Decompiled with JetBrains decompiler
// Type: PostShootProjectileModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class PostShootProjectileModifier : MonoBehaviour
{
  public int NumberBouncesToSet;

  private void Start()
  {
    this.GetComponent<Gun>().PostProcessProjectile += new Action<Projectile>(this.PostProcessProjectile);
  }

  private void PostProcessProjectile(Projectile obj)
  {
    BounceProjModifier component = obj.GetComponent<BounceProjModifier>();
    if (!(bool) (UnityEngine.Object) component)
      return;
    component.numberOfBounces = this.NumberBouncesToSet;
  }
}
