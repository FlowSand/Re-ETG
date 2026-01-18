using UnityEngine;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class BossFinalRobotIntroDoer : SpecificIntroDoer
    {
        protected override void OnDestroy() => base.OnDestroy();

        public override void EndIntro() => this.aiAnimator.StopVfx("torch_intro");
    }

