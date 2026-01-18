#nullable disable

public class MakeVulnerableOnRoomClear : BraveBehaviour
    {
        [CheckDirectionalAnimation(null)]
        public string vulnerableAnim;
        public bool disableBehaviors = true;

        public void Start() => this.aiActor.ParentRoom.OnEnemiesCleared += new System.Action(this.RoomCleared);

        protected override void OnDestroy()
        {
            if ((bool) (UnityEngine.Object) this.aiActor && this.aiActor.ParentRoom != null)
                this.aiActor.ParentRoom.OnEnemiesCleared -= new System.Action(this.RoomCleared);
            base.OnDestroy();
        }

        private void RoomCleared()
        {
            if (!this.healthHaver.PreventAllDamage)
                return;
            this.healthHaver.PreventAllDamage = false;
            if (!string.IsNullOrEmpty(this.vulnerableAnim))
                this.aiAnimator.PlayUntilCancelled(this.vulnerableAnim, true);
            if (this.disableBehaviors)
            {
                this.aiActor.enabled = false;
                this.aiActor.IsHarmlessEnemy = true;
                this.behaviorSpeculator.InterruptAndDisable();
            }
            this.aiActor.CollisionDamage = 0.0f;
            this.aiActor.CollisionKnockbackStrength = 0.0f;
        }
    }

