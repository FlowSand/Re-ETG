using System.Collections;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

public class ActiveSummonItem : PlayerItem
    {
        [EnemyIdentifier]
        public string CompanionGuid;
        public bool HasDoubleSynergy;
        [LongNumericEnum]
        public CustomSynergyType DoubleSynergy;
        public IntVector2 CustomClearance;
        public Vector2 CustomOffset;
        public bool IsTimed;
        public float Lifespan = 60f;
        public string IntroDirectionalAnimation;
        public string OutroDirectionalAnimation;
        public GameObject DepartureVFXPrefab;
        private GameObject m_extantCompanion;
        private GameObject m_extantSecondCompanion;
        private bool m_synergyActive;

        public override bool CanBeUsed(PlayerController user)
        {
            return user.CurrentRoom != null && user.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear) && base.CanBeUsed(user);
        }

        private void CreateCompanion(PlayerController owner)
        {
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(this.CompanionGuid);
            IntVector2 intVector2 = IntVector2.Max(this.CustomClearance, orLoadByGuid.Clearance);
            IntVector2? nearestAvailableCell1 = owner.CurrentRoom.GetNearestAvailableCell(owner.transform.position.XY(), new IntVector2?(intVector2), new CellTypes?(CellTypes.FLOOR));
            if (!nearestAvailableCell1.HasValue)
                return;
            GameObject targetCompanion = Object.Instantiate<GameObject>(orLoadByGuid.gameObject, (nearestAvailableCell1.Value.ToVector2() + this.CustomOffset).ToVector3ZUp(), Quaternion.identity);
            this.m_extantCompanion = targetCompanion;
            CompanionController orAddComponent1 = this.m_extantCompanion.GetOrAddComponent<CompanionController>();
            orAddComponent1.companionID = CompanionController.CompanionIdentifier.GATLING_GULL;
            orAddComponent1.Initialize(owner);
            if (this.IsTimed)
                owner.StartCoroutine(this.HandleLifespan(targetCompanion, owner));
            if (!string.IsNullOrEmpty(this.IntroDirectionalAnimation))
                orAddComponent1.GetComponent<AIAnimator>().PlayUntilFinished(this.IntroDirectionalAnimation, true);
            if (!this.HasDoubleSynergy || !owner.HasActiveBonusSynergy(this.DoubleSynergy))
                return;
            IntVector2? nearestAvailableCell2 = owner.CurrentRoom.GetNearestAvailableCell(owner.transform.position.XY() + new Vector2(-1f, -1f), new IntVector2?(intVector2), new CellTypes?(CellTypes.FLOOR));
            if (!nearestAvailableCell2.HasValue)
                return;
            this.m_extantSecondCompanion = Object.Instantiate<GameObject>(orLoadByGuid.gameObject, (nearestAvailableCell2.Value.ToVector2() + this.CustomOffset).ToVector3ZUp(), Quaternion.identity);
            CompanionController orAddComponent2 = this.m_extantSecondCompanion.GetOrAddComponent<CompanionController>();
            orAddComponent2.Initialize(owner);
            if (string.IsNullOrEmpty(this.IntroDirectionalAnimation))
                return;
            orAddComponent2.GetComponent<AIAnimator>().PlayUntilFinished(this.IntroDirectionalAnimation, true);
        }

        private void DestroyCompanion()
        {
            if ((bool) (Object) this.m_extantCompanion)
            {
                if (!string.IsNullOrEmpty(this.OutroDirectionalAnimation))
                {
                    GameManager.Instance.Dungeon.StartCoroutine(this.HandleDeparture(true, this.m_extantCompanion.GetComponent<AIAnimator>()));
                }
                else
                {
                    Object.Destroy((Object) this.m_extantCompanion);
                    this.m_extantCompanion = (GameObject) null;
                }
            }
            if (!(bool) (Object) this.m_extantSecondCompanion)
                return;
            if (!string.IsNullOrEmpty(this.OutroDirectionalAnimation))
            {
                GameManager.Instance.Dungeon.StartCoroutine(this.HandleDeparture(false, this.m_extantSecondCompanion.GetComponent<AIAnimator>()));
            }
            else
            {
                Object.Destroy((Object) this.m_extantSecondCompanion);
                this.m_extantSecondCompanion = (GameObject) null;
            }
        }

        [DebuggerHidden]
        private IEnumerator HandleDeparture(bool isPrimary, AIAnimator anim)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ActiveSummonItem__HandleDeparturec__Iterator0()
            {
                anim = anim,
                isPrimary = isPrimary,
                _this = this
            };
        }

        protected override void DoEffect(PlayerController user)
        {
            this.DestroyCompanion();
            this.CreateCompanion(user);
        }

        protected override void OnPreDrop(PlayerController user)
        {
            base.OnPreDrop(user);
            if (!this.IsCurrentlyActive)
                return;
            this.IsCurrentlyActive = false;
            if (!(bool) (Object) this.m_extantCompanion)
                return;
            this.DestroyCompanion();
        }

        [DebuggerHidden]
        private IEnumerator HandleLifespan(GameObject targetCompanion, PlayerController owner)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new ActiveSummonItem__HandleLifespanc__Iterator1()
            {
                owner = owner,
                targetCompanion = targetCompanion,
                _this = this
            };
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

