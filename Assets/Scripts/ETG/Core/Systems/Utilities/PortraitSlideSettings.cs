// Decompiled with JetBrains decompiler
// Type: PortraitSlideSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class PortraitSlideSettings
    {
      [StringTableString("enemies")]
      public string bossNameString;
      [StringTableString("enemies")]
      public string bossSubtitleString;
      [StringTableString("enemies")]
      public string bossQuoteString;
      public Texture bossArtSprite;
      public IntVector2 bossSpritePxOffset;
      public IntVector2 topLeftTextPxOffset;
      public IntVector2 bottomRightTextPxOffset;
      public Color bgColor = Color.blue;
    }

}
