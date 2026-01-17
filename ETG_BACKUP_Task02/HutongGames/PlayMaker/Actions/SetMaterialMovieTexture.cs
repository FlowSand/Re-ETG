// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetMaterialMovieTexture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets a named texture in a game object's material to a movie texture.")]
[ActionCategory(ActionCategory.Material)]
public class SetMaterialMovieTexture : ComponentAction<Renderer>
{
  [CheckForComponent(typeof (Renderer))]
  [HutongGames.PlayMaker.Tooltip("The GameObject that the material is applied to.")]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
  public FsmInt materialIndex;
  [HutongGames.PlayMaker.Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
  public FsmMaterial material;
  [HutongGames.PlayMaker.Tooltip("A named texture in the shader.")]
  [UIHint(UIHint.NamedTexture)]
  public FsmString namedTexture;
  [ObjectType(typeof (MovieTexture))]
  [RequiredField]
  public FsmObject movieTexture;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.materialIndex = (FsmInt) 0;
    this.material = (FsmMaterial) null;
    this.namedTexture = (FsmString) "_MainTex";
    this.movieTexture = (FsmObject) null;
  }

  public override void OnEnter()
  {
    this.DoSetMaterialTexture();
    this.Finish();
  }

  private void DoSetMaterialTexture()
  {
    MovieTexture movieTexture = this.movieTexture.Value as MovieTexture;
    string name = this.namedTexture.Value;
    if (name == string.Empty)
      name = "_MainTex";
    if ((Object) this.material.Value != (Object) null)
    {
      this.material.Value.SetTexture(name, (Texture) movieTexture);
    }
    else
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      if ((Object) this.renderer.material == (Object) null)
        this.LogError("Missing Material!");
      else if (this.materialIndex.Value == 0)
      {
        this.renderer.material.SetTexture(name, (Texture) movieTexture);
      }
      else
      {
        if (this.renderer.materials.Length <= this.materialIndex.Value)
          return;
        Material[] materials = this.renderer.materials;
        materials[this.materialIndex.Value].SetTexture(name, (Texture) movieTexture);
        this.renderer.materials = materials;
      }
    }
  }
}
