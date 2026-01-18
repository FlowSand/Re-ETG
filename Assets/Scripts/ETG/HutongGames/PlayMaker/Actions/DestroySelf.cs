using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Destroys the Owner of the Fsm! Useful for spawned Prefabs that need to kill themselves, e.g., a projectile that explodes on impact.")]
  [ActionCategory(ActionCategory.GameObject)]
  public class DestroySelf : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Detach children before destroying the Owner.")]
    public FsmBool detachChildren;

    public override void Reset() => this.detachChildren = (FsmBool) false;

    public override void OnEnter()
    {
      if ((Object) this.Owner != (Object) null)
      {
        if (this.detachChildren.Value)
          this.Owner.transform.DetachChildren();
        Object.Destroy((Object) this.Owner);
      }
      this.Finish();
    }
  }
}
