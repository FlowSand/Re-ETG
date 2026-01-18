using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/General/Quit On Click")]
public class dfQuitOnClick : MonoBehaviour
    {
        private void OnClick() => Application.Quit();
    }

