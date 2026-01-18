using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets a Game Object's Name.")]
  [ActionCategory(ActionCategory.GameObject)]
  public class SetName : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    public FsmString name;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.name = (FsmString) null;
    }

    public override void OnEnter()
    {
      this.DoSetLayer();
      this.Finish();
    }

    private void DoSetLayer()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      ownerDefaultTarget.name = this.name.Value;
    }
  }
}
