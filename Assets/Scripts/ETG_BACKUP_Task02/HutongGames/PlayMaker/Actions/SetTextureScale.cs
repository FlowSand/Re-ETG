// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetTextureScale
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the Scale of a named texture in a Game Object's Material. Useful for special effects.")]
[ActionCategory(ActionCategory.Material)]
public class SetTextureScale : ComponentAction<Renderer>
{
  [CheckForComponent(typeof (Renderer))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  public FsmInt materialIndex;
  [UIHint(UIHint.NamedColor)]
  public FsmString namedTexture;
  [RequiredField]
  public FsmFloat scaleX;
  [RequiredField]
  public FsmFloat scaleY;
  public bool everyFrame;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.materialIndex = (FsmInt) 0;
    this.namedTexture = (FsmString) "_MainTex";
    this.scaleX = (FsmFloat) 1f;
    this.scaleY = (FsmFloat) 1f;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoSetTextureScale();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoSetTextureScale();

  private void DoSetTextureScale()
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      return;
    if ((Object) this.renderer.material == (Object) null)
      this.LogError("Missing Material!");
    else if (this.materialIndex.Value == 0)
    {
      this.renderer.material.SetTextureScale(this.namedTexture.Value, new Vector2(this.scaleX.Value, this.scaleY.Value));
    }
    else
    {
      if (this.renderer.materials.Length <= this.materialIndex.Value)
        return;
      Material[] materials = this.renderer.materials;
      materials[this.materialIndex.Value].SetTextureScale(this.namedTexture.Value, new Vector2(this.scaleX.Value, this.scaleY.Value));
      this.renderer.materials = materials;
    }
  }
}
