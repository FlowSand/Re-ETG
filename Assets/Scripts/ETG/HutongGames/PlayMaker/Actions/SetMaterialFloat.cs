using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets a named float in a game object's material.")]
  [ActionCategory(ActionCategory.Material)]
  public class SetMaterialFloat : ComponentAction<Renderer>
  {
    [CheckForComponent(typeof (Renderer))]
    [HutongGames.PlayMaker.Tooltip("The GameObject that the material is applied to.")]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
    public FsmInt materialIndex;
    [HutongGames.PlayMaker.Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
    public FsmMaterial material;
    [HutongGames.PlayMaker.Tooltip("A named float parameter in the shader.")]
    [RequiredField]
    public FsmString namedFloat;
    [HutongGames.PlayMaker.Tooltip("Set the parameter value.")]
    [RequiredField]
    public FsmFloat floatValue;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is animated.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.materialIndex = (FsmInt) 0;
      this.material = (FsmMaterial) null;
      this.namedFloat = (FsmString) string.Empty;
      this.floatValue = (FsmFloat) 0.0f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetMaterialFloat();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetMaterialFloat();

    private void DoSetMaterialFloat()
    {
      if ((Object) this.material.Value != (Object) null)
      {
        this.material.Value.SetFloat(this.namedFloat.Value, this.floatValue.Value);
      }
      else
      {
        if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
          return;
        if ((Object) this.renderer.material == (Object) null)
          this.LogError("Missing Material!");
        else if (this.materialIndex.Value == 0)
        {
          this.renderer.material.SetFloat(this.namedFloat.Value, this.floatValue.Value);
        }
        else
        {
          if (this.renderer.materials.Length <= this.materialIndex.Value)
            return;
          Material[] materials = this.renderer.materials;
          materials[this.materialIndex.Value].SetFloat(this.namedFloat.Value, this.floatValue.Value);
          this.renderer.materials = materials;
        }
      }
    }
  }
}
