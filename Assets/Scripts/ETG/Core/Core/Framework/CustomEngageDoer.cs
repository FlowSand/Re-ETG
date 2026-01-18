using System.Collections;
using System.Diagnostics;

#nullable disable

    public abstract class CustomEngageDoer : BraveBehaviour
    {
        public virtual void StartIntro()
        {
        }

        public virtual bool IsFinished => true;

        public virtual void OnCleanup()
        {
        }

[DebuggerHidden]
        public IEnumerator TimeInvariantWait(float duration)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new CustomEngageDoer__TimeInvariantWaitc__Iterator0()
            {
                duration = duration
            };
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

