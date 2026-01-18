// Decompiled with JetBrains decompiler
// Type: GameUIAmmoType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

[Serializable]
public class GameUIAmmoType
  {
    public GameUIAmmoType.AmmoType ammoType;
    public string customAmmoType = string.Empty;
    public dfTiledSprite ammoBarFG;
    public dfTiledSprite ammoBarBG;

    public enum AmmoType
    {
      SMALL_BULLET,
      MEDIUM_BULLET,
      BEAM,
      GRENADE,
      SHOTGUN,
      SMALL_BLASTER,
      MEDIUM_BLASTER,
      NAIL,
      MUSKETBALL,
      ARROW,
      MAGIC,
      BLUE_SHOTGUN,
      SKULL,
      FISH,
      CUSTOM,
    }
  }

