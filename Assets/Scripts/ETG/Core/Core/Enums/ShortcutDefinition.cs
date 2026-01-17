// Decompiled with JetBrains decompiler
// Type: ShortcutDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Core.Enums
{
    [Serializable]
    public struct ShortcutDefinition
    {
      public string targetLevelName;
      [LongEnum]
      public GungeonFlags requiredFlag;
      public string sherpaTextKey;
      public string elevatorFloorSpriteName;
      public bool IsBossRush;
      public bool IsSuperBossRush;
    }

}
