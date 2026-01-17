// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetMaterialColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets a named color value in a game object's material.")]
  [ActionCategory(ActionCategory.Material)]
  public class SetMaterialColor : ComponentAction<Renderer>
  {
    [CheckForComponent(typeof (Renderer))]
    [HutongGames.PlayMaker.Tooltip("The GameObject that the material is applied to.")]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
    public FsmInt materialIndex;
    [HutongGames.PlayMaker.Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
    public FsmMaterial material;
    [HutongGames.PlayMaker.Tooltip("A named color parameter in the shader.")]
    [UIHint(UIHint.NamedColor)]
    public FsmString namedColor;
    [HutongGames.PlayMaker.Tooltip("Set the parameter value.")]
    [RequiredField]
    public FsmColor color;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is animated.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.materialIndex = (FsmInt) 0;
      this.material = (FsmMaterial) null;
      this.namedColor = (FsmString) "_Color";
      this.color = (FsmColor) Color.black;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetMaterialColor();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetMaterialColor();

    private void DoSetMaterialColor()
    {
      if (this.color.IsNone)
        return;
      string name = this.namedColor.Value;
      if (name == string.Empty)
        name = "_Color";
      if ((Object) this.material.Value != (Object) null)
      {
        this.material.Value.SetColor(name, this.color.Value);
      }
      else
      {
        if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
          return;
        if ((Object) this.renderer.material == (Object) null)
          this.LogError("Missing Material!");
        else if (this.materialIndex.Value == 0)
        {
          this.renderer.material.SetColor(name, this.color.Value);
        }
        else
        {
          if (this.renderer.materials.Length <= this.materialIndex.Value)
            return;
          Material[] materials = this.renderer.materials;
          materials[this.materialIndex.Value].SetColor(name, this.color.Value);
          this.renderer.materials = materials;
        }
      }
    }
  }
}
