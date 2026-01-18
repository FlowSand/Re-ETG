using System.Collections.Generic;

using FullInspector;
using UnityEngine;

#nullable disable

[fiInspectorOnly]
    public abstract class AssetBundleDatabase<T, U> : ScriptableObject
        where T : Object
        where U : AssetBundleDatabaseEntry
    {
[InspectorCollectionRotorzFlags(HideRemoveButtons = true)]
        public List<U> Entries;

        public U InternalGetDataByGuid(string guid)
        {
            int index = 0;
            for (int count = this.Entries.Count; index < count; ++index)
            {
                U entry = this.Entries[index];
                if ((object) entry != null && entry.myGuid == guid)
                    return entry;
            }
            return (U) null;
        }

        public virtual void DropReferences()
        {
            int index = 0;
            for (int count = this.Entries.Count; index < count; ++index)
                this.Entries[index].DropReference();
        }
    }

