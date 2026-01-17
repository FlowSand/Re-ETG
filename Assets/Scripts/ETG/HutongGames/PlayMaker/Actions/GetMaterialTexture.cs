// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetMaterialTexture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get a texture from a material on a GameObject")]
  [ActionCategory(ActionCategory.Material)]
  public class GetMaterialTexture : ComponentAction<Renderer>
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject the Material is applied to.")]
    [RequiredField]
    [CheckForComponent(typeof (Renderer))]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The index of the Material in the Materials array.")]
    public FsmInt materialIndex;
    [UIHint(UIHint.NamedTexture)]
    [HutongGames.PlayMaker.Tooltip("The texture to get. See Unity Shader docs for names.")]
    public FsmString namedTexture;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    [Title("StoreTexture")]
    [HutongGames.PlayMaker.Tooltip("Store the texture in a variable.")]
    public FsmTexture storedTexture;
    [HutongGames.PlayMaker.Tooltip("Get the shared version of the texture.")]
    public bool getFromSharedMaterial;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.materialIndex = (FsmInt) 0;
      this.namedTexture = (FsmString) "_MainTex";
      this.storedTexture = (FsmTexture) null;
      this.getFromSharedMaterial = false;
    }

    public override void OnEnter()
    {
      this.DoGetMaterialTexture();
      this.Finish();
    }

    private void DoGetMaterialTexture()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      string name = this.namedTexture.Value;
      if (name == string.Empty)
        name = "_MainTex";
      if (this.materialIndex.Value == 0 && !this.getFromSharedMaterial)
        this.storedTexture.Value = this.renderer.material.GetTexture(name);
      else if (this.materialIndex.Value == 0 && this.getFromSharedMaterial)
        this.storedTexture.Value = this.renderer.sharedMaterial.GetTexture(name);
      else if (this.renderer.materials.Length > this.materialIndex.Value && !this.getFromSharedMaterial)
      {
        Material[] materials = this.renderer.materials;
        this.storedTexture.Value = this.renderer.materials[this.materialIndex.Value].GetTexture(name);
        this.renderer.materials = materials;
      }
      else
      {
        if (this.renderer.materials.Length <= this.materialIndex.Value || !this.getFromSharedMaterial)
          return;
        Material[] sharedMaterials = this.renderer.sharedMaterials;
        this.storedTexture.Value = this.renderer.sharedMaterials[this.materialIndex.Value].GetTexture(name);
        this.renderer.materials = sharedMaterials;
      }
    }
  }
}
