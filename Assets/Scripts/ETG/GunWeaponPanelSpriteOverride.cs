// Decompiled with JetBrains decompiler
// Type: GunWeaponPanelSpriteOverride
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class GunWeaponPanelSpriteOverride : MonoBehaviour
{
  public IntVector2[] spritePairs;

  public int GetMatch(int input)
  {
    for (int index = 0; index < this.spritePairs.Length; ++index)
    {
      if (this.spritePairs[index].x == input)
        return this.spritePairs[index].y;
    }
    return input;
  }
}
