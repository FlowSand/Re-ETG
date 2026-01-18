using FullInspector;
using UnityEngine;

using Dungeonator;

#nullable disable

[InspectorDropdownName("Bosses/HighPriest/SimpleMergoBehavior")]
public class HighPriestSimpleMergoBehavior : BasicAttackBehavior
    {
        public BulletScriptSelector wallBulletScript;
        public int numShots = 2;
        private const float c_wallBuffer = 5f;
        private float m_timer;
        private float m_wallShotTimer;

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_timer);
        }

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady())
                return BehaviorResult.Continue;
            for (int index = 0; index < this.numShots; ++index)
                this.ShootWallBulletScript();
            this.UpdateCooldowns();
            return BehaviorResult.Continue;
        }

        private void ShootWallBulletScript()
        {
            float rotation;
            Vector2 a = this.RandomWallPoint(out rotation);
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
                PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
                if (!(bool) (Object) allPlayer || allPlayer.healthHaver.IsDead || (double) Vector2.Distance(a, allPlayer.CenterPosition) < 8.0)
                    return;
            }
            GameObject gameObject = new GameObject("Mergo wall shoot point");
            BulletScriptSource orAddComponent = gameObject.GetOrAddComponent<BulletScriptSource>();
            gameObject.GetOrAddComponent<BulletSourceKiller>();
            orAddComponent.transform.position = (Vector3) a;
            orAddComponent.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);
            orAddComponent.BulletManager = this.m_aiActor.bulletBank;
            orAddComponent.BulletScript = this.wallBulletScript;
            orAddComponent.Initialize();
        }

        private Vector2 RandomWallPoint(out float rotation)
        {
            float num = 4f;
            CellArea area = this.m_aiActor.ParentRoom.area;
            Vector2 vector2_1 = area.basePosition.ToVector2() + new Vector2(0.5f, 1.5f);
            Vector2 vector2_2 = (area.basePosition + area.dimensions).ToVector2() - new Vector2(0.5f, 0.5f);
            if (BraveUtility.RandomBool())
            {
                if (BraveUtility.RandomBool())
                {
                    rotation = -90f;
                    return new Vector2(Random.Range(vector2_1.x + 5f, vector2_2.x - 5f), (float) ((double) vector2_2.y + (double) num + 2.0));
                }
                rotation = 90f;
                return new Vector2(Random.Range(vector2_1.x + 5f, vector2_2.x - 5f), vector2_1.y - num);
            }
            if (BraveUtility.RandomBool())
            {
                rotation = 0.0f;
                return new Vector2(vector2_1.x - num, Random.Range(vector2_1.y + 5f, vector2_2.y - 5f));
            }
            rotation = 180f;
            return new Vector2(vector2_2.x + num, Random.Range(vector2_1.y + 5f, vector2_2.y - 5f));
        }
    }

