using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.GameObject)]
  [HutongGames.PlayMaker.Tooltip("Sets a Game Object's Layer.")]
  public class SetLayer : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Layer)]
    public int layer;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.layer = 0;
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
      ownerDefaultTarget.layer = this.layer;
    }
  }
}
