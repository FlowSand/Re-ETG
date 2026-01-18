// Decompiled with JetBrains decompiler
// Type: rtSampleEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Rich Text/Rich Text Events")]
public class rtSampleEvents : MonoBehaviour
  {
    public void OnLinkClicked(dfRichTextLabel sender, dfMarkupTagAnchor tag)
    {
      string href = tag.HRef;
      if (!href.ToLowerInvariant().StartsWith("http:"))
        return;
      Application.OpenURL(href);
    }
  }

