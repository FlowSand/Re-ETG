// Decompiled with JetBrains decompiler
// Type: TextboxPrompt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[AddComponentMenu("Daikon Forge/Examples/General/Textbox Prompt")]
public class TextboxPrompt : MonoBehaviour
{
  public Color32 promptColor = (Color32) Color.gray;
  public Color32 textColor = (Color32) Color.white;
  public string promptText = "(enter some text)";
  private dfTextbox _textbox;

  public void OnEnable()
  {
    this._textbox = this.GetComponent<dfTextbox>();
    if (!string.IsNullOrEmpty(this._textbox.Text) && !(this._textbox.Text == this.promptText))
      return;
    this._textbox.Text = this.promptText;
    this._textbox.TextColor = this.promptColor;
  }

  public void OnDisable()
  {
    if (!((Object) this._textbox != (Object) null) || !(this._textbox.Text == this.promptText))
      return;
    this._textbox.Text = string.Empty;
  }

  public void OnEnterFocus(dfControl control, dfFocusEventArgs args)
  {
    if (this._textbox.Text == this.promptText)
      this._textbox.Text = string.Empty;
    this._textbox.TextColor = this.textColor;
  }

  public void OnLeaveFocus(dfControl control, dfFocusEventArgs args)
  {
    if (!string.IsNullOrEmpty(this._textbox.Text))
      return;
    this._textbox.Text = this.promptText;
    this._textbox.TextColor = this.promptColor;
  }

  public void OnTextChanged(dfControl control, string value)
  {
    if (!(value != this.promptText))
      return;
    this._textbox.TextColor = this.textColor;
  }
}
