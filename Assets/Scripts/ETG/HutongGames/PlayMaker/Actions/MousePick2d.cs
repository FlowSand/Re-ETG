using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Perform a Mouse Pick on a 2d scene and stores the results. Use Ray Distance to set how close the camera must be to pick the 2d object.")]
    [ActionCategory(ActionCategory.Input)]
    public class MousePick2d : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Store if a GameObject was picked in a Bool variable. True if a GameObject was picked, otherwise false.")]
        [UIHint(UIHint.Variable)]
        public FsmBool storeDidPickObject;
        [HutongGames.PlayMaker.Tooltip("Store the picked GameObject in a variable.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject storeGameObject;
        [HutongGames.PlayMaker.Tooltip("Store the picked point in a variable.")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 storePoint;
        [HutongGames.PlayMaker.Tooltip("Pick only from these layers.")]
        [UIHint(UIHint.Layer)]
        public FsmInt[] layerMask;
        [HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
        public FsmBool invertMask;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.storeDidPickObject = (FsmBool) null;
            this.storeGameObject = (FsmGameObject) null;
            this.storePoint = (FsmVector2) null;
            this.layerMask = new FsmInt[0];
            this.invertMask = (FsmBool) false;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoMousePick2d();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoMousePick2d();

        private void DoMousePick2d()
        {
            RaycastHit2D rayIntersection = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), float.PositiveInfinity, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
            bool flag = (Object) rayIntersection.collider != (Object) null;
            this.storeDidPickObject.Value = flag;
            if (flag)
            {
                this.storeGameObject.Value = rayIntersection.collider.gameObject;
                this.storePoint.Value = rayIntersection.point;
            }
            else
            {
                this.storeGameObject.Value = (GameObject) null;
                this.storePoint.Value = (Vector2) Vector3.zero;
            }
        }
    }
}
