using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class DraGunIntroDoer : SpecificIntroDoer
    {
        private bool m_isFinished;
        private tk2dSpriteAnimator m_introDummy;
        private tk2dSpriteAnimator m_transitionDummy;
        private tk2dSpriteAnimator m_deathDummy;
        private GameObject m_neck;
        private GameObject m_wings;
        private GameObject m_leftArm;
        private GameObject m_rightArm;

        public void Start() => this.aiActor.IgnoreForRoomClear = true;

        protected override void OnDestroy() => base.OnDestroy();

        public override IntVector2 OverrideExitBasePosition(
            DungeonData.Direction directionToWalk,
            IntVector2 exitBaseCenter)
        {
            return exitBaseCenter + new IntVector2(0, DraGunRoomPlaceable.HallHeight);
        }

        public override Vector2? OverrideIntroPosition
        {
            get => new Vector2?(this.specRigidbody.UnitCenter + new Vector2(0.0f, 4f));
        }

        public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
        {
            this.m_introDummy = this.transform.Find("IntroDummy").GetComponent<tk2dSpriteAnimator>();
            this.m_transitionDummy = this.transform.Find("TransitionDummy").GetComponent<tk2dSpriteAnimator>();
            this.m_deathDummy = this.transform.Find("DeathDummy").GetComponent<tk2dSpriteAnimator>();
            this.m_introDummy.aiAnimator = this.aiAnimator;
            this.m_transitionDummy.aiAnimator = this.aiAnimator;
            this.m_deathDummy.aiAnimator = this.aiAnimator;
            this.m_neck = this.transform.Find("Neck").gameObject;
            this.m_wings = this.transform.Find("Wings").gameObject;
            this.m_leftArm = this.transform.Find("LeftArm").gameObject;
            this.m_rightArm = this.transform.Find("RightArm").gameObject;
            this.m_introDummy.gameObject.SetActive(true);
            this.m_transitionDummy.gameObject.SetActive(false);
            this.renderer.enabled = false;
            this.m_neck.SetActive(false);
            this.m_wings.SetActive(false);
            this.m_leftArm.SetActive(false);
            this.m_rightArm.SetActive(false);
            this.aiActor.IgnoreForRoomClear = false;
            this.aiActor.ParentRoom.SealRoom();
            this.StartCoroutine(this.RunEmbers());
        }

        [DebuggerHidden]
        private IEnumerator RunEmbers()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunIntroDoer__RunEmbersc__Iterator0()
            {
                _this = this
            };
        }

        public override void StartIntro(List<tk2dSpriteAnimator> animators)
        {
            animators.Add(this.m_introDummy);
            animators.Add(this.m_transitionDummy);
            this.StartCoroutine(this.DoIntro());
        }

        [DebuggerHidden]
        public IEnumerator DoIntro()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DraGunIntroDoer__DoIntroc__Iterator1()
            {
                _this = this
            };
        }

        public override bool IsIntroFinished => this.m_isFinished;

        public override void EndIntro()
        {
            this.m_introDummy.gameObject.SetActive(false);
            this.m_transitionDummy.gameObject.SetActive(false);
            this.renderer.enabled = true;
            this.m_neck.SetActive(true);
            this.m_wings.SetActive(true);
            this.m_leftArm.SetActive(true);
            this.m_rightArm.SetActive(true);
            this.aiAnimator.EndAnimation();
            DraGunController component = this.GetComponent<DraGunController>();
            component.ModifyCamera(true);
            component.BlockPitTiles(true);
            component.HasDoneIntro = true;
            this.m_isFinished = true;
        }
    }

