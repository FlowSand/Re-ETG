// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ThievingRatGrabby
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Pathfinding;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(".NPCs")]
public class ThievingRatGrabby : FsmStateAction
{
  public PickupObject TargetObject;
  public NoteDoer notePrefab;
  private Vector2 m_lastPosition;
  private TalkDoerLite m_talkDoer;
  private bool m_grabby;

  public override void Awake()
  {
    base.Awake();
    this.m_talkDoer = this.Owner.GetComponent<TalkDoerLite>();
  }

  public override void OnEnter() => this.m_lastPosition = this.m_talkDoer.specRigidbody.UnitCenter;

  public override void OnUpdate()
  {
    base.OnUpdate();
    if (this.m_talkDoer.CurrentPath == null)
      return;
    if (!this.m_talkDoer.CurrentPath.WillReachFinalGoal)
    {
      this.m_talkDoer.transform.position = (Vector3) (this.TargetObject.sprite.WorldCenter + new Vector2(0.0f, 1f));
      this.m_talkDoer.specRigidbody.Reinitialize();
      PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.m_talkDoer.specRigidbody, new int?(CollisionMask.LayerToMask(CollisionLayer.PlayerCollider)));
      this.m_talkDoer.talkDoer.CurrentPath = (Path) null;
    }
    else
    {
      this.m_talkDoer.specRigidbody.Velocity = this.m_talkDoer.GetPathVelocityContribution(this.m_lastPosition, 32 /*0x20*/);
      this.m_lastPosition = this.m_talkDoer.specRigidbody.UnitCenter;
    }
  }

  [DebuggerHidden]
  private IEnumerator HandleGrabby()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ThievingRatGrabby.\u003CHandleGrabby\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public override void OnLateUpdate()
  {
    if (this.m_talkDoer.CurrentPath != null || this.m_grabby)
      return;
    this.m_talkDoer.StartCoroutine(this.HandleGrabby());
  }
}
