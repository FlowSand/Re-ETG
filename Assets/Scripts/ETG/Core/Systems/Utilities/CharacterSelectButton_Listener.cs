using UnityEngine;

#nullable disable

public class CharacterSelectButton_Listener : MonoBehaviour
    {
        public GameObject playerToSelect;

        private void OnClick(dfControl control, dfMouseEventArgs mouseEvent)
        {
            GameManager.PlayerPrefabForNewGame = this.playerToSelect;
            GameManager.Instance.LoadNextLevel();
        }
    }

