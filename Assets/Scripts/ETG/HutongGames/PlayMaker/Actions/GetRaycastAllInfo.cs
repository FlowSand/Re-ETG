#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Gets info on the last RaycastAll and store in array variables.")]
    [ActionCategory(ActionCategory.Physics)]
    public class GetRaycastAllInfo : FsmStateAction
    {
        [Tooltip("Store the GameObjects hit in an array variable.")]
        [ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
        [UIHint(UIHint.Variable)]
        public FsmArray storeHitObjects;
        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.Vector3, "", 0, 0, 65536)]
        [Tooltip("Get the world position of all ray hit point and store them in an array variable.")]
        public FsmArray points;
        [UIHint(UIHint.Variable)]
        [Tooltip("Get the normal at all hit points and store them in an array variable.")]
        [ArrayEditor(VariableType.Vector3, "", 0, 0, 65536)]
        public FsmArray normals;
        [Tooltip("Get the distance along the ray to all hit points and store tjem in an array variable.")]
        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.Float, "", 0, 0, 65536)]
        public FsmArray distances;
        [Tooltip("Repeat every frame. Warning, this could be affecting performances")]
        public bool everyFrame;

        public override void Reset()
        {
            this.storeHitObjects = (FsmArray) null;
            this.points = (FsmArray) null;
            this.normals = (FsmArray) null;
            this.distances = (FsmArray) null;
            this.everyFrame = false;
        }

        private void StoreRaycastAllInfo()
        {
            if (RaycastAll.RaycastAllHitInfo == null)
                return;
            this.storeHitObjects.Resize(RaycastAll.RaycastAllHitInfo.Length);
            this.points.Resize(RaycastAll.RaycastAllHitInfo.Length);
            this.normals.Resize(RaycastAll.RaycastAllHitInfo.Length);
            this.distances.Resize(RaycastAll.RaycastAllHitInfo.Length);
            for (int index = 0; index < RaycastAll.RaycastAllHitInfo.Length; ++index)
            {
                this.storeHitObjects.Values[index] = (object) RaycastAll.RaycastAllHitInfo[index].collider.gameObject;
                this.points.Values[index] = (object) RaycastAll.RaycastAllHitInfo[index].point;
                this.normals.Values[index] = (object) RaycastAll.RaycastAllHitInfo[index].normal;
                this.distances.Values[index] = (object) RaycastAll.RaycastAllHitInfo[index].distance;
            }
        }

        public override void OnEnter()
        {
            this.StoreRaycastAllInfo();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.StoreRaycastAllInfo();
    }
}
