// Decompiled with JetBrains decompiler
// Type: PlatformVisibility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Examples/General/Platform-based Visibility")]
public class PlatformVisibility : MonoBehaviour
{
  public bool HideOnWeb;
  public bool HideOnMobile;
  public bool HideInEditor;

  private void Start()
  {
    dfControl component = this.GetComponent<dfControl>();
    if ((Object) component == (Object) null || !this.HideInEditor || !Application.isEditor)
      return;
    component.Hide();
  }
}
