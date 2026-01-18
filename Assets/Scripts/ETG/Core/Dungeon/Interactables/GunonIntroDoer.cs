using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class GunonIntroDoer : SpecificIntroDoer
    {
        protected override void OnDestroy() => base.OnDestroy();

        public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
        {
            this.aiAnimator.LockFacingDirection = true;
            this.aiAnimator.FacingDirection = -90f;
            RoomHandler parentRoom = this.aiActor.ParentRoom;
            if (parentRoom == null)
                return;
            List<TorchController> componentsInRoom = parentRoom.GetComponentsInRoom<TorchController>();
            for (int index = 0; index < componentsInRoom.Count; ++index)
            {
                TorchController torchController = componentsInRoom[index];
                if ((bool) (Object) torchController && (bool) (Object) torchController.specRigidbody)
                    torchController.specRigidbody.CollideWithOthers = false;
            }
        }

        public override void EndIntro()
        {
            this.aiAnimator.LockFacingDirection = false;
            this.aiAnimator.EndAnimation();
        }
    }

