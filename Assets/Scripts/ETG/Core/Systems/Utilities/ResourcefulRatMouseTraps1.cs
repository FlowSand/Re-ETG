using System.Collections;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/ResourcefulRat/MouseTraps1")]
public class ResourcefulRatMouseTraps1 : Script
    {
        private const int FlightTime = 60;
        private const int NumTraps = 10;
        private static int[] s_xValues;
        private static int[] s_yValues;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ResourcefulRatMouseTraps1__Topc__Iterator0()
            {
                _this = this
            };
        }

        private void Fire(Vector2 goal)
        {
            float angle = (goal - this.Position).ToAngle();
            GameObject[] mouseTraps = this.BulletBank.GetComponent<ResourcefulRatController>().MouseTraps;
            this.Fire(new Brave.BulletScript.Direction(angle), (Bullet) new ResourcefulRatMouseTraps1.MouseTrapBullet(goal, BraveUtility.RandomElement<GameObject>(mouseTraps)));
        }

        private class MouseTrapBullet : Bullet
        {
            private Vector2 m_goalPos;
            private GameObject m_mouseTrapPrefab;

            public MouseTrapBullet(Vector2 goalPos, GameObject mouseTrapPrefab)
                : base("mousetrap", true)
            {
                this.m_goalPos = goalPos;
                this.m_mouseTrapPrefab = mouseTrapPrefab;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new ResourcefulRatMouseTraps1.MouseTrapBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

