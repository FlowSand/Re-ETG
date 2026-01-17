// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Tk2dTextMeshSetProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory("2D Toolkit/TextMesh")]
[HutongGames.PlayMaker.Tooltip("Set the textMesh properties in one go just for convenience. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
public class Tk2dTextMeshSetProperties : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
  [CheckForComponent(typeof (tk2dTextMesh))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The Text")]
  public FsmString text;
  [HutongGames.PlayMaker.Tooltip("InlineStyling")]
  [UIHint(UIHint.Variable)]
  public FsmBool inlineStyling;
  [HutongGames.PlayMaker.Tooltip("anchor")]
  public TextAnchor anchor;
  [HutongGames.PlayMaker.Tooltip("The anchor as a string (text Anchor setting will be ignore if set). \npossible values ( case insensitive): LowerLeft,LowerCenter,LowerRight,MiddleLeft,MiddleCenter,MiddleRight,UpperLeft,UpperCenter or UpperRight ")]
  [UIHint(UIHint.FsmString)]
  public FsmString OrTextAnchorString;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Kerning")]
  public FsmBool kerning;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("maxChars")]
  public FsmInt maxChars;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("number of drawn characters")]
  public FsmInt NumDrawnCharacters;
  [HutongGames.PlayMaker.Tooltip("The Main Color")]
  [UIHint(UIHint.Variable)]
  public FsmColor mainColor;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The Gradient Color. Only used if gradient is true")]
  public FsmColor gradientColor;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Use gradient")]
  public FsmBool useGradient;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Texture gradient")]
  public FsmInt textureGradient;
  [HutongGames.PlayMaker.Tooltip("Scale")]
  [UIHint(UIHint.Variable)]
  public FsmVector3 scale;
  [HutongGames.PlayMaker.Tooltip("Commit changes")]
  [UIHint(UIHint.FsmString)]
  public FsmBool commit;
  private tk2dTextMesh _textMesh;

  private void _getTextMesh()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
  }

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.text = (FsmString) null;
    this.inlineStyling = (FsmBool) null;
    this.textureGradient = (FsmInt) null;
    this.mainColor = (FsmColor) null;
    this.gradientColor = (FsmColor) null;
    this.useGradient = (FsmBool) null;
    this.anchor = TextAnchor.LowerLeft;
    this.scale = (FsmVector3) null;
    this.kerning = (FsmBool) null;
    this.maxChars = (FsmInt) null;
    this.NumDrawnCharacters = (FsmInt) null;
    this.commit = (FsmBool) true;
  }

  public override void OnEnter()
  {
    this._getTextMesh();
    this.DoSetProperties();
    this.Finish();
  }

  private void DoSetProperties()
  {
    if ((Object) this._textMesh == (Object) null)
    {
      this.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
    }
    else
    {
      bool flag = false;
      if (this._textMesh.text != this.text.Value)
      {
        this._textMesh.text = this.text.Value;
        flag = true;
      }
      if (this._textMesh.inlineStyling != this.inlineStyling.Value)
      {
        this._textMesh.inlineStyling = this.inlineStyling.Value;
        flag = true;
      }
      if (this._textMesh.textureGradient != this.textureGradient.Value)
      {
        this._textMesh.textureGradient = this.textureGradient.Value;
        flag = true;
      }
      if (this._textMesh.useGradient != this.useGradient.Value)
      {
        this._textMesh.useGradient = this.useGradient.Value;
        flag = true;
      }
      if (this._textMesh.color != this.mainColor.Value)
      {
        this._textMesh.color = this.mainColor.Value;
        flag = true;
      }
      if (this._textMesh.color2 != this.gradientColor.Value)
      {
        this._textMesh.color2 = this.gradientColor.Value;
        flag = true;
      }
      this.scale.Value = this._textMesh.scale;
      this.kerning.Value = this._textMesh.kerning;
      this.maxChars.Value = this._textMesh.maxChars;
      this.NumDrawnCharacters.Value = this._textMesh.NumDrawnCharacters();
      this.textureGradient.Value = this._textMesh.textureGradient;
      if (!this.commit.Value || !flag)
        return;
      this._textMesh.Commit();
    }
  }
}
