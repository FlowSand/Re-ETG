// Decompiled with JetBrains decompiler
// Type: PlatformInterfaceGenericPC
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class PlatformInterfaceGenericPC : PlatformInterface
  {
    protected override void OnStart()
    {
      Debug.Log((object) "Starting Generic PC platform interface.");
    }

    protected override void OnAchievementUnlock(Achievement achievement, int playerIndex)
    {
    }

    protected override void OnLateUpdate()
    {
    }

    protected override StringTableManager.GungeonSupportedLanguages OnGetPreferredLanguage()
    {
      return StringTableManager.GungeonSupportedLanguages.ENGLISH;
    }
  }

