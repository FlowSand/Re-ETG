using UnityEngine;

#nullable disable

public class BulletArcLightningController : MonoBehaviour
    {
        private Vector2 m_center;
        private float m_velocity;
        private float m_startAngle;
        private float m_endAngle;
        private float m_currentRadius;
        private string m_ownerName;
        private LineRenderer m_line;
        private Vector3[] m_linePoints;

        public void Initialize(
            Vector2 centerPoint,
            float velocity,
            string OwnerName,
            float startAngle = 0.0f,
            float endAngle = 360f,
            float startRadius = 0.0f)
        {
            this.m_ownerName = OwnerName;
            this.m_center = centerPoint;
            this.m_velocity = velocity;
            this.m_startAngle = startAngle;
            this.m_endAngle = endAngle;
            this.m_currentRadius = startRadius;
            this.m_line = this.gameObject.GetOrAddComponent<LineRenderer>();
            this.m_linePoints = new Vector3[16 /*0x10*/];
            this.m_line.SetVertexCount(this.m_linePoints.Length);
            this.m_line.SetPositions(this.m_linePoints);
            this.m_line.SetWidth(1f, 1f);
            this.m_line.material = BraveResources.Load("Global VFX/ArcLightningMaterial", ".mat") as Material;
            while ((double) this.m_startAngle > (double) this.m_endAngle)
                this.m_endAngle += 360f;
        }

        public void UpdateCenter(Vector2 newCenter) => this.m_center = newCenter;

        public void Update()
        {
            this.m_currentRadius += this.m_velocity * BraveTime.DeltaTime;
            this.UpdateRendering();
            this.UpdateCollision();
        }

        public void OnDespawned()
        {
            if ((bool) (Object) this.m_line)
            {
                for (int index = 0; index < this.m_linePoints.Length; ++index)
                    this.m_linePoints[index] = Vector3.zero;
                this.m_line.SetPositions(this.m_linePoints);
                Object.Destroy((Object) this.m_line);
            }
            this.m_line = (LineRenderer) null;
            this.m_linePoints = (Vector3[]) null;
            Object.Destroy((Object) this);
        }

        private void UpdateRendering()
        {
            float num = (this.m_endAngle - this.m_startAngle) / (float) this.m_linePoints.Length;
            for (int index = 0; index < this.m_linePoints.Length; ++index)
                this.m_linePoints[index] = (this.m_center + BraveMathCollege.DegreesToVector(this.m_startAngle + (float) index * num, this.m_currentRadius)).ToVector3ZisY();
            this.m_line.SetPositions(this.m_linePoints);
        }

        private bool IsBetweenAngles(
            Vector2 circleCenter,
            Vector2 point,
            float startAngle,
            float endAngle)
        {
            float num = (float) (((double) (point - circleCenter).ToAngle() + 360.0) % 360.0);
            return (double) endAngle >= (double) num && (double) startAngle <= (double) num;
        }

        public bool ArcIntersectsLine(
            Vector2 circleCenter,
            float radius,
            float startAngle,
            float endAngle,
            Vector2 point1,
            Vector2 point2)
        {
            Vector2 vector2_1 = point1 - circleCenter;
            Vector2 b = point2 - circleCenter;
            Vector2 vector2_2 = b - vector2_1;
            float num1 = Vector2.Dot(vector2_2, vector2_2);
            float f1 = 2f * Vector2.Dot(vector2_1, vector2_2);
            float num2 = Vector2.Dot(vector2_1, vector2_1) - radius * radius;
            float f2 = Mathf.Pow(f1, 2f) - 4f * num1 * num2;
            if ((double) f2 <= 0.0)
                return false;
            float num3 = (double) f1 < 0.0 ? (float) ((-(double) f1 + (double) Mathf.Sqrt(f2)) / 2.0) : (float) ((-(double) f1 - (double) Mathf.Sqrt(f2)) / 2.0);
            float t1 = num3 / num1;
            float t2 = num2 / num3;
            if ((double) t2 < (double) t1)
            {
                float num4 = t2;
                t2 = t1;
                t1 = num4;
            }
            if (0.0 <= (double) t1 && (double) t1 <= 1.0)
            {
                Vector2 point = circleCenter + Vector2.Lerp(vector2_1, b, t1);
                if (this.IsBetweenAngles(circleCenter, point, startAngle, endAngle))
                    return true;
            }
            if (0.0 <= (double) t2 && (double) t2 <= 1.0)
            {
                Vector2 point = circleCenter + Vector2.Lerp(vector2_1, b, t2);
                if (this.IsBetweenAngles(circleCenter, point, startAngle, endAngle))
                    return true;
            }
            return false;
        }

        private bool ArcSliceIntersectsAABB(
            Vector2 centerPoint,
            float startAngle,
            float endAngle,
            float startRadius,
            float endRadius,
            Vector2 aabbBottomLeft,
            Vector2 aabbTopRight)
        {
            Vector2 point1 = aabbBottomLeft;
            Vector2 point2 = aabbTopRight;
            Vector2 vector2_1 = new Vector2(point2.x, point1.y);
            Vector2 vector2_2 = new Vector2(point1.x, point2.y);
            bool flag = this.ArcIntersectsLine(centerPoint, endRadius, startAngle, endAngle, point1, vector2_1);
            if (!flag)
                flag = this.ArcIntersectsLine(centerPoint, endRadius, startAngle, endAngle, vector2_1, point2);
            if (!flag)
                flag = this.ArcIntersectsLine(centerPoint, endRadius, startAngle, endAngle, vector2_2, point2);
            if (!flag)
                flag = this.ArcIntersectsLine(centerPoint, endRadius, startAngle, endAngle, point1, vector2_2);
            return flag;
        }

        private void UpdateCollision()
        {
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
                PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
                if ((bool) (Object) allPlayer && !allPlayer.IsGhost && (bool) (Object) allPlayer.healthHaver && allPlayer.healthHaver.IsAlive && allPlayer.healthHaver.IsVulnerable)
                {
                    Vector2 zero = Vector2.zero;
                    if (this.ArcSliceIntersectsAABB(this.m_center, this.m_startAngle, this.m_endAngle, this.m_currentRadius, this.m_currentRadius, allPlayer.specRigidbody.HitboxPixelCollider.UnitBottomLeft, allPlayer.specRigidbody.HitboxPixelCollider.UnitTopRight))
                        allPlayer.healthHaver.ApplyDamage(0.5f, Vector2.zero, this.m_ownerName);
                }
            }
        }
    }

