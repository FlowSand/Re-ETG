// Decompiled with JetBrains decompiler
// Type: DFTextureSwapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class DFTextureSwapper : MonoBehaviour
  {
    public string EnglishSpriteName;
    public string OtherSpriteName;

    private void Start()
    {
      dfControl component = this.GetComponent<dfControl>();
      if (!(bool) (Object) component)
        return;
      component.IsVisibleChanged += new PropertyChangedEventHandler<bool>(this.HandleVisibilityChanged);
      if (!component.IsVisible)
        return;
      this.HandleVisibilityChanged(component, true);
    }

    private void HandleVisibilityChanged(dfControl control, bool value)
    {
      switch (control)
      {
        case dfSlicedSprite _:
          (control as dfSlicedSprite).SpriteName = GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH ? this.OtherSpriteName : this.EnglishSpriteName;
          break;
        case dfSprite _:
          (control as dfSprite).SpriteName = GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH ? this.OtherSpriteName : this.EnglishSpriteName;
          break;
      }
    }
  }

