using UnityEngine;

#nullable disable

public class UndieListener : MonoBehaviour
    {
        private void OnClick(dfControl control, dfMouseEventArgs mouseEvent)
        {
            dfGUIManager.PopModal();
            Pixelator.Instance.LerpToLetterbox(0.5f, 0.0f);
            GameManager.Instance.PrimaryPlayer.healthHaver.FullHeal();
            this.transform.parent.gameObject.SetActive(false);
            GameManager.Instance.Unpause();
            GameManager.Instance.PrimaryPlayer.ClearDeadFlags();
        }
    }

