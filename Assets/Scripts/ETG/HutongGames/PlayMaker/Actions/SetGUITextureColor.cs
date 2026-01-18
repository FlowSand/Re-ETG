using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUIElement)]
    [HutongGames.PlayMaker.Tooltip("Sets the Color of the GUITexture attached to a Game Object.")]
    public class SetGUITextureColor : ComponentAction<GUITexture>
    {
        [RequiredField]
        [CheckForComponent(typeof (GUITexture))]
        public FsmOwnerDefault gameObject;
        [RequiredField]
        public FsmColor color;
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.color = (FsmColor) Color.white;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetGUITextureColor();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoSetGUITextureColor();

        private void DoSetGUITextureColor()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                return;
            this.guiTexture.color = this.color.Value;
        }
    }
}
