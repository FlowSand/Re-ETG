// Decompiled with JetBrains decompiler
// Type: DraGunRoarBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/RoarBehavior")]
public class DraGunRoarBehavior : BasicAttackBehavior
  {
    public GameObject ShootPoint;
    public BulletScriptSelector BulletScript;
    private DraGunController m_dragun;
    private tk2dSpriteAnimator m_roarDummy;
    private BulletScriptSource m_bulletSource;
    private float m_timer;

    public override void Start()
    {
      base.Start();
      this.m_dragun = this.m_aiActor.GetComponent<DraGunController>();
      this.m_roarDummy = this.m_aiActor.transform.Find("RoarDummy").GetComponent<tk2dSpriteAnimator>();
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_timer);
    }

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      this.m_aiActor.ToggleRenderers(false);
      this.m_dragun.head.OverrideDesiredPosition = new Vector2?((Vector2) (this.m_aiActor.transform.position + new Vector3(3.63f, 10.8f)));
      this.m_roarDummy.gameObject.SetActive(true);
      this.m_roarDummy.GetComponent<Renderer>().enabled = true;
      this.m_roarDummy.Play("roar");
      this.Fire();
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      if (this.m_roarDummy.IsPlaying("roar"))
        return ContinuousBehaviorResult.Continue;
      this.m_roarDummy.Play("blank");
      this.m_roarDummy.gameObject.SetActive(false);
      this.m_aiActor.ToggleRenderers(true);
      this.m_dragun.head.OverrideDesiredPosition = new Vector2?();
      return ContinuousBehaviorResult.Finished;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      this.m_roarDummy.Play("blank");
      this.m_roarDummy.gameObject.SetActive(false);
      this.m_aiActor.ToggleRenderers(true);
      this.m_dragun.head.OverrideDesiredPosition = new Vector2?();
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    private void Fire()
    {
      if (!(bool) (Object) this.m_bulletSource)
        this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
      this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
      this.m_bulletSource.BulletScript = this.BulletScript;
      this.m_bulletSource.Initialize();
    }
  }

