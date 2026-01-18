using UnityEngine;

#nullable disable

public class StringTableStringAttribute : PropertyAttribute
    {
        public string stringTableTarget;
        public bool isInToggledState;
        public string keyToWrite = string.Empty;
        public StringTableStringAttribute.TargetStringTableType targetStringTable;

        public StringTableStringAttribute(string tableTarget = null)
        {
            this.stringTableTarget = tableTarget;
        }

        public enum TargetStringTableType
        {
            DEFAULT,
            ENEMIES,
            ITEMS,
            UI,
        }
    }

