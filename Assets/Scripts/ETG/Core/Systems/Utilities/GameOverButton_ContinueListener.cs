using UnityEngine;

#nullable disable

public class GameOverButton_ContinueListener : MonoBehaviour
  {
    private void OnClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
      Object.Destroy((Object) GameManager.Instance);
      GameManager.Instance.LoadMainMenu();
    }
  }

