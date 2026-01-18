using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Physics)]
  [HutongGames.PlayMaker.Tooltip("Forces a Game Object's Rigid Body to wake up.")]
  public class WakeUp : ComponentAction<Rigidbody>
  {
    [CheckForComponent(typeof (Rigidbody))]
    [RequiredField]
    public FsmOwnerDefault gameObject;

    public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

    public override void OnEnter()
    {
      this.DoWakeUp();
      this.Finish();
    }

    private void DoWakeUp()
    {
      if (!this.UpdateCache(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner))
        return;
      this.rigidbody.WakeUp();
    }
  }
}
