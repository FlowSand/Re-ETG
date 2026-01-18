using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/Grenade1")]
public class DraGunGrenade1 : Script
  {
    public int NumRockets = 11;
    public float Magnitude = 4.5f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DraGunGrenade1__Topc__Iterator0()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator FireWave(bool reverse, bool offset, float sinOffset)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DraGunGrenade1__FireWavec__Iterator1()
      {
        offset = offset,
        sinOffset = sinOffset,
        reverse = reverse,
        _this = this
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

