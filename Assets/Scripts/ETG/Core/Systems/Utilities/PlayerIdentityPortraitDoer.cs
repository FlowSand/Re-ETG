// Decompiled with JetBrains decompiler
// Type: PlayerIdentityPortraitDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class PlayerIdentityPortraitDoer : MonoBehaviour
  {
    public bool IsPlayerTwo;

    public static string GetPortraitSpriteName(PlayableCharacters character)
    {
      switch (character)
      {
        case PlayableCharacters.Pilot:
          return "Player_Rogue_001";
        case PlayableCharacters.Convict:
          return "Player_Convict_001";
        case PlayableCharacters.Robot:
          return "Player_Robot_001";
        case PlayableCharacters.Ninja:
          return "Player_Ninja_001";
        case PlayableCharacters.Cosmonaut:
          return "Player_Cosmonaut_001";
        case PlayableCharacters.Soldier:
          return "Player_Marine_001";
        case PlayableCharacters.Guide:
          return "Player_Guide_001";
        case PlayableCharacters.CoopCultist:
          return "Player_Coop_Pink_001";
        case PlayableCharacters.Bullet:
          return "Player_Bullet_001";
        case PlayableCharacters.Eevee:
          return "Player_Eevee_minimap_001";
        case PlayableCharacters.Gunslinger:
          return "Player_Slinger_001";
        default:
          return "Player_Rogue_001";
      }
    }

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new PlayerIdentityPortraitDoer__Startc__Iterator0()
      {
        _this = this
      };
    }
  }

