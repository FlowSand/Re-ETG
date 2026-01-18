using System.Collections;
using System.Diagnostics;

using UnityEngine;

#nullable disable

public class FoyerCostumeController : BraveBehaviour, IPlayerInteractable
    {
        [LongEnum]
        public GungeonFlags RequiredFlag;
        public tk2dSpriteAnimation TargetLibrary;
        private bool m_active;

        [DebuggerHidden]
        private IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new FoyerCostumeController__Startc__Iterator0()
            {
                _this = this
            };
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            return !this.m_active ? 1000f : Vector2.Distance(point, this.sprite.WorldCenter);
        }

        public float GetOverrideMaxDistance() => -1f;

        public void Interact(PlayerController interactor)
        {
            if (!this.m_active)
                return;
            if (interactor.IsUsingAlternateCostume)
            {
                if ((Object) interactor.AlternateCostumeLibrary == (Object) this.TargetLibrary)
                {
                    interactor.SwapToAlternateCostume();
                }
                else
                {
                    interactor.SwapToAlternateCostume();
                    interactor.AlternateCostumeLibrary = this.TargetLibrary;
                    interactor.SwapToAlternateCostume();
                }
            }
            else
            {
                if ((bool) (Object) this.TargetLibrary)
                    interactor.AlternateCostumeLibrary = this.TargetLibrary;
                interactor.SwapToAlternateCostume();
            }
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!this.m_active)
                return;
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this.m_active)
                return;
            SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        }
    }

