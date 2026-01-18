using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(".NPCs")]
    public class SelectGunslingGun : BraveFsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Loot table to choose an item from.")]
        public GenericLootTable lootTable;
        [NonSerialized]
        public GameObject SelectedObject;

        public override void Reset() => this.lootTable = (GenericLootTable) null;

        public override void OnEnter()
        {
            if ((UnityEngine.Object) this.SelectedObject == (UnityEngine.Object) null)
                this.SelectedObject = this.lootTable.SelectByWeightWithoutDuplicatesFullPrereqs((List<GameObject>) null, false);
            if ((UnityEngine.Object) this.SelectedObject == (UnityEngine.Object) null)
                this.SelectedObject = this.lootTable.defaultItemDrops.elements[UnityEngine.Random.Range(0, this.lootTable.defaultItemDrops.elements.Count)].gameObject;
            EncounterTrackable component = this.SelectedObject.GetComponent<EncounterTrackable>();
            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
                this.SetReplacementString(component.journalData.GetPrimaryDisplayName());
            this.Finish();
        }
    }
}
