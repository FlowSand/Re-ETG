using UnityEngine;

#nullable disable

public class BarkAtMimicsBehavior : MovementBehaviorBase
    {
        public float PathInterval = 0.25f;
        public bool DisableInCombat = true;
        public float IdealRadius = 3f;
        public string BarkAnimation = "bark";
        private float m_repathTimer;

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_repathTimer);
        }

        public override BehaviorResult Update()
        {
            PlayerController primaryPlayer = GameManager.Instance.PrimaryPlayer;
            if ((Object) primaryPlayer == (Object) null || primaryPlayer.CurrentRoom == null)
                return BehaviorResult.Continue;
            if (primaryPlayer.CurrentRoom.IsSealed && this.DisableInCombat)
            {
                this.m_aiAnimator.EndAnimationIf(this.BarkAnimation);
                return BehaviorResult.Continue;
            }
            Chest chest = (Chest) null;
            for (int index = 0; index < StaticReferenceManager.AllChests.Count; ++index)
            {
                if ((bool) (Object) StaticReferenceManager.AllChests[index] && !StaticReferenceManager.AllChests[index].IsOpen && StaticReferenceManager.AllChests[index].IsMimic && StaticReferenceManager.AllChests[index].GetAbsoluteParentRoom() == primaryPlayer.CurrentRoom)
                {
                    chest = StaticReferenceManager.AllChests[index];
                    break;
                }
            }
            if ((Object) chest == (Object) null || (Object) chest.specRigidbody == (Object) null)
            {
                this.m_aiAnimator.EndAnimationIf(this.BarkAnimation);
                return BehaviorResult.Continue;
            }
            this.m_aiAnimator.EndAnimationIf("pet");
            if ((double) Vector2.Distance(chest.specRigidbody.UnitCenter, this.m_aiActor.CenterPosition) <= (double) this.IdealRadius)
            {
                this.m_aiActor.ClearPath();
                if (!this.m_aiAnimator.IsPlaying(this.BarkAnimation))
                    this.m_aiAnimator.PlayUntilCancelled(this.BarkAnimation);
                return BehaviorResult.SkipRemainingClassBehaviors;
            }
            if ((double) this.m_repathTimer <= 0.0)
            {
                this.m_repathTimer = this.PathInterval;
                this.m_aiActor.PathfindToPosition(chest.specRigidbody.UnitCenter);
            }
            return BehaviorResult.SkipRemainingClassBehaviors;
        }
    }

