// Decompiled with JetBrains decompiler
// Type: RuntimePixelPerfect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [AddComponentMenu("Daikon Forge/Examples/General/Pixel-Perfect Platform Settings")]
    public class RuntimePixelPerfect : MonoBehaviour
    {
      public bool PixelPerfectInEditor;
      public bool PixelPerfectAtRuntime = true;

      private void Awake()
      {
        dfGUIManager component = this.GetComponent<dfGUIManager>();
        if ((Object) component == (Object) null)
          throw new MissingComponentException("dfGUIManager instance not found");
        if (Application.isEditor)
          component.PixelPerfectMode = this.PixelPerfectInEditor;
        else
          component.PixelPerfectMode = this.PixelPerfectAtRuntime;
      }
    }

}
