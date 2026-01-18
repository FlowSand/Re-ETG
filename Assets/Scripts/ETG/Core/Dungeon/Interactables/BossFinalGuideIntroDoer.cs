using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class BossFinalGuideIntroDoer : SpecificIntroDoer
    {
        private AIAnimator m_topAnimator;
        private AIAnimator m_drAnimator;
        private bool m_finished;

        public void Start() => this.m_topAnimator = this.aiAnimator.ChildAnimator;

        protected override void OnDestroy() => base.OnDestroy();

        public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
        {
            this.aiAnimator.FacingDirection = -90f;
            this.m_topAnimator.FacingDirection = -90f;
            List<HealthHaver> allHealthHavers = StaticReferenceManager.AllHealthHavers;
            for (int index = 0; index < allHealthHavers.Count; ++index)
            {
                if (!allHealthHavers[index].IsBoss && allHealthHavers[index].name.Contains("DrWolf", true))
                {
                    allHealthHavers[index].GetComponent<ObjectVisibilityManager>().ChangeToVisibility(RoomHandler.VisibilityStatus.CURRENT);
                    allHealthHavers[index].specRigidbody.CollideWithOthers = true;
                    allHealthHavers[index].aiActor.IsGone = false;
                    allHealthHavers[index].aiActor.State = AIActor.ActorState.Normal;
                    this.m_drAnimator = allHealthHavers[index].aiAnimator;
                    animators.Add(this.m_drAnimator.spriteAnimator);
                    break;
                }
            }
            this.aiAnimator.renderer.enabled = false;
            SpriteOutlineManager.ToggleOutlineRenderers(this.aiAnimator.sprite, false);
            this.aiAnimator.ChildAnimator.renderer.enabled = false;
            SpriteOutlineManager.ToggleOutlineRenderers(this.aiAnimator.ChildAnimator.sprite, false);
            Object.FindObjectOfType<DungeonDoorSubsidiaryBlocker>().Seal();
        }

        public override void StartIntro(List<tk2dSpriteAnimator> animators)
        {
            this.m_topAnimator.PlayUntilFinished("intro");
            this.m_drAnimator.PlayUntilFinished("intro");
            this.StartCoroutine(this.DoIntro());
        }

        public override bool IsIntroFinished => this.m_finished;

        public override void EndIntro()
        {
            this.aiAnimator.renderer.enabled = true;
            SpriteOutlineManager.ToggleOutlineRenderers(this.aiAnimator.sprite, true);
            this.aiAnimator.ChildAnimator.renderer.enabled = true;
            SpriteOutlineManager.ToggleOutlineRenderers(this.aiAnimator.ChildAnimator.sprite, true);
            this.aiActor.ToggleShadowVisiblity(true);
            this.m_finished = true;
            this.StopAllCoroutines();
            this.m_topAnimator.EndAnimationIf("intro");
            this.m_drAnimator.EndAnimationIf("intro");
        }

        [DebuggerHidden]
        private IEnumerator DoIntro()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalGuideIntroDoer__DoIntroc__Iterator0()
            {
                _this = this
            };
        }
    }

