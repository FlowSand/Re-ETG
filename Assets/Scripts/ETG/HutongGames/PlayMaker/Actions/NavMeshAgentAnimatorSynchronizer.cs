using UnityEngine;
using UnityEngine.AI;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Synchronize a NavMesh Agent velocity and rotation with the animator process.")]
  [ActionCategory(ActionCategory.Animator)]
  public class NavMeshAgentAnimatorSynchronizer : FsmStateAction
  {
    [CheckForComponent(typeof (Animator))]
    [CheckForComponent(typeof (NavMeshAgent))]
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The Agent target. An Animator component and a NavMeshAgent component are required")]
    public FsmOwnerDefault gameObject;
    private Animator _animator;
    private NavMeshAgent _agent;
    private Transform _trans;

    public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

    public override void OnPreprocess() => this.Fsm.HandleAnimatorMove = true;

    public override void OnEnter()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
      {
        this.Finish();
      }
      else
      {
        this._agent = ownerDefaultTarget.GetComponent<NavMeshAgent>();
        this._animator = ownerDefaultTarget.GetComponent<Animator>();
        if ((Object) this._animator == (Object) null)
          this.Finish();
        else
          this._trans = ownerDefaultTarget.transform;
      }
    }

    public override void DoAnimatorMove()
    {
      this._agent.velocity = this._animator.deltaPosition / Time.deltaTime;
      this._trans.rotation = this._animator.rootRotation;
    }
  }
}
