using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Transforms position from world space into screen space. NOTE: Uses the MainCamera!")]
    [ActionCategory(ActionCategory.Camera)]
    public class WorldToScreenPoint : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("World position to transform into screen coordinates.")]
        public FsmVector3 worldPosition;
        [HutongGames.PlayMaker.Tooltip("World X position.")]
        public FsmFloat worldX;
        [HutongGames.PlayMaker.Tooltip("World Y position.")]
        public FsmFloat worldY;
        [HutongGames.PlayMaker.Tooltip("World Z position.")]
        public FsmFloat worldZ;
        [HutongGames.PlayMaker.Tooltip("Store the screen position in a Vector3 Variable. Z will equal zero.")]
        [UIHint(UIHint.Variable)]
        public FsmVector3 storeScreenPoint;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Store the screen X position in a Float Variable.")]
        public FsmFloat storeScreenX;
        [HutongGames.PlayMaker.Tooltip("Store the screen Y position in a Float Variable.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeScreenY;
        [HutongGames.PlayMaker.Tooltip("Normalize screen coordinates (0-1). Otherwise coordinates are in pixels.")]
        public FsmBool normalize;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            this.worldPosition = (FsmVector3) null;
            FsmFloat fsmFloat1 = new FsmFloat();
            fsmFloat1.UseVariable = true;
            this.worldX = fsmFloat1;
            FsmFloat fsmFloat2 = new FsmFloat();
            fsmFloat2.UseVariable = true;
            this.worldY = fsmFloat2;
            FsmFloat fsmFloat3 = new FsmFloat();
            fsmFloat3.UseVariable = true;
            this.worldZ = fsmFloat3;
            this.storeScreenPoint = (FsmVector3) null;
            this.storeScreenX = (FsmFloat) null;
            this.storeScreenY = (FsmFloat) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoWorldToScreenPoint();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoWorldToScreenPoint();

        private void DoWorldToScreenPoint()
        {
            if ((Object) Camera.main == (Object) null)
            {
                this.LogError("No MainCamera defined!");
                this.Finish();
            }
            else
            {
                Vector3 zero = Vector3.zero;
                if (!this.worldPosition.IsNone)
                    zero = this.worldPosition.Value;
                if (!this.worldX.IsNone)
                    zero.x = this.worldX.Value;
                if (!this.worldY.IsNone)
                    zero.y = this.worldY.Value;
                if (!this.worldZ.IsNone)
                    zero.z = this.worldZ.Value;
                Vector3 screenPoint = Camera.main.WorldToScreenPoint(zero);
                if (this.normalize.Value)
                {
                    screenPoint.x /= (float) Screen.width;
                    screenPoint.y /= (float) Screen.height;
                }
                this.storeScreenPoint.Value = screenPoint;
                this.storeScreenX.Value = screenPoint.x;
                this.storeScreenY.Value = screenPoint.y;
            }
        }
    }
}
