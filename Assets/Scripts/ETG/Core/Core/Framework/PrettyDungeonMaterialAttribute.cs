using UnityEngine;

#nullable disable

public class PrettyDungeonMaterialAttribute : PropertyAttribute
  {
    public string tilesetProperty;

    public PrettyDungeonMaterialAttribute(string tilesetPropertyName)
    {
      this.tilesetProperty = tilesetPropertyName;
    }
  }

