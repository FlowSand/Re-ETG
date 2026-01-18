using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Color Picker/Set Character Color")]
public class SetCharacterColor : MonoBehaviour
  {
    public SkinnedMeshRenderer CharacterRenderer;

    public Color BeltColor
    {
      get => this.CharacterRenderer.material.GetColor("_TeamColor");
      set => this.CharacterRenderer.material.SetColor("_TeamColor", value);
    }
  }

