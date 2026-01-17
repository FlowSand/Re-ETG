// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetMaterial
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Material)]
[HutongGames.PlayMaker.Tooltip("Sets the material on a game object.")]
public class SetMaterial : ComponentAction<Renderer>
{
  [CheckForComponent(typeof (Renderer))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  public FsmInt materialIndex;
  [RequiredField]
  public FsmMaterial material;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.material = (FsmMaterial) null;
    this.materialIndex = (FsmInt) 0;
  }

  public override void OnEnter()
  {
    this.DoSetMaterial();
    this.Finish();
  }

  private void DoSetMaterial()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    if (this.materialIndex.Value == 0)
    {
      this.renderer.material = this.material.Value;
    }
    else
    {
      if (this.renderer.materials.Length <= this.materialIndex.Value)
        return;
      Material[] materials = this.renderer.materials;
      materials[this.materialIndex.Value] = this.material.Value;
      this.renderer.materials = materials;
    }
  }
}
