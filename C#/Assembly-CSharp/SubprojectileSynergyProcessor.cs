// Decompiled with JetBrains decompiler
// Type: SubprojectileSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class SubprojectileSynergyProcessor : MonoBehaviour
{
  [LongNumericEnum]
  public CustomSynergyType RequiredSynergy;
  public Projectile Subprojectile;
  public bool DoesOrbit = true;
  public float OrbitMinRadius = 1f;
  public float OrbitMaxRadius = 1f;
  private Projectile m_projectile;

  private void Start()
  {
    this.m_projectile = this.GetComponent<Projectile>();
    if (!(bool) (Object) this.m_projectile || !(bool) (Object) this.m_projectile.PossibleSourceGun || !this.m_projectile.PossibleSourceGun.OwnerHasSynergy(this.RequiredSynergy))
      return;
    this.m_projectile.StartCoroutine(this.CreateSubprojectile());
  }

  [DebuggerHidden]
  private IEnumerator CreateSubprojectile()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SubprojectileSynergyProcessor.\u003CCreateSubprojectile\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
