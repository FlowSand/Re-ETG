// Decompiled with JetBrains decompiler
// Type: DFScaleFixer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DFScaleFixer : MonoBehaviour
{
  private dfGUIManager m_manager;

  private void Start() => this.m_manager = this.GetComponent<dfGUIManager>();

  private void Update()
  {
    this.m_manager.UIScaleLegacyMode = false;
    this.m_manager.UIScale = (float) this.m_manager.RenderCamera.pixelHeight / (float) this.m_manager.FixedHeight;
  }
}
