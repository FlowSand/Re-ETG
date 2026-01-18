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

