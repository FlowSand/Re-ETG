// Decompiled with JetBrains decompiler
// Type: BulletScriptSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using UnityEngine;

#nullable disable
public class BulletScriptSource : BraveBehaviour
{
  public BulletScriptSelector BulletScript;
  private int m_lastUpdatedFrame = -1;

  public Bullet RootBullet { get; set; }

  public AIBulletBank BulletManager { get; set; }

  public bool FreezeTopPosition { get; set; }

  public void Awake() => StaticReferenceManager.AllBulletScriptSources.Add(this);

  public void Start()
  {
    if (!(bool) (Object) this.BulletManager)
      this.BulletManager = this.bulletBank;
    if (this.RootBullet != null)
      return;
    this.Initialize();
  }

  public void Update()
  {
    if (this.RootBullet == null || this.RootBullet.IsEnded || this.m_lastUpdatedFrame == UnityEngine.Time.frameCount)
      return;
    if (!this.FreezeTopPosition)
    {
      this.RootBullet.Position = this.transform.position.XY();
      this.RootBullet.Direction = this.transform.rotation.eulerAngles.z;
    }
    this.RootBullet.TimeScale = this.BulletManager.TimeScale;
    this.RootBullet.FrameUpdate();
    this.m_lastUpdatedFrame = UnityEngine.Time.frameCount;
  }

  protected override void OnDestroy()
  {
    StaticReferenceManager.AllBulletScriptSources.Remove(this);
    base.OnDestroy();
  }

  public void ForceStop()
  {
    if (this.RootBullet == null)
      return;
    this.RootBullet.ForceEnd();
    this.RootBullet.Destroyed = true;
    this.RootBullet = (Bullet) null;
  }

  public void Initialize()
  {
    this.RootBullet = this.BulletScript.CreateInstance();
    if (this.RootBullet != null)
    {
      this.RootBullet.BulletManager = (IBulletManager) this.BulletManager;
      this.RootBullet.RootTransform = this.transform;
      this.RootBullet.Position = this.transform.position.XY();
      this.RootBullet.Direction = this.transform.rotation.eulerAngles.z;
      this.RootBullet.Initialize();
    }
    this.Update();
  }

  public bool IsEnded => this.RootBullet == null || this.RootBullet.IsEnded;
}
