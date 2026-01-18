using System;

#nullable disable

[Serializable]
public class PrototypePlacedObjectFieldData
  {
    public PrototypePlacedObjectFieldData.FieldType fieldType;
    public string fieldName;
    public float floatValue;
    public bool boolValue;

    public enum FieldType
    {
      FLOAT,
      BOOL,
    }
  }

