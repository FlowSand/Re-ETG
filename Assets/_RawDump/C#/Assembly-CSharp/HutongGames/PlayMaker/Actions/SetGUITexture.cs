// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetGUITexture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GUIElement)]
[HutongGames.PlayMaker.Tooltip("Sets the Texture used by the GUITexture attached to a Game Object.")]
public class SetGUITexture : ComponentAction<GUITexture>
{
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The GameObject that owns the GUITexture.")]
  [CheckForComponent(typeof (GUITexture))]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("Texture to apply.")]
  public FsmTexture texture;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.texture = (FsmTexture) null;
  }

  public override void OnEnter()
  {
    if (this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      this.guiTexture.texture = this.texture.Value;
    this.Finish();
  }
}
