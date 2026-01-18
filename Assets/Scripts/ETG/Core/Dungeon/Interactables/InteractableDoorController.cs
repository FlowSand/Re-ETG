using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

public class InteractableDoorController : DungeonPlaceableBehaviour, IPlayerInteractable
    {
        public List<InteractableLock> WorldLocks;
        public bool OpensAutomaticallyOnUnlocked;
        private bool m_hasOpened;

        public override GameObject InstantiateObject(
            RoomHandler targetRoom,
            IntVector2 loc,
            bool deferConfiguration = false)
        {
            return base.InstantiateObject(targetRoom, loc, deferConfiguration);
        }

        private void Start()
        {
            if (this.WorldLocks.Count > 0 && this.WorldLocks[0].lockMode == InteractableLock.InteractableLockMode.NPC_JAIL)
                GameStatsManager.Instance.NumberRunsValidCellWithoutSpawn = 0;
            RoomHandler absoluteParentRoom = this.GetAbsoluteParentRoom();
            for (int index = 0; index < this.WorldLocks.Count; ++index)
                absoluteParentRoom.RegisterInteractable((IPlayerInteractable) this.WorldLocks[index]);
            if (!(bool) (Object) this.specRigidbody)
                return;
            this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
        }

        private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
        {
            if (rigidbodyCollision == null || !(bool) (Object) rigidbodyCollision.OtherRigidbody || !(bool) (Object) rigidbodyCollision.OtherRigidbody.GetComponent<KeyProjModifier>())
                return;
            for (int index = 0; index < this.WorldLocks.Count; ++index)
            {
                if ((bool) (Object) this.WorldLocks[index] && this.WorldLocks[index].IsLocked && this.WorldLocks[index].lockMode == InteractableLock.InteractableLockMode.NORMAL)
                    this.WorldLocks[index].ForceUnlock();
            }
        }

        private void Update()
        {
            if (this.m_hasOpened || !this.OpensAutomaticallyOnUnlocked || !this.IsValidForUse())
                return;
            this.Open();
        }

        private bool IsValidForUse()
        {
            if (this.m_hasOpened)
                return false;
            bool flag = true;
            for (int index = 0; index < this.WorldLocks.Count; ++index)
            {
                if (this.WorldLocks[index].IsLocked || this.WorldLocks[index].spriteAnimator.IsPlaying(this.WorldLocks[index].spriteAnimator.CurrentClip))
                    flag = false;
            }
            return flag;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!(bool) (Object) this)
                return;
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 0.1f);
            this.sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!(bool) (Object) this)
                return;
            SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
            this.sprite.UpdateZDepth();
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (this.OpensAutomaticallyOnUnlocked || !this.IsValidForUse())
                return 1000f;
            Bounds bounds = this.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
            float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
        }

        public float GetOverrideMaxDistance() => -1f;

        private void Open()
        {
            this.m_hasOpened = true;
            this.spriteAnimator.Play();
            this.specRigidbody.enabled = false;
        }

        public void Interact(PlayerController player)
        {
            if (!this.IsValidForUse())
                return;
            this.Open();
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

