using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;
using Dungeonator;

#nullable disable

[InspectorDropdownName("Bosses/Infinilich/CarnageWalls1")]
public class InfinilichCarnageWalls1 : Script
    {
        public bool Done;
        private const float c_wallBuffer = 5f;
        private static List<Vector2> s_wallPoints = new List<Vector2>(4);

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new InfinilichCarnageWalls1__Topc__Iterator0()
            {
                _this = this
            };
        }

        private void RandomWallPoints(out Vector2 startPoint, out Vector2 endPoint)
        {
            CellArea area = this.BulletBank.aiActor.ParentRoom.area;
            Vector2 vector2_1 = area.basePosition.ToVector2() + new Vector2(0.75f, 2f);
            Vector2 vector2_2 = (area.basePosition + area.dimensions).ToVector2() - new Vector2(0.75f, 0.5f);
            InfinilichCarnageWalls1.s_wallPoints.Clear();
            InfinilichCarnageWalls1.s_wallPoints.Add(new Vector2(Random.Range(vector2_1.x + 5f, vector2_2.x - 5f), vector2_2.y));
            InfinilichCarnageWalls1.s_wallPoints.Add(new Vector2(Random.Range(vector2_1.x + 5f, vector2_2.x - 5f), vector2_1.y));
            InfinilichCarnageWalls1.s_wallPoints.Add(new Vector2(vector2_1.x, Random.Range(vector2_1.y + 5f, vector2_2.y - 5f)));
            InfinilichCarnageWalls1.s_wallPoints.Add(new Vector2(vector2_2.x, Random.Range(vector2_1.y + 5f, vector2_2.y - 5f)));
            Vector2 vector2_3 = BraveUtility.RandomElement<Vector2>(InfinilichCarnageWalls1.s_wallPoints);
            InfinilichCarnageWalls1.s_wallPoints.Remove(vector2_3);
            startPoint = vector2_3;
            float num1 = 0.0f;
            int index1 = 0;
            for (int index2 = 0; index2 < InfinilichCarnageWalls1.s_wallPoints.Count; ++index2)
            {
                float num2 = Vector2Extensions.SqrDistance(InfinilichCarnageWalls1.s_wallPoints[index2], startPoint);
                if (index2 == 0 || (double) num2 < (double) num1)
                {
                    num1 = num2;
                    index1 = index2;
                }
            }
            InfinilichCarnageWalls1.s_wallPoints.RemoveAt(index1);
            endPoint = BraveUtility.RandomElement<Vector2>(InfinilichCarnageWalls1.s_wallPoints);
        }

        public class TipBullet : Bullet
        {
            private InfinilichCarnageWalls1 m_parentScript;

            public TipBullet(InfinilichCarnageWalls1 parentScript)
                : base("carnageTip")
            {
                this.m_parentScript = parentScript;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new InfinilichCarnageWalls1.TipBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }

        public class ChainBullet : Bullet
        {
            private const float WiggleMagnitude = 0.75f;
            private const float WigglePeriodMultiplier = 0.333f;
            private InfinilichCarnageWalls1 m_parentScript;
            private InfinilichCarnageWalls1.TipBullet m_tip;
            private int m_spawnDelay;
            private float m_tipSpeed;

            public ChainBullet(
                InfinilichCarnageWalls1 parentScript,
                InfinilichCarnageWalls1.TipBullet tip,
                int spawnDelay,
                float tipSpeed)
                : base()
            {
                this.m_parentScript = parentScript;
                this.m_tip = tip;
                this.m_spawnDelay = spawnDelay;
                this.m_tipSpeed = tipSpeed;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new InfinilichCarnageWalls1.ChainBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

