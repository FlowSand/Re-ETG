using System.Collections;
using System.Diagnostics;

#nullable disable
namespace Brave.BulletScript
{
  public class TimedBullet : Bullet
  {
    private int m_destroyTimer;

    public TimedBullet(int destroyTimer)
      : base()
    {
      this.m_destroyTimer = destroyTimer;
    }

    public TimedBullet(string name, int destroyTimer)
      : base(name)
    {
      this.m_destroyTimer = destroyTimer;
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TimedBullet__Topc__Iterator0()
      {
        _this = this
      };
    }
  }
}
