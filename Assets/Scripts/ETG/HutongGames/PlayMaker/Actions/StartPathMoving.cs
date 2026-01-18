using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".NPCs")]
  [HutongGames.PlayMaker.Tooltip("Starts an NPC's PathMover component.")]
  public class StartPathMoving : FsmStateAction
  {
    public FsmOwnerDefault GameObject;
    public FsmBool DisableCollideWithOthers;

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
      PathMover component = this.Fsm.GetOwnerDefaultTarget(this.GameObject).GetComponent<PathMover>();
      if ((bool) (Object) component)
      {
        component.Paused = false;
        if (this.DisableCollideWithOthers.Value && (bool) (Object) component.specRigidbody)
          component.specRigidbody.CollideWithOthers = false;
      }
      this.Finish();
    }
  }
}
