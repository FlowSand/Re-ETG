using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Color Picker/Auto-rotate Model")]
public class AutoRotateModel : MonoBehaviour
  {
    private void Update() => this.transform.Rotate(Vector3.up * BraveTime.DeltaTime * 45f);
  }

