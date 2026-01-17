// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetMaterial
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Get a material at index on a gameObject and store it in a variable")]
[ActionCategory(ActionCategory.Material)]
public class GetMaterial : ComponentAction<Renderer>
{
  [HutongGames.PlayMaker.Tooltip("The GameObject the Material is applied to.")]
  [RequiredField]
  [CheckForComponent(typeof (Renderer))]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The index of the Material in the Materials array.")]
  public FsmInt materialIndex;
  [RequiredField]
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Store the material in a variable.")]
  public FsmMaterial material;
  [HutongGames.PlayMaker.Tooltip("Get the shared material of this object. NOTE: Modifying the shared material will change the appearance of all objects using this material, and change material settings that are stored in the project too.")]
  public bool getSharedMaterial;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.material = (FsmMaterial) null;
    this.materialIndex = (FsmInt) 0;
    this.getSharedMaterial = false;
  }

  public override void OnEnter()
  {
    this.DoGetMaterial();
    this.Finish();
  }

  private void DoGetMaterial()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    if (this.materialIndex.Value == 0 && !this.getSharedMaterial)
      this.material.Value = this.renderer.material;
    else if (this.materialIndex.Value == 0 && this.getSharedMaterial)
      this.material.Value = this.renderer.sharedMaterial;
    else if (this.renderer.materials.Length > this.materialIndex.Value && !this.getSharedMaterial)
    {
      Material[] materials = this.renderer.materials;
      this.material.Value = materials[this.materialIndex.Value];
      this.renderer.materials = materials;
    }
    else
    {
      if (this.renderer.materials.Length <= this.materialIndex.Value || !this.getSharedMaterial)
        return;
      Material[] sharedMaterials = this.renderer.sharedMaterials;
      this.material.Value = sharedMaterials[this.materialIndex.Value];
      this.renderer.sharedMaterials = sharedMaterials;
    }
  }
}
