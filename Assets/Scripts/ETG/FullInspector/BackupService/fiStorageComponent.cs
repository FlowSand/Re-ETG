using System.Collections.Generic;

using FullInspector.Internal;
using UnityEngine;

#nullable disable
namespace FullInspector.BackupService
{
    [AddComponentMenu("")]
    public class fiStorageComponent : MonoBehaviour, fiIEditorOnlyTag
    {
        public List<fiSerializedObject> Objects = new List<fiSerializedObject>();

        public void RemoveInvalidBackups()
        {
            int index = 0;
            while (index < this.Objects.Count)
            {
                if (this.Objects[index].Target.Target == (Object) null)
                    this.Objects.RemoveAt(index);
                else
                    ++index;
            }
        }
    }
}
