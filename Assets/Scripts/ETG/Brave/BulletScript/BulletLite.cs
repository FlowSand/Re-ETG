#nullable disable
namespace Brave.BulletScript
{
  public class BulletLite : Bullet
  {
    public BulletLite(string bankName = null, bool suppressVfx = false, bool firstBulletOfAttack = false) : base(bankName, suppressVfx, firstBulletOfAttack)
    {
    }

    public override void Initialize() => this.m_tasks.Add((Bullet.ITask) new Bullet.TaskLite(this));

    public virtual void Start()
    {
    }

    public virtual int Update(ref int state) => this.Done();

    protected int Done() => -1;
  }
}
