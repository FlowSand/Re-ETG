using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("GUI Label.")]
    [ActionCategory(ActionCategory.GUI)]
    public class GUILabel : GUIContentAction
    {
        public override void OnGUI()
        {
            base.OnGUI();
            if (string.IsNullOrEmpty(this.style.Value))
                GUI.Label(this.rect, this.content);
            else
                GUI.Label(this.rect, this.content, (GUIStyle) this.style.Value);
        }
    }
}
