using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class ProceduralChestContents : ScriptableObject
    {
        [BetterList]
        public List<ProceduralChestItem> items;

        public PickupObject GetItem(float val)
        {
            float num1 = 0.0f;
            for (int index = 0; index < this.items.Count; ++index)
                num1 += this.items[index].chance;
            float num2 = 0.0f;
            for (int index = 0; index < this.items.Count; ++index)
            {
                num2 += this.items[index].chance;
                if ((double) num2 / (double) num1 > (double) val)
                    return this.items[index].item;
            }
            return this.items[this.items.Count - 1].item;
        }
    }

