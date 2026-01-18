using UnityEngine;

#nullable disable

public class PlatformInterfaceGenericPC : PlatformInterface
    {
        protected override void OnStart()
        {
            Debug.Log((object) "Starting Generic PC platform interface.");
        }

        protected override void OnAchievementUnlock(Achievement achievement, int playerIndex)
        {
        }

        protected override void OnLateUpdate()
        {
        }

        protected override StringTableManager.GungeonSupportedLanguages OnGetPreferredLanguage()
        {
            return StringTableManager.GungeonSupportedLanguages.ENGLISH;
        }
    }

