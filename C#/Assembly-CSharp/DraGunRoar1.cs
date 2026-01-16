// Decompiled with JetBrains decompiler
// Type: DraGunRoar1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/DraGun/Roar1")]
public class DraGunRoar1 : Script
{
  public int NumRockets = 3;
  private static int[] s_xValues;
  private static int[] s_yValues;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new DraGunRoar1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void FireRocket(GameObject skyRocket, Vector2 target)
  {
    SkyRocket component = SpawnManager.SpawnProjectile(skyRocket, (Vector3) this.Position, Quaternion.identity).GetComponent<SkyRocket>();
    component.TargetVector2 = target;
    tk2dSprite componentInChildren = component.GetComponentInChildren<tk2dSprite>();
    component.transform.position = component.transform.position.WithY(component.transform.position.y - componentInChildren.transform.localPosition.y);
    component.ExplosionData.ignoreList.Add(this.BulletBank.specRigidbody);
  }
}
