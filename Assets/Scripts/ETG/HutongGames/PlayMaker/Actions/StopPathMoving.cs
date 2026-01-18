using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Stops an NPC's PathMover component.")]
  [ActionCategory(".NPCs")]
  public class StopPathMoving : FsmStateAction
  {
    public FsmOwnerDefault GameObject;
    public FsmBool ReenableCollideWithOthers;

    public override string ErrorCheck()
    {
      string empty = string.Empty;
      UnityEngine.GameObject gameObject = this.GameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.GameObject.GameObject.Value : this.Owner;
      if ((bool) (Object) gameObject)
      {
        if (!(bool) (Object) gameObject.GetComponent<PathMover>())
          empty += "Must have a PathMover component.\n";
      }
      else if (!this.GameObject.GameObject.UseVariable)
        return "No object specified";
      return empty;
    }

    public override void OnEnter()
    {
      UnityEngine.GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.GameObject);
      if (!(bool) (Object) ownerDefaultTarget)
        return;
      PathMover component = ownerDefaultTarget.GetComponent<PathMover>();
      if (!(bool) (Object) component)
        return;
      component.Paused = true;
      if (this.ReenableCollideWithOthers.Value && (bool) (Object) component.specRigidbody)
        component.specRigidbody.CollideWithOthers = true;
      this.Finish();
    }
  }
}
