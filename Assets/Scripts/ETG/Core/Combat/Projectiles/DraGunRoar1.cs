using System.Collections;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

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
            return (IEnumerator) new DraGunRoar1__Topc__Iterator0()
            {
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

