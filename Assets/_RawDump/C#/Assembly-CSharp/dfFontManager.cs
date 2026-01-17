// Decompiled with JetBrains decompiler
// Type: dfFontManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public static class dfFontManager
{
  private static dfList<dfFontBase> dirty = new dfList<dfFontBase>();
  private static dfList<dfFontBase> rebuildList = new dfList<dfFontBase>();

  public static void FlagPendingRequests(dfFontBase font)
  {
    dfDynamicFont dfDynamicFont = font as dfDynamicFont;
    if (!((Object) dfDynamicFont != (Object) null) || dfFontManager.rebuildList.Contains((dfFontBase) dfDynamicFont))
      return;
    dfFontManager.rebuildList.Add((dfFontBase) dfDynamicFont);
  }

  public static void Invalidate(dfFontBase font)
  {
    if ((Object) font == (Object) null || !(font is dfDynamicFont) || dfFontManager.dirty.Contains(font))
      return;
    dfFontManager.dirty.Add(font);
  }

  public static bool IsDirty(dfFontBase font) => dfFontManager.dirty.Contains(font);

  public static bool RebuildDynamicFonts()
  {
    dfFontManager.rebuildList.Clear();
    dfList<dfControl> activeInstances = dfControl.ActiveInstances;
    for (int index = 0; index < activeInstances.Count; ++index)
    {
      if (activeInstances[index] is IRendersText rendersText)
        rendersText.UpdateFontInfo();
    }
    bool flag = dfFontManager.rebuildList.Count > 0;
    for (int index = 0; index < dfFontManager.rebuildList.Count; ++index)
    {
      dfDynamicFont rebuild = dfFontManager.rebuildList[index] as dfDynamicFont;
      if ((Object) rebuild != (Object) null)
        rebuild.FlushCharacterRequests();
    }
    dfFontManager.rebuildList.Clear();
    dfFontManager.dirty.Clear();
    return flag;
  }
}
