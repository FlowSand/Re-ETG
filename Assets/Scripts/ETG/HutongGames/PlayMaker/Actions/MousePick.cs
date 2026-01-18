using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Perform a Mouse Pick on the scene from the Main Camera and stores the results. Use Ray Distance to set how close the camera must be to pick the object.")]
  [ActionCategory(ActionCategory.Input)]
  public class MousePick : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Set the length of the ray to cast from the Main Camera.")]
    [RequiredField]
    public FsmFloat rayDistance = (FsmFloat) 100f;
    [HutongGames.PlayMaker.Tooltip("Set Bool variable true if an object was picked, false if not.")]
    [UIHint(UIHint.Variable)]
    public FsmBool storeDidPickObject;
    [HutongGames.PlayMaker.Tooltip("Store the picked GameObject.")]
    [UIHint(UIHint.Variable)]
    public FsmGameObject storeGameObject;
    [HutongGames.PlayMaker.Tooltip("Store the point of contact.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 storePoint;
    [HutongGames.PlayMaker.Tooltip("Store the normal at the point of contact.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 storeNormal;
    [HutongGames.PlayMaker.Tooltip("Store the distance to the point of contact.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeDistance;
    [HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
    [UIHint(UIHint.Layer)]
    public FsmInt[] layerMask;
    [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
    public FsmBool invertMask;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.rayDistance = (FsmFloat) 100f;
      this.storeDidPickObject = (FsmBool) null;
      this.storeGameObject = (FsmGameObject) null;
      this.storePoint = (FsmVector3) null;
      this.storeNormal = (FsmVector3) null;
      this.storeDistance = (FsmFloat) null;
      this.layerMask = new FsmInt[0];
      this.invertMask = (FsmBool) false;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoMousePick();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoMousePick();

    private void DoMousePick()
    {
      RaycastHit raycastHit = ActionHelpers.MousePick(this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
      bool flag = (Object) raycastHit.collider != (Object) null;
      this.storeDidPickObject.Value = flag;
      if (flag)
      {
        this.storeGameObject.Value = raycastHit.collider.gameObject;
        this.storeDistance.Value = raycastHit.distance;
        this.storePoint.Value = raycastHit.point;
        this.storeNormal.Value = raycastHit.normal;
      }
      else
      {
        this.storeGameObject.Value = (GameObject) null;
        this.storeDistance.Value = float.PositiveInfinity;
        this.storePoint.Value = Vector3.zero;
        this.storeNormal.Value = Vector3.zero;
      }
    }
  }
}
