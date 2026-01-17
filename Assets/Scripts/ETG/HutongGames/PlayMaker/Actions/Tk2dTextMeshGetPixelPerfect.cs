// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dTextMeshGetPixelPerfect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Get the pixelPerfect flag of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
  [ActionCategory("2D Toolkit/TextMesh")]
  public class Tk2dTextMeshGetPixelPerfect : FsmStateAction
  {
    [Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
    [RequiredField]
    [CheckForComponent(typeof (tk2dTextMesh))]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [Tooltip("(Deprecated in 2D Toolkit 2.0) Is the text pixelPerfect")]
    [RequiredField]
    public FsmBool pixelPerfect;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.pixelPerfect = (FsmBool) null;
    }

    public override void OnEnter() => this.Finish();
  }
}
