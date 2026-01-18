using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Begin a GUILayout area that follows the specified game object. Useful for overlays (e.g., playerName). NOTE: Block must end with a corresponding GUILayoutEndArea.")]
    [ActionCategory(ActionCategory.GUILayout)]
    public class GUILayoutBeginAreaFollowObject : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject to follow.")]
        public FsmGameObject gameObject;
        [RequiredField]
        public FsmFloat offsetLeft;
        [RequiredField]
        public FsmFloat offsetTop;
        [RequiredField]
        public FsmFloat width;
        [RequiredField]
        public FsmFloat height;
        [HutongGames.PlayMaker.Tooltip("Use normalized screen coordinates (0-1).")]
        public FsmBool normalized;
        [HutongGames.PlayMaker.Tooltip("Optional named style in the current GUISkin")]
        public FsmString style;

        public override void Reset()
        {
            this.gameObject = (FsmGameObject) null;
            this.offsetLeft = (FsmFloat) 0.0f;
            this.offsetTop = (FsmFloat) 0.0f;
            this.width = (FsmFloat) 1f;
            this.height = (FsmFloat) 1f;
            this.normalized = (FsmBool) true;
            this.style = (FsmString) string.Empty;
        }

        public override void OnGUI()
        {
            GameObject gameObject = this.gameObject.Value;
            if ((Object) gameObject == (Object) null || (Object) Camera.main == (Object) null)
            {
                GUILayoutBeginAreaFollowObject.DummyBeginArea();
            }
            else
            {
                Vector3 position = gameObject.transform.position;
                if ((double) Camera.main.transform.InverseTransformPoint(position).z < 0.0)
                {
                    GUILayoutBeginAreaFollowObject.DummyBeginArea();
                }
                else
                {
                    Vector2 screenPoint = (Vector2) Camera.main.WorldToScreenPoint(position);
                    Rect screenRect = new Rect(screenPoint.x + (!this.normalized.Value ? this.offsetLeft.Value : this.offsetLeft.Value * (float) Screen.width), screenPoint.y + (!this.normalized.Value ? this.offsetTop.Value : this.offsetTop.Value * (float) Screen.width), this.width.Value, this.height.Value);
                    if (this.normalized.Value)
                    {
                        screenRect.width *= (float) Screen.width;
                        screenRect.height *= (float) Screen.height;
                    }
                    screenRect.y = (float) Screen.height - screenRect.y;
                    GUILayout.BeginArea(screenRect, this.style.Value);
                }
            }
        }

        private static void DummyBeginArea() => GUILayout.BeginArea(new Rect());
    }
}
