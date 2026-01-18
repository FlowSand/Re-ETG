using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets info on the last Trigger 2d event and store in variables.  See Unity and PlayMaker docs on Unity 2D physics.")]
  [ActionCategory(ActionCategory.Physics2D)]
  public class GetTrigger2dInfo : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Get the GameObject hit.")]
    [UIHint(UIHint.Variable)]
    public FsmGameObject gameObjectHit;
    [HutongGames.PlayMaker.Tooltip("The number of separate shaped regions in the collider.")]
    [UIHint(UIHint.Variable)]
    public FsmInt shapeCount;
    [HutongGames.PlayMaker.Tooltip("Useful for triggering different effects. Audio, particles...")]
    [UIHint(UIHint.Variable)]
    public FsmString physics2dMaterialName;

    public override void Reset()
    {
      this.gameObjectHit = (FsmGameObject) null;
      this.shapeCount = (FsmInt) null;
      this.physics2dMaterialName = (FsmString) null;
    }

    private void StoreTriggerInfo()
    {
      if ((Object) this.Fsm.TriggerCollider2D == (Object) null)
        return;
      this.gameObjectHit.Value = this.Fsm.TriggerCollider2D.gameObject;
      this.shapeCount.Value = this.Fsm.TriggerCollider2D.shapeCount;
      this.physics2dMaterialName.Value = !((Object) this.Fsm.TriggerCollider2D.sharedMaterial != (Object) null) ? string.Empty : this.Fsm.TriggerCollider2D.sharedMaterial.name;
    }

    public override void OnEnter()
    {
      this.StoreTriggerInfo();
      this.Finish();
    }
  }
}
