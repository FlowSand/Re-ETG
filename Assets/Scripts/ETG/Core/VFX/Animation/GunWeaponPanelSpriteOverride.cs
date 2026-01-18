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

