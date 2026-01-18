using UnityEngine;

using Dungeonator;

#nullable disable

public class BreakableShieldController : BraveBehaviour, SingleSpawnableGunPlacedObject
    {
        [CheckAnimation(null)]
        public string introAnimation;
        [CheckAnimation(null)]
        public string idleAnimation;
        [CheckAnimation(null)]
        public string idleBreak1Animation;
        [CheckAnimation(null)]
        public string idleBreak2Animation;
        [CheckAnimation(null)]
        public string idleBreak3Animation;
        public float maxDuration = 60f;
        private float m_elapsed;
        private PlayerController ownerPlayer;
        private RoomHandler m_room;

        public void Deactivate() => this.majorBreakable.Break(Vector2.zero);

        public void Initialize(Gun sourceGun)
        {
            if ((bool) (Object) sourceGun && (bool) (Object) sourceGun.CurrentOwner)
            {
                this.ownerPlayer = sourceGun.CurrentOwner as PlayerController;
                this.transform.position = sourceGun.CurrentOwner.CenterPosition.ToVector3ZUp();
                this.specRigidbody.Reinitialize();
            }
            this.m_room = this.transform.position.GetAbsoluteRoom();
            this.spriteAnimator.Play(this.introAnimation);
        }

        private void HandleIdleAnimation()
        {
            if (this.spriteAnimator.IsPlaying(this.introAnimation))
                return;
            float healthPercentage = this.majorBreakable.GetCurrentHealthPercentage();
            string name = this.idleAnimation;
            if ((double) healthPercentage < 0.25)
                name = this.idleBreak3Animation;
            else if ((double) healthPercentage < 0.5)
                name = this.idleBreak2Animation;
            else if ((double) healthPercentage < 0.75)
                name = this.idleBreak1Animation;
            if (this.spriteAnimator.IsPlaying(name))
                return;
            this.spriteAnimator.Play(name);
        }

        private void Update()
        {
            if (this.majorBreakable.IsDestroyed)
                return;
            this.m_elapsed += BraveTime.DeltaTime;
            this.HandleIdleAnimation();
            if ((double) this.m_elapsed > (double) this.maxDuration)
                this.majorBreakable.Break(Vector2.zero);
            if (!(bool) (Object) this.ownerPlayer || this.ownerPlayer.CurrentRoom == this.m_room)
                return;
            this.majorBreakable.Break(Vector2.zero);
        }
    }

