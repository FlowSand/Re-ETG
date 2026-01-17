// Decompiled with JetBrains decompiler
// Type: DefaultLabelController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class DefaultLabelController : BraveBehaviour
{
  public dfLabel label;
  public dfPanel panel;
  public Transform targetObject;
  public Vector3 offset;
  private dfGUIManager m_manager;

  public void Trigger() => this.StartCoroutine(this.Expand_CR());

  public void Trigger(Transform aTarget, Vector3 anOffset)
  {
    this.offset = anOffset;
    this.targetObject = aTarget;
    this.Trigger();
  }

  [DebuggerHidden]
  private IEnumerator Expand_CR()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new DefaultLabelController.\u003CExpand_CR\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void LateUpdate()
  {
    this.UpdatePosition();
    this.UpdateForLanguage();
  }

  public void UpdateForLanguage()
  {
    if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.KOREAN)
      this.label.Padding.top = 0;
    else
      this.label.Padding.top = -6;
  }

  public void UpdatePosition()
  {
    if ((Object) this.m_manager == (Object) null)
      this.m_manager = this.panel.GetManager();
    if (!(bool) (Object) this.targetObject)
      return;
    this.transform.position = dfFollowObject.ConvertWorldSpaces(this.targetObject.transform.position + this.offset, GameManager.Instance.MainCameraController.Camera, this.m_manager.RenderCamera).WithZ(0.0f);
    this.transform.position = this.transform.position.QuantizeFloor(this.panel.PixelsToUnits() / (Pixelator.Instance.ScaleTileScale / Pixelator.Instance.CurrentTileScale));
  }

  protected override void OnDestroy() => base.OnDestroy();
}
