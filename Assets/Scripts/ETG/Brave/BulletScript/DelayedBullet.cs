using System.Collections;
using System.Diagnostics;

#nullable disable
namespace Brave.BulletScript
{
    public class DelayedBullet : Bullet
    {
        private int m_delayFrames;

        public DelayedBullet(int delayFrames)
            : base()
        {
            this.m_delayFrames = delayFrames;
        }

        public DelayedBullet(string name, int delayFrames)
            : base(name)
        {
            this.m_delayFrames = delayFrames;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new DelayedBullet__Topc__Iterator0()
            {
                _this = this
            };
        }
    }
}
