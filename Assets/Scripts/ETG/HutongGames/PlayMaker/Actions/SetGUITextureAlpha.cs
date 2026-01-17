// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetGUITextureAlpha
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.GUIElement)]
  [HutongGames.PlayMaker.Tooltip("Sets the Alpha of the GUITexture attached to a Game Object. Useful for fading GUI elements in/out.")]
  public class SetGUITextureAlpha : ComponentAction<GUITexture>
  {
    [CheckForComponent(typeof (GUITexture))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    public FsmFloat alpha;
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.alpha = (FsmFloat) 1f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoGUITextureAlpha();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGUITextureAlpha();

    private void DoGUITextureAlpha()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      Color color = this.guiTexture.color;
      this.guiTexture.color = new Color(color.r, color.g, color.b, this.alpha.Value);
    }
  }
}
