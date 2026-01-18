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

