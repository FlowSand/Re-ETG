// Decompiled with JetBrains decompiler
// Type: Brave.BulletScript.BulletLite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace Brave.BulletScript
{
  public class BulletLite(string bankName = null, bool suppressVfx = false, bool firstBulletOfAttack = false) : 
    Bullet(bankName, suppressVfx, firstBulletOfAttack)
  {
    public override void Initialize() => this.m_tasks.Add((Bullet.ITask) new Bullet.TaskLite(this));

    public virtual void Start()
    {
    }

    public virtual int Update(ref int state) => this.Done();

    protected int Done() => -1;
  }
}
