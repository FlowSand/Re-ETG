using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Material)]
    [HutongGames.PlayMaker.Tooltip("Sets a named texture in a game object's material.")]
    public class SetMaterialTexture : ComponentAction<Renderer>
    {
        [CheckForComponent(typeof (Renderer))]
        [HutongGames.PlayMaker.Tooltip("The GameObject that the material is applied to.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
        public FsmInt materialIndex;
        [HutongGames.PlayMaker.Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
        public FsmMaterial material;
        [HutongGames.PlayMaker.Tooltip("A named parameter in the shader.")]
        [UIHint(UIHint.NamedTexture)]
        public FsmString namedTexture;
        public FsmTexture texture;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.materialIndex = (FsmInt) 0;
            this.material = (FsmMaterial) null;
            this.namedTexture = (FsmString) "_MainTex";
            this.texture = (FsmTexture) null;
        }

        public override void OnEnter()
        {
            this.DoSetMaterialTexture();
            this.Finish();
        }

        private void DoSetMaterialTexture()
        {
            string name = this.namedTexture.Value;
            if (name == string.Empty)
                name = "_MainTex";
            if ((Object) this.material.Value != (Object) null)
            {
                this.material.Value.SetTexture(name, this.texture.Value);
            }
            else
            {
                if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                    return;
                if ((Object) this.renderer.material == (Object) null)
                    this.LogError("Missing Material!");
                else if (this.materialIndex.Value == 0)
                {
                    this.renderer.material.SetTexture(name, this.texture.Value);
                }
                else
                {
                    if (this.renderer.materials.Length <= this.materialIndex.Value)
                        return;
                    Material[] materials = this.renderer.materials;
                    materials[this.materialIndex.Value].SetTexture(name, this.texture.Value);
                    this.renderer.materials = materials;
                }
            }
        }
    }
}
