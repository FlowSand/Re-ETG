// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetTextureOffset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Material)]
[HutongGames.PlayMaker.Tooltip("Sets the Offset of a named texture in a Game Object's Material. Useful for scrolling texture effects.")]
public class SetTextureOffset : ComponentAction<Renderer>
{
  [RequiredField]
  [CheckForComponent(typeof (Renderer))]
  public FsmOwnerDefault gameObject;
  public FsmInt materialIndex;
  [RequiredField]
  [UIHint(UIHint.NamedColor)]
  public FsmString namedTexture;
  [RequiredField]
  public FsmFloat offsetX;
  [RequiredField]
  public FsmFloat offsetY;
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.materialIndex = (FsmInt) 0;
    this.namedTexture = (FsmString) "_MainTex";
    this.offsetX = (FsmFloat) 0.0f;
    this.offsetY = (FsmFloat) 0.0f;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoSetTextureOffset();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoSetTextureOffset();

  private void DoSetTextureOffset()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    if ((Object) this.renderer.material == (Object) null)
      this.LogError("Missing Material!");
    else if (this.materialIndex.Value == 0)
    {
      this.renderer.material.SetTextureOffset(this.namedTexture.Value, new Vector2(this.offsetX.Value, this.offsetY.Value));
    }
    else
    {
      if (this.renderer.materials.Length <= this.materialIndex.Value)
        return;
      Material[] materials = this.renderer.materials;
      materials[this.materialIndex.Value].SetTextureOffset(this.namedTexture.Value, new Vector2(this.offsetX.Value, this.offsetY.Value));
      this.renderer.materials = materials;
    }
  }
}
