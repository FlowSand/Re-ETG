using System;

using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/General/Load Level On Click")]
[Serializable]
public class LoadLevelByName : MonoBehaviour
    {
        public string LevelName;

        private void OnClick()
        {
            if (string.IsNullOrEmpty(this.LevelName))
                return;
            SceneManager.LoadScene(this.LevelName);
        }
    }

