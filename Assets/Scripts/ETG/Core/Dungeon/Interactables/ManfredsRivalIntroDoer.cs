using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class ManfredsRivalIntroDoer : SpecificIntroDoer
    {
        private List<AIActor> m_knights = new List<AIActor>();

        protected override void OnDestroy() => base.OnDestroy();

        public override void StartIntro(List<tk2dSpriteAnimator> animators)
        {
            List<AIActor> activeEnemies = this.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            for (int index = 0; index < activeEnemies.Count; ++index)
            {
                if (!((Object) activeEnemies[index] == (Object) this.aiActor))
                {
                    animators.Add(activeEnemies[index].spriteAnimator);
                    activeEnemies[index].aiAnimator.LockFacingDirection = true;
                    activeEnemies[index].aiAnimator.FacingDirection = -90f;
                    this.m_knights.Add(activeEnemies[index]);
                }
            }
        }

        public override void OnCleanup()
        {
            for (int index = 0; index < this.m_knights.Count; ++index)
                this.m_knights[index].aiAnimator.LockFacingDirection = true;
        }
    }

