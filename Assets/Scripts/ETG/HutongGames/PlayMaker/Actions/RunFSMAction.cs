// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.RunFSMAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Base class for actions that want to run a sub FSM.")]
  public abstract class RunFSMAction : FsmStateAction
  {
    protected Fsm runFsm;

    public override void Reset() => this.runFsm = (Fsm) null;

    public override bool Event(FsmEvent fsmEvent)
    {
      if (this.runFsm != null && (fsmEvent.IsGlobal || fsmEvent.IsSystemEvent))
        this.runFsm.Event(fsmEvent);
      return false;
    }

    public override void OnEnter()
    {
      if (this.runFsm == null)
      {
        this.Finish();
      }
      else
      {
        this.runFsm.OnEnable();
        if (!this.runFsm.Started)
          this.runFsm.Start();
        this.CheckIfFinished();
      }
    }

    public override void OnUpdate()
    {
      if (this.runFsm != null)
      {
        this.runFsm.Update();
        this.CheckIfFinished();
      }
      else
        this.Finish();
    }

    public override void OnFixedUpdate()
    {
      if (this.runFsm != null)
      {
        this.runFsm.FixedUpdate();
        this.CheckIfFinished();
      }
      else
        this.Finish();
    }

    public override void OnLateUpdate()
    {
      if (this.runFsm != null)
      {
        this.runFsm.LateUpdate();
        this.CheckIfFinished();
      }
      else
        this.Finish();
    }

    public override void DoTriggerEnter(Collider other)
    {
      if (!this.runFsm.HandleTriggerEnter)
        return;
      this.runFsm.OnTriggerEnter(other);
    }

    public override void DoTriggerStay(Collider other)
    {
      if (!this.runFsm.HandleTriggerStay)
        return;
      this.runFsm.OnTriggerStay(other);
    }

    public override void DoTriggerExit(Collider other)
    {
      if (!this.runFsm.HandleTriggerExit)
        return;
      this.runFsm.OnTriggerExit(other);
    }

    public override void DoCollisionEnter(Collision collisionInfo)
    {
      if (!this.runFsm.HandleCollisionEnter)
        return;
      this.runFsm.OnCollisionEnter(collisionInfo);
    }

    public override void DoCollisionStay(Collision collisionInfo)
    {
      if (!this.runFsm.HandleCollisionStay)
        return;
      this.runFsm.OnCollisionStay(collisionInfo);
    }

    public override void DoCollisionExit(Collision collisionInfo)
    {
      if (!this.runFsm.HandleCollisionExit)
        return;
      this.runFsm.OnCollisionExit(collisionInfo);
    }

    public override void DoParticleCollision(GameObject other)
    {
      if (!this.runFsm.HandleParticleCollision)
        return;
      this.runFsm.OnParticleCollision(other);
    }

    public override void DoControllerColliderHit(ControllerColliderHit collisionInfo)
    {
      this.runFsm.OnControllerColliderHit(collisionInfo);
    }

    public override void DoTriggerEnter2D(Collider2D other)
    {
      if (!this.runFsm.HandleTriggerEnter)
        return;
      this.runFsm.OnTriggerEnter2D(other);
    }

    public override void DoTriggerStay2D(Collider2D other)
    {
      if (!this.runFsm.HandleTriggerStay)
        return;
      this.runFsm.OnTriggerStay2D(other);
    }

    public override void DoTriggerExit2D(Collider2D other)
    {
      if (!this.runFsm.HandleTriggerExit)
        return;
      this.runFsm.OnTriggerExit2D(other);
    }

    public override void DoCollisionEnter2D(Collision2D collisionInfo)
    {
      if (!this.runFsm.HandleCollisionEnter)
        return;
      this.runFsm.OnCollisionEnter2D(collisionInfo);
    }

    public override void DoCollisionStay2D(Collision2D collisionInfo)
    {
      if (!this.runFsm.HandleCollisionStay)
        return;
      this.runFsm.OnCollisionStay2D(collisionInfo);
    }

    public override void DoCollisionExit2D(Collision2D collisionInfo)
    {
      if (!this.runFsm.HandleCollisionExit)
        return;
      this.runFsm.OnCollisionExit2D(collisionInfo);
    }

    public override void OnGUI()
    {
      if (this.runFsm == null || !this.runFsm.HandleOnGUI)
        return;
      this.runFsm.OnGUI();
    }

    public override void OnExit()
    {
      if (this.runFsm == null)
        return;
      this.runFsm.Stop();
    }

    protected virtual void CheckIfFinished()
    {
      if (this.runFsm != null && !this.runFsm.Finished)
        return;
      this.Finish();
    }
  }
}
