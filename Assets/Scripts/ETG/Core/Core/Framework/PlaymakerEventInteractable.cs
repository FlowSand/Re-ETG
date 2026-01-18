using UnityEngine;

#nullable disable

public class PlaymakerEventInteractable : BraveBehaviour, IPlayerInteractable
    {
        public string EventToTrigger;

        public float GetDistanceToPoint(Vector2 point)
        {
            Bounds bounds = this.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
            float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!(bool) (Object) this.sprite)
                return;
            SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
        }

        public void Interact(PlayerController interactor)
        {
            GameManager.BroadcastRoomFsmEvent(this.EventToTrigger, interactor.CurrentRoom);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public float GetOverrideMaxDistance() => -1f;
    }

