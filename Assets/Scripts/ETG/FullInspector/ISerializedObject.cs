using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace FullInspector
{
    public interface ISerializedObject
    {
        void RestoreState();

        void SaveState();

        bool IsRestored { get; set; }

        List<Object> SerializedObjectReferences { get; set; }

        List<string> SerializedStateKeys { get; set; }

        List<string> SerializedStateValues { get; set; }
    }
}
