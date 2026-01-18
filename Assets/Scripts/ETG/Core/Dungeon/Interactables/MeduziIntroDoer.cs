using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class MeduziIntroDoer : SpecificIntroDoer
    {
        private bool m_isFinished;

        protected override void OnDestroy() => base.OnDestroy();

        public override void StartIntro(List<tk2dSpriteAnimator> animators)
        {
            this.StartCoroutine(this.DoIntro());
        }

        [DebuggerHidden]
        public IEnumerator DoIntro()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MeduziIntroDoer__DoIntroc__Iterator0()
            {
                _this = this
            };
        }

        public override bool IsIntroFinished => true;

        public override void EndIntro() => this.m_isFinished = true;
    }

