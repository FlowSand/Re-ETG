using UnityEngine;

using Dungeonator;

#nullable disable

public class TachyonProjectile : Projectile
    {
        public float ProjectileRadius = 5f / 16f;
        public VFXPool SpawnVFX;
        private RoomHandler m_room;

        public override void Start()
        {
            base.Start();
            Vector2 unitPosition = this.specRigidbody.Position.UnitPosition;
            Vector2 expectedEndPoint = this.FindExpectedEndPoint();
            this.baseData.range = Vector2.Distance(expectedEndPoint, this.transform.position.XY());
            this.transform.position = expectedEndPoint.ToVector3ZisY();
            this.specRigidbody.Reinitialize();
            this.Direction = (expectedEndPoint - unitPosition).normalized;
            this.SendInDirection(this.Direction * -1f, true);
            this.m_distanceElapsed = 0.0f;
            this.LastPosition = this.transform.position;
            this.SpawnVFX.SpawnAtPosition(expectedEndPoint.ToVector3ZisY());
        }

        public override void Update()
        {
            base.Update();
            if (this.specRigidbody.UnitCenter.GetAbsoluteRoom() == this.m_room)
                return;
            this.DieInAir();
        }

        protected Vector2 FindExpectedEndPoint()
        {
            Dungeon dungeon = GameManager.Instance.Dungeon;
            Vector2 unitCenter = this.specRigidbody.UnitCenter;
            Vector2 b = unitCenter + this.Direction.normalized * this.baseData.range;
            this.m_room = unitCenter.GetAbsoluteRoom();
            bool flag1 = false;
            Vector2 vector2 = unitCenter;
            IntVector2 intVector2_1 = vector2.ToIntVector2(VectorConversions.Floor);
            if (dungeon.data.CheckInBoundsAndValid(intVector2_1))
                flag1 = dungeon.data[intVector2_1].isExitCell;
            float f1 = b.x - unitCenter.x;
            float f2 = b.y - unitCenter.y;
            float num1 = Mathf.Sign(b.x - unitCenter.x);
            float num2 = Mathf.Sign(b.y - unitCenter.y);
            bool flag2 = (double) num1 > 0.0;
            bool flag3 = (double) num2 > 0.0;
            int num3 = 0;
            while ((double) Vector2.Distance(vector2, b) > 0.10000000149011612 && num3 < 10000)
            {
                ++num3;
                float num4 = Mathf.Abs(((!flag2 ? Mathf.Floor(vector2.x) : Mathf.Ceil(vector2.x)) - vector2.x) / f1);
                float num5 = Mathf.Abs(((!flag3 ? Mathf.Floor(vector2.y) : Mathf.Ceil(vector2.y)) - vector2.y) / f2);
                int x = Mathf.FloorToInt(vector2.x);
                int y = Mathf.FloorToInt(vector2.y);
                IntVector2 intVector2_2 = new IntVector2(x, y);
                bool flag4 = false;
                if (dungeon.data.CheckInBoundsAndValid(intVector2_2))
                {
                    CellData cellData = dungeon.data[intVector2_2];
                    if (cellData.nearestRoom == this.m_room && cellData.isExitCell == flag1)
                    {
                        if (cellData.type != CellType.WALL)
                            flag4 = true;
                        if (flag4)
                            intVector2_1 = intVector2_2;
                        if ((double) num4 < (double) num5)
                        {
                            int num6 = x + 1;
                            vector2.x += (float) ((double) num4 * (double) f1 + 0.10000000149011612 * (double) Mathf.Sign(f1));
                            vector2.y += (float) ((double) num4 * (double) f2 + 0.10000000149011612 * (double) Mathf.Sign(f2));
                        }
                        else
                        {
                            int num7 = y + 1;
                            vector2.x += (float) ((double) num5 * (double) f1 + 0.10000000149011612 * (double) Mathf.Sign(f1));
                            vector2.y += (float) ((double) num5 * (double) f2 + 0.10000000149011612 * (double) Mathf.Sign(f2));
                        }
                    }
                    else
                        break;
                }
                else
                    break;
            }
            return intVector2_1.ToCenterVector2();
        }

        protected override void Move() => base.Move();

        protected override void OnDestroy() => base.OnDestroy();
    }

