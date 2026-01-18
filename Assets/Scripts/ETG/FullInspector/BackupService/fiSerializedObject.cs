using System;
using System.Collections.Generic;

using FullInspector.Internal;

#nullable disable
namespace FullInspector.BackupService
{
    [Serializable]
    public class fiSerializedObject
    {
        public fiUnityObjectReference Target;
        public string SavedAt;
        public bool ShowDeserialized;
        public fiDeserializedObject DeserializedState;
        public List<fiSerializedMember> Members = new List<fiSerializedMember>();
        public List<fiUnityObjectReference> ObjectReferences = new List<fiUnityObjectReference>();
    }
}
