using System.Collections;
using System.Diagnostics;

#nullable disable
namespace Brave.BulletScript
{
    public class SpeedChangingBullet : Bullet
    {
        private float m_newSpeed;
        private int m_term;
        private int m_destroyTimer;

        public SpeedChangingBullet(float newSpeed, int term, int destroyTimer = -1)
            : base()
        {
            this.m_newSpeed = newSpeed;
            this.m_term = term;
            this.m_destroyTimer = destroyTimer;
        }

        public SpeedChangingBullet(
            string name,
            float newSpeed,
            int term,
            int destroyTimer = -1,
            bool suppressVfx = false)
            : base(name, suppressVfx)
        {
            this.m_newSpeed = newSpeed;
            this.m_term = term;
            this.m_destroyTimer = destroyTimer;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new SpeedChangingBullet__Topc__Iterator0()
            {
                _this = this
            };
        }
    }
}
