using UnityEngine;

#nullable disable

public class DemoDieListener : MonoBehaviour
  {
    private void OnClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
      dfGUIManager.PopModal();
      GameManager.Instance.PrimaryPlayer.healthHaver.Die(Vector2.zero);
      this.transform.parent.gameObject.SetActive(false);
    }
  }

