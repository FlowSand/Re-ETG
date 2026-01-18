using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Get the textMesh properties in one go just for convenience. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
    [ActionCategory("2D Toolkit/TextMesh")]
    public class Tk2dTextMeshGetProperties : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
        [CheckForComponent(typeof (tk2dTextMesh))]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Text")]
        [UIHint(UIHint.Variable)]
        public FsmString text;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("InlineStyling")]
        public FsmBool inlineStyling;
        [HutongGames.PlayMaker.Tooltip("Anchor")]
        [UIHint(UIHint.Variable)]
        public FsmString anchor;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Kerning")]
        public FsmBool kerning;
        [HutongGames.PlayMaker.Tooltip("maxChars")]
        [UIHint(UIHint.Variable)]
        public FsmInt maxChars;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("number of drawn characters")]
        public FsmInt NumDrawnCharacters;
        [HutongGames.PlayMaker.Tooltip("The Main Color")]
        [UIHint(UIHint.Variable)]
        public FsmColor mainColor;
        [HutongGames.PlayMaker.Tooltip("The Gradient Color. Only used if gradient is true")]
        [UIHint(UIHint.Variable)]
        public FsmColor gradientColor;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Use gradient")]
        public FsmBool useGradient;
        [HutongGames.PlayMaker.Tooltip("Texture gradient")]
        [UIHint(UIHint.Variable)]
        public FsmInt textureGradient;
        [HutongGames.PlayMaker.Tooltip("Scale")]
        [UIHint(UIHint.Variable)]
        public FsmVector3 scale;
        private tk2dTextMesh _textMesh;

        private void _getTextMesh()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
        }

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.text = (FsmString) null;
            this.inlineStyling = (FsmBool) null;
            this.textureGradient = (FsmInt) null;
            this.mainColor = (FsmColor) null;
            this.gradientColor = (FsmColor) null;
            this.useGradient = (FsmBool) null;
            this.anchor = (FsmString) null;
            this.scale = (FsmVector3) null;
            this.kerning = (FsmBool) null;
            this.maxChars = (FsmInt) null;
            this.NumDrawnCharacters = (FsmInt) null;
        }

        public override void OnEnter()
        {
            this._getTextMesh();
            this.DoGetProperties();
            this.Finish();
        }

        private void DoGetProperties()
        {
            if ((Object) this._textMesh == (Object) null)
            {
                this.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
            }
            else
            {
                this.text.Value = this._textMesh.text;
                this.inlineStyling.Value = this._textMesh.inlineStyling;
                this.textureGradient.Value = this._textMesh.textureGradient;
                this.mainColor.Value = this._textMesh.color;
                this.gradientColor.Value = this._textMesh.color2;
                this.useGradient.Value = this._textMesh.useGradient;
                this.anchor.Value = this._textMesh.anchor.ToString();
                this.scale.Value = this._textMesh.scale;
                this.kerning.Value = this._textMesh.kerning;
                this.maxChars.Value = this._textMesh.maxChars;
                this.NumDrawnCharacters.Value = this._textMesh.NumDrawnCharacters();
                this.textureGradient.Value = this._textMesh.textureGradient;
            }
        }
    }
}
