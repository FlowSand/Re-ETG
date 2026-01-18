using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Physics)]
  [HutongGames.PlayMaker.Tooltip("Gets info on the last Raycast and store in variables.")]
  public class GetRaycastHitInfo : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Get the GameObject hit by the last Raycast and store it in a variable.")]
    [UIHint(UIHint.Variable)]
    public FsmGameObject gameObjectHit;
    [UIHint(UIHint.Variable)]
    [Title("Hit Point")]
    [HutongGames.PlayMaker.Tooltip("Get the world position of the ray hit point and store it in a variable.")]
    public FsmVector3 point;
    [HutongGames.PlayMaker.Tooltip("Get the normal at the hit point and store it in a variable.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 normal;
    [HutongGames.PlayMaker.Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat distance;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObjectHit = (FsmGameObject) null;
      this.point = (FsmVector3) null;
      this.normal = (FsmVector3) null;
      this.distance = (FsmFloat) null;
      this.everyFrame = false;
    }

    private void StoreRaycastInfo()
    {
      if (!((Object) this.Fsm.RaycastHitInfo.collider != (Object) null))
        return;
      this.gameObjectHit.Value = this.Fsm.RaycastHitInfo.collider.gameObject;
      this.point.Value = this.Fsm.RaycastHitInfo.point;
      this.normal.Value = this.Fsm.RaycastHitInfo.normal;
      this.distance.Value = this.Fsm.RaycastHitInfo.distance;
    }

    public override void OnEnter()
    {
      this.StoreRaycastInfo();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.StoreRaycastInfo();
  }
}
