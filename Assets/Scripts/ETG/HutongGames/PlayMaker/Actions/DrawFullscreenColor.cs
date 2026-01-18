using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Fills the screen with a Color. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
    [ActionCategory(ActionCategory.GUI)]
    public class DrawFullscreenColor : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("Color. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
        public FsmColor color;

        public override void Reset() => this.color = (FsmColor) Color.white;

        public override void OnGUI()
        {
            Color color = GUI.color;
            GUI.color = this.color.Value;
            GUI.DrawTexture(new Rect(0.0f, 0.0f, (float) Screen.width, (float) Screen.height), (Texture) ActionHelpers.WhiteTexture);
            GUI.color = color;
        }
    }
}
