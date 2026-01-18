using UnityEngine;

#nullable disable

    public static class PastCameraUtility
    {
        public static void UnlockConversation()
        {
            GameManager.Instance.PrimaryPlayer.ClearInputOverride("past");
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                GameManager.Instance.SecondaryPlayer.ClearInputOverride("past");
            Pixelator.Instance.LerpToLetterbox(0.5f, 0.25f);
            Pixelator.Instance.DoFinalNonFadedLayer = false;
            GameUIRoot.Instance.ToggleLowerPanels(true, source: string.Empty);
            GameUIRoot.Instance.ShowCoreUI(string.Empty);
            GameManager.Instance.MainCameraController.SetManualControl(false);
        }

        public static void LockConversation(Vector2 lockPos)
        {
            GameManager.Instance.PrimaryPlayer.SetInputOverride("past");
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                GameManager.Instance.SecondaryPlayer.SetInputOverride("past");
            Pixelator.Instance.LerpToLetterbox(0.35f, 0.25f);
            Pixelator.Instance.DoFinalNonFadedLayer = true;
            GameUIRoot.Instance.ToggleLowerPanels(false, source: string.Empty);
            GameUIRoot.Instance.HideCoreUI(string.Empty);
            Vector2 vector = lockPos;
            CameraController cameraController = GameManager.Instance.MainCameraController;
            cameraController.SetManualControl(true);
            cameraController.OverridePosition = vector.ToVector3ZUp();
        }
    }

